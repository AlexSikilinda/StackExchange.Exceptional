using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

namespace StackExchange.Exceptional
{
    public static class SystemWebExtensions
    {
        public static Error Log(this ErrorStore store,
            Exception exception,
            HttpContext context,
            bool appendFullStackTrace = false,
            bool rollupPerServer = false,
            Dictionary<string, string> customData = null,
            string applicationName = null)
        {
            Action<Error> setProperties = e =>
            {
                if (context == null) return;
                var request = context.Request;
                e.StatusCode = context.Response.StatusCode;

                e.ServerVariables = GetNvc(request, r => r.ServerVariables);
                e.QueryString = GetNvc(request, r => r.QueryString);
                e.Form = GetNvc(request, r => r.Form);
                try
                {
                    var nvc = new NameValueCollection(request.Cookies.Count);
                    foreach (var k in request.Cookies.AllKeys)
                    {
                        var httpCookie = request.Cookies[k];
                        if (httpCookie != null)
                            e.Cookies.Add(k, httpCookie.Value);
                    }
                    e.Cookies = nvc;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error parsing cookie collection: " + ex.Message);
                }
                e.RequestHeaders = new NameValueCollection(request.Headers);
            };

            return ErrorStore.Default.Log(
                exception,
                appendFullStackTrace: appendFullStackTrace,
                rollupPerServer: rollupPerServer,
                customData: customData,
                applicationName: applicationName,
                setProperties: setProperties);
        }

        private static NameValueCollection GetNvc(HttpRequest request, Func<HttpRequest, NameValueCollection> getter)
        {
            try
            {
                return new NameValueCollection(getter(request));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error parsing collection: " + ex.Message);
                return new NameValueCollection {{Error.CollectionErrorKey, ex.Message}};
            }
        }
    }
}
