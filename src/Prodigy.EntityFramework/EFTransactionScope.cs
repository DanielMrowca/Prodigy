using System.Threading.Tasks;
using System.Transactions;
using Prodigy.Repositories;

namespace Prodigy.EntityFramework
{
    internal class EFTransactionScope : ITransactionScope
    {
        public Task<ITransaction> StartTransactionAsync()
        {
            return Task.FromResult<ITransaction>(new EFTransaction(new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted},
                TransactionScopeAsyncFlowOption.Enabled)));
        }
    }
}
