using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.CQRS.Commands
{
    public interface ICommand
    {
    }

    public interface ICommand<TResult>
    {
    }
}
