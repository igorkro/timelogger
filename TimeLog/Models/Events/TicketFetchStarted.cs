using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Events
{
    class TicketFetchStarted: TLEvent
    {
        public string TicketId { get; set; }

        public TicketFetchStarted(string ticketId)
        {
            TicketId = ticketId;
        }
    }
}
