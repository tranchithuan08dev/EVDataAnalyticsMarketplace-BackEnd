using EV.DataProviderService.API.Data;
using EV.DataProviderService.API.Data.IRepositories;
using EV.DataProviderService.API.Data.Repositories;
using EV.DataProviderService.API.Repositories;
using EV.DataProviderService.API.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// 1. Đăng ký Services & Repositories
builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>(); 
builder.Services.AddScoped<IRevenueService, RevenueService>();
// 2. Đăng ký DbContext với các tùy chọn cần thiết cho Docker
builder.Services.AddDbContext<EvdataAnalyticsMarketplaceDbContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null); 
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline., (SỬA) 
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();               
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EV Data Provider API V1"));
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();