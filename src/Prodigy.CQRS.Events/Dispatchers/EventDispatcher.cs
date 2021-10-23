﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Prodigy.CQRS.Events.Dispatchers
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventDispatcher(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task SendAsync<T>(T @event) where T : class, IEvent
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<T>>();
                await handler.HandleAsync(@event);
            }
        }
    }
}
