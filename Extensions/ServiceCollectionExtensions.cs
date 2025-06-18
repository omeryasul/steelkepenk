using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using WEBPROJE.WEBUI.Middleware;

namespace WEBPROJE.WEBUI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Model validation configuration
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return new BadRequestObjectResult(new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    });
                };
            });

            // Data Protection
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("./keys"))
                .SetApplicationName("WEBPROJE")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            // Session configuration
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            // Request size limits
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
            });

            return services;
        }

        public static IApplicationBuilder UseSecurityMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseMiddleware<XssProtectionMiddleware>();
            return app;
        }
    }
}