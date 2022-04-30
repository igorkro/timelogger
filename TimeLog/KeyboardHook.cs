// This code was taken from here: https://stackoverflow.com/a/27309185

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeLog
{
    public sealed class KeyboardHook : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }


        private class CommandShortcut
        {
            public ModifierKeys Modifiers { get; set; }
            public Keys Key { get; set; }

            public CommandShortcut()
            {

            }

            public CommandShortcut(ModifierKeys modifiers, Keys key)
            {
                this.Modifiers = modifiers;
                this.Key = key;
            }
        }

        private Window _window = new Window();
        private int _currentId;

        public KeyboardHook()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        public bool RegisterHotKeyFromString(string shortcut)
        {
            CommandShortcut commandShortcut = ParseShortcut(Services.Get().Config.CommandShortcut);
            if (commandShortcut == null)
            {
                return false;
            }

            try
            {
                RegisterHotKey(commandShortcut.Modifiers, commandShortcut.Key);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private CommandShortcut ParseShortcut(string commandShortcut)
        {
            string[] parts = commandShortcut.Split('+');
            List<Keys> keys = new List<Keys>();
            KeysConverter keysConverter = new KeysConverter();
            foreach (string part in parts)
            {
                try
                {
                    Keys key = (Keys)keysConverter.ConvertFromString(part);
                    keys.Add(key);
                }
                catch
                {
                    return null;
                }
            }

            CommandShortcut shortcut = new CommandShortcut();

            for (int i = 0; i < keys.Count; ++i)
            {
                if (i + 1 < keys.Count)
                {
                    if (keys[i] == Keys.Control)
                        shortcut.Modifiers = shortcut.Modifiers | ModifierKeys.Control;
                    else if (keys[i] == Keys.Shift)
                        shortcut.Modifiers = shortcut.Modifiers | ModifierKeys.Shift;
                    else if (keys[i] == Keys.Alt)
                        shortcut.Modifiers = shortcut.Modifiers | ModifierKeys.Alt;
                    else if (keys[i] == Keys.LWin || keys[i] == Keys.RWin)
                        shortcut.Modifiers = shortcut.Modifiers | ModifierKeys.Win;
                }
                else
                {
                    shortcut.Key = keys[i];
                }
            }
            return shortcut;
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public void UnregisterAll()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }
            _currentId = 0;
        }

        #region IDisposable Members

        public void Dispose()
        {
            UnregisterAll();

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier
        {
            get { return _modifier; }
        }

        public Keys Key
        {
            get { return _key; }
        }
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }
}
