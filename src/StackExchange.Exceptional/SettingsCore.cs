#if COREFX
using Microsoft.Extensions.Configuration;

namespace StackExchange.Exceptional
{
    /// <summary>
    /// The Settings element for Exceptional's configuration
    /// </summary>
    public partial class Settings
    {
        private static readonly IConfigurationRoot Config = new ConfigurationBuilder().AddJsonFile("exceptional.json").Build();
        private static readonly Settings _settings = new Settings();
        /// <summary>
        /// Current instance of the settings element
        /// </summary>
        public static Settings Current = _settings;
        
        /// <summary>
        /// Application name to log with
        /// </summary>
        public string ApplicationName => Config["applicationName"];

        /// <summary>
        /// The Regex pattern of data keys to include. For example, "Redis.*" would include all keys that start with Redis
        /// </summary>
        public string DataIncludePattern => Config["dataIncludePattern"];
    }
}
#endif