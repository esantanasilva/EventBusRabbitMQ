using EventBus.Interfaces;
using MicroServiceA.ServiceEvents.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceA.ServiceEvents.Handlers
{
    public class HelloWorldEventHandler : IEventHandler<HelloWorldEvent>
    {
        private readonly ILogger _logger;

        public HelloWorldEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(HelloWorldEvent @event)
        {
            _logger.Information(
                $"-> [RECEIVING] event: From {@event.ServiceId} - " +
                $" EventId: {@event.Id} " +
                $" CreationDate: {@event.CreationDateEpoch}" +
                $" Mensagem:  ({@event.Message})");

            Console.WriteLine($"-> [RECEIVING] event: From {@event.ServiceId} - " +
                            $" EventId: {@event.Id} " +
                            $" CreationDate: {@event.CreationDateEpoch}" +
                            $" Mensagem:  ({@event.Message})");

            return Task.CompletedTask;
        }
    }
}
