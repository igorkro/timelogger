
namespace TimeLog.Forms
{
    partial class UnflushedLogsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnflushedLogsForm));
            this.listWorklogs = new System.Windows.Forms.ListView();
            this.columnFlushStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTicket = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnComment = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnReported = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panelActions = new System.Windows.Forms.Panel();
            this.dateTimeToLoad = new System.Windows.Forms.DateTimePicker();
            this.buttonFlush = new System.Windows.Forms.Button();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // listWorklogs
            // 
            this.listWorklogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFlushStatus,
            this.columnId,
            this.columnTicket,
            this.columnComment,
            this.columnReported,
            this.columnDuration});
            this.listWorklogs.FullRowSelect = true;
            this.listWorklogs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listWorklogs.HideSelection = false;
            this.listWorklogs.Location = new System.Drawing.Point(0, 0);
            this.listWorklogs.Name = "listWorklogs";
            this.listWorklogs.Size = new System.Drawing.Size(800, 405);
            this.listWorklogs.SmallImageList = this.imageList1;
            this.listWorklogs.TabIndex = 0;
            this.listWorklogs.UseCompatibleStateImageBehavior = false;
            this.listWorklogs.View = System.Windows.Forms.View.Details;
            // 
            // columnFlushStatus
            // 
            this.columnFlushStatus.Text = "";
            this.columnFlushStatus.Width = 32;
            // 
            // columnId
            // 
            this.columnId.Text = "ID";
            this.columnId.Width = 80;
            // 
            // columnTicket
            // 
            this.columnTicket.Text = "Ticket";
            this.columnTicket.Width = 120;
            // 
            // columnComment
            // 
            this.columnComment.Text = "Comment";
            this.columnComment.Width = 300;
            // 
            // columnReported
            // 
            this.columnReported.Text = "Reported At";
            this.columnReported.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnReported.Width = 120;
            // 
            // columnDuration
            // 
            this.columnDuration.Text = "Duration";
            this.columnDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnDuration.Width = 80;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "green_tick");
            this.imageList1.Images.SetKeyName(1, "red_cross");
            // 
            // panelActions
            // 
            this.panelActions.Controls.Add(this.dateTimeToLoad);
            this.panelActions.Controls.Add(this.buttonFlush);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(0, 411);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(800, 39);
            this.panelActions.TabIndex = 1;
            this.panelActions.Resize += new System.EventHandler(this.panelActions_Resize);
            // 
            // dateTimeToLoad
            // 
            this.dateTimeToLoad.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimeToLoad.Location = new System.Drawing.Point(12, 9);
            this.dateTimeToLoad.Name = "dateTimeToLoad";
            this.dateTimeToLoad.Size = new System.Drawing.Size(131, 21);
            this.dateTimeToLoad.TabIndex = 1;
            this.dateTimeToLoad.ValueChanged += new System.EventHandler(this.dateTimeToLoad_ValueChanged);
            // 
            // buttonFlush
            // 
            this.buttonFlush.Location = new System.Drawing.Point(695, 8);
            this.buttonFlush.Name = "buttonFlush";
            this.buttonFlush.Size = new System.Drawing.Size(98, 23);
            this.buttonFlush.TabIndex = 0;
            this.buttonFlush.Text = "Flush selected";
            this.buttonFlush.UseVisualStyleBackColor = true;
            this.buttonFlush.Click += new System.EventHandler(this.buttonFlush_Click);
            // 
            // UnflushedLogsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.listWorklogs);
            this.Font = new System.Drawing.Font("Roboto", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "UnflushedLogsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unflushed worklogs";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UnflushedLogsForm_FormClosed);
            this.Shown += new System.EventHandler(this.UnflushedLogsForm_Shown);
            this.SizeChanged += new System.EventHandler(this.UnflushedLogsForm_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UnflushedLogsForm_KeyDown);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listWorklogs;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.ColumnHeader columnId;
        private System.Windows.Forms.ColumnHeader columnTicket;
        private System.Windows.Forms.ColumnHeader columnComment;
        private System.Windows.Forms.ColumnHeader columnReported;
        private System.Windows.Forms.ColumnHeader columnDuration;
        private System.Windows.Forms.Button buttonFlush;
        private System.Windows.Forms.ColumnHeader columnFlushStatus;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.DateTimePicker dateTimeToLoad;
    }
}