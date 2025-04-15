using System;
using System.Collections.Generic;
using Singleton;

namespace EventBus
{
    /// <summary>
    /// Centralized event management system to handle event registrations and unregistrations
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        private readonly List<(Type eventType, object binding)> registeredBindings = new();
    
        /// <summary>
        /// Register an event handler
        /// </summary>
        public void RegisterEvent<T>(Action<T> handler) where T : IEvent
        {
            var binding = new EventBinding<T>(handler);
            EventBus<T>.Register(binding);
            registeredBindings.Add((typeof(T), binding));
        }
    
        /// <summary>
        /// Register an event handler with no parameters
        /// </summary>
        public void RegisterEvent<T>(Action handler) where T : IEvent
        {
            var binding = new EventBinding<T>(handler);
            EventBus<T>.Register(binding);
            registeredBindings.Add((typeof(T), binding));
        }
    
        /// <summary>
        /// Raise an event
        /// </summary>
        public void Raise<T>(T eventData) where T : IEvent
        {
            EventBus<T>.Raise(eventData);
        }
    
        /// <summary>
        /// Unregisters all events when this component is destroyed
        /// </summary>
        private void OnDestroy()
        {
            foreach (var reg in registeredBindings)
            {
                var methodInfo = typeof(EventBus<>).MakeGenericType(reg.eventType).GetMethod("UnRegister");
                if (methodInfo != null) methodInfo.Invoke(null, new[] { reg.binding });
            }
        }
    }
} 