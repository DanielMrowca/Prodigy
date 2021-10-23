using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.Attributes;
using Prodigy.CQRS.Events.Dispatchers;

namespace Prodigy.CQRS.Events
{
    public static class Extensions
    {
        /// <summary>
        ///     Register all Event Handlers defined in specified assemblies. 
        /// </summary>
        /// <param name="builder">HoneComb builder</param>
        /// <param name="assemblies">Assemblies context. If assemblies are null then getting all assemblies from the execution context</param>
        /// <param name="eventHandlersFactoryScope">Additional registrations only in this event handlers scope. Dedicated for decorators registration.</param>
        /// <returns><see cref="IProdigyBuilder"/></returns>
        public static IProdigyBuilder AddEventHandlers(
            this IProdigyBuilder builder, 
            Assembly[] assemblies = null,
            Action<IServiceCollection> eventHandlersFactoryScope = null)
        {
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            IServiceCollection services = new ServiceCollection();

            services.Scan(s => s
                .FromAssemblies(assemblies)
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
                    c.WithAttribute<AutoRegisterAttribute>(x => x.AutoRegister);
                })
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            eventHandlersFactoryScope?.Invoke(services);
            builder.AddRange(services);

            return builder;
        }

        public static IProdigyBuilder AddEventDispatcher(this IProdigyBuilder builder)
        {
            if (builder.Services.Any(x => x.ServiceType == typeof(IEventDispatcher)))
                return builder;

            builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
            return builder;
        }
    }
}
