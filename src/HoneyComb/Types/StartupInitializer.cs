using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.Types
{
    public class StartupInitializer : IStartupInitializer
    {
        private readonly IList<IInitializer> _initializers;

        public StartupInitializer()
        {
            _initializers = new List<IInitializer>();
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
    }
}
