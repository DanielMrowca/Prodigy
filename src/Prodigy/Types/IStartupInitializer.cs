using System;

namespace Prodigy.Types
{
    public interface IStartupInitializer : IInitializer
    {
        void AddInitializer(IInitializer initializer);
        void RemoveInitializer(Type initializer);
    }
}
