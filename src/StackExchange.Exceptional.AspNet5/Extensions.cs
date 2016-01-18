using System;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.Primitives;

namespace StackExchange.Exceptional
{
    public static class Extensions
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

                // TODO: Generate ServerVairables
                //e.ServerVariables = GetNvc(r => r.ServerVariables);
                e.QueryString = GetNvc(request.Query);
                e.Form = GetNvc(request.Form);
                e.Cookies = GetNvc(request.Cookies);
                e.RequestHeaders = GetNvc(request.Headers);
            };

            return ErrorStore.Default.Log(
                exception,
                appendFullStackTrace: appendFullStackTrace,
                rollupPerServer: rollupPerServer,
                customData: customData,
                applicationName: applicationName,
                setProperties: setProperties);
        }

        private static NameValueCollection GetNvc(IEnumerable<KeyValuePair<string, string>> col)
        {
            var nvc = new NameValueCollection();
            if (col == null) return nvc;
            foreach (var i in col)
            {
                nvc.Add(i.Key, i.Value);
            }
            return nvc;
        }

        private static NameValueCollection GetNvc(IEnumerable<KeyValuePair<string, StringValues>> col)
        {
            var nvc = new NameValueCollection();
            if (col == null) return nvc;
            foreach (var i in col)
            {
                foreach (var v in i.Value)
                {
                    nvc.Add(i.Key, v);
                }
            }
            return nvc;
        }
    }
}
