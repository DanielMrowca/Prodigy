using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoneyComb.MessageBroker.RabbitMQ.Publishers
{
    public class RabbitMqPublisher : IBusPublisher
    {
        private readonly IRabbitMqClient _client;
        private readonly IConventionProvider _conventionProvider;

        public RabbitMqPublisher(IRabbitMqClient client, IConventionProvider conventionProvider)
        {
            _client = client;
            _conventionProvider = conventionProvider;
        }

        public Task PublishAsync<T>(T message, string messageId = null, string correlationId = null, string spanContext = null,
            object messageContext = null, IDictionary<string, object> headers = null) where T : class
        {
            var convention = _conventionProvider.Get(message.GetType());
            _client.Send(message, convention, messageId, correlationId, spanContext, messageContext, headers);
            return Task.CompletedTask;
        }
    }
}
