using System.Threading.Tasks;

namespace HoneyComb.CQRS.Commands
{
    public interface ICommandDispatcher
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;
        Task<TResult> SendAsync<TCommand, TResult>(TCommand command) where TCommand : class, ICommand<TResult>;
    }
}
