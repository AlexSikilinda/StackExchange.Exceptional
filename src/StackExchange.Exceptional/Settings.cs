﻿#if !COREFX
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace StackExchange.Exceptional
{
    /// <summary>
    /// The Settings element for Exceptional's configuration
    /// </summary>
    public partial class Settings : ConfigurationSection
    {
        private static readonly Settings _settings = ConfigurationManager.GetSection("Exceptional") as Settings;
        /// <summary>
        /// Current instance of the settings element
        /// </summary>
        public static Settings Current => _settings ?? new Settings();

        /// <summary>
        /// Application name to log with
        /// </summary>
        [ConfigurationProperty("applicationName", IsRequired = true)]
        public string ApplicationName => this["applicationName"] as string;

        /// <summary>
        /// The Regex pattern of data keys to include. For example, "Redis.*" would include all keys that start with Redis
        /// </summary>
        [ConfigurationProperty("dataIncludePattern")]
        public string DataIncludePattern => this["dataIncludePattern"] as string;

        /// <summary>
        /// A collection of list types all with a Name attribute
        /// </summary>
        /// <typeparam name="T">The type of collection, inherited from SettingsCollectionElement</typeparam>
        public class SettingsCollection<T> : ConfigurationElementCollection where T : SettingsCollectionElement, new()
        {
            /// <summary>
            /// Accessor by key
            /// </summary>
            public new T this[string key] => BaseGet(key) as T;

            /// <summary>
            /// Accessor by index
            /// </summary>
            public T this[int index] => BaseGet(index) as T;

            /// <summary>
            /// Default constructor for this element
            /// </summary>
            protected override ConfigurationElement CreateNewElement()
            {
                return new T();
            }
            /// <summary>
            /// Default by-key fetch for this element
            /// </summary>
            protected override object GetElementKey(ConfigurationElement element)
            {
                return element.ToString();
            }
            /// <summary>
            /// Returns all the elements in this collection, type-cased out
            /// </summary>
            public List<T> All() => this.Cast<T>().ToList();
        }

        /// <summary>
        /// An element in a settings collection that has a Name property, a generic base for SettingsCollection collections
        /// </summary>
        public abstract class SettingsCollectionElement : ConfigurationElement
        {
            /// <summary>
            /// String representation for this entry, the Name
            /// </summary>
            public override string ToString() { return Name; }
            /// <summary>
            /// A unique name for this entry
            /// </summary>
            public abstract string Name { get; }
        }
    }
}
#endif