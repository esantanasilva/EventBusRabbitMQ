using System;
using RabbitMQ.Client;

namespace EventBus.RMQConnection
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
