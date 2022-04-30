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
using System.Windows.Forms;

namespace TimeLog.Forms
{
    public partial class DateStartQueryForm : Form
    {
        public DateStartQueryForm()
        {
            InitializeComponent();
        }

        public DateTime GetSelectedTime()
        {
            return dateTimePicker1.Value;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            DateTime selectedTime = dateTimePicker1.Value;

            if (selectedTime.Hour > currentTime.Hour || (selectedTime.Hour == currentTime.Hour && selectedTime.Minute > currentTime.Minute))
            {
                MessageBox.Show("Cannot specify the future time.");
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
