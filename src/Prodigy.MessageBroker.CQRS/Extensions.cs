﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.CQRS.Commands;
using Prodigy.CQRS.Events;

namespace Prodigy.MessageBroker.CQRS
{
    public static class Extensions
    {
        public static Task SendAsync<TCommand>(this IBusPublisher busPublisher, TCommand command)
            where TCommand : class, ICommand
            => busPublisher.PublishAsync(command);

        public static Task PublishAsync<TEvent>(this IBusPublisher busPublisher, TEvent @event)
            where TEvent : class, IEvent
            => busPublisher.PublishAsync(@event);

        public static IBusSubscriber SubscribeCommand<T>(this IBusSubscriber busSubscriber) where T : class, ICommand
        {
            return busSubscriber.Subscribe<T>((sp, command, ctx) =>
            {
                var commandHandler = sp.GetRequiredService<ICommandHandler<T>>();
                return commandHandler.HandleAsync(command);
            });
        }

        public static IBusSubscriber SubscribeEvent<T>(this IBusSubscriber busSubscriber) where T : class, IEvent
        {
            return busSubscriber.Subscribe<T>((sp, @event, ctx) =>
            {
                var eventDispatcher = sp.GetRequiredService<IEventDispatcher>();
                return eventDispatcher.SendAsync(@event);
            });
        }

    }
}
