using Microsoft.Extensions.DependencyInjection;

namespace HoneyComb
{
    public class HoneyCombBuilder : IHoneyCombBuilder
    {
        private readonly IServiceCollection _services;
        public IServiceCollection Services => _services;

        private HoneyCombBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public static IHoneyCombBuilder Create(IServiceCollection services)
            => new HoneyCombBuilder(services);
    }
}
