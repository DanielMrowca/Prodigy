using HoneyComb.Attributes;
using HoneyComb.CQRS.Queries.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HoneyComb.CQRS.Queries
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddQueryHandlers(this IHoneyCombBuilder builder)
        {
            builder.Services.Scan(s =>
                s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
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

            return builder;
        }

        public static IHoneyCombBuilder AddQueryDispatcher(this IHoneyCombBuilder builder)
        {
            builder.Services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
            return builder;
        }
    }
}
