var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// CORS để Gateway gọi qua được
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseCors("AllowFrontend");
// ✅ Cho phép CORS
app.UseCors("AllowGateway");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 

app.MapReverseProxy();

app.Run();
