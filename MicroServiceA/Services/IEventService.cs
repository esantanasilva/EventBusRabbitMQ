using EventBus.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceActions
{
    public interface IEventService
    {
        Task PublishThroughEventBusAsync(Event @event);
    }
}
