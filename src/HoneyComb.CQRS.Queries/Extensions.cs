using HoneyComb.Attributes;
using HoneyComb.CQRS.Queries.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace HoneyComb.CQRS.Queries
{
    public static class Extensions
    {
        /// <summary>
        ///     Register all Query Handlers defined in specified assemblies. 
        /// </summary>
        /// <param name="builder">HoneComb builder</param>
        /// <param name="assemblies">Assemblies context. If assemblies are null then getting all assemblies from the execution context</param>
        /// <param name="queryHandlersFactoryScope">Additional registrations only in this query handlers scope. Dedicated for decorators registration</param>
        /// <returns><see cref="IHoneyCombBuilder"/></returns>
        public static IHoneyCombBuilder AddQueryHandlers(
            this IHoneyCombBuilder builder,
            Assembly[] assemblies = null,
            Action<IServiceCollection> queryHandlersFactoryScope = null)
        {
            assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            var services = new ServiceCollection();

            services.Scan(s =>
                s.FromAssemblies(assemblies)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IQueryHandler<,>));
                    c.WithoutAttribute<AutoRegisterAttribute>();
                })
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IQueryHandler<,>));
                    c.WithAttribute<AutoRegisterAttribute>(x => x.AutoRegister);
                })
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            queryHandlersFactoryScope?.Invoke(services);
            builder.AddRange(services);

            return builder;
        }

        public static IHoneyCombBuilder AddQueryDispatcher(this IHoneyCombBuilder builder)
        {
            if (builder.Services.Any(x => x.ServiceType == typeof(IQueryDispatcher)))
                return builder;

            builder.Services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
            return builder;
        }
    }
}
