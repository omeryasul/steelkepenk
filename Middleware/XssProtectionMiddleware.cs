// Middleware/XssProtectionMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace WEBPROJE.WEBUI.Middleware
{
    public class XssProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XssProtectionMiddleware> _logger;

        private readonly string[] _xssPatterns = {
            @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
            @"javascript\s*:",
            @"vbscript\s*:",
            @"on\w+\s*=",
            @"<iframe",
            @"<object",
            @"<embed",
            @"expression\s*\(",
            @"url\s*\(",
            @"<meta",
            @"<link"
        };

        public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Sadece POST isteklerini kontrol et
            if (context.Request.Method == "POST")
            {
                var originalBody = context.Request.Body;

                try
                {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (ContainsXss(body))
                    {
                        var clientIp = GetClientIpAddress(context);
                        _logger.LogWarning("XSS attempt detected. IP: {IP}, Body: {Body}",
                            clientIp, body.Substring(0, Math.Min(body.Length, 500)));

                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Bad Request: Potentially malicious content detected");
                        return;
                    }
                }
                finally
                {
                    context.Request.Body = originalBody;
                }
            }

            await _next(context);
        }

        private bool ContainsXss(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return _xssPatterns.Any(pattern =>
                Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
        }

        private string GetClientIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}