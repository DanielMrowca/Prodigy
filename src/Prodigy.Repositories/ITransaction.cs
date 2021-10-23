using System;
using System.Threading.Tasks;

namespace Prodigy.Repositories
{
    public interface ITransaction : IDisposable
    {
        bool IsInTransaction { get; }
        Task<bool> CommitTransactionAsync();
        Task AbortTransactionAsync();
    }
}
