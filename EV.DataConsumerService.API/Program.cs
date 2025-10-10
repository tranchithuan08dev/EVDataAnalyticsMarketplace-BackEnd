using EV.DataConsumerService.API.Data;
using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Data.Repositories;
using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Models.Entities;
using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Prometheus;
using Serilog;

// ========================================
// 1. CẤU HÌNH SERILOG
// ========================================

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .Enrich.FromLogContext()
    .WriteTo.Console() // Log ra Console (cho Docker)
    .CreateLogger();
try
{
    Log.Information("Starting Data Provider service host...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();
    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký DbContext với các tùy chọn cần thiết cho Docker
builder.Services.AddDbContext<EvdataAnalyticsMarketplaceDbContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            // THÊM CHÍNH SÁCH THỬ LẠI (RETRY POLICY) 
            // CỰC KỲ QUAN TRỌNG trong Docker để xử lý lỗi mạng tạm thời
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null); // Tự động thêm các mã lỗi mạng phổ biến

            // Đảm bảo các thuộc tính khác như MultipleActiveResultSets được thêm vào
            // thông qua chuỗi kết nối
        });
});
// 1. Đăng ký Services & Repositories
builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
// builder.Services.AddDbContext<ApplicationDbContext>(/*...*/);

// 2. Tạo EdmModel cho OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<DatasetSearchResultDto>("Datasets");


// 3. Đăng ký OData

builder.Services.AddControllers()
   .AddOData(options => options
    .Select().Filter().OrderBy().Expand().Count().SetMaxTop(null)
    .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

// CORS để Gateway gọi qua được
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





var app = builder.Build();



// Middleware Prometheus
app.UseMetricServer();   // expose /metrics
app.UseHttpMetrics();    // track HTTP metrics automatically

app.MapGet("/", () => "Hello Prometheus!");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// ✅ Cho phép CORS
app.UseCors("AllowGateway");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    // Đảm bảo flush log buffer trước khi thoát
    Log.CloseAndFlush();
}