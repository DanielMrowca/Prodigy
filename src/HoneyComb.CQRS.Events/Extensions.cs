using HoneyComb.Attributes;
using HoneyComb.CQRS.Events.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HoneyComb.CQRS.Events
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddEventHandlers(this IHoneyCombBuilder builder)
        {
            builder.Services.Scan(s => s
                .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => 
                { 
                    c.AssignableTo(typeof(IEventHandler<>));
                    c.WithoutAttribute<AutoRegisterAttribute>();
                })
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IEventHandler<>));
                    c.WithAttribute<AutoRegisterAttribute>(x=>x.AutoRegister);
                })
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
