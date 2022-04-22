using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeLog.Forms;
using TimeLog.Models.Events;

namespace TimeLog
{
    public class TimeLogApplicationContext : ApplicationContext, EventHandler
    {
        private NotifyIcon trayIcon;
        private MainForm mainForm = new MainForm();
        private MenuItem menuItemRetry = new MenuItem("Retry");
        private MenuItem menuItemRetrySeparator = new MenuItem("-");

        private SettingsForm currentSettingsForm = null;

        public TimeLogApplicationContext()
        {
            Services.Get().SynchronizationContext = System.Threading.SynchronizationContext.Current;

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.MainIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Notify", DoNotify),
                    new MenuItem("-"),
                    new MenuItem("Time table...", DoShowTimeTable),
                    new MenuItem("Settings...", DoShowSettings),
                    new MenuItem("-"),
                    new MenuItem("Help...", DoShowHelp),
                    new MenuItem("About...", DoShowAbout),
                    new MenuItem("-"),
                    menuItemRetry,
                    menuItemRetrySeparator,
                    new MenuItem("Exit", Exit)
                }),
                Visible = true
            };
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            menuItemRetry.Visible = false;
            menuItemRetry.Click += DoRetry;

            menuItemRetrySeparator.Visible = false;

            Services.Get().KeyHook.KeyPressed += KeyboardHook_KeyPressed;

            if (!Services.Get().KeyHook.RegisterHotKeyFromString(Services.Get().Config.CommandShortcut))
            {
                try
                {
                    Services.Get().KeyHook.RegisterHotKey(ModifierKeys.Control, Keys.Space);

                    Services.Get().Config.CommandShortcut = "Ctrl+Space";
                    MessageBox.Show(string.Format("Provided shortcut cannot be assigned: {0}. Reset to standard Ctrl+Space for this app session.", Services.Get().Config.CommandShortcut), "Failed to assign the shortcut", MessageBoxButtons.OK);
                }
                catch
                {
                    MessageBox.Show(string.Format("Provided shortcut cannot be assigned: {0}", Services.Get().Config.CommandShortcut), "Failed to assign the shortcut", MessageBoxButtons.OK);
                }
            }

            EventBroker.Get().Subscribe(typeof(NetworkIssueDetected), this);
            EventBroker.Get().Subscribe(typeof(InvalidJiraCredentialsDetected), this);
            EventBroker.Get().Subscribe(typeof(NetworkIssueResolved), this);
        }


        private void DoShowHelp(object sender, EventArgs e)
        {
            HelpForm.Display();
        }


        private void DoShowAbout(object sender, EventArgs e)
        {
            AboutBox.Display();
        }


        private void DoShowSettings(object sender, EventArgs e)
        {
            if (currentSettingsForm != null)
            {
                currentSettingsForm.BringToFront();
                return;
            }

            currentSettingsForm = new SettingsForm();
            currentSettingsForm.ShowDialog();
            currentSettingsForm = null;
        }

        private void DoShowTimeTable(object sender, EventArgs e)
        {
            ListForm.Display();
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
        }

        private void KeyboardHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            mainForm.DoShow();
        }

        void DoNotify(object sender, EventArgs e)
        {
            if (MessageBox.Show("Record the notification?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                EventBroker.Get().Notify(new WorkLogNotification());
        }

        void Exit(object sender, EventArgs e)
        {
            EventBroker.Get().Notify(new AppFinish());

            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Services.Get().Dispose();

            Application.Exit();
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (e is NetworkIssueDetected)
            {
                NetworkIssueDetected networkIssue = (NetworkIssueDetected)e;
                trayIcon.Icon = Properties.Resources.MainIconError;

                menuItemRetry.Visible = true;
                menuItemRetrySeparator.Visible = true;

                trayIcon.ShowBalloonTip(10000, "Network issue", string.Format("{0}. The network activity is stopped until the issue is resolved.", networkIssue.UserFriendlyText), ToolTipIcon.Error);
            }
            else if (e is InvalidJiraCredentialsDetected)
            {
                trayIcon.Icon = Properties.Resources.MainIconError;

                menuItemRetry.Visible = true;
                menuItemRetrySeparator.Visible = true;

                trayIcon.ShowBalloonTip(10000, "Invalid JIRA credentials", "Changed the password recently? The network activity is stopped until the issue is resolved.", ToolTipIcon.Error);
            }
            else if (e is NetworkIssueResolved)
            {
                trayIcon.Icon = Properties.Resources.MainIcon;

                menuItemRetry.Visible = false;
                menuItemRetrySeparator.Visible = false;
            }
        }

        private void DoRetry(object sender, EventArgs e)
        {
            menuItemRetry.Visible = false;
            menuItemRetrySeparator.Visible = false;

            Services.Get().Net.Resume();
        }
    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Services.Get().Initialize();

            Application.Run(new TimeLogApplicationContext());
        }
    }
}
