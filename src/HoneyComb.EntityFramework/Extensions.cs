using System;
using System.Collections.Generic;
using System.Text;
using HoneyComb.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HoneyComb.EntityFramework
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddEFTransactions(this IHoneyCombBuilder builder)
        {
            builder.Services.AddScoped<ITransactionScope, EFTransactionScope>();

            return builder;
        }
    }
}
