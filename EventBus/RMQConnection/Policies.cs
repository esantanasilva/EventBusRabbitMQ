using System;
using System.Net.Sockets;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace EventBus.RMQConnection
{
    public static class Policies
    {
        public static RetryPolicy WaitRetryPolicy(int attempts) =>
            Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(attempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        Log.Warning(ex,
                            $"RabbitMQ client could not respond after {time.TotalSeconds:n1} s ({ ex.Message})");
                    }
                );
    }
}
