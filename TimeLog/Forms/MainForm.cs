using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TimeLog.Forms;
using TimeLog.Utils;
using TimeLog.Models.Events;
using TimeLog.Models;
using System.Drawing;

namespace TimeLog
{
    public partial class MainForm : Form, EventHandler
    {
        private class SizeAnimationFinished: TLEvent
        {
            public bool WasExpanding { get; set; }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private string currentTicketId;

        private Timer sizeTimer = new Timer();
        private int finalHeight = 0;
        private float heightDelta = 0.0f;
        private float currentHeight = 0.0f;
        private bool isExpanding = false;


        public MainForm()
        {
            InitializeComponent();

            sizeTimer.Interval = 25;
            sizeTimer.Enabled = false;
            sizeTimer.Tick += SizeTimer_Tick;
            sizeTimer.Tag = null;

            EventBroker.Get().Subscribe(typeof(TicketFetchFinished), this);
            EventBroker.Get().Subscribe(typeof(SizeAnimationFinished), this);
            EventBroker.Get().Subscribe(typeof(PreviousTicketSelected), this);
        }

        private void SizeTimer_Tick(object sender, EventArgs e)
        {
            if (!Visible)
                sizeTimer.Stop();

            currentHeight += heightDelta;
            if (Math.Abs(currentHeight - finalHeight) < 0.1f)
            {
                currentHeight = finalHeight;
                sizeTimer.Stop();

                Invoke((MethodInvoker)delegate { EventBroker.Get().Notify(new SizeAnimationFinished { WasExpanding = isExpanding }); });
            }
            Height = (int)currentHeight;
        }

        private void AnimateSize(int finalHeight, int timeInMS, bool expanding)
        {
            isExpanding = expanding;
            if (finalHeight == Height)
            {
                Invoke((MethodInvoker)delegate { EventBroker.Get().Notify(new SizeAnimationFinished { WasExpanding = isExpanding }); });
                return;
            }

            this.finalHeight = finalHeight;
            currentHeight = Height;

            heightDelta = (finalHeight - currentHeight) / ((float)timeInMS / (float)sizeTimer.Interval);

            sizeTimer.Tag = 1;
            sizeTimer.Start();
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    ProcessCommand();
                    break;
                case Keys.Space:
                    ProcessTicketFetching();
                    break;
                case Keys.Escape:
                    CloseCommandWindow();
                    break;
                case Keys.Down:
                    DisplayLastCommands();
                    break;
                case Keys.F1:
                    Hide();
                    HelpForm.Display();
                    break;
            }
        }

        private void DisplayLastCommands()
        {
            var tickets = new LastReportedTickets();
            tickets.Left = Left + commandTextBox.Left + (commandTextBox.Width - tickets.Width) / 2;
            tickets.Top = Top + commandTextBox.Top + commandTextBox.Height;
            tickets.Show();
            tickets.Focus();
        }

        private void CloseCommandWindow()
        {
            commandTextBox.Text = "";
            ticketDescription.Hide();
            sizeTimer.Stop();
            Height = 54;

            Hide();
        }

        private void ProcessCommand()
        {
            var cmd = commandTextBox.Text;
            if (processCommand(cmd))
            {
                commandTextBox.Text = "";
                ticketDescription.Hide();
                Height = 54;
            }
        }

        private void ProcessTicketFetching()
        {
            int lastPos = 0;
            string ticket = JiraKeyExtractor.ExtractRegExp(commandTextBox.Text, ref lastPos).ToUpper();
            if (ticket == currentTicketId)
                return;

            currentTicketId = ticket;

            if (string.IsNullOrEmpty(currentTicketId))
            {
                ticketDescription.Hide();
                AnimateSize(54, 300, false);
                return;
            }

            EventBroker.Get().Notify(new TicketSetEvent(currentTicketId));

            string cachedInfo = Services.Get().Tickets.Get(currentTicketId.ToUpper());
            if (cachedInfo == null)
            {
                infiniteProgress.Show();

                Services.Get().JiraRepo.FetchTicketInfo(currentTicketId);
            }
            else
            {
                ticketDescription.Text = string.Format("{0}: {1}", currentTicketId, cachedInfo);
                ticketDescription.ForeColor = Color.Black;
                ticketDescription.Show();

                AnimateSize(77, 300, true);
                infiniteProgress.Hide();
            }
        }

