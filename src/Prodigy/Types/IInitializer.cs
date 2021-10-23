using System.Threading.Tasks;

namespace Prodigy.Types
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}
