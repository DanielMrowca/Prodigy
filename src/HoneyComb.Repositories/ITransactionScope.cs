using System.Threading.Tasks;

namespace HoneyComb.Repositories
{
    public interface ITransactionScope
    {
        Task<ITransaction> StartTransactionAsync();
    }
}
