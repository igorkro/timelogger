using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLog.Models
{
    class Config
    {
        public string LastLogEntryId { get; set; }
        public string LastFlushedLogEntryId { get; set; }
        public string CommandShortcut { get; set; }
        public string MiscTicket { get; set; }
        public string JiraURL { get; set; }

        public bool IsAccessTokenAuth { get; set; }
        public bool ShowTotalLogged { get; set; }

        public Config()
        {
            CommandShortcut = "Ctrl+Space";
        }
    }
}
