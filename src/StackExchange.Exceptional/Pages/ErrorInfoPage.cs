using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using StackExchange.Exceptional.Extensions;

namespace StackExchange.Exceptional.Pages
{
    internal class ErrorInfo : WebPage
    {
        public override string PageTitle => "";
        public Guid Guid { get; set; }

        public ErrorInfo(Guid guid)
        {
            Guid = guid;
            var log = ErrorStore.Default;
            Error = log.Get(guid);
        }

        public void RenderVariableTable(string title, string className, NameValueCollection vars)
        {
            if (vars == null || vars.Count == 0) return;
            var hiddenRows = new StringBuilder();

            var fetchError = vars[Error.CollectionErrorKey];
            var errored = fetchError.HasValue();
            var keys = vars.AllKeys
                .Where(key => !HiddenHttpKeys.Contains(key) && key != Error.CollectionErrorKey)
                .OrderBy(k => k);

            sb.AppendLine($"    <div class=\"{className}\">");
            sb.AppendLine($"      <h3 class=\"kv-title{(errored ? " title-error" : "")}\">{title}{(errored ? " - Error while gathering data" : "")}</h3>");
            if (keys.Any())
            {
                sb.AppendLine("      <div class=\"side-scroll\">");
                sb.AppendLine("        <table class=\"kv-table\">");
                foreach (var k in keys)
                {
                    // If this has no value, skip it
                    if (vars[k].IsNullOrEmpty())
                    {
                        continue;
                    }
                    // If this is a hidden row, buffer it up, since CSS has no clean mechanism for :visible:nth-row(odd) type styling behavior
                    var hidden = DefaultHttpKeys.Contains(k);
                    (hidden ? hiddenRows : sb).AppendFormat("          <tr{2}><td class=\"key\">{0}</td><td class=\"value\">{1}</td></tr>", k,
                        Linkify(vars[k]), hidden ? " class=\"hidden\"" : "");
                }
                string host = vars["HTTP_HOST"],
                       path = vars["URL"],
                       queryString = vars["QUERY_STRING"].HasValue() ? "?" + vars["QUERY_STRING"] : "";
                if (host.HasValue() && vars["URL"].HasValue())
                {
                    var ssl = vars["HTTP_X_FORWARDED_PROTO"] == "https" || vars["HTTP_X_SSL"].HasValue() || vars["HTTPS"] == "on";
                    var url = $"http{(ssl ? "s" : "")}://{host}{path}{queryString}";
                    var renderUrl = vars["REQUEST_METHOD"] == "GET" ? Linkify(url) : url.Encode();
                    sb.AppendLine($"          <tr><td class=\"key\">URL and Query</td><td class=\"value\">{renderUrl}</td></tr>");
                }
                sb.Append(hiddenRows);
                sb.AppendLine("        </table>");
                sb.AppendLine("      </div>");
            }
            if (errored)
            {
                sb.AppendFormat("      <span class=\"custom-error-label\">Get {0} threw an exception:</span>", title);
                sb.AppendFormat("      <pre class=\"error-detail\">{0}</pre>", fetchError.Encode());
            }
            sb.AppendFormat("    </div>");
        }
        
        protected override void RenderBody()
        {
            var e = Error;
            sb.Append("<div id=\"ErrorInfo\">").AppendLine();
            if (Error != null)
            {
                sb.AppendLine($"  <h1 class=\"error-title\">{e.Message.Encode()}</h1>");
                sb.AppendLine($"  <div class=\"error-type\">{e.Type.Encode()}</div>");
                sb.AppendLine($"  <pre class=\"error-detail\">{e.Detail.Encode()}</pre>");
                sb.AppendLine( "  <p class=\"error-time\">occurred ");
                sb.AppendLine($"    <b title=\"{e.CreationDate.ToString("D")} at {e.CreationDate.ToString("T")}\">{e.CreationDate.ToRelativeTime()}</b>");
                sb.AppendLine($"    on {e.MachineName}");
                sb.AppendLine($"    <span class=\"info-delete-link\">(<a class=\"info-link\" href=\"delete?guid={e.GUID.ToString()}\">delete</a>)</span>");
                sb.AppendLine( "  </p>");
                if (!string.IsNullOrEmpty(e.SQL))
                {
                    sb.AppendLine( "  <h3>SQL</h3>");
                    sb.AppendLine($"  <pre class=\"sql-detail\">{e.SQL.Encode()}</pre>");
                }

                RenderVariableTable("Server Variables", "server-variables", e.ServerVariables);
                if (e.CustomData?.Count > 0)
                {
                    var errored = e.CustomData.ContainsKey(ErrorStore.CustomDataErrorKey);
                    var cdKeys = e.CustomData.Keys.Where(k => k != ErrorStore.CustomDataErrorKey).ToList();
                    sb.AppendLine("  <div class=\"custom-data\">");
                    if (errored)
                    {
                        sb.AppendLine("    <h3 class=\"kv-title title-error\">Custom - Error while gathering custom data</h3>");
                        sb.AppendLine("    <span class=\"custom-error-label\">GetCustomData threw an exception:</span>");
                        sb.AppendLine($"    <pre class=\"error-detail\">{e.CustomData[ErrorStore.CustomDataErrorKey]}</pre>");
                    }
                    else if (cdKeys.Any())
                    {
                        sb.AppendLine("    <h3 class=\"kv-title\">Custom</h3>");
                        sb.AppendLine("    <div class=\"side-scroll\">");
                        sb.AppendLine("      <table class=\"kv-table\">");
                        foreach (var cd in cdKeys)
                        {
                            sb.AppendLine( "        <tr>");
                            sb.AppendLine($"          <td class=\"key\">{cd.Encode()}</td>");
                            sb.AppendLine($"          <td class=\"value\">{Linkify(e.CustomData[cd])}</td>");
                            sb.AppendLine( "        </tr>");
                        }
                        sb.AppendLine("      </table>");
                        sb.AppendLine("    </div>");
                    }
                    sb.AppendLine("  </div>");
                }
                RenderVariableTable("QueryString", "querystring", e.QueryString);
                RenderVariableTable("Form", "form", e.Form);
                RenderVariableTable("Cookies", "cookies", e.Cookies);
                RenderVariableTable("RequestHeaders", "headers", e.RequestHeaders);
            }
            else
            {
                sb.Append("  <h1>Error not found.</h1>").AppendLine();
            }
            sb.Append("</div>");
        }
    }
}