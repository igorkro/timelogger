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

using System.IO;
using System.Threading;
using TimeLog.Models;
using TimeLog.Utils;

namespace TimeLog
{
    class Services
    {
        private static Services instance = new Services();

        public DbService Db { get; private set; }
        public Config Config { get; private set; }

        public TimeLogRepository TimeLogRepo { get; private set; }
        public JiraRepository JiraRepo { get; private set; }
        public CredentialRepository Creds { get; private set; }
        public KeyboardHook KeyHook { get; set; }
        public NetworkService Net { get; set; }

        public TicketCache Tickets { get; set; }

        private ConfigRepository configRepository { get; set; }

        private bool _initialized = false;

        public SynchronizationContext SynchronizationContext { get; set; }

        public static Services Get()
        {
            return instance;
        }

        public Services()
        {
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            string exeName = Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);

            new Logger(exeName, null, 1000000).Start();

            Db = new DbService();
            Db.Initialize();
            Config = new Config();
            configRepository = new ConfigRepository();
            configRepository.Load(Config);

            Net = new NetworkService();
            Net.Start();

            Tickets = new TicketCache();

            TimeLogRepo = new TimeLogRepository();
            TimeLogRepo.Initialize();

            JiraRepo = new JiraRepository();

            Creds = new CredentialRepository();

            KeyHook = new KeyboardHook();
        }

        public void Dispose()
        {
            Net.Dispose();
            Logger.Get().Dispose();
        }

        public void SaveConfig()
        {
            configRepository.Save(Config);
        }
    }
}
