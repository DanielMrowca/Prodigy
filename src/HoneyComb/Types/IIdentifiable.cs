using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.Types
{
    public interface IIdentifiable<out TKey>
    {
        TKey Id { get; }
    }
}
