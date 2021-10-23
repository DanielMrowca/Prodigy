using System.Threading.Tasks;
using Prodigy.CQRS.Commands;

namespace WebApi.Commands.Handlers
{
    public class EmptyCommandHandler : ICommandHandler< EmptyCommand>
    {
        public Task HandleAsync(EmptyCommand cmd)
        {
            return Task.CompletedTask;
        }
    }
}
