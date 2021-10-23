using System.Threading.Tasks;

namespace Prodigy.CQRS.Commands
{
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        Task HandleAsync(TCommand cmd);
    }

    public interface ICommandHandler<in TCommand, TResult> where TCommand : class, ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand cmd);
    }
}
