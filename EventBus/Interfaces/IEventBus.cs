using EventBus.Models;

namespace EventBus.Interfaces
{
    public interface IEventBus
    {
        void Publish(Event @event);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;
        

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler;
        
    }
}
