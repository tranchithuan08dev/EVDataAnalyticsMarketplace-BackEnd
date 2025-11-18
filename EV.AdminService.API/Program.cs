using Amazon.S3;
using EV.AdminService.API.AI.Services.BackgroundServices;
using EV.AdminService.API.AI.Services.Implements;
using EV.AdminService.API.AI.Services.Interfaces;
using EV.AdminService.API.Models;
using EV.AdminService.API.Repositories.Implements;
using EV.AdminService.API.Repositories.Interfaces;
using EV.AdminService.API.Services.Implements;
using EV.AdminService.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using OfficeOpenXml;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialPersonal("EVDataAnalyticsMarketplace");

// Add services to the container.
builder.Services.AddDbContext<EVDataAnalyticsMarketplaceDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var allowedOriginsEnv = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "http://localhost:5173";
Console.WriteLine($"ALLOWED_ORIGINS='{allowedOriginsEnv}'");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var origins = allowedOriginsEnv
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Trim().TrimEnd('/'))
            .Where(o => !string.IsNullOrEmpty(o))
            .ToArray();

        if (origins.Length == 1 && origins[0] == "*")
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            Console.WriteLine("CORS: AllowAnyOrigin (ALLOWED_ORIGINS='*').");
        }
        else
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var r2Config = builder.Configuration.GetSection("CloudflareR2");
var s3Client = new AmazonS3Client(
    r2Config["AccessKey"],
    r2Config["SecretKey"],
    new AmazonS3Config
    {
        ServiceURL = r2Config["EndpointUrl"],
        AuthenticationRegion = "us-east-1",
        ForcePathStyle = true
    }
);

builder.Services.AddSingleton<IAmazonS3>(s3Client);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServicesProvider, ServicesProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IDataAnalysisService, DataAnalysisService>();
builder.Services.AddScoped<IAdminModerationService, AdminModerationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IProviderImportService, ProviderImportService>();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
        };
    });

// CORS để Gateway gọi qua được
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

builder.Services.AddSingleton<MLContext>();
builder.Services.AddHostedService<DataQualityProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
// ✅ Cho phép CORS
app.UseCors("AllowGateway");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