        private bool processCommand(string command)
        {
            char[] whitespaceCharset = { ' ', '\t' };

            command = command.TrimStart(whitespaceCharset);

            if (command.Length < 3 && command != "-")
            {
                Hide();
                return true;
            }

            if (command[0] == '!')
            {
                processSystemCommand(command.Substring(1));
                return true;
            }
            else if (command[0] == '+')
            {
                processCustomEntry(command.Substring(1));
                return true;
            }
            else if (command[0] == '-')
            {
                Hide();
                if (MessageBox.Show("Record the notification?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    EventBroker.Get().Notify(new WorkLogNotification());
                return true;
            }

            return processNewEntry(command);
        }

        private void processSystemCommand(string command)
        {
            Hide();
            command = command.ToLower();

            switch (command)
            {
                case "list":
                    ListForm.Display();
                    break;

                case "unflushed":
                    UnflushedLogsForm.Display();
                    break;

                case "config":
                    SettingsForm.Display();
                    break;

                case "about":
                    AboutBox.Display();
                    break;
            }
        }

        private TimeSpan processOffset(ref string command)
        {
            int lastPos = command.LastIndexOf('|');
            if (lastPos == -1)
                return new TimeSpan();

            string offsetStr = command.Substring(lastPos + 1).Trim();
            command = command.Remove(lastPos).Trim();

            TimeSpan offset;
            if (!TimeSpan.TryParse(offsetStr, out offset))
                return new TimeSpan();

            return offset;
        }


        private bool processNewEntry(string command)
        {
            // ticket-Num comment | 0:12    - decreases the duration from left (leaves gap between last reported and work start) 

            DateTime? lastReportedEntry = Services.Get().TimeLogRepo.LastReportedEntry;
            if (lastReportedEntry == null || lastReportedEntry.Value.Day != DateTime.Now.Day)
            {
                var queryForm = new DateStartQueryForm();
                if (queryForm.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                DateTime selectedTime = queryForm.GetSelectedTime();
                DateTime dayStartTime = DateTime.Now;
                dayStartTime = new DateTime(dayStartTime.Year, dayStartTime.Month, dayStartTime.Day, selectedTime.Hour, selectedTime.Minute, 0);

                Services.Get().TimeLogRepo.RecordNotification(dayStartTime);

                Services.Get().TimeLogRepo.LastReportedEntry = dayStartTime;
            }

            int lastPos = 0;
            string ticketId = JiraKeyExtractor.ExtractRegExp(command, ref lastPos);
            if (lastPos > 0)
            {
                command = command.Substring(lastPos).TrimStart();

                Services.Get().Tickets.PushLastReported(ticketId.ToUpper());
            }
            else
            {
                ticketId = Services.Get().Config.MiscTicket;
                if (string.IsNullOrEmpty(ticketId))
                {
                    MessageBox.Show("There is no misc ticket configured, thus cannot log the current entry. Please configure the misc ticket first.", "Misc ticket issue");
                    return false;
                }
            }

            ticketId = ticketId.ToLower();

            TimeSpan offset = processOffset(ref command);

            DateTime timeReported = DateTime.Now;
            TimeSpan duration = timeReported - Services.Get().TimeLogRepo.LastReportedEntry.Value + offset;

            infiniteProgress.Show();
            Invoke((MethodInvoker)delegate
            {
                Services.Get().TimeLogRepo.AddEntry(new Models.TimeLogEntry
                {
                    TicketId = ticketId,
                    Comment = command,
                    TimeReported = timeReported,
                    Duration = duration,
                    Flushed = false,
                    SkipFlushing = false
                });

                infiniteProgress.Hide();
                Hide();
            });
            return true;
        }


        private void processCustomEntry(String command)
        {
            Hide();

            // + 14:00-15:00 [ticket-id] comment

            command = command.Trim();
            int dashIndex = command.IndexOf('-');
            if (dashIndex == -1)
            {
                Logger.Get().Log('E', "Invalid custom entry format. Dash is missing");
                return;
            }

            int whitespaceIndex = command.IndexOf(' ');
            if (whitespaceIndex == -1)
            {
                Logger.Get().Log('E', "Invalid custom entry format. Nothing after the time frame");
                return;
            }

            string startTime = command.Substring(0, dashIndex);
            TimeSpan startTimeOffset;
            if (!TimeSpan.TryParse(startTime, out startTimeOffset))
            {
                Logger.Get().Log('E', "Invalid custom entry format. Wrong start time format");
                return;
            }

            string endTime = command.Substring(dashIndex + 1, whitespaceIndex - dashIndex - 1);
            TimeSpan endTimeOffset;
            if (!TimeSpan.TryParse(endTime, out endTimeOffset))
            {
                Logger.Get().Log('E', "Invalid custom entry format. Wrong end time format");
                return;
            }

            string ticketAndComment = command.Substring(whitespaceIndex + 1).Trim();

            int lastPos = 0;
            string ticketId = JiraKeyExtractor.Extract(ticketAndComment, ref lastPos);
            if (lastPos > 0)
            {
                ticketAndComment = ticketAndComment.Substring(lastPos);

                Services.Get().Tickets.PushLastReported(ticketId.ToUpper());
            }
            else
            {
                ticketId = Services.Get().Config.MiscTicket;
                if (string.IsNullOrEmpty(ticketId))
                {
                    MessageBox.Show("There is no misc ticket configured, thus cannot log the current entry. Please configure the misc ticket first.", "Misc ticket issue");
                    return;
                }
            }

            ticketId = ticketId.ToLower();

            DateTime timeReported = DateTime.Now.Date + endTimeOffset;
            TimeSpan duration = endTimeOffset - startTimeOffset;

            infiniteProgress.Show();
            Invoke((MethodInvoker)delegate
            {
                Services.Get().TimeLogRepo.AddEntry(new Models.TimeLogEntry
                {
                    TicketId = ticketId,
                    Comment = ticketAndComment,
                    TimeReported = timeReported,
                    Duration = duration,
                    Flushed = false,
                    SkipFlushing = false
                });

                infiniteProgress.Hide();
                Hide();
            });
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (!Visible)
                return;

            if (e is TicketFetchFinished)
            {
                TicketFetchFinished ticketFetchFinished = (TicketFetchFinished)e;
                if (ticketFetchFinished.TicketId == currentTicketId)
                {
                    if (ticketFetchFinished.TicketInfo != null)
                    {
                        Services.Get().Tickets.Add(currentTicketId.ToUpper(), ticketFetchFinished.TicketInfo.Title);

                        ticketDescription.Text = string.Format("{0}: {1}", currentTicketId, ticketFetchFinished.TicketInfo.Title);
                        ticketDescription.ForeColor = Color.Black;
                    }
                    else
                    {
                        ticketDescription.Text = string.Format("{0}: unable to load ticket details", currentTicketId);
                        ticketDescription.ForeColor = Color.Maroon;
                    }
                    ticketDescription.Show();

                    AnimateSize(77, 300, true);
                }
                infiniteProgress.Hide();
            }
            else if (e is PreviousTicketSelected)
            {
                PreviousTicketSelected previousTicketSelected = (PreviousTicketSelected)e;
                if (!string.IsNullOrEmpty(previousTicketSelected.Ticket))
                {
                    commandTextBox.Text = previousTicketSelected.Ticket;
                    commandTextBox.SelectionStart = commandTextBox.Text.Length;
                    commandTextBox.SelectionLength = 0;
                }
            }
        }

        public void DoShow()
        {
            currentTicketId = "";

            TimeLogRepository repo = Services.Get().TimeLogRepo;
            DateTime now = DateTime.Now;
            if (!repo.LastReportedEntry.HasValue || repo.LastReportedEntry.Value.Date.CompareTo(now.Date) != 0)
            {
                repo.LoadForDay(now);
                repo.ReloadNotifications(now);
            }

            if (repo.LastReportedEntry.HasValue && repo.LastReportedEntry.Value.Date.CompareTo(now.Date) == 0)
            {
                labelLastLogged.Text = repo.LastReportedEntry.Value.ToString("HH:mm");
            }
            else
            {
                labelLastLogged.Text = "---";
            }

            var showTotalLogged = Services.Get().Config.ShowTotalLogged;
            labelTotalLogged.Visible = showTotalLogged;
            labelTotalLoggedTime.Visible = showTotalLogged;

            Width = showTotalLogged ? 801 : 728;

            if (showTotalLogged)
            {
                labelTotalLoggedTime.Text = Services.Get().TimeLogRepo.GetTotalReportedTimeFormatted();
            }

            Show();
            Focus();
            commandTextBox.Focus();
        }
    }
}
