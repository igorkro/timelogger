namespace TimeLog.Models.Events
{
    class SequenceGridTicketClicked: TLEvent
    {
        public string TicketId { get; private set; }

        public SequenceGridTicketClicked(string ticket)
        {
            TicketId = ticket;
        }
    }
}
