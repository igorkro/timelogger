using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Events
{
    class WorkLogFlushed: TLEvent
    {
        public Int64 Id { get; private set; }
        public string TicketId { get; private set; }
        public string JiraWorklogId { get; private set; }

        public WorkLogFlushed(Int64 id, string ticketId, string jiraWorklogId)
        {
            Id = id;
            TicketId = ticketId;
            JiraWorklogId = jiraWorklogId;
        }
    }
}
