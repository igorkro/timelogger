using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Events
{
    class PreviousTicketSelected: TLEvent
    {
        public string Ticket { get; set; }

        public PreviousTicketSelected(string ticket)
        {
            Ticket = ticket;
        }
    }
}
