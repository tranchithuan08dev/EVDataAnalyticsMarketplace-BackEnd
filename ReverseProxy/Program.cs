using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseRouting();
app.UseHttpMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/admin/swagger/v1/swagger.json", "Admin API v1");
        c.SwaggerEndpoint("/api/consumer/swagger/v1/swagger.json", "Consumer API v1");
        c.SwaggerEndpoint("/api/provider/swagger/v1/swagger.json", "Provider API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

app.MapMetrics();

app.Run();
