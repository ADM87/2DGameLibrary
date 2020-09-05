using System.Collections.Generic;
using UnityEngine;

namespace Abduction.Events
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventType"></typeparam>
    /// <typeparam name="TEventData"></typeparam>
    public class EventEmitter<TEventType, TEventData>
    {
        public delegate void EventHandler(TEventData data);

        private Dictionary<TEventType, EventHandler> handlers;

        public EventEmitter()
        {
            handlers = new Dictionary<TEventType, EventHandler>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void Subscribe(TEventType type, EventHandler handler)
        {
            if (handlers.TryGetValue(type, out EventHandler eventHandlers))
                handlers[type] = eventHandlers + handler;
            else
                handlers[type] = handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void Unsubscribe(TEventType type, EventHandler handler)
        {
            if (handlers.TryGetValue(type, out EventHandler eventHandlers))
                handlers[type] = eventHandlers - handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Dispatch(TEventType type, TEventData data)
        {
            if (handlers.TryGetValue(type, out EventHandler eventHandlers))
                eventHandlers?.Invoke(data);
        }
    }
}
