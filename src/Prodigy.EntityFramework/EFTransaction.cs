using System;
using System.Threading.Tasks;
using System.Transactions;
using Prodigy.Repositories;

namespace Prodigy.EntityFramework
{
    internal class EFTransaction : ITransaction
    {
        private readonly TransactionScope _transactionScope;

        public EFTransaction(TransactionScope transactionScope)
        {
            _transactionScope = transactionScope;
        }

        public bool IsInTransaction => _transactionScope != null;

        public Task AbortTransactionAsync()
        {
            return Task.CompletedTask;
        }

        public Task<bool> CommitTransactionAsync()
        {
            _transactionScope.Complete();
            return Task.FromResult(true);
        }

        public void Dispose()
        {
            _transactionScope.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
