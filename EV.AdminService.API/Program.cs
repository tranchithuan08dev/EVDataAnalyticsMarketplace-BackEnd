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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EVDataAnalyticsMarketplaceDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServicesProvider, ServicesProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IDataAnalysisService, DataAnalysisService>();
builder.Services.AddScoped<IAdminModerationService, AdminModerationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
