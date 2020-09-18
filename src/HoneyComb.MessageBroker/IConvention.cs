using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker
{
    public interface IConvention
    {
        Type Type { get; }
        string RoutingKey { get; }
        string Exchange { get; }
        string Queue { get; }
    }
}
