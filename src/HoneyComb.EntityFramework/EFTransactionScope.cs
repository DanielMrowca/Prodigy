using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using HoneyComb.Repositories;

namespace HoneyComb.EntityFramework
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
