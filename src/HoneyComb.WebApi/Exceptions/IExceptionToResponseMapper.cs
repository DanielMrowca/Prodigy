using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.WebApi.Exceptions
{
    public interface IExceptionToResponseMapper
    {
        ExceptionResponse Map(Exception exception);
    }
}
