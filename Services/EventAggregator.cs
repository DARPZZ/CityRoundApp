using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vamdrup_rundt.Services
{
    public class EventAggregator
    {
        private static readonly Lazy<EventAggregator> _instance = new Lazy<EventAggregator>(() => new EventAggregator());

        private readonly Dictionary<Type, Action<object>> _subscribers = new Dictionary<Type, Action<object>>();

        public static EventAggregator Instance => _instance.Value;

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            _subscribers[typeof(TEvent)] = e => handler((TEvent)e);
        }

        public void Publish<TEvent>(TEvent eventData)
        {
            if (_subscribers.TryGetValue(typeof(TEvent), out var handler))
            {
                handler(eventData);
            }
        }
    }
}
