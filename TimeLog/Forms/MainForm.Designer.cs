
namespace TimeLog
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.ticketDescription = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLastLogged = new System.Windows.Forms.Label();
            this.labelTotalLogged = new System.Windows.Forms.Label();
            this.labelTotalLoggedTime = new System.Windows.Forms.Label();
            this.infiniteProgress = new TimeLog.Controls.InfiniteProgress();
            this.SuspendLayout();
            // 
            // commandTextBox
            // 
            this.commandTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.commandTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.commandTextBox.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.commandTextBox.Location = new System.Drawing.Point(28, 12);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.Size = new System.Drawing.Size(616, 30);
            this.commandTextBox.TabIndex = 0;
            this.commandTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // ticketDescription
            // 
            this.ticketDescription.AutoEllipsis = true;
            this.ticketDescription.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ticketDescription.Font = new System.Drawing.Font("Roboto", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.ticketDescription.Location = new System.Drawing.Point(29, 45);
            this.ticketDescription.Name = "ticketDescription";
            this.ticketDescription.Size = new System.Drawing.Size(615, 23);
            this.ticketDescription.TabIndex = 3;
            this.ticketDescription.Text = "ECI-123 Test";
            this.ticketDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ticketDescription.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.label1.Location = new System.Drawing.Point(651, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Last logged at:";
            // 
            // labelLastLogged
            // 
            this.labelLastLogged.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.labelLastLogged.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.labelLastLogged.Location = new System.Drawing.Point(651, 27);
            this.labelLastLogged.Name = "labelLastLogged";
            this.labelLastLogged.Size = new System.Drawing.Size(72, 15);
            this.labelLastLogged.TabIndex = 5;
            this.labelLastLogged.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTotalLogged
            // 
            this.labelTotalLogged.AutoSize = true;
            this.labelTotalLogged.Font = new System.Drawing.Font("Roboto", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.labelTotalLogged.Location = new System.Drawing.Point(727, 12);
            this.labelTotalLogged.Name = "labelTotalLogged";
            this.labelTotalLogged.Size = new System.Drawing.Size(64, 13);
            this.labelTotalLogged.TabIndex = 4;
            this.labelTotalLogged.Text = "Total logged:";
            // 
            // labelTotalLoggedTime
            // 
            this.labelTotalLoggedTime.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.labelTotalLoggedTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.labelTotalLoggedTime.Location = new System.Drawing.Point(730, 27);
            this.labelTotalLoggedTime.Name = "labelTotalLoggedTime";
            this.labelTotalLoggedTime.Size = new System.Drawing.Size(61, 15);
            this.labelTotalLoggedTime.TabIndex = 5;
            this.labelTotalLoggedTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // infiniteProgress
            // 
            this.infiniteProgress.ItemColor = System.Drawing.Color.Red;
            this.infiniteProgress.Location = new System.Drawing.Point(5, 18);
            this.infiniteProgress.Name = "infiniteProgress";
            this.infiniteProgress.NodesCount = 24;
            this.infiniteProgress.Size = new System.Drawing.Size(20, 20);
            this.infiniteProgress.StepInterval = 20;
            this.infiniteProgress.TabIndex = 2;
            this.infiniteProgress.Text = "infiniteProgress1";
            this.infiniteProgress.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(801, 54);
            this.Controls.Add(this.labelTotalLoggedTime);
            this.Controls.Add(this.labelLastLogged);
            this.Controls.Add(this.labelTotalLogged);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ticketDescription);
            this.Controls.Add(this.infiniteProgress);
            this.Controls.Add(this.commandTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Time Logger";
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox commandTextBox;
        private Controls.InfiniteProgress infiniteProgress;
        private System.Windows.Forms.Label ticketDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLastLogged;
        private System.Windows.Forms.Label labelTotalLogged;
        private System.Windows.Forms.Label labelTotalLoggedTime;
    }
}

