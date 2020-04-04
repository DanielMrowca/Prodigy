using System;
using System.Collections.Generic;
using System.Text;

namespace HoneyComb.Logging.Settings
{
    public class LoggerSettings
    {
        public string Level { get; set; }
        public ConsoleSettings Console { get; set; }
        public FileSettings File { get; set; }
        public SeqSettings Seq { get; set; }
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

    }
}
