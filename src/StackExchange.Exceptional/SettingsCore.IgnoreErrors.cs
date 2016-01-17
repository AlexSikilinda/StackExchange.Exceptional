#if COREFX
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace StackExchange.Exceptional
{
    public partial class Settings
    {
        /// <summary>
        /// The Ignore section of the configuration, optional and no errors will be blocked from logging if not specified
        /// </summary>
        public IgnoreSettings IgnoreErrors { get; set; }

        /// <summary>
        /// Ignore element for deserilization from a configuration, e.g. web.config or app.config
        /// </summary>
        public class IgnoreSettings
        {
            /// <summary>
            /// Regular expressions collection for errors to ignore.  Any errors with a .ToString() matching any regex here will not be logged
            /// </summary>
            public List<IgnoreRegex> Regexes { get; set; }

            /// <summary>
            /// Types collection for errors to ignore.  Any errors with a Type matching any name here will not be logged
            /// </summary>
            public List<IgnoreType> Types { get; set; }
        }
    }

    /// <summary>
    /// A regex entry, to match against error messages to see if we should ignore them
    /// </summary>
    public class IgnoreRegex
    {
        /// <summary>
        /// The name that describes this regex
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Pattern to match on the exception message
        /// </summary>
        public string Pattern { get; set; }

        private Regex _patternRegEx;
        /// <summary>
        /// Regex object representing the pattern specified, compiled once for use against all future exceptions
        /// </summary>
        public Regex PatternRegex => _patternRegEx ?? (_patternRegEx = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline));
    }

    /// <summary>
    /// A type entry, to match against error messages types to see if we should ignore them
    /// </summary>
    public class IgnoreType
    {
        /// <summary>
        /// The name that describes this ignored type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The fully qualified type of the exception to ignore
        /// </summary>
        public string Type { get; set; }
    }
}
#endif