using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.CQRS.Queries
{
    public sealed class QueryHandlerAttribute : Attribute
    {
        /// <summary>
        ///     Disable or enable of auto registration to DI container
        /// </summary>
        public bool AutoRegister { get; private set; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="autoRegister">Disable or enable of auto registration to DI container</param>
        public QueryHandlerAttribute(bool autoRegister)
        {
            AutoRegister = autoRegister;
        }
    }
}
