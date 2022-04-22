
namespace TimeLog.Models.Events
{
    class TicketSetEvent: TLEvent
    {
        public string TicketId { get; set; }

        public TicketSetEvent(string ticketId)
        {
            TicketId = ticketId;
        }
    }
}
