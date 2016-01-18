using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace StackExchange.Exceptional
{
    public class ExceptionalMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ErrorStore.Default.Log(ex, context, appendFullStackTrace: true);
                throw;
            }
        }
    }

    public static class ExceptionalMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptional(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionalMiddleware>();
        }
    }
}
