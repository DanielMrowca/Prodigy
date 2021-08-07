using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.CQRS.Commands
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
