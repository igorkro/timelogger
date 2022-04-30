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

namespace TimeLog.Forms
{
    public partial class HelpForm : Form
    {
        private static HelpForm instance = null;

        public static void Display()
        {
            if (instance != null)
            {
                instance.BringToFront();
                return;
            }

            instance = new HelpForm();
            instance.ShowDialog();
        }


        public HelpForm()
        {
            InitializeComponent();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            webBrowser1.DocumentText = Properties.Resources.readme;
        }

        private void HelpForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void webBrowser1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void HelpForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }
    }
}
