// Middleware/SecurityHeadersMiddleware.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WEBPROJE.WEBUI.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            ILogger<SecurityHeadersMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Security headers ekle
            AddSecurityHeaders(context);

            // Request logging
            LogRequest(context);

            await _next(context);

            // Response logging
            LogResponse(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            var response = context.Response;

            // Content Security Policy
            var csp = BuildContentSecurityPolicy();
            response.Headers.Add("Content-Security-Policy", csp);

            // X-Frame-Options
            response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

            // X-Content-Type-Options
            response.Headers.Add("X-Content-Type-Options", "nosniff");

            // X-XSS-Protection
            response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // Referrer Policy
            response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions Policy
            response.Headers.Add("Permissions-Policy",
                "geolocation=(), microphone=(), camera=(), payment=(), usb=()");

            // HSTS (sadece HTTPS'de)
            if (context.Request.IsHttps)
            {
                response.Headers.Add("Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains; preload");
            }

            // Remove server header
            response.Headers.Remove("Server");
            response.Headers.Add("Server", "WebServer");

            // Development cache control
            if (_environment.IsDevelopment())
            {
                response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                response.Headers.Add("Pragma", "no-cache");
                response.Headers.Add("Expires", "0");
            }
        }

        private string BuildContentSecurityPolicy()
        {
            var csp = new List<string>
            {
                "default-src 'self'",
                "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://www.google-analytics.com https://www.googletagmanager.com",
                "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com",
                "img-src 'self' data: https: blob:",
                "font-src 'self' https://cdnjs.cloudflare.com",
                "connect-src 'self' https://www.google-analytics.com",
                "frame-src 'self' https://www.google.com https://www.youtube.com",
                "object-src 'none'",
                "base-uri 'self'",
                "form-action 'self'",
                "frame-ancestors 'self'"
            };

            return string.Join("; ", csp);
        }

        private void LogRequest(HttpContext context)
        {
            var request = context.Request;
            var clientIp = GetClientIpAddress(context);

            // Şüpheli request pattern kontrolü
            if (IsSuspiciousRequest(request))
            {
                _logger.LogWarning("Suspicious request detected. IP: {IP}, Path: {Path}, UserAgent: {UserAgent}",
                    clientIp, request.Path, request.Headers.UserAgent);
            }

            // Rate limiting için IP tracking
            TrackIpRequest(clientIp);
        }

        private void LogResponse(HttpContext context)
        {
            var response = context.Response;

            // Error response logging
            if (response.StatusCode >= 400)
            {
                var clientIp = GetClientIpAddress(context);
                _logger.LogWarning("HTTP {StatusCode} response for {Path}. IP: {IP}",
                    response.StatusCode, context.Request.Path, clientIp);
            }
        }

        private bool IsSuspiciousRequest(HttpRequest request)
        {
            var path = request.Path.Value?.ToLower() ?? "";
            var userAgent = request.Headers.UserAgent.ToString().ToLower();
            var queryString = request.QueryString.Value?.ToLower() ?? "";

            // Şüpheli path pattern'leri
            var suspiciousPaths = new[]
            {
                "/admin", "/wp-admin", "/administrator", "/login", "/phpmyadmin",
                "/.env", "/config", "/backup", "/test", "/debug", "/api/swagger",
                "/xmlrpc.php", "/wp-login.php", "/.well-known/", "/robots.txt"
            };

            // Şüpheli query parameter'ları
            var suspiciousQueries = new[]
            {
                "union", "select", "drop", "delete", "insert", "update",
                "script", "javascript", "vbscript", "onload", "onerror",
                "../", "..\\", "%2e%2e", "etc/passwd", "cmd.exe"
            };

            // Şüpheli user agent'lar
            var suspiciousUserAgents = new[]
            {
                "sqlmap", "nikto", "nmap", "masscan", "zap", "burp",
                "crawler", "spider", "bot", "scan"
            };

            return suspiciousPaths.Any(p => path.Contains(p)) ||
                   suspiciousQueries.Any(q => queryString.Contains(q)) ||
                   suspiciousUserAgents.Any(ua => userAgent.Contains(ua)) ||
                   string.IsNullOrEmpty(userAgent);
        }

        private void TrackIpRequest(string clientIp)
        {
            // Memory cache kullanarak IP bazlı rate limiting
            // Bu implementasyon production'da Redis ile yapılmalı
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // Proxy arkasındaysa gerçek IP'yi al
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedIps = context.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedIps))
                {
                    ipAddress = forwardedIps.Split(',')[0].Trim();
                }
            }
            else if (context.Request.Headers.ContainsKey("X-Real-IP"))
            {
                ipAddress = context.Request.Headers["X-Real-IP"].ToString();
            }

            return ipAddress ?? "Unknown";
        }
    }
}

