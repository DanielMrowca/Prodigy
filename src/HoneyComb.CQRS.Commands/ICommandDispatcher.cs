using System.Threading.Tasks;

namespace HoneyComb.CQRS.Commands
{
    public interface ICommandDispatcher
    {
        Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command);
        //Task<TResult> SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand<TResult>;
        
    }
}
