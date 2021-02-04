﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog;

namespace EventBus.RMQConnection
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly object _syncRoot = new object();
        private IConnection _connection;
        private bool _disposed;

        public RabbitMqConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No connection was found");

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            Console.WriteLine("Trying to connect with RabbitMQ");
            lock (_syncRoot)
            {
                Policies.WaitRetryPolicy(3)
                    .Execute(() =>
                    {
                        _connection = _connectionFactory
                            .CreateConnection();
                    });

                return IsConnected;
            }
        }
    }
}
