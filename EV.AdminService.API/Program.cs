using EV.AdminService.API.ConfigurationModels;
using EV.AdminService.API.Helpers;
using EV.AdminService.API.Middleware;
using EV.AdminService.API.Models.DataModels;
using EV.AdminService.API.Repositories.Implements;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Implements;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Key) ||
    string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    var missing = new List<string>();
    if (string.IsNullOrEmpty(jwtSection["Key"])) missing.Add("JwtSettings:Key (env JwtSettings__Key)");
    if (string.IsNullOrEmpty(jwtSection["Issuer"])) missing.Add("JwtSettings:Issuer (env JwtSettings__Issuer)");
    if (string.IsNullOrEmpty(jwtSection["Audience"])) missing.Add("JwtSettings:Audience (env JwtSettings__Audience)");
    Console.WriteLine("ERROR: Missing JWT settings: " + string.Join(", ", missing));
    throw new Exception("Invalid JWT settings in configuration. Missing: " + string.Join(", ", missing));
}
builder.Services.AddSingleton<JwtSettings>(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddDbContext<EVDataAnalyticsMarketplaceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

builder.Services.AddControllers();

//builder.Services.AddDataProtection();
//builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServicesProvider, ServicesProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IModerationService, ModerationService>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
//builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddSingleton(Metrics.CreateCounter("payments_processed_total", "Total payments processed"));
builder.Services.AddSingleton<JwtHelper>();

//builder.Services.AddScoped<DataProtectorHelper>();

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

app.Run();
