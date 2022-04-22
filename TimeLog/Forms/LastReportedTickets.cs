using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeLog.Models.Events;

namespace TimeLog.Forms
{
    public partial class LastReportedTickets : Form
    {
        public LastReportedTickets()
        {
            InitializeComponent();
            populateList();
        }

        private void populateList()
        {
            var cache = Services.Get().Tickets;
            listView1.BeginUpdate();
            try
            {
                bool isFirst = true;
                foreach (string ticket in cache.LastReported)
                {
                    ListViewItem newItem = new ListViewItem();
                    newItem.Text = ticket.ToUpper();
                    newItem.SubItems.Add(cache.Get(ticket.ToUpper()));
                    newItem.Tag = ticket;

                    listView1.Items.Add(newItem);

                    if (isFirst)
                    {
                        newItem.Selected = true;
                        isFirst = false;
                    }
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
            listView1.Focus();
        }

        private void LastReportedTickets_Leave(object sender, EventArgs e)
        {
        }

        private void LastReportedTickets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            else if (e.KeyCode == Keys.Return)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    EventBroker.Get().Notify(new PreviousTicketSelected((string)listView1.SelectedItems[0].Tag));
                }
                Close();
            }
        }

        private void LastReportedTickets_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
