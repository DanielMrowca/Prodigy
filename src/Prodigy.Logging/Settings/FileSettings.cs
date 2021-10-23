namespace Prodigy.Logging.Settings
{
    public class FileSettings
    {
        public bool IsEnabled { get; set; } = true;
        public string Path { get; set; } = "logs/log.txt";
    }
}
