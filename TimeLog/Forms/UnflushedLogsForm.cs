using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TimeLog.Models;
using TimeLog.Models.Events;

namespace TimeLog.Forms
{
    public partial class UnflushedLogsForm : Form, EventHandler
    {
        private static UnflushedLogsForm instance = null;

        private Dictionary<long, ListViewItem> idToItem = new Dictionary<long, ListViewItem>();

        public UnflushedLogsForm()
        {
            InitializeComponent();

            EventBroker.Get().Subscribe(typeof(WorkLogFlushingError), this);
            EventBroker.Get().Subscribe(typeof(WorkLogFlushed), this);

            dateTimeToLoad.Value = DateTime.Now.Date;
            UpdateList();
        }

        public static void Display()
        {
            if (instance != null)
                instance.BringToFront();
            else
            {
                instance = new UnflushedLogsForm();
                instance.ShowDialog();
            }
        }

        private void UpdateSizes()
        {
            listWorklogs.Height = ClientSize.Height - panelActions.Height - 2;
            listWorklogs.Width = ClientSize.Width;
        }

        private void UpdateList()
        {
            DateTime dateToLoad = dateTimeToLoad.Value;
            List<TimeLogEntry> entries = Services.Get().TimeLogRepo.LoadForDay(dateToLoad, false, false);

            listWorklogs.BeginUpdate();
            try
            {
                listWorklogs.Items.Clear();
                idToItem.Clear();

                foreach (TimeLogEntry entry in entries)
                {
                    ListViewItem newItem = new ListViewItem();
                    newItem.Tag = entry;
                    newItem.Text = "";
                    newItem.SubItems.Add(entry.Id.ToString());
                    newItem.SubItems.Add(entry.TicketId.ToUpper());
                    newItem.SubItems.Add(entry.Comment);
                    newItem.SubItems.Add(entry.TimeReported.Value.ToString("HH:mm"));
                    newItem.SubItems.Add(entry.Duration.Value.ToString("hh\\:mm"));

                    listWorklogs.Items.Add(newItem);
                    idToItem[entry.Id] = newItem;
                }
            }
            finally
            {
                listWorklogs.EndUpdate();
            }
        }

        private void UnflushedLogsForm_Shown(object sender, EventArgs e)
        {
            UpdateSizes();
        }

        private void UnflushedLogsForm_SizeChanged(object sender, EventArgs e)
        {
            UpdateSizes();
        }

        private void panelActions_Resize(object sender, EventArgs e)
        {
            buttonFlush.Left = panelActions.Width - buttonFlush.Width - 8;
        }

        private void UnflushedLogsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;

            EventBroker.Get().UnsubscribeFromAll(this);
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (e is WorkLogFlushed)
            {
                WorkLogFlushed workLogFlushed = (WorkLogFlushed)e;

                if (idToItem.ContainsKey(workLogFlushed.Id))
                {
                    ListViewItem item = idToItem[workLogFlushed.Id];
                    item.ImageIndex = 0;
                }
            }
            else if (e is WorkLogFlushingError)
            {
                WorkLogFlushingError workLogFlushingError = (WorkLogFlushingError)e;
                if (idToItem.ContainsKey(workLogFlushingError.InternalWorklogId))
                {
                    ListViewItem item = idToItem[workLogFlushingError.InternalWorklogId];
                    item.ImageIndex = 1;
                }
            }
        }

        private void dateTimeToLoad_ValueChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void buttonFlush_Click(object sender, EventArgs e)
        {
            FlushSelected();
        }

        private void FlushSelected()
        {
            NetworkService network = Services.Get().Net;
            foreach(ListViewItem listViewItem in listWorklogs.SelectedItems)
            {
                TimeLogEntry entry = (TimeLogEntry)listViewItem.Tag;
                if (!network.IsLogFlushingQueued(entry.Id))
                    Services.Get().JiraRepo.AddEntry(entry);
            }
        }

        private void UnflushedLogsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
