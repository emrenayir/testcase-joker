using System;

//Event binding interface
public interface IEventBinding<T>
{
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
}

//Event binding class for event bus with generic type T
public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    private Action<T> onEvent = _ => { };
    private Action onEventNoArgs = () => { };

    //Get and set the event handler
    Action<T> IEventBinding<T>.OnEvent
    {
        get => onEvent;
        set => onEvent = value;
    }

    //Get and set the event handler for no arguments
    Action IEventBinding<T>.OnEventNoArgs
    {
        get => onEventNoArgs;
        set => onEventNoArgs = value;
    }

    //Constructor for event binding with generic type T
    public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;

    //Constructor for event binding with no arguments
    public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

    //Add an event handler for no arguments
    public void Add(Action onEvent) => onEventNoArgs += onEvent;

    //Remove an event handler for no arguments
    public void Remove(Action onEvent) => onEventNoArgs -= onEvent;
    
    //Add an event handler for generic type T
    public void Add(Action<T> onEvent) => this.onEvent += onEvent;

    //Remove an event handler for generic type T
    public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
}