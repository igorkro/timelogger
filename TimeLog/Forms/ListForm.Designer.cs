
namespace TimeLog.Forms
{
    partial class ListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListForm));
            this.panelTotals = new System.Windows.Forms.Panel();
            this.labelLastReported = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTotalReported = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sequenceGrid1 = new TimeLog.Controls.SequenceGrid();
            this.panelTotals.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTotals
            // 
            this.panelTotals.Controls.Add(this.labelLastReported);
            this.panelTotals.Controls.Add(this.label2);
            this.panelTotals.Controls.Add(this.labelTotalReported);
            this.panelTotals.Controls.Add(this.label1);
            this.panelTotals.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelTotals.Location = new System.Drawing.Point(0, 418);
            this.panelTotals.Name = "panelTotals";
            this.panelTotals.Size = new System.Drawing.Size(800, 32);
            this.panelTotals.TabIndex = 3;
            // 
            // labelLastReported
            // 
            this.labelLastReported.AutoSize = true;
            this.labelLastReported.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.labelLastReported.Location = new System.Drawing.Point(321, 10);
            this.labelLastReported.Name = "labelLastReported";
            this.labelLastReported.Size = new System.Drawing.Size(0, 13);
            this.labelLastReported.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last reported:";
            // 
            // labelTotalReported
            // 
            this.labelTotalReported.AutoSize = true;
            this.labelTotalReported.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.labelTotalReported.Location = new System.Drawing.Point(94, 10);
            this.labelTotalReported.Name = "labelTotalReported";
            this.labelTotalReported.Size = new System.Drawing.Size(0, 13);
            this.labelTotalReported.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total reported:";
            // 
            // sequenceGrid1
            // 
            this.sequenceGrid1.Font = new System.Drawing.Font("Roboto", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.sequenceGrid1.Location = new System.Drawing.Point(0, 0);
            this.sequenceGrid1.MinimumSize = new System.Drawing.Size(600, 320);
            this.sequenceGrid1.Name = "sequenceGrid1";
            this.sequenceGrid1.ScrollX = 0;
            this.sequenceGrid1.ScrollY = 0;
            this.sequenceGrid1.Size = new System.Drawing.Size(800, 402);
            this.sequenceGrid1.TabIndex = 2;
            this.sequenceGrid1.Text = "sequenceGrid1";
            this.sequenceGrid1.TicketClick += new TimeLog.Controls.SequenceGrid.TicketClickHandler(this.sequenceGrid1_TicketClick);
            // 
            // ListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelTotals);
            this.Controls.Add(this.sequenceGrid1);
            this.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "ListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Time table";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ListForm_FormClosed);
            this.Shown += new System.EventHandler(this.ListForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListForm_KeyDown);
            this.Resize += new System.EventHandler(this.ListForm_Resize);
            this.panelTotals.ResumeLayout(false);
            this.panelTotals.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Controls.SequenceGrid sequenceGrid1;
        private System.Windows.Forms.Panel panelTotals;
        private System.Windows.Forms.Label labelTotalReported;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLastReported;
        private System.Windows.Forms.Label label2;
    }
}