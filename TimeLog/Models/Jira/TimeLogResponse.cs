using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Jira
{
    class TimeLogResponse
    {
        public string created { get; set; }
        public string updated { get; set; }
        public string started { get; set; }
        public string timeSpent { get; set; }
        public long timeSpentSeconds { get; set; }
        public string id { get; set; }
        public string issueId { get; set; }
    }
}
