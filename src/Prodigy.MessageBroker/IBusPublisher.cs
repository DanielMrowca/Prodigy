﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prodigy.MessageBroker
{
    public interface IBusPublisher
    {
        Task PublishAsync<T>(T message, string messageId = null, string correlationId = null, string spanContext = null,
            object messageContext = null, IDictionary<string, object> headers = null, IConvention convention = null) where T : class;
    }
}
