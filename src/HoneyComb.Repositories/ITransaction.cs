using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.Repositories
{
    public interface ITransaction : IDisposable
    {
        bool IsInTransaction { get; }
        Task<bool> CommitTransactionAsync();
        Task AbortTransactionAsync();
    }
}
