using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Interfaces;
using EventBus.Managers;
using EventBus.RMQConnection;
using MicroServiceA.ServiceEvents.Events;
using MicroServiceA.ServiceEvents.Handlers;
using MicroServiceA.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Services.ServiceActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroServiceA
{
    public class Program
    {
        public static readonly string AppName = Assembly.GetCallingAssembly().GetName().Name;
        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            await Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync();


        }
        private static void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            // Add any Autofac modules or registrations.
            // This is called AFTER ConfigureServices so things you
            // register here OVERRIDE things registered in ConfigureServices.
            //
            // You must have the call to `UseServiceProviderFactory(new AutofacServiceProviderFactory())`
            // when building the host or this won't be called.
            containerBuilder.Register<ILogger>((c, p) => new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://127.0.0.1:9200")))
                .CreateLogger()).SingleInstance();
            containerBuilder.RegisterType<SubscriptionsManager>().As<ISubscriptionsManage>();
            containerBuilder.RegisterType<HelloWorldEventHandler>();
            containerBuilder.RegisterType<EventService>()
                .As<IEventService>();

            // var rabbitHost = new Uri("amqp://guest:guest@rabbitmq:5672 ");
            // var rabbitHostdev = new Uri("amqp://guest:guest@127.0.0.1");
            ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";
            Console.WriteLine("registering RabbitMQ connection");
            containerBuilder.Register(_ => new RabbitMqConnection(factory)).As<IRabbitMqConnection>();

            containerBuilder.Register(componentContext =>
            {
                var connection = componentContext.Resolve<IRabbitMqConnection>();
                var subsManage = componentContext.Resolve<ISubscriptionsManage>();
                var lifeTimeScope = componentContext.Resolve<ILifetimeScope>();
                var logger = componentContext.Resolve<ILogger>();

                var eventBus = new RabbitMQEventBus(connection, lifeTimeScope, subsManage, logger, AppName);
                ConfigureSubscribes(eventBus);

                return eventBus;
            }).As<IEventBus>();
        }

        private static void ConfigureSubscribes(IEventBus eventBus) =>
            eventBus.Subscribe<HelloWorldEvent, HelloWorldEventHandler>();

        private static void ConfigureServices(IServiceCollection services) => services.AddHostedService<Worker>();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
