using System.Collections.Generic;

namespace Prodigy.Logging.Settings
{
    public class LoggerSettings
    {
        public string Level { get; set; }
        public bool UseCustomRequestLogging { get; set; } = true;
        public ConsoleSettings Console { get; set; }
        public FileSettings File { get; set; }
        public SeqSettings Seq { get; set; }
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public IDictionary<string, string> Override { get; set; } = new Dictionary<string, string>();

    }
}
