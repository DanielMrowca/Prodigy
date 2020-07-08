using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.MessageBroker.RabbitMQ
{
    public interface IConnectionFactory
    {
        IConnection GetConnection();
    }
}
