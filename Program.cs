using Application;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCaching();

// Swagger/OpenAPI - SADECE DEVELOPMENT
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WEBPROJE API",
            Version = "v1",
            Description = "WEBPROJE Web API - Development Environment"
        });

        // JWT Authentication için Swagger yapılandırması
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Services (MediatR, AutoMapper, FluentValidation)
builder.Services.AddApplicationServices();

// Infrastructure Services (DbContext, Repositories, Services)
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();

// CORS Policy - GÜVENLİ
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development", policy =>
        {
            policy.WithOrigins(
                "https://localhost:3000",
                "http://localhost:3000",
                "https://localhost:4200",
                "http://localhost:4200",
                "https://localhost:7131",
                "http://localhost:5021",
                "https://localhost:7036",
                "http://localhost:5142"
            )
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
            .AllowCredentials();
        });
    }
    else
    {
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins("https://yourdomain.com") // GERÇEK DOMAIN'İ BURAYA YAZ
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .WithHeaders("Content-Type", "Authorization")
                  .AllowCredentials();
        });
    }
});

// JWT Authentication - GÜVENLİ
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrEmpty(jwtKey))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtKey = "development-key-change-in-production-123456789abcdef-VERY-LONG-KEY";
    }
    else
    {
        throw new InvalidOperationException("JWT_SECRET_KEY environment variable must be set in production");
    }
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
            // Ek güvenlik
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        // Event handlers for better logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    Console.WriteLine($"JWT Authentication failed: {context.Exception}");
                }
                return Task.CompletedTask;
            }
        };
    });

// Rate Limiting (ASP.NET Core 7+)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
});

var app = builder.Build();

// GÜVENLIK HEADER'LARI
app.Use(async (context, next) =>
{
    // API için CSP
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");

    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WEBPROJE API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Rate limiting
app.UseRateLimiter();

// CORS - Ortama göre
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controller endpoints
app.MapControllers().RequireRateLimiting("ApiPolicy");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
})).AllowAnonymous();

// API Info endpoint - Sadece development
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => "WEBPROJE API çalışıyor! Swagger için /swagger adresine gidin.").AllowAnonymous();
}

try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("API starting in {Environment} environment", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "API failed to start");
    throw;
}