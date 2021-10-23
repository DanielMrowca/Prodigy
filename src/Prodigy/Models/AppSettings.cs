using System.Reflection;

namespace Prodigy.Models
{
    public class AppSettings
    {
        public string Name { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
        public string Title { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
        public string Subtitle { get; set; }
        public string Service { get; set; }
        public string Instance { get; set; }
        public string VersionNumber { get; set; } = Assembly.GetEntryAssembly().GetName().Version.ToString();
        public string Version { get; set; } = Assembly.GetEntryAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
        public bool DisplayBanner { get; set; } = true;
        public bool DisplayVersion { get; set; } = true;
    }
}
