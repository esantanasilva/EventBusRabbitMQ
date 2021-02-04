using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Models
{
    public class SubscriptionInfo
    {
        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }

        public Type HandlerType { get; }
    }
}
