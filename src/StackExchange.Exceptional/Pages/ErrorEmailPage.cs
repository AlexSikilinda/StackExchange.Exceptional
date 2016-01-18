using System.Collections.Specialized;
using System.Linq;
using System.Text;
using StackExchange.Exceptional.Extensions;

namespace StackExchange.Exceptional.Pages
{
    public class ErrorEmailPage : WebPage
    {
        public override string PageTitle => "";

        public void RenderVariableTable(string title, string className, NameValueCollection vars)
        {
            if (vars == null || vars.Count == 0) return;
            var result = new StringBuilder();

            var fetchError = vars[Error.CollectionErrorKey];
            var errored = fetchError.HasValue();
            var keys = vars.AllKeys
                .Where(key => !HiddenHttpKeys.Contains(key) && key != Error.CollectionErrorKey)
                .OrderBy(k => k);

            result.AppendFormat("    <div>").AppendLine();
            result.AppendFormat(
                "        <h3 style=\"color: #224C00; font-family: Verdana, Tahoma, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-size: 14px; margin: 10px 0 5px 0;\">{0}{1}</h3>",
                title, errored ? " - Error while gathering data" : "").AppendLine();
            if (keys.Any())
            {
                result.AppendFormat(
                    "        <table style=\"font-family: Verdana, Tahoma, Arial, 'Helvetica Neue', Helvetica, sans-serif; font-size: 12px; width: 100%; border-collapse: collapse; border: 0;\">")
                    .AppendLine();
                var i = 0;
                foreach (var k in keys)
                {
                    // If this has no value, skip it
                    if (vars[k].IsNullOrEmpty() || DefaultHttpKeys.Contains(k))
                    {
                        continue;
                    }
                    result.AppendFormat(
                        "          <tr{2}><td style=\"padding: 0.4em; width: 200px;\">{0}</td><td style=\"padding: 0.4em;\">{1}</td></tr>",
                        k, Linkify(vars[k]), i%2 == 0 ? " style=\"background-color: #F2F2F2;\"" : "").AppendLine();
                    i++;
                }
                if (vars["HTTP_HOST"].HasValue() && vars["URL"].HasValue())
                {
                    var url = string.Format("http://{0}{1}{2}", vars["HTTP_HOST"], vars["URL"],
                        vars["QUERY_STRING"].HasValue() ? "?" + vars["QUERY_STRING"] : "");
                    result.AppendFormat(
                        "          <tr><td style=\"padding: 0.4em; width: 200px;\">URL and Query</td><td style=\"padding: 0.4em;\">{0}</td></tr>",
                        vars["REQUEST_METHOD"] == "GET" ? Linkify(url) : url.Encode()).AppendLine();
                }
                result.AppendFormat("        </table>").AppendLine();
            }
            if (errored)
            {
                result.AppendFormat("        <span style=\"color: maroon;\">Get {0} threw an exception:</span>", title)
                    .AppendLine();
                result.AppendFormat(
                    "        <pre  style=\"background-color: #EEE; font-family: Consolas, Monaco, monospace; padding: 8px;\">{0}</pre>",
                    fetchError.Encode()).AppendLine();
            }
            result.AppendFormat("    </div>").AppendLine();
        }

        private static readonly string BigStuff = "font-family: Verdana, Tahoma, Arial, \'Helvetica Neue\', Helvetica, sans-serif";
        private static readonly string H3Style = $"style=\"color: #224C00;{BigStuff}; font-size: 14px; margin: 10px 0 5px 0;\"";
        private static readonly string H3ErrorStyle = $"style=\"color: maroon;{BigStuff}; font-size: 14px; margin: 10px 0 5px 0;\"";

        protected override void RenderBody()
        {
            var e = Error;
            sb.AppendLine("<div style=\"font-family: Arial, \'Helvetica Neue\', Helvetica, sans-serif;\">");
            sb.AppendLine($"  <h1 style=\"color: maroon; font-size: 16px; padding: 0; margin: 0;\">{e.Message}</h1>");
            sb.AppendLine($"  <div style=\"font-size: 12px; color: #444; padding: 0; margin: 2px 0;\">{e.Type.Encode()}</div>");
            sb.AppendLine( "  <pre style=\"background-color: #FFFFCC; font-family: Consolas, Monaco, monospace; font-size: 12px; margin: 2px 0; padding: 12px;\">");
            sb.Append(e.Detail.Encode());
            sb.AppendLine("  </pre>");

            sb.AppendLine("  <p style=\"font-size: 13px; color: #555; margin: 5px 0;\">occurred ");
            sb.AppendLine($"    <b title=\"{e.CreationDate.ToString("D")} at {e.CreationDate.ToString("T")}\">{e.CreationDate.ToRelativeTime()} UTC</b>");
            sb.AppendLine($"    on {e.MachineName}");
            sb.AppendLine($"    <span class=\"info-delete-link\">(<a class=\"info-link\" href=\"delete?guid={e.GUID.ToString()}\">delete</a>)</span>");
            sb.AppendLine("  </p>");

            if (!string.IsNullOrEmpty(e.SQL))
            {
                sb.AppendLine($"  <h3 {H3Style}>SQL</h3>");
                sb.AppendLine($"  <pre style=\"background-color: #EEE; font-family: Consolas, monospace; padding: 8px; margin: 2px 0;\">{e.SQL.Encode()}</pre>");
            }

            RenderVariableTable("Server Variables", "server-variables", e.ServerVariables);
            if (e.CustomData?.Count > 0)
            {
                var errored = e.CustomData.ContainsKey(ErrorStore.CustomDataErrorKey);
                var cdKeys = e.CustomData.Keys.Where(k => k != ErrorStore.CustomDataErrorKey).ToList();
                sb.AppendLine("  <div>");
                if (errored)
                {
                    sb.AppendLine($"    <h3 {H3ErrorStyle}>Custom - Error while gathering custom data</h3>");
                    sb.AppendLine( "    <span style=\"color: maroon;\">GetCustomData threw an exception:</span>");
                    sb.AppendLine($"   <pre style=\"background-color: #EEE; font-family: Consolas, Monaco, monospace; padding: 8px;\">{e.CustomData[ErrorStore.CustomDataErrorKey]}</pre>");
                }
                else if (cdKeys.Any(k => k != ErrorStore.CustomDataErrorKey))
                {
                    var i = -1;
                    sb.AppendLine($"    <h3 {H3Style}>Custom</h3>");
                    sb.AppendLine( "    <table style=\" font-size: 12px; width: 100%; border-collapse: collapse; border: 0;\">");
                    foreach (var cd in cdKeys)
                    {
                        i++;
                        sb.AppendLine($"      <tr{(i%2 == 0 ? " style =\"background-color: #F2F2F2;\"" : "")}>");
                        sb.AppendLine($"        <td style=\"padding: 0.4em; width: 200px;\">{cd.Encode()}</td>");
                        sb.AppendLine($"        <td style=\"padding: 0.4em;\">{Linkify(e.CustomData[cd])}</td>");
                        sb.AppendLine( "      </tr>");
                    }
                    sb.AppendLine("    </table>");
                }
                sb.AppendLine("  </div>");
            }
            RenderVariableTable("QueryString", "querystring", e.QueryString);
            RenderVariableTable("Form", "form", e.Form);
            RenderVariableTable("Cookies", "cookies", e.Cookies);
            sb.AppendLine("</div>");
        }
    }
}