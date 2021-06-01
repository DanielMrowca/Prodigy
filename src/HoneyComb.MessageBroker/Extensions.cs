using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.MessageBroker
{
    public static class Extensions
    {
        public static bool? ToBool(this TriState triState)
        {
            switch (triState)
            {
                case TriState.True:
                    return true;
                case TriState.False:
                    return false;
                default:
                    return null;
            }
        }
    }
}
