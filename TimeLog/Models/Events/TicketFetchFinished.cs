using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLog.Utils;

namespace TimeLog.Models.Events
{
    class TicketFetchFinished: TLEvent
    {
        public string TicketId { get; set; }

        public JiraRepository.TicketInfo TicketInfo { get; set; }

        public TicketFetchFinished(string ticketId, JiraRepository.TicketInfo info)
        {
            TicketId = ticketId;
            TicketInfo = info;
        }
    }
}
