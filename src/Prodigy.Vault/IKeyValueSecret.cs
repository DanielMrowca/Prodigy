using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prodigy.Vault
{
    public interface IKeyValueSecret
    {
        Task<T> GetAsync<T>(string path);
        Task<IDictionary<string, object>> GetAsync(string path);
    }
}
