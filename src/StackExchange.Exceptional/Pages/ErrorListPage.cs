using System.Collections.Generic;
using System.Linq;
using StackExchange.Exceptional.Extensions;

namespace StackExchange.Exceptional.Pages
{
    internal class ErrorListPage : WebPage
    {
        public override string PageTitle => "Error Log";

        protected override void RenderBody()
        {
            var log = ErrorStore.Default;
            var errors = new List<Error>();
            var total = log.GetAll(errors);
            errors = errors.OrderByDescending(e => e.CreationDate).ToList();

            if (log.InFailureMode)
            {
                var le = ErrorStore.LastRetryException;
                sb.Append("<div class=\"failure-mode\">").AppendLine();
                sb.Append("  Error log is in failure mode, ")
                    .Append(ErrorStore.WriteQueue.Count)
                    .Append(" ")
                    .Append(ErrorStore.WriteQueue.Count == 1 ? "entry" : "entries")
                    .Append(" queued to log.");
                sb.Append("<br />");
                if (le != null)
                {
                    sb.Append("  Last Logging Exception: ").Encoded(le.Message);
                }
                sb.Append("</div>").AppendLine();
                sb.Append("<!-- Exception Details:").AppendLine();
                if (ErrorStore.LastRetryException != null)
                {
                    sb.Encoded(ErrorStore.LastRetryException.Message).AppendLine();
                    sb.Encoded(ErrorStore.LastRetryException.StackTrace).AppendLine();
                }
                sb.Append("-->").AppendLine();
            }
            if (!errors.Any())
            {
                sb.Append("<h1>No errors yet, yay!</h1>").AppendLine();
            }
            else
            {
                var last = errors.FirstOrDefault(); // oh the irony
                sb.Append("<h1 id=\"errorcount\">")
                    .Encoded(ErrorStore.ApplicationName)
                    .Append(" - ")
                    .Append(total)
                    .Append(" Errors; last ")
                    .Append(last.CreationDate.ToRelativeTime())
                    .Append("</h1>").AppendLine();
                sb.Append("<table id=\"ErrorLog\" class=\"alt-rows\">").AppendLine();
                sb.Append("  <thead>").AppendLine();
                sb.Append("    <tr>").AppendLine();
                sb.Append("      <th class=\"type-col\">&nbsp;</th>").AppendLine();
                sb.Append("      <th class=\"type-col\">Type</th>").AppendLine();
                sb.Append("      <th>Error</th>").AppendLine();
                sb.Append("      <th>Url</th>").AppendLine();
                sb.Append("      <th>Remote IP</th>").AppendLine();
                sb.Append("      <th>Time</th>").AppendLine();
                sb.Append("      <th>Site</th>").AppendLine();
                sb.Append("      <th>Server</th>").AppendLine();
                sb.Append("    </tr>").AppendLine();
                sb.Append("  </thead>").AppendLine();
                sb.Append("  <tbody>").AppendLine();
                foreach (var e in errors)
                {
                    var fullUrl = "http://" + e.Host + e.Url;
                    var errorClass = e.IsProtected ? " protected" : "";
                    sb.Append("    <tr data-id=\"")
                        .Append(e.GUID.ToString())
                        .Append("\" class=\"error")
                        .Append(errorClass)
                        .Append("\">")
                        .AppendLine();
                    sb.Append("      <td>")
                        .Append("<a href=\"").Append(Url("delete")).Append("\" class=\"delete-link\" title=\"Delete this error\">&nbsp;X&nbsp;</a>");
                    if (!e.IsProtected)
                    {
                        sb.Append("<a href=\"").Append(Url("protect")).Append("\" class=\"protect-link\" title=\"Protect this error\">&nbsp;P&nbsp;</a>")
                            .AppendLine();
                    }
                    sb.Append("      <td class=\"type-col\" title=\"").Encoded(e.Type).Append("\">")
                           .Encoded(e.Type.ToShortException())
                           .AppendLine();
                    sb.Append("      <td class=\"error-col\">")
                           .Append("<a href=\"").Append(Url("info?guid=")).Append(e.GUID.ToString()).Append("\" class=\"info-link\">")
                           .Encoded(e.Message)
                           .Append("</a>")
                           .AppendLine();
                    if (e.DuplicateCount > 1)
                    {
                        sb.Append("<span class=\"duplicate-count\" title=\"number of similar errors occurring close to this error\">(").Append(e.DuplicateCount.ToString()).Append(")</span>")
                            .AppendLine();
                    }
                    sb.Append("      <td>");
                    if (e.HTTPMethod == "GET" && e.Url.HasValue())
                    {
                        sb.Append("<a href=\"").UrlEncoded(fullUrl).Append("\" title=\"").Encoded(fullUrl).Append("\" class=\"url-link\">")
                            .Encoded(e.Url.TruncateWithEllipsis(40))
                            .Append("</a>");
                    }
                    else if (e.Url.HasValue())
                    {
                        sb.Append("<span title=\"").Encoded(fullUrl).Append("\">")
                            .Encoded(e.Url.TruncateWithEllipsis(40))
                            .Append("</span>");
                    }
                    sb.AppendLine();
                    sb.Append("      <td>")
                        .Encoded(e.IPAddress)
                        .AppendLine();
                    sb.Append("      <td><span title=\"")
                        .Append(e.CreationDate.ToString("G"))
                        .Append(" -- ")
                        .Append(e.CreationDate.ToUniversalTime().ToString("u"))
                        .Append("\">")
                        .Append(e.CreationDate.ToRelativeTime())
                        .Append("</span>")
                        .AppendLine();
                    sb.Append("      <td>").Encoded(e.Host).AppendLine();
                    sb.Append("      <td>").Encoded(e.MachineName).AppendLine();
                    sb.Append("    </tr>").AppendLine();
                }
                sb.Append("  </tbody>").AppendLine();
                sb.Append("</table>").AppendLine();
                if (errors.Any(e => !e.IsProtected))
                {
                    sb.Append("<div class=\"clear-all-div\">").AppendLine();
                    sb.Append("  <a class=\"delete-link clear-all-link\" href=\"")
                        .Append(Url("delete-all"))
                        .Append("\">&nbsp;X&nbsp;- Clear all non-protected errors</a>")
                        .AppendLine();
                    sb.Append("</div>").AppendLine();
                }
            }
        }
    }
}
