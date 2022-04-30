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
using TimeLog.Models;
using TimeLog.Models.Events;

namespace TimeLog.Forms
{
    public partial class SettingsForm : Form
    {
        private static SettingsForm instance = null;
        public SettingsForm()
        {
            InitializeComponent();
        }

        public static void Display()
        {
            if (instance != null)
                instance.BringToFront();
            else
            {
                instance = new SettingsForm();
                instance.ShowDialog();
            }
        }

        private void textCommandShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Back && e.KeyCode != Keys.Escape)
            {
                Keys modifierKeys = e.Modifiers;
                Keys pressedKey = e.KeyData ^ modifierKeys; //remove modifier keys

                if (modifierKeys != Keys.None && pressedKey != Keys.None)
                {
                    //do stuff with pressed and modifier keys
                    var converter = new KeysConverter();
                    textCommandShortcut.Text = converter.ConvertToString(e.KeyData);
                    //At this point, we know a one or more modifiers and another key were pressed
                    //modifierKeys contains the modifiers
                    //pressedKey contains the other pressed key
                    //Do stuff with results here
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                e.Handled = false;
                e.SuppressKeyPress = true;

                textCommandShortcut.Text = "";
            }
        }

        private void initSettings()
        {
            Config config = Services.Get().Config;

            textCommandShortcut.Text = config.CommandShortcut;
            textMiscTicket.Text = config.MiscTicket;

            textJiraURL.Text = config.JiraURL;

            basicAuthRadio.Checked = !config.IsAccessTokenAuth;
            accessTokenRadio.Checked = config.IsAccessTokenAuth;

            textJiraUsername.Enabled = basicAuthRadio.Checked;
            textJiraPassword.Enabled = basicAuthRadio.Checked;
            textAccessToken.Enabled = accessTokenRadio.Checked;

            var cred = Services.Get().Creds.Load();
            if (cred != null)
            {
                textJiraUsername.Text = cred.Username;
                textJiraPassword.Text = "     ";
                textJiraUsername.Modified = false;
                textJiraPassword.Modified = false;

                textAccessToken.Text = "     ";
                textAccessToken.Modified = false;
            }
            else
            {
                textJiraUsername.Text = Environment.UserName;
                textJiraPassword.Text = "";
                textJiraUsername.Modified = false;
                textJiraPassword.Modified = false;

                textAccessToken.Text = "";
                textAccessToken.Modified = false;
            }

            checkShowTotalLogged.Checked = config.ShowTotalLogged;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            initSettings();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            Services.Get().KeyHook.UnregisterAll();

            if (!Services.Get().KeyHook.RegisterHotKeyFromString(textCommandShortcut.Text))
            {
                try
                {
                    Services.Get().KeyHook.RegisterHotKeyFromString(Services.Get().Config.CommandShortcut);

                    MessageBox.Show(string.Format("Provided shortcut cannot be assigned: {0}. Reset to standard Ctrl+Space for this app session.", Services.Get().Config.CommandShortcut), "Failed to assign the shortcut", MessageBoxButtons.OK);
                }
                catch
                {
                    MessageBox.Show(string.Format("Provided shortcut cannot be assigned: {0}", Services.Get().Config.CommandShortcut), "Failed to assign the shortcut", MessageBoxButtons.OK);
                }
            }

            Config config = Services.Get().Config;
            config.CommandShortcut = textCommandShortcut.Text;
            config.MiscTicket = textMiscTicket.Text.Trim();
            config.JiraURL = textJiraURL.Text;
            config.IsAccessTokenAuth = accessTokenRadio.Checked;
            config.ShowTotalLogged = checkShowTotalLogged.Checked;

            Services.Get().SaveConfig();

            if (textJiraUsername.Modified || textJiraPassword.Modified || textAccessToken.Modified)
            {
                if (basicAuthRadio.Checked)
                    Services.Get().Creds.Save(textJiraUsername.Modified ? textJiraUsername.Text : null, textJiraPassword.Modified ? textJiraPassword.Text : null);
                else if (accessTokenRadio.Checked)
                    Services.Get().Creds.Save("", textAccessToken.Text);

                EventBroker.Get().Notify(new JiraCredentialsUpdated());
            }

            DialogResult = DialogResult.OK;
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }

        private void basicAuthRadio_CheckedChanged(object sender, EventArgs e)
        {
            textJiraUsername.Enabled = basicAuthRadio.Checked;
            textJiraPassword.Enabled = basicAuthRadio.Checked;

            textAccessToken.Enabled = accessTokenRadio.Checked;
        }
    }
}
