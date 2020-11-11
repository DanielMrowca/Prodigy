using System;
using System.Collections.Concurrent;

namespace HoneyComb.MessageBroker.RabbitMQ.Conventions
{
    public class ConventionProvider : IConventionProvider
    {
        private readonly IConventionBuilder _builder;
        private readonly ConcurrentDictionary<Type, IConvention> _conventions;

        public ConventionProvider(IConventionBuilder builder)
        {
            _builder = builder;
            _conventions = new ConcurrentDictionary<Type, IConvention>();
        }

        public IConvention Get<T>()
        {
            return Get(typeof(T));
        }

        public IConvention Get(Type type)
        {
            if (_conventions.TryGetValue(type, out var convention))
                return convention;

            convention = new Convention(type,
                _builder.GetRoutingKey(type),
                _builder.GetExchange(type),
                _builder.GetQueue(type),
                _builder.GetQueuePrefix(type),
                _builder.GetMultiThread(type),
                _builder.GetAutoAck(type),
                _builder.GetAckOnError(type));
            _conventions.TryAdd(type, convention);
            return convention;
        }

    }
}
