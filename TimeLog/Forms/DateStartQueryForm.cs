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
