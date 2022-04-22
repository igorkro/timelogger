using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Jira
{
    class TicketLogRequest
    {
        public long timeSpentSeconds { get; set; }
        public string comment { get; set; }
        public string started { get; set; }
    }
}
