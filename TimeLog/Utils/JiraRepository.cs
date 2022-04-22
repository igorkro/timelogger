using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeLog.Models;
using TimeLog.Models.Events;
using TimeLog.Models.Jira;

namespace TimeLog.Utils
{
    class JiraRepository: EventHandler
    {
        public class TicketInfo
        {
            public string TicketId { get; set; }
            public string Title { get; set; }
        }

        public class JobInfo
        {
            public string Type;
            public string TicketId;
            public Int64 InternalWorklogId;

            public JobInfo(string type, string ticketId, Int64 internalWorklogId = 0)
            {
                Type = type;
                TicketId = ticketId;
                InternalWorklogId = internalWorklogId;
            }
        }

        private long counter = 0;
        private HashSet<string> pendingJobs = new HashSet<string>();

        public JiraRepository()
        {
            EventBroker.Get().Subscribe(typeof(NetworkJobFinished), this);
            EventBroker.Get().Subscribe(typeof(SequenceGridTicketClicked), this);
        }

        private Uri constructUrl(string[] additionalParts)
        {
            StringBuilder urlStr = new StringBuilder(Services.Get().Config.JiraURL);
            if (urlStr[urlStr.Length - 1] == '/')
                urlStr.Remove(urlStr.Length - 1, 1);

            foreach (string part in additionalParts)
            {
                if (urlStr[0] != '?')
                    urlStr.Append('/');
                urlStr.Append(part);
            }

            return new Uri(urlStr.ToString());
        }

        public void FetchTicketInfo(string ticketId)
        {
            EventBroker.Get().Notify(new TicketFetchStarted(ticketId));

            try
            {
                Uri ticketUrl = constructUrl(new string[] { "rest/api/2/issue", ticketId, "?fields=summary" });
                string jobId = new StringBuilder(ticketId).Append("_").Append(Interlocked.Increment(ref counter).ToString()).ToString();

                lock(pendingJobs)
                {
                    pendingJobs.Add(jobId);
                }

                Services.Get().Net.PushJob(new Models.NetworkJob("fetch JIRA ticket", jobId, ticketUrl, HttpMethod.Get, string.Empty, new JobInfo("fetch", ticketId)));
            }
            catch(Exception ex)
            {
                Logger.Get().Log('E', "Exception: " + ex.ToString());
                EventBroker.Get().Notify(new TicketFetchFinished(ticketId, null));
            }
        }

        public void AddEntry(TimeLogEntry logEntry)
        {
            if (!logEntry.TimeReported.HasValue || !logEntry.Duration.HasValue)
                return;

            TicketLogRequest logRequest = new TicketLogRequest();
            logRequest.comment = logEntry.Comment;

            DateTime startedTime = logEntry.TimeReported.Value - logEntry.Duration.Value;
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

            StringBuilder startedTimeStr = new StringBuilder(startedTime.ToString("yyyy-MM-ddTHH:mm"));
            startedTimeStr.Append(":00.000+");
            startedTimeStr.Append(offset.Hours.ToString("00"));
            startedTimeStr.Append(offset.Minutes.ToString("00"));
            logRequest.started = startedTimeStr.ToString();
            logRequest.timeSpentSeconds = ((long)logEntry.Duration.Value.TotalMinutes * 60);

            string jsonRequest = JsonConvert.SerializeObject(logRequest);

            try
            {
                Uri ticketUrl = constructUrl(new string[] { "rest/api/2/issue", logEntry.TicketId, "worklog" });
                string jobId = new StringBuilder(logEntry.TicketId).Append("_").Append(Interlocked.Increment(ref counter).ToString()).ToString();

                lock (pendingJobs)
                {
                    pendingJobs.Add(jobId);
                }

                Services.Get().Net.PushJob(new Models.NetworkJob("add worklog", jobId, ticketUrl, HttpMethod.Post, jsonRequest, new JobInfo("log", logEntry.TicketId, logEntry.Id)));
            }
            catch (Exception ex)
            {
                Logger.Get().Log('E', string.Format("Jira job creating failed. Error:{0}", ex.Message));
            }
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (e is NetworkJobFinished)
            {
                NetworkJobFinished networkJobFinished = (NetworkJobFinished)e;
                lock (pendingJobs)
                {
                    if (!pendingJobs.Contains(networkJobFinished.Job.JobId))
                        return;

                    pendingJobs.Remove(networkJobFinished.Job.JobId);
                }

                JobInfo info = (JobInfo)networkJobFinished.Job.UserData;
                if (info == null)
                    return;

                if (info.Type == "fetch")
                {
                    processFetchResult(info.TicketId, networkJobFinished.StatusCode, networkJobFinished.Payload);
                }
                else if (info.Type == "log")
                {
                    processJobFlushed(info.TicketId, networkJobFinished.StatusCode, networkJobFinished.Payload, info.InternalWorklogId);
                }
            }
            else if (e is SequenceGridTicketClicked)
            {
                SequenceGridTicketClicked gridTicketClicked = (SequenceGridTicketClicked)e;
                if (string.IsNullOrEmpty(gridTicketClicked.TicketId))
                    return;

                Uri ticketUrl = constructUrl(new string[] { "browse", gridTicketClicked.TicketId.ToUpper() });
                Process.Start(ticketUrl.ToString());
            }
        }

        private void processJobFlushed(string ticketId, int statusCode, string payload, Int64 internalWorklogId)
        {
            if (statusCode == 200 || statusCode == 201)
            {
                TimeLogResponse timeLogResponse = JsonConvert.DeserializeObject<TimeLogResponse>(payload);
                if (timeLogResponse == null)
                {
                    Logger.Get().Log('E', string.Format("Failed to parse worklog response for ticket {0}", ticketId));
                    return;
                }

                if (string.IsNullOrEmpty(timeLogResponse.id))
                {
                    Logger.Get().Log('E', string.Format("Parsed the worklog response, but failed to get worklogId from it. Ticket {0}", ticketId));
                    return;
                }

                EventBroker.Get().Notify(new WorkLogFlushed(internalWorklogId, ticketId, timeLogResponse.id));
            }
            else
            {
                EventBroker.Get().Notify(new WorkLogFlushingError(ticketId, internalWorklogId, statusCode));
            }
        }

        private void processFetchResult(string ticketId, int statusCode, string payload)
        {
            if (statusCode == 200)
            {

                dynamic info = JsonConvert.DeserializeObject(payload);
                if (info == null)
                {
                    EventBroker.Get().Notify(new TicketFetchFinished(ticketId, null));
                }
                else
                {
                    EventBroker.Get().Notify(new TicketFetchFinished(ticketId, new TicketInfo()
                    {
                        TicketId = ticketId,
                        Title = info.fields?.summary ?? "unknown"
                    }));
                }
            }
            else
                EventBroker.Get().Notify(new TicketFetchFinished(ticketId, null));
        }
    }
}