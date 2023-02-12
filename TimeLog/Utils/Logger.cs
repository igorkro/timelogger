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
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using TimeLog.Models.Events;

namespace TimeLog.Utils
{
    class Logger: EventHandler
    {
        struct LogEntry
        {
            public DateTime LogTime;
            public char EntryType;
            public string Text;

            public override string ToString()
            {
                return string.Format("{0} [{1}] {2}", LogTime.ToString("HH:mm:ss.ffff"), EntryType, Text);
            }
        }

        static Logger _instance = null;

        bool isRunning = false;
        string logFolder;
        int logFileLimit;
        long currentLogSize = 0;
        string exeName;

        StreamWriter streamWriter = null;
        Thread flushingThread = null;

        Queue<LogEntry> logEntries = new Queue<LogEntry>();
        Queue<LogEntry> tempEntries = new Queue<LogEntry>();

        ManualResetEventSlim mre = new ManualResetEventSlim(false);
        object lockObject = new object();

        public Logger(string exeName, string logFolder, int logFileLimit)
        {
            this.logFileLimit = logFileLimit;
            this.logFolder = logFolder;
            this.exeName = exeName;

            if (this.logFolder == null)
            {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IK", "TimeLogger");

                try
                {
                    if (!Directory.Exists(logPath))
                        Directory.CreateDirectory(logPath);

                    this.logFolder = logPath;
                }
                catch
                {
                    System.Windows.Forms.Application.Exit();
                    return;
                }
            }

            _instance = this;

            EventBroker.Get().Subscribe(typeof(AppFinish), this);
        }

        public static Logger Get()
        {
            return _instance;
        }

        public string getFileName()
        {
            DateTime currentTime = DateTime.Now;
            string dateTimeFormatted = currentTime.ToString("yyyyMMdd_HHmmss");
            string fileName = string.Format("{0}_{1}.log", exeName, dateTimeFormatted);
            return Path.Combine(logFolder, fileName);
        }

        private void checkAndStartStream()
        {
            if (currentLogSize >= logFileLimit)
            {
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter = null;
                currentLogSize = 0;
            }

            if (streamWriter == null)
            {
                streamWriter = new StreamWriter(getFileName());
            }
        }

        public void Log(char type, string text)
        {
            lock(lockObject)
            {
                logEntries.Enqueue(new LogEntry
                {
                    LogTime = DateTime.Now,
                    EntryType = type,
                    Text = text
                });

                if (logEntries.Count >= 5)
                    mre.Set();
            }
        }

        public void Dispose()
        {
            EventBroker.Get().UnsubscribeFromAll(this);
            Stop();
        }

        public void Stop()
        {
            if (!isRunning)
                return;
            isRunning = false;
            mre.Set();

            flushingThread.Join();
        }

        private void doFlushing()
        {
            lock (lockObject)
            {
                Queue<LogEntry> interm = logEntries;
                logEntries = tempEntries;
                tempEntries = interm;
            }

            if (tempEntries.Count > 0)
            {
                foreach (LogEntry entry in tempEntries)
                {
                    checkAndStartStream();

                    string entryStr = entry.ToString();
                    streamWriter.WriteLine(entryStr);
                    currentLogSize += entryStr.Length;
                }

                if (streamWriter != null)
                    streamWriter.Flush();

                tempEntries.Clear();
            }
        }
        
        private void flushingThreadFunc()
        {
            while (isRunning)
            {
                doFlushing();

                mre.Wait(3000);
                mre.Reset();
            }

            doFlushing();

            if (streamWriter != null)
            {
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public void Start()
        {
            if (isRunning)
                return;

            EventBroker.Get().Subscribe(typeof(AppFinish), this);

            isRunning = true;
            flushingThread = new Thread(flushingThreadFunc);
            flushingThread.Start();
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (e is AppFinish)
            {
                Stop();
            }
        }
    }
}
