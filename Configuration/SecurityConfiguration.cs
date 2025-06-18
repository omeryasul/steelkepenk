using WEBPROJE.WEBUI.Extensions;

namespace WEBPROJE.WEBUI.Configuration
{
    public class SecurityConfiguration
    {
        public static void ConfigureSecurityServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            // Enhanced Cookie Policy
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            });

            // HSTS Configuration
            if (environment.IsProduction())
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(365);
                });
            }

            // Request Localization (güvenlik için)
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "tr-TR", "en-US" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            // File Upload Security
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
                options.MultipartHeadersLengthLimit = 16384;
                options.MemoryBufferThreshold = int.MaxValue;
            });
        }

        public static void ConfigureSecurityPipeline(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            // Security Middleware Pipeline Order
            if (environment.IsProduction())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Custom security middleware
            app.UseSecurityMiddleware();

            // Built-in security middleware
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Static file security headers
                    ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=31536000,immutable");
                    ctx.Context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }
            });

            app.UseRouting();
            app.UseRequestLocalization();
            app.UseSession();
            app.UseCookiePolicy();
        }
    }
}