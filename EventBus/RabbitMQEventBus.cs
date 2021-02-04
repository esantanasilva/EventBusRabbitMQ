using Autofac;
using EventBus.Interfaces;
using EventBus.Managers;
using EventBus.Models;
using EventBus.RMQConnection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private const string AutofacScopeName = "Autofac";
        private const string ExchangeName = "HelloWorld";

        private readonly ILifetimeScope _autofac;
        private readonly IRabbitMqConnection _connection;
        private readonly ISubscriptionsManage _subscriptionsManage;
        private readonly ILogger _logger;

        private IModel _consumerChannel;
        private readonly string _queueName;

        public RabbitMQEventBus(IRabbitMqConnection connection,
            ILifetimeScope autofac, ISubscriptionsManage subsManager, ILogger logger, string queueName = null)
        {
            _connection =
                connection ?? throw new ArgumentNullException(nameof(connection));

            _autofac = autofac
             ?? throw new ArgumentNullException(nameof(autofac));

            _logger = logger
          ?? throw new ArgumentNullException(nameof(logger));

            _subscriptionsManage = subsManager ?? new SubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subscriptionsManage.Clear();
        }

        public void Publish(Event @event)
        {
            if (!_connection.IsConnected) _connection.TryConnect();

            using (var channel = _connection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                channel.ExchangeDeclare(ExchangeName, "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                Policies.WaitRetryPolicy(3).Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;

                    channel.BasicPublish(ExchangeName,
                        eventName,
                        true,
                        properties,
                        body);
                });
            }





        }

        private void OnSubscriptionManagerEventAdded(string eventName)
        {
            var containsKey = _subscriptionsManage.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_connection.IsConnected)
                    _connection.TryConnect();

                using (var channel = _connection.CreateModel())
                {
                    channel.QueueBind(_queueName,
                      ExchangeName,
                      eventName);
                }

            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName,
                "direct");

            channel.QueueDeclare(_queueName,
                true,
                false,
                false,
                null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                await ProcessEvent(eventName, message);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(_queueName,
                false,
                consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subscriptionsManage.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(AutofacScopeName))
                {
                    foreach (var subscription in _subscriptionsManage.GetHandlersForEvent(eventName))
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler != null)
                        {
                            var eventType = _subscriptionsManage.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle")
                                .Invoke(handler, new[] { integrationEvent });
                        }
                    }
                }
            }
        }

        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            OnSubscriptionManagerEventAdded(eventName);

            _logger.Information($"[SUBSCRIBING] to event {eventName} with {typeof(TEventHandler)}");

            Console.WriteLine($"[SUBSCRIBING] to event {eventName} with {typeof(TEventHandler)}");
            
            _subscriptionsManage.AddSubscription<TEvent, TEventHandler>();
        }
    }
}
