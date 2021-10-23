using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.Attributes;
using Prodigy.CQRS.Commands.Dispatchers;

namespace Prodigy.CQRS.Commands
{
    public static class Extensions
    {
        /// <summary>
        ///     Register all Command Handlers defined in specified assemblies. 
        /// </summary>
        /// <param name="builder">HoneComb builder</param>
        /// <param name="assemblies">Assemblies context. If assemblies are null then getting all assemblies from the execution context</param>
        /// <param name="commandHandlerFactoryScope">Additional registrations only in this command handlers scope. Dedicated for decorators registration.</param>
        /// <returns><see cref="IProdigyBuilder"/></returns>
        public static IProdigyBuilder AddCommandHandlers(
            this IProdigyBuilder builder,
            Assembly[] assemblies = null,
            Action<IServiceCollection> commandHandlerFactoryScope = null)
        {
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();

            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.Scan(s =>
            s.FromAssemblies(assemblies)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(ICommandHandler<>));
                    c.WithoutAttribute<AutoRegisterAttribute>();
                })
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(ICommandHandler<>));
                    c.WithAttribute<AutoRegisterAttribute>(x => x.AutoRegister);
                })
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .AddClasses(c => 
                {
                    c.AssignableTo(typeof(ICommandHandler<,>));
                    c.WithoutAttribute<AutoRegisterAttribute>();
                })
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .AddClasses(c =>
                 {
                     c.AssignableTo(typeof(ICommandHandler<,>));
                     c.WithAttribute<AutoRegisterAttribute>(x => x.AutoRegister);
                 })
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            commandHandlerFactoryScope?.Invoke(serviceCollection);
            builder.AddRange(serviceCollection);

            return builder;
        }

        public static IProdigyBuilder AddCommandDispatcher(this IProdigyBuilder builder)
        {
            if (builder.Services.Any(x => x.ServiceType == typeof(ICommandDispatcher)))
                return builder;

            builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            return builder;
        }
    }
}
