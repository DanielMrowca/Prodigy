using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HoneyComb.CQRS.Commands.Dispatchers
{
    internal sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public CommandDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public async Task SendAsync<T>(T command) where T : class, ICommand
        {
            using var scope = _serviceFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }

        public async Task SendAsync(ICommand command)
        {
            using var scope = _serviceFactory.CreateScope();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
            await (Task)handler.GetType().GetMethod("HandleAsync")?.Invoke(handler, new[] { command });
        }

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command)
        {
            using var scope = _serviceFactory.CreateScope();

            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
            return await (Task<TResult>)handler.GetType().GetMethod("HandleAsync")?.Invoke(handler, new[] { command });


            //var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            //dynamic handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            //return await handler.HandleAsync(command);
        }

       
    }
}
