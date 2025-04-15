using System.Collections.Generic;
using UnityEngine;

//Event bus class for generic type T
namespace EventBus
{
    public static class EventBus<T> where T : IEvent
    {
        static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

        //Register an event binding
        public static void Register(EventBinding<T> binding) => bindings.Add(binding);

        //Unregister an event binding
        public static void UnRegister(EventBinding<T> binding) => bindings.Remove(binding);
    
        //Raise an event check if the event is registered and invoke the event handler
        public static void Raise(T @event)
        {
            var snapshot = new HashSet<IEventBinding<T>>(bindings);

            foreach (var binding in snapshot)
            {
                if (bindings.Contains(binding))
                {
                    binding.OnEvent.Invoke(@event);
                    binding.OnEventNoArgs.Invoke();
                }
            }
        }
    }
}