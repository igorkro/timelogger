using System;
using System.Windows.Forms;
using TimeLog.Models;
using TimeLog.Models.Events;
using TimeLog.Utils;

namespace TimeLog.Forms
{
    public partial class ListForm : Form
    {
        private static ListForm instance = null;

        public static void Display()
        {
            if (instance != null)
                instance.BringToFront();
            else
            {
                instance = new ListForm();
                instance.ShowDialog();
            }
        }

        public ListForm()
        {
            InitializeComponent();

            WindowUtils.GeometryFromString(Properties.Settings.Default.ListFormGeometry, this);

            DateTime? lastReported = Services.Get().TimeLogRepo.LastReportedEntry;
            if (lastReported == null || lastReported.Value.Date != DateTime.Now.Date)
            {
                Services.Get().TimeLogRepo.ReloadForToday();
            }

            sequenceGrid1.SetTimeLogEntries(Services.Get().TimeLogRepo.TodayEntries);
            sequenceGrid1.SetSeparatorTimes(Services.Get().TimeLogRepo.TodayNotifications);
            sequenceGrid1.SetHours(6, 24);

            UpdateGridSize();
            UpdateTotalReportedTime();
            UpdateLastReportedTime();
        }

        private void UpdateLastReportedTime()
        {
            DateTime? lastReported = Services.Get().TimeLogRepo.LastReportedEntry;
            if (!lastReported.HasValue)
                labelLastReported.Text = "---";
            else
            {
                DateTime lastReportedDateTime = lastReported.Value;
                if (lastReportedDateTime.Date != DateTime.Now.Date)
                {
                    labelLastReported.Text = "---";
                }
                else
                {
                    labelLastReported.Text = lastReportedDateTime.ToString("HH:mm");
                }
            }
        }

        private void UpdateTotalReportedTime()
        {
            labelTotalReported.Text = Services.Get().TimeLogRepo.GetTotalReportedTimeFormatted();
        }

        private void UpdateGridSize()
        {
            sequenceGrid1.Height = ClientSize.Height - panelTotals.Height - 2;
            sequenceGrid1.Width = ClientSize.Width;
        }

        private void ListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void ListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }

        private void ListForm_Resize(object sender, System.EventArgs e)
        {
            UpdateGridSize();
        }

        private void sequenceGrid1_TicketClick(object sender, string ticket)
        {
            EventBroker.Get().Notify(new SequenceGridTicketClicked(ticket));
        }

        private void ListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ListFormGeometry = WindowUtils.GeometryToString(this);
            Properties.Settings.Default.Save();
        }

        private void ListForm_Shown(object sender, EventArgs e)
        {
            sequenceGrid1.ScrollToTime(DateTime.Now);
        }
    }
}
