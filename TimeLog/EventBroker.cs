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

using System;
using System.Collections.Generic;

namespace TimeLog
{
    public class TLEvent
    { }

    interface EventHandler
    {
        void OnTLEvent(TLEvent e, ref bool stopProcessing);
    }

    class EventBroker
    {
        Dictionary<Type, HashSet<EventHandler>> subscribers = new Dictionary<Type, HashSet<EventHandler>>();

        private static EventBroker _instance = new EventBroker();

        public static EventBroker Get()
        {
            return _instance;
        }

        private EventBroker()
        {
        }

        public void Subscribe(Type eventType, EventHandler handler)
        {
            if (!subscribers.ContainsKey(eventType))
                subscribers.Add(eventType, new HashSet<EventHandler>());

            subscribers[eventType].Add(handler);
        }

        public void Unsubscribe(Type eventType, EventHandler handler)
        {
            if (!subscribers.ContainsKey(eventType))
                return;

            subscribers[eventType].Remove(handler);
        }

        public void UnsubscribeAll(Type eventType)
        {
            if (subscribers.ContainsKey(eventType))
                subscribers.Remove(eventType);
        }

        public void UnsubscribeFromAll(EventHandler handler)
        {
            foreach (var kv in subscribers)
            {
                kv.Value.Remove(handler);
            }
        }

        public void Notify(TLEvent e)
        {
            if (!subscribers.ContainsKey(e.GetType()))
                return;

            Services.Get().SynchronizationContext.Post(delegate
            {
                bool stopProcessing = false;
                foreach (var handler in subscribers[e.GetType()])
                {
                    handler.OnTLEvent(e, ref stopProcessing);

                    if (stopProcessing)
                        return;
                }
            }, null);
        }
    }
}
