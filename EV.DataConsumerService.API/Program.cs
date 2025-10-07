using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Data.Repositories;
using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EV.DataConsumerService.API.Data.EvdataAnalyticsMarketplaceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 1. Đăng ký Services & Repositories
builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<IDatasetService, DatasetService>();
// builder.Services.AddDbContext<ApplicationDbContext>(/*...*/);

// 2. Tạo EdmModel cho OData
var modelBuilder = new ODataConventionModelBuilder();
// Đăng ký EntitySet mà Controller sẽ sử dụng. Tên phải khớp với route.
modelBuilder.EntitySet<DatasetSearchResultDto>("Datasets");

// 3. Đăng ký OData

builder.Services.AddControllers()
    .AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()
    ));



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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
