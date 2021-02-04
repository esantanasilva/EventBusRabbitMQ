using EventBus.Interfaces;
using EventBus.Models;
using Serilog;
using Services.ServiceActions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceA.Services
{
    class EventService : IEventService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public EventService(IEventBus eventBus, ILogger logger)
        {
            _logger = logger;
            _eventBus = _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public Task PublishThroughEventBusAsync(Event @event)
        {
            try
            {
                _logger.Information(
                    $"--> [PULBISH] event: {@event.Id} - ({@event})");

                Console.WriteLine($"-> [PULBISH] event: From {@event.ServiceId} - " +
                    $" EventId: {@event.Id} " +
                    $" CreationDate: {@event.CreationDateEpoch}");

                _eventBus.Publish(@event);
            }
            catch (Exception e)
            {
                _logger.Error(
                    e, $"--> [PUBLISH ERROR] event: {@event.Id} - ({@event})");
            }

            return Task.CompletedTask;
        }

    }
}
