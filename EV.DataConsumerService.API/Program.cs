using EV.DataConsumerService.API.Data;
using EV.DataConsumerService.API.Data.IRepositories;
using EV.DataConsumerService.API.Data.Repositories;
using EV.DataConsumerService.API.Models.DTOs;
using EV.DataConsumerService.API.Models.Entities;
using EV.DataConsumerService.API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using System.Text;

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


    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    // ... (ở đầu file Program.cs)

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                          policy =>
                          {
                              // ⭐️ Sử dụng AllowAnyOrigin() để cho phép mọi domain
                              policy.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
    });


    // Trong khối builder.Services.AddOpenTelemetry()

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder => tracerProviderBuilder
            // 1. Tracing cho HTTP Request
            .AddAspNetCoreInstrumentation()

            // Bổ sung: Ghi lại các lệnh gọi HTTP ra bên ngoài (tới ProviderService, v.v.)
            .AddHttpClientInstrumentation()

            // Bổ sung: Ghi lại các truy vấn DB (Đảm bảo có trace ngay cả khi EF Core instrumentation lỗi)
            .AddSqlClientInstrumentation()

            // 2. SỬ DỤNG ENTITY FRAMEWORK CORE INSTRUMENTATION
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
            })

            // 3. Tên Service
            .AddSource("EV.DataConsumerService.API")
            .SetResourceBuilder(
                OpenTelemetry.Resources.ResourceBuilder.CreateEmpty()
                    .AddService("consumer-api") // Tên service bạn tìm kiếm trong Jaeger UI
            )

            // 4. Exporter (ĐÃ FIX ENDPOINT)
            .AddOtlpExporter(o =>
            {
                // CỰC KỲ QUAN TRỌNG: Chỉ sử dụng host và port cho gRPC
                o.Endpoint = new Uri("http://jaeger:4317");
                o.Protocol = OtlpExportProtocol.Grpc;
            })
        );

    // 1. Đăng ký Services & Repositories
    builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
    builder.Services.AddScoped<IDatasetService, DatasetService>();
    builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    builder.Services.AddScoped<IOrganizationService, OrganizationService>();
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

    // Đăng ký Repository và Service
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // Cấu hình JWT Authentication
    var tokenKey = builder.Configuration.GetSection("AppSettings:Token").Value;

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(tokenKey)),
                ValidateIssuer = false, // Không cần kiểm tra Issuer
                ValidateAudience = false // Không cần kiểm tra Audience
            };
        });

    var app = builder.Build();
    app.UseCors(MyAllowSpecificOrigins);


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