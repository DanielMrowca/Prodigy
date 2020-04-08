using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HoneyComb.Models
{
    public class AppSettings
    {
        public string Name { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
        public string Title { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
        public string Subtitle { get; set; }
        public string Service { get; set; }
        public string Instance { get; set; }
        public string Version { get; set; } = Assembly.GetEntryAssembly().GetName().Version.ToString();
        public bool DisplayBanner { get; set; } = true;
        public bool DisplayVersion { get; set; } = true;
    }
}
