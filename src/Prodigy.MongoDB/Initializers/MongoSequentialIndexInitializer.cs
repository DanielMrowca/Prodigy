using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Prodigy.MongoDB.SequentialIndex;
using Prodigy.Types;

namespace Prodigy.MongoDB.Initializers
{
    public class MongoSequentialIndexInitializer : IInitializer
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MongoSequentialIndexInitializer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InitializeAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sequentialIndexProviders = scope.ServiceProvider.GetServices(typeof(ISequentialIndexProvider<,>));
                foreach (dynamic provider in sequentialIndexProviders)
                {
                    long index = await provider.GetLastIndexFromDb();
                    if (index == -1)
                        provider.SetCurrentIndex(0);
                    else
                        provider.SetCurrentIndex(index);
                }
            }

            
        }
    }
}
