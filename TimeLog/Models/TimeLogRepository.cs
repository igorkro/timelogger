using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using TimeLog.Models.Events;
using TimeLog.Utils;

namespace TimeLog.Models
{
    class TimeLogRepository: EventHandler
    {
        private long _lastId = 0;
        private DateTime? lastReportedEntry = null;

        public DateTime? LastReportedEntry 
        { 
            get
            {
                return lastReportedEntry;
            }

            set
            {
                lastReportedEntry = value;
            }
        }

        public List<TimeLogEntry> TodayEntries { get; private set; }
        public List<DateTime> TodayNotifications { get; private set; }

        public TimeLogRepository()
        {
        }

        public String GetTotalReportedTimeFormatted()
        {
            TimeSpan totalReported = new TimeSpan();
            foreach (TimeLogEntry entry in TodayEntries)
            {
                totalReported += entry.Duration.Value;
            }

            return string.Format("{0}h {1}m", (int)totalReported.TotalHours, totalReported.Minutes);
        }

        public void Initialize()
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"SELECT MAX(Id) FROM log_entries";
            object result = command.ExecuteScalar();
            if (result == null || result is DBNull)
                _lastId = 0;
            else
                _lastId = Int32.Parse(result.ToString());

            ReloadForToday();
            LoadLastReported();

            Logger.Get().Log('I', string.Format("Time log initialized. Last entry ID:{0}", _lastId));

            EventBroker.Get().Subscribe(typeof(WorkLogNotification), this);
            EventBroker.Get().Subscribe(typeof(WorkLogFlushed), this);
            EventBroker.Get().Subscribe(typeof(WorkLogFlushingError), this);
        }

