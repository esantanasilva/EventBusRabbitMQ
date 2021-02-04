using MicroServiceA.ServiceEvents.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.ServiceActions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace MicroServiceA
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEventService _eventService;

        public Worker(ILogger<Worker> logger, IEventService eventService)
        {
            _logger = logger;
            _eventService = eventService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _eventService.PublishThroughEventBusAsync(new HelloWorldEvent
                {
                    ServiceId = Program.AppName,
                    Message = "Hello World"
                });

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
