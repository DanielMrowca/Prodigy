using Microsoft.Extensions.DependencyInjection;

namespace HoneyComb
{
    public interface IHoneyCombBuilder
    {
        IServiceCollection Services { get; }
    }
}