        public String DateTimeToStr(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public DateTime DateTimeFromStr(String value)
        {
            return DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public String TimeSpanToStr(TimeSpan timeSpan)
        {
            return timeSpan.ToString("hh\\:mm\\:ss");
        }

        public TimeSpan TimeSpanFromStr(String value)
        {
            return TimeSpan.ParseExact(value, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);
        }

        public void AddEntry(TimeLogEntry entry)
        {
            Logger.Get().Log('I', string.Format("Writing entry. Ticket:{0}", entry.TicketId));

            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"INSERT INTO log_entries(id, ticket_id, comment, time_reported, duration, is_flushed, skip_flushing) 
                VALUES(@id, @ticket_id, @comment, @time_reported, @duration, @is_flushed, @skip_flushing)";

            if (entry.Id == 0)
                entry.Id = ++_lastId;

            if (entry.TimeReported == null)
            {
                entry.TimeReported = DateTime.Now;
            }

            if (entry.Duration == null)
            {
                entry.Duration = entry.TimeReported - lastReportedEntry;
            }

            // Update LastReported only if custom entry end time is in the future
            if (!lastReportedEntry.HasValue)
            {
                lastReportedEntry = entry.TimeReported.Value;
            }
            else
            {
                if (entry.TimeReported.Value > lastReportedEntry)
                    lastReportedEntry = entry.TimeReported.Value;
            }

            command.Parameters.AddWithValue("@id", entry.Id);
            command.Parameters.AddWithValue("@ticket_id", entry.TicketId);
            command.Parameters.AddWithValue("@comment", entry.Comment);
            command.Parameters.AddWithValue("@time_reported", entry.TimeReported.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@duration", entry.Duration.Value.ToString("hh\\:mm\\:ss"));
            command.Parameters.AddWithValue("@is_flushed", entry.Flushed ? 1 : 0);
            command.Parameters.AddWithValue("@skip_flushing", entry.SkipFlushing ? 1 : 0);
            command.Prepare();

            if (command.ExecuteNonQuery() == 1)
            {
                Logger.Get().Log('I', string.Format("Entry recorded successfully. ID:{0}", entry.Id));

                EventBroker.Get().Notify(new NewLogEntryAdded(entry));
            }

            UpdateLastReported(lastReportedEntry.Value);
            Services.Get().JiraRepo.AddEntry(entry);

            if (entry.TimeReported.Value.Date.CompareTo(DateTime.Now.Date) == 0)
            {
                TodayEntries.Add(entry);
            }
        }


        public bool UpdateEntry(TimeLogEntry entry)
        {
            if (entry.Id == 0)
                return false;

            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"UPDATE log_entries SET ticket_id=@ticket_id, comment=@comment, time_reported=@time_reported, 
                duration=@duration, is_flushed=@is_flushed, skip_flushing=skip_flushing
                WHERE id=@id"; 

            command.Parameters.AddWithValue("@id", entry.Id);
            command.Parameters.AddWithValue("@ticket_id", entry.TicketId);
            command.Parameters.AddWithValue("@comment", entry.Comment);
            command.Parameters.AddWithValue("@time_reported", entry.TimeReported.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@duration", entry.Duration.Value.ToString("hh:mm:ss"));
            command.Parameters.AddWithValue("@is_flushed", entry.Flushed ? 1 : 0);
            command.Parameters.AddWithValue("@skip_flushing", entry.SkipFlushing ? 1 : 0);
            command.Prepare();

            return command.ExecuteNonQuery() == 1;
        }


        private bool MarkAsFlushed(string ticketId, Int64 internalWorklogId, string jiraWorklogId)
        {
            Logger.Get().Log('I', string.Format("Marking as flushed. Ticket:{0}", ticketId));

            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"UPDATE log_entries SET worklog_id=@worklogId, is_flushed=1 WHERE id=@id";
            command.Parameters.AddWithValue("@worklogId", jiraWorklogId);
            command.Parameters.AddWithValue("@id", internalWorklogId);
            command.Prepare();

            return command.ExecuteNonQuery() == 1;
        }

        private bool MarkAsSkipFlushing(string ticketId, Int64 internalWorklogId)
        {
            Logger.Get().Log('I', string.Format("Skip flushing worklog. Ticket:{0} WorklogId:{1}", ticketId, internalWorklogId));

            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"UPDATE log_entries SET is_flushed=0, skip_flushing=1 WHERE id=@id";
            command.Parameters.AddWithValue("@id", internalWorklogId);
            command.Prepare();

            return command.ExecuteNonQuery() == 1;
        }


        private void reloadEntriesAndNotifications()
        {
            TodayEntries = LoadForDay(DateTime.Now);
            TodayEntries.Sort((a, b) =>
            {
                if (!a.TimeReported.HasValue)
                    return -1;
                if (!b.TimeReported.HasValue)
                    return 1;
                return a.TimeReported.Value.CompareTo(b.TimeReported.Value);
            });
            ReloadNotifications(DateTime.Now);
        }

        public void ReloadForToday()
        {
            reloadEntriesAndNotifications();
            TimeLogEntry lastEntry = TodayEntries.LastOrDefault();
            LastReportedEntry = lastEntry?.TimeReported;
        }

        public void LoadLastReported()
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"SELECT time_reported FROM work_log WHERE id=1";
            object result = command.ExecuteScalar();
            if (result is null || result is DBNull)
                return;

            lastReportedEntry = DateTimeFromStr((string)result);
        }

        public void ReloadNotifications(DateTime day)
        {
            TodayNotifications = LoadNotificationsForDay(day);
        }

        public List<DateTime> LoadNotificationsForDay(DateTime day)
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"SELECT time_reported
                FROM notifications
                WHERE (time_reported BETWEEN @startDateTime AND @endDateTime)";

            DateTime startDateTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            DateTime endDateTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0).AddDays(1).AddMinutes(-1);

            command.Parameters.AddWithValue("@startDateTime", DateTimeToStr(startDateTime));
            command.Parameters.AddWithValue("@endDateTime", DateTimeToStr(endDateTime));
            command.Prepare();

            List<DateTime> timeLogEntries = new List<DateTime>();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    timeLogEntries.Add(DateTimeFromStr(reader.GetString(0)));
                }
            }

