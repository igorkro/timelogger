using System.Collections.Generic;

namespace TimeLog.Models
{
    class TicketCache
    {
        object lockObject = new object();
        private Dictionary<string, string> ticketInfo = new Dictionary<string, string>();

        Deque<string> lastReported = new Deque<string>();

        public Deque<string> LastReported
        {
            get
            {
                return lastReported;
            }
        }

        public TicketCache()
        {
        }

        public bool Contains(string ticket)
        {
            lock (lockObject)
            {
                return ticketInfo.ContainsKey(ticket);
            }
        }

        public void Add(string ticketId, string summary)
        {
            lock (lockObject)
            {
                ticketInfo[ticketId] = summary;
            }
        }

        public string Get(string ticketId)
        {
            lock (lockObject)
            {
                if (ticketInfo.ContainsKey(ticketId))
                    return ticketInfo[ticketId];
                return null;
            }
        }

        public void PushLastReported(string ticketId)
        {
            int pos;
            while ((pos = lastReported.IndexOf(ticketId)) != -1)
            {
                lastReported.RemoveAt(pos);
            }

            lastReported.AddFront(ticketId);
            while (lastReported.Count > 10)
                lastReported.RemoveBack();
        }
    }
}
 