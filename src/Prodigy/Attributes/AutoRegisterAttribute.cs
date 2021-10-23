using System;

namespace Prodigy.Attributes
{
    /// <summary>
    ///     Attribute for disable or enable of auto registration to DI container
    /// </summary>
    public sealed class AutoRegisterAttribute : Attribute
    {
        /// <summary>
        ///     Disable or enable of auto registration to DI container.
        /// </summary>
        /// <remarks>
        ///     When using <i>CommandHandler</i>, <i>EventHandler</i> or <i>QueryHandler</i> decorator we should set "AutoRegister" to false
        /// </remarks>
        public bool AutoRegister { get; private set; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="autoRegister">Disable or enable of auto registration to DI container</param>
        public AutoRegisterAttribute(bool autoRegister)
        {
            AutoRegister = autoRegister;
        }
    }
}
