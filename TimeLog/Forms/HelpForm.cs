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
