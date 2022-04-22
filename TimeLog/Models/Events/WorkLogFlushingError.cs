using System;

namespace TimeLog.Models.Events
{
    class WorkLogFlushingError: TLEvent
    {
        public string TicketId { get; private set; }
        public Int64 InternalWorklogId { get; private set; }
        public int StatusCode { get; private set; }

        public WorkLogFlushingError(string ticketId, Int64 internalWorkLogId, int statusCode)
        {
            TicketId = ticketId;
            InternalWorklogId = internalWorkLogId;
            StatusCode = statusCode;
        }
    }
}
