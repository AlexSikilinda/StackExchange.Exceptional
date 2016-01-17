using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using StackExchange.Exceptional.Extensions;
#if !COREFX
using System.Web;
#endif

namespace StackExchange.Exceptional.Pages
{
    internal abstract class WebPage
    {
        public abstract string PageTitle { get; }
        public Error Error { get; set; }

        // TODO: Move to MVC6 lib
#if !COREFX
        protected string BasePath => HttpContext.Current.Request.ServerVariables["URL"];
#endif

        public string Url(string path)
        {
#if !COREFX
            return BasePath.EndsWith("/") ? BasePath + path : BasePath + "/" + path;
#else
            return path;
#endif
        }

        protected StringBuilder sb { get; private set; }
        
        protected static readonly HashSet<string> HiddenHttpKeys = new HashSet<string>
        {
            "ALL_HTTP",
            "ALL_RAW",
            "HTTP_CONTENT_LENGTH",
            "HTTP_CONTENT_TYPE",
            "HTTP_COOKIE",
            "QUERY_STRING"
        };

        protected static readonly HashSet<string> DefaultHttpKeys = new HashSet<string>
        {
            "APPL_MD_PATH",
            "APPL_PHYSICAL_PATH",
            "GATEWAY_INTERFACE",
            "HTTP_ACCEPT",
            "HTTP_ACCEPT_CHARSET",
            "HTTP_ACCEPT_ENCODING",
            "HTTP_ACCEPT_LANGUAGE",
            "HTTP_CONNECTION",
            "HTTP_HOST",
            "HTTP_KEEP_ALIVE",
            "HTTPS",
            "INSTANCE_ID",
            "INSTANCE_META_PATH",
            "PATH_INFO",
            "PATH_TRANSLATED",
            "REMOTE_PORT",
            "SCRIPT_NAME",
            "SERVER_NAME",
            "SERVER_PORT",
            "SERVER_PORT_SECURE",
            "SERVER_PROTOCOL",
            "SERVER_SOFTWARE"
        };

        protected abstract void RenderBody();

        public void Render(StringBuilder builder)
        {
            sb = builder;
            var log = ErrorStore.Default;
            sb.Append(@"<!DOCTYPE html>").AppendLine();
            sb.Append("<html>").AppendLine();
            sb.Append("  <head>").AppendLine();
            sb.Append("    <title>").Encoded(PageTitle).AppendLine("</title>");
            sb.Append(@"   <link rel=""stylesheet"" type=""text/css"" href=""").Append(Url("css")).Append(@""" />").AppendLine();
            foreach (var css in ErrorStore.CSSIncludes)
            {
                sb.Append("    <link rel=\"stylesheet\" type=\"text/css\" href=\"").Append(css).AppendLine(@""" />");
            }
            if (Error != null)
            {
                sb.Append("    <script>var Exception = ").Append(Error.ToDetailedJson()).AppendLine(";</script>");
            }
            sb.Append(@"    <script src=""").Append(ErrorStore.jQueryURL).AppendLine(@"""></script>");
            sb.Append(@"    <script src=""").Append(Url("js")).AppendLine(@"""></script>");
            sb.Append(@"    <script>var baseUrl = '").Append(Url("")).AppendLine("\';</script>");
            foreach (var js in ErrorStore.JSIncludes)
            {
                sb.Append(@"    <script src=""").Append(js).AppendLine(@"""></script>");
            }

            var headerClass = ErrorStore.Default.InFailureMode ? " failure" : "";

            sb.Append("  </head>").AppendLine();
            sb.Append("  <body>").AppendLine();
            sb.Append("    <div class=\"top-header").Append(headerClass).AppendLine("\">");
            sb.Append("      <div class=\"top-label\">Exceptions Log: ").Encoded(Settings.Current.ApplicationName).AppendLine("</div>");
            sb.Append("    </div>").AppendLine();
            sb.Append("    <section id=\"content\">").AppendLine();
            RenderBody();

            var vAttribute = GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var version = vAttribute?.Version ?? "Unknown Version";

            sb.Append("    </section>").AppendLine();
            sb.Append("    <footer>").AppendLine();
            sb.Append("      <div class=\"version-info\">Exceptional ").Append(version).AppendLine()
              .Append("<br/>").Append(log.Name).Append("</div>").AppendLine();
            sb.Append("      <div class=\"server-time\">Server time is ").Append(DateTime.Now.ToString(CultureInfo.CurrentUICulture)).AppendLine("</div>");
            sb.Append("    </footer>").AppendLine();
            sb.Append("  </body>").AppendLine();
            sb.Append("</html>").AppendLine();
        }

        public static string Encode(string s) => WebUtility.HtmlEncode(s);
        public static string UrlEncode(string s) => WebUtility.UrlEncode(s);

        private static readonly Regex SanitizeUrlRegex = new Regex(@"[^-a-z0-9+&@#/%?=~_|!:,.;\*\(\)\{\}]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static string SanitizeUrl(string url) => url.IsNullOrEmpty() ? url : SanitizeUrlRegex.Replace(url, "");

        protected string Linkify(string s)
        {
            if (s.IsNullOrEmpty())
            {
                return "";
            }
            if (Regex.IsMatch(s, @"%[A-Z0-9][A-Z0-9]"))
            {
                s = WebUtility.UrlDecode(s);
            }
            if (Regex.IsMatch(s, "^(https?|ftp|file)://"))
            {
                var sane = SanitizeUrl(s);
                if (sane == s) // only link if it's not suspicious
                    return $@"<a href=""{sane}"">{s.Encode()}</a>";
            }
            return s.Encode();
        }
    }

    internal static class StringExtensions
    {
        public static string Encode(this string s) => WebUtility.HtmlEncode(s);

        public static string UrlEncode(this string s) => WebUtility.UrlEncode(s);

        public static StringBuilder Encoded(this StringBuilder sb, string s) => sb.Append(WebUtility.HtmlEncode(s));

        public static StringBuilder UrlEncoded(this StringBuilder sb, string s) => sb.Append(WebUtility.UrlEncode(s));
    }
}
