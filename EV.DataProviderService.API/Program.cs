using EV.DataProviderService.API.Data;
using EV.DataProviderService.API.Repositories;
using EV.DataProviderService.API.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<EvdataAnalyticsMarketplaceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();