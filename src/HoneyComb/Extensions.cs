using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddHoneyComb(this IServiceCollection services)
        {
            var builder = HoneyCombBuilder.Create(services);
            return builder;
        }
    }
}
