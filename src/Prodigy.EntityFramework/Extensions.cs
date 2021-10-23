using Microsoft.Extensions.DependencyInjection;
using Prodigy.Repositories;

namespace Prodigy.EntityFramework
{
    public static class Extensions
    {
        public static IProdigyBuilder AddEFTransactions(this IProdigyBuilder builder)
        {
            builder.Services.AddScoped<ITransactionScope, EFTransactionScope>();

            return builder;
        }
    }
}
