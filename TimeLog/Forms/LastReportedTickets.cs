// Copyright (C) 2022  Igor Krushch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Email: dev@krushch.com

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
