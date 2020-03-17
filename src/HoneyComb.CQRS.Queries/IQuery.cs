using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.CQRS.Queries
{
    public interface IQuery
    {
    }

    public interface IQuery<T> : IQuery
    {
    }
}
