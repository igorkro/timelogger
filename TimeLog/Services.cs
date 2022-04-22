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
