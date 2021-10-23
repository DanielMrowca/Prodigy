using System.Threading.Tasks;

namespace Prodigy.Repositories
{
    public interface ITransactionScope
    {
        Task<ITransaction> StartTransactionAsync();
    }
}
