using CurrencyConverter.Api.Extensions;
using CurrencyConverter.Api.MinimalApi.CurrencyRoutes;
using CurrencyConverter.Infrastructure;
using CurrencyConverter.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Serilog;
using AspNetCoreRateLimit;
using CurrencyConverter.API.Extensions;
using CurrencyConverter.API.Middleware;
using CurrencyConverter.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Serilog.Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// OpenTelemetry Tracing
builder.Services.AddCurrencyConverterOpenTelemetry();

// Rate limiting configuration
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<AspNetCoreRateLimit.IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitConfiguration, AspNetCoreRateLimit.RateLimitConfiguration>();

// Swagger for minimal APIs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = JwtConstants.RequireHttpsMetadata;
        options.SaveToken = JwtConstants.SaveToken;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = JwtConstants.ValidateIssuer,
            ValidateAudience = JwtConstants.ValidateAudience,
            ValidateLifetime = JwtConstants.ValidateLifetime,
            ValidateIssuerSigningKey = JwtConstants.ValidateIssuerSigningKey
        };
    });

// Register distributed cache
builder.Services.AddDistributedMemoryCache();

// Register Frankfurter options from config
builder.Services.Configure<CurrencyConverter.Infrastructure.FrankfurterOptions>(builder.Configuration.GetSection("Frankfurter"));

// Register HttpClient for FrankfurterProvider using options pattern
builder.Services.AddHttpClient<CurrencyConverter.Infrastructure.Providers.FrankfurterProvider>((sp, c) =>
{
    var options = sp.GetRequiredService<IOptions<CurrencyConverter.Infrastructure.FrankfurterOptions>>().Value;
    var baseUrl = options.BaseUrl.EndsWith("/") ? options.BaseUrl : options.BaseUrl + "/";
    c.BaseAddress = new Uri(baseUrl);
    c.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});

// Register provider and factory
builder.Services.AddScoped<CurrencyConverter.Infrastructure.Providers.FrankfurterProvider>();
builder.Services.AddScoped<CurrencyConverter.Infrastructure.ICurrencyProvider, CurrencyConverter.Infrastructure.Providers.FrankfurterProvider>();
builder.Services.AddScoped<CurrencyConverter.Infrastructure.ICurrencyProviderFactory, CurrencyConverter.Infrastructure.CurrencyProviderFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLogging();

// Routes are grouped under the provided prefix (default: "/api/v1").
CurrencyConverter.Api.MinimalApi.CurrencyRoutes.RouteHandlers.MapRoutes(app.MapGroup(""));

app.Run();
