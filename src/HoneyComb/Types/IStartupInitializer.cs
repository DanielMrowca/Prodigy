using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.Types
{
    public interface IStartupInitializer : IInitializer
    {
        void AddInitializer(IInitializer initializer);
        void RemoveInitializer(Type initializer);
    }
}
