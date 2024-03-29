﻿using System.Collections.Generic;
using System.Reflection;

namespace Prodigy.MessageBroker.RabbitMQ
{
    public class RabbitMqOptions
    {
        public string ConnectionName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
        public bool ConnectionRetryForever { get; set; } = true;
        public int ConnectionRetryCount { get; set; } = 10;
        public IEnumerable<string> HostNames { get; set; }
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int RequestedConnectionTimeout { get; set; } = 30000;
        public int SocketReadTimeout { get; set; } = 30000;
        public int SocketWriteTimeout { get; set; } = 30000;
        public ushort RequestedChannelMax { get; set; }
        public uint RequestedFrameMax { get; set; }
        public ushort RequestedHeartbeat { get; set; }
        public bool UseBackgroundThreadsForIO { get; set; }
        public string ConventionsCasing { get; set; }
        public int Retries { get; set; }
        public int RetryInterval { get; set; }
        public ContextOptions Context { get; set; }
        public ExchangeOptions Exchange { get; set; }
        public LoggerOptions Logger { get; set; }
        public MessageProcessorOptions MessageProcessor { get; set; }
        public SslOptions Ssl { get; set; }
        public QueueOptions Queue { get; set; }
        public QosOptions Qos { get; set; }
        public string SpanContextHeader { get; set; }
        public int MaxProducerChannels { get; set; }
        public bool AutoAck { get; set; }
        public bool RequeueOnError { get; set; }
        public bool MultiThread { get; set; }
        public int ConsumerDispatchConcurrency { get; set; } = 1;

        public class LoggerOptions
        {
            public bool Enabled { get; set; }
            public string Level { get; set; }
        }

        public class ContextOptions
        {
            public bool Enabled { get; set; }
            public string Header { get; set; }
        }

        public class ExchangeOptions
        {
            public bool Declare { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public bool Durable { get; set; }
            public bool AutoDelete { get; set; }
        }

        public class QueueOptions
        {
            public bool Declare { get; set; }
            public bool Durable { get; set; }
            public bool Exclusive { get; set; }
            public bool AutoDelete { get; set; }
        }

        public class MessageProcessorOptions
        {
            public bool Enabled { get; set; }
            public string Type { get; set; }
            public int MessageExpirySeconds { get; set; }
        }

        public class SslOptions
        {
            public bool Enabled { get; set; }
            public string ServerName { get; set; }
            public string CertificatePath { get; set; }
        }

        public class QosOptions
        {
            public uint PrefetchSize { get; set; }
            public ushort PrefetchCount { get; set; }
            public bool Global { get; set; }
        }

       
    }
}
