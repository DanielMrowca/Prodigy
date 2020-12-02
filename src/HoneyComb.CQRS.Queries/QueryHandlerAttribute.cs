using System;

namespace HoneyComb.CQRS.Queries
{
    /// <summary>
    ///     Query handler attribute
    /// </summary>
    public sealed class QueryHandlerAttribute : Attribute
    {
        /// <summary>
        ///     Disable or enable of auto registration to DI container.
        ///     When using QueryHandler decorator we should set "AutoRegister" to false
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