            return timeLogEntries;
        }

        public List<TimeLogEntry> LoadForDay(DateTime day, bool includeFlushed = true, bool updateLastReported = true)
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"SELECT id, ticket_id, comment, time_reported, duration, is_flushed, skip_flushing 
                FROM log_entries 
                WHERE (time_reported BETWEEN @startDateTime AND @endDateTime) ";

            if (!includeFlushed)
            {
                command.CommandText += "AND (is_flushed == 0)";
            }

            DateTime startDateTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            DateTime endDateTime = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0).AddDays(1).AddMinutes(-1);

            command.Parameters.AddWithValue("@startDateTime", DateTimeToStr(startDateTime));
            command.Parameters.AddWithValue("@endDateTime", DateTimeToStr(endDateTime));
            command.Prepare();

            List<TimeLogEntry> timeLogEntries = new List<TimeLogEntry>();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    TimeLogEntry newEntry = new TimeLogEntry();
                    newEntry.Id = reader.GetInt64(0);
                    newEntry.TicketId = reader.GetString(1);
                    newEntry.Comment = reader.GetString(2);
                    newEntry.TimeReported = DateTimeFromStr(reader.GetString(3));
                    newEntry.Duration = TimeSpanFromStr(reader.GetString(4));
                    newEntry.Flushed = reader.GetInt16(5) == 1;
                    newEntry.SkipFlushing = reader.GetInt16(6) == 1;

                    timeLogEntries.Add(newEntry);

                    if (updateLastReported)
                    {
                        if (!LastReportedEntry.HasValue || LastReportedEntry.Value < newEntry.TimeReported)
                        {
                            LastReportedEntry = newEntry.TimeReported;
                        }
                    }
                }
            }

            return timeLogEntries;
        }


        public void UpdateLastReported(DateTime lastLog)
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"insert or replace into work_log(id, time_reported) values (
                @id, @time_reported
            )";
            command.Parameters.AddWithValue("@id", 1);
            command.Parameters.AddWithValue("@time_reported", DateTimeToStr(lastLog));
            command.Prepare();

            command.ExecuteNonQuery();
        }

        public void RecordNotification(DateTime dateTime)
        {
            SQLiteCommand command = Services.Get().Db.createCommand();
            command.CommandText = @"INSERT INTO notifications(time_reported) VALUES(@time_reported)";
            command.Parameters.AddWithValue("@time_reported", DateTimeToStr(dateTime));
            command.Prepare();

            command.ExecuteNonQuery();

            TodayNotifications.Add(dateTime);
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (e is WorkLogNotification)
            {
                /*SQLiteCommand command = Services.Get().Db.createCommand();
                command.CommandText = @"INSERT INTO work_log(time_reported) VALUES (@time_reported);";
                command.Parameters.AddWithValue(@"time_reported", DateTimeToStr(DateTime.Now));
                command.Prepare();
                command.ExecuteNonQuery();*/

                if (lastReportedEntry == null || lastReportedEntry.Value.Date != DateTime.Now.Date)
                    reloadEntriesAndNotifications();

                lastReportedEntry = DateTime.Now;
                UpdateLastReported(lastReportedEntry.Value);

                RecordNotification(lastReportedEntry.Value);
            }
            else if (e is WorkLogFlushed)
            {
                WorkLogFlushed workLogFlushed = (WorkLogFlushed)e;

                MarkAsFlushed(workLogFlushed.TicketId, workLogFlushed.Id, workLogFlushed.JiraWorklogId);

                TimeLogEntry foundEntry = TodayEntries.Find(tl => tl.Id == workLogFlushed.Id);
                if (foundEntry != null)
                {
                    foundEntry.Flushed = true;
                }
            }
            else if (e is WorkLogFlushingError)
            {
                WorkLogFlushingError workLogFlushingError = (WorkLogFlushingError)e;

                // MarkAsSkipFlushing(workLogFlushingError.TicketId, workLogFlushingError.InternalWorklogId);

                TimeLogEntry foundEntry = TodayEntries.Find(tl => tl.Id == workLogFlushingError.InternalWorklogId);
                if (foundEntry != null)
                {
                    foundEntry.Flushed = false;
                    foundEntry.SkipFlushing = false;
                }
            }
        }
    }
}
