using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksManagement_API.Interfaces;

namespace TasksManagement_API.Mailing
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> subscribers = new();

        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            if (!subscribers.ContainsKey(typeof(T)))
            {
                subscribers[typeof(T)] = new List<Action<IEvent>>();
            }
            subscribers[typeof(T)].Add(e => handler((T)e));
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            if (subscribers.ContainsKey(@event.GetType()))
            {
                foreach (var handler in subscribers[@event.GetType()])
                {
                    handler(@event);
                }
            }
        }
    }
}