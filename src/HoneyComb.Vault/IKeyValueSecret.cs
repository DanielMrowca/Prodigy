using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.Vault
{
    public interface IKeyValueSecret
    {
        Task<T> GetAsync<T>(string path);
        Task<IDictionary<string, object>> GetAsync(string path);
    }
}
