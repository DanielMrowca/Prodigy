using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prodigy.Types
{
    public class StartupInitializer : IStartupInitializer
    {
        private readonly List<IInitializer> _initializers;

        public StartupInitializer(IEnumerable<IInitializer> initializers)
        {
            _initializers = new List<IInitializer>(initializers);
        }

        public void AddInitializer(IInitializer initializer)
        {
            if (initializer is null || _initializers.Contains(initializer))
                return;

            _initializers.Add(initializer);
        }

        public async Task InitializeAsync()
        {
            foreach (var initializer in _initializers)
            {
                await initializer.InitializeAsync();
            }
        }

        public void RemoveInitializer(Type initializer)
        {
            var toRemove = _initializers.SingleOrDefault(x => x.GetType() == initializer);
            if (toRemove != null)
                _initializers.Remove(toRemove);
        }
    }
}
