using Application;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();

// Database - Güvenli baðlantý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Services (MediatR, AutoMapper, FluentValidation)
builder.Services.AddApplicationServices();

// Persistence Services (DbContext, Repositories, Services)
builder.Services.AddPersistence(builder.Configuration);

// Memory Cache - Production'da da güvenli
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = builder.Environment.IsDevelopment() ? 100 : 1000;
    options.CompactionPercentage = 0.25;
});

// Identity - Güçlendirilmiþ güvenlik
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Þifre gereksinimleri
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8; // 6'dan 8'e çýkarýldý
    options.Password.RequireNonAlphanumeric = true; // true yapýldý
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 4; // Eklendi

    // Hesap kilitleme
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Güvenlik
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure Identity cookies - Güçlendirilmiþ
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Account/Login";
    options.LogoutPath = "/Admin/Account/Logout";
    options.AccessDeniedPath = "/Admin/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.Name = "AdminAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Always olarak deðiþtirildi
    options.Cookie.SameSite = SameSiteMode.Strict; // Eklendi
    options.Cookie.IsEssential = true; // Eklendi
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// GÜVENLIK: Anti-forgery token
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__RequestVerificationToken";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Database initialization
await InitializeDatabaseAsync(app);

// GÜVENLIK HEADER'LARI - Her ortam için
app.Use(async (context, next) =>
{
    // Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://www.google-analytics.com https://www.googletagmanager.com; " +
        "style-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://cdnjs.cloudflare.com; " +
        "connect-src 'self' https://www.google-analytics.com; " +
        "frame-src 'self' https://www.google.com;");

    // Diðer güvenlik header'larý
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

    // Development cache ayarlarý
    if (app.Environment.IsDevelopment())
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    await next();
});

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin area routing
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Admin}/{action=Index}/{id?}");

// Account controller için özel route
app.MapAreaControllerRoute(
    name: "adminAccount",
    areaName: "Admin",
    pattern: "Admin/Account/{action=Login}",
    defaults: new { controller = "Account" });

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Database initialization - GÜVENLÝ
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Apply migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied.");
        }

        // Seed roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            logger.LogInformation("Admin role created.");
        }

        // Seed admin user - GÜVENLÝ ÞÝFRE
        var adminEmail = "admin@furkankepenek.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            // Güçlü þifre oluþtur
            var adminPassword = GenerateSecurePassword();

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                IsAdmin = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Admin user created successfully.");
                // ÞÝFREYÝ LOGLAMA - Sadece güvenli ortamda
                if (app.Environment.IsDevelopment())
                {
                    logger.LogWarning("DEVELOPMENT - Admin Password: {Password}", adminPassword);
                }
            }
            else
            {
                logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists: {Email}", adminEmail);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");
    }
}

// Güvenli þifre oluþturucu
static string GenerateSecurePassword()
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
    var random = new Random();
    return new string(Enumerable.Repeat(chars, 12)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}