using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HoneyComb.Types
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}
