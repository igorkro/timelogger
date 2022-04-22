using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
