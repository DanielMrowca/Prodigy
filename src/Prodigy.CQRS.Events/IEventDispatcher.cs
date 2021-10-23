using System.Threading.Tasks;

namespace Prodigy.CQRS.Events
{
    public interface IEventDispatcher
    {
        Task SendAsync<T>(T @event) where T : class, IEvent;
    }
}
