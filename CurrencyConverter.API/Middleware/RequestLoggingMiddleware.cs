using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CurrencyConverter.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var endpoint = context.Request.Path;
            var method = context.Request.Method;
            var responseCode = context.Response.StatusCode;
            var responseTime = sw.ElapsedMilliseconds;
            string? clientId = null;
            if (context.User.Identity?.IsAuthenticated == true)
            {
                clientId = context.User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "client_id")?.Value;
            }
            Log.Information("Request: {Method} {Endpoint} | IP: {IP} | ClientId: {ClientId} | Status: {Status} | Time: {Time}ms",
                method, endpoint, clientIp, clientId, responseCode, responseTime);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
