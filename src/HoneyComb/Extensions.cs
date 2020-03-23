using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoneyComb
{
    public static class Extensions
    {
        public static IHoneyCombBuilder AddHoneyComb(this IServiceCollection services)
        {
            var builder = HoneyCombBuilder.Create(services);
            return builder;
        }

        public static T GetSettings<T>(this IHoneyCombBuilder builder, string sectionName)
            where T : new()
        {
            using (var serviceProvicer = builder.Services.BuildServiceProvider())
            {
                var config = serviceProvicer.GetRequiredService<IConfiguration>();
                return config.GetSettings<T>(sectionName);
            }
        }

        public static T GetSettings<T>(this IConfiguration config, string sectionName)
            where T : new()
        {
            var model = new T();
            config.GetSection(sectionName).Bind(model);
            return model;
        }
    }
}
