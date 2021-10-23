using System.Threading.Tasks;

namespace Prodigy.CQRS.Commands
{
    public interface ICommandDispatcher
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;
        Task SendAsync(ICommand command);
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command);
        //Task<TResult> SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand<TResult>;
        
    }
}
