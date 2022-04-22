
namespace TimeLog.Forms
{
    partial class SettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textJiraUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textJiraPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textCommandShortcut = new System.Windows.Forms.TextBox();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textMiscTicket = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textJiraURL = new System.Windows.Forms.TextBox();
            this.basicAuthRadio = new System.Windows.Forms.RadioButton();
            this.accessTokenRadio = new System.Windows.Forms.RadioButton();
            this.textAccessToken = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkShowTotalLogged = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "JIRA Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "User name";
            // 
            // textJiraUsername
            // 
            this.textJiraUsername.Location = new System.Drawing.Point(125, 64);
            this.textJiraUsername.Name = "textJiraUsername";
            this.textJiraUsername.Size = new System.Drawing.Size(226, 22);
            this.textJiraUsername.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 14);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password";
            // 
            // textJiraPassword
            // 
            this.textJiraPassword.Location = new System.Drawing.Point(125, 92);
            this.textJiraPassword.Name = "textJiraPassword";
            this.textJiraPassword.PasswordChar = '*';
            this.textJiraPassword.Size = new System.Drawing.Size(226, 22);
            this.textJiraPassword.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Roboto", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.label4.Location = new System.Drawing.Point(14, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "General Settings";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 290);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 14);
            this.label5.TabIndex = 6;
            this.label5.Text = "Command Shortcut";
            // 
            // textCommandShortcut
            // 
            this.textCommandShortcut.Location = new System.Drawing.Point(188, 287);
            this.textCommandShortcut.Name = "textCommandShortcut";
            this.textCommandShortcut.Size = new System.Drawing.Size(163, 22);
            this.textCommandShortcut.TabIndex = 4;
            this.textCommandShortcut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textCommandShortcut_KeyDown);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(276, 364);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 5;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(17, 364);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 14);
            this.label6.TabIndex = 1;
            this.label6.Text = "Misc ticket";
            // 
            // textMiscTicket
            // 
            this.textMiscTicket.Location = new System.Drawing.Point(125, 183);
            this.textMiscTicket.Name = "textMiscTicket";
            this.textMiscTicket.Size = new System.Drawing.Size(226, 22);
            this.textMiscTicket.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 225);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 14);
            this.label7.TabIndex = 1;
            this.label7.Text = "Jira URL";
            // 
            // textJiraURL
            // 
            this.textJiraURL.Location = new System.Drawing.Point(125, 222);
            this.textJiraURL.Name = "textJiraURL";
            this.textJiraURL.Size = new System.Drawing.Size(226, 22);
            this.textJiraURL.TabIndex = 3;
            // 
            // basicAuthRadio
            // 
            this.basicAuthRadio.AutoSize = true;
            this.basicAuthRadio.Location = new System.Drawing.Point(17, 36);
            this.basicAuthRadio.Name = "basicAuthRadio";
            this.basicAuthRadio.Size = new System.Drawing.Size(82, 18);
            this.basicAuthRadio.TabIndex = 7;
            this.basicAuthRadio.TabStop = true;
            this.basicAuthRadio.Text = "Basic auth";
            this.basicAuthRadio.UseVisualStyleBackColor = true;
            this.basicAuthRadio.CheckedChanged += new System.EventHandler(this.basicAuthRadio_CheckedChanged);
            // 
            // accessTokenRadio
            // 
            this.accessTokenRadio.AutoSize = true;
            this.accessTokenRadio.Location = new System.Drawing.Point(17, 121);
            this.accessTokenRadio.Name = "accessTokenRadio";
            this.accessTokenRadio.Size = new System.Drawing.Size(98, 18);
            this.accessTokenRadio.TabIndex = 8;
            this.accessTokenRadio.TabStop = true;
            this.accessTokenRadio.Text = "Access Token";
            this.accessTokenRadio.UseVisualStyleBackColor = true;
            // 
            // textAccessToken
            // 
            this.textAccessToken.Location = new System.Drawing.Point(125, 144);
            this.textAccessToken.Name = "textAccessToken";
            this.textAccessToken.PasswordChar = '*';
            this.textAccessToken.Size = new System.Drawing.Size(226, 22);
            this.textAccessToken.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 147);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 14);
            this.label8.TabIndex = 10;
            this.label8.Text = "Access token";
            // 
            // checkShowTotalLogged
            // 
            this.checkShowTotalLogged.AutoSize = true;
            this.checkShowTotalLogged.Location = new System.Drawing.Point(40, 331);
            this.checkShowTotalLogged.Name = "checkShowTotalLogged";
            this.checkShowTotalLogged.Size = new System.Drawing.Size(246, 18);
            this.checkShowTotalLogged.TabIndex = 11;
            this.checkShowTotalLogged.Text = "Show Total Logged on Command Screen";
            this.checkShowTotalLogged.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 403);
            this.Controls.Add(this.checkShowTotalLogged);
            this.Controls.Add(this.textAccessToken);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.accessTokenRadio);
            this.Controls.Add(this.basicAuthRadio);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.textCommandShortcut);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textJiraPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textJiraURL);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textMiscTicket);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textJiraUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(254)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textJiraUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textJiraPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textCommandShortcut;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textMiscTicket;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textJiraURL;
        private System.Windows.Forms.RadioButton basicAuthRadio;
        private System.Windows.Forms.RadioButton accessTokenRadio;
        private System.Windows.Forms.TextBox textAccessToken;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkShowTotalLogged;
    }
}