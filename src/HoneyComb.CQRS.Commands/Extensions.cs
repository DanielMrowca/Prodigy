using HoneyComb.CQRS.Commands.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HoneyComb.CQRS.Commands
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddCommandHandlers(this IHoneyCombBuilder builder)
        {
            IServiceCollection cs = new ServiceCollection();

            var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(i => Assembly.Load(i));
            cs.Scan(s =>
                s.FromAssemblies(assemblies.)
                    .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return builder;

        }

        public static IHoneyCombBuilder AddCommandDispatcher(this IHoneyCombBuilder builder)
        {
            builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            return builder;
        }
    }
}
