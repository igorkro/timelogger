// Copyright (C) 2022  Igor Krushch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Email: dev@krushch.com

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
 