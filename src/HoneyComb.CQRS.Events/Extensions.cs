using HoneyComb.CQRS.Events.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace HoneyComb.CQRS.Events
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddEventHandlers(this IHoneyCombBuilder builder)
        {
            builder.Services.Scan(s => s
                .FromExecutingAssembly()
                .FromCallingAssembly()
                .FromApplicationDependencies()
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return builder;
        }

        public static IHoneyCombBuilder AddEventDispatcher(this IHoneyCombBuilder builder)
        {
            builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
            return builder;
        }
    }
}
