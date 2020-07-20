using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.Logging.Settings
{
    public class FileSettings
    {
        public bool IsEnabled { get; set; } = true;
        public string Path { get; set; } = "logs/log-{Date}.txt";
    }
}
