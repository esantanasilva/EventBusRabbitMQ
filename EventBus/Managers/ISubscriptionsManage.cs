using EventBus.Interfaces;
using EventBus.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Managers
{
    public interface ISubscriptionsManage
    {
        void Clear();

        void AddSubscription<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;

        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
