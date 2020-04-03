using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.CQRS.Events
{
    public interface IEventDispatcher
    {
        Task SendAsync<T>(T @event) where T : class, IEvent;
    }
}
