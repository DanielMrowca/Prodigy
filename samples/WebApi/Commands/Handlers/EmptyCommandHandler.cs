using System.Threading.Tasks;
using HoneyComb.CQRS.Commands;

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
