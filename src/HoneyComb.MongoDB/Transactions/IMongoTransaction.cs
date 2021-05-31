using HoneyComb.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB.Transactions
{
    public interface IMongoTransaction : ITransaction
    {
        void AddTransactionCommand(Func<Task> mongoCommand);
    }
}
