using Backend_lection_EF.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    // добавляем настройки Cors для приложения
    options.AddDefaultPolicy(policy =>
    {
        // указываем адрес, с которого мы можем выполнять различные запросы к нашему источнику
        policy.WithOrigins("http://localhost:4200");
        // разрешаем использовать заголовки запросов
        policy.AllowAnyHeader();
        // разрешаем использовать методы
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

// добавляем политику Cors в приложение
app.UseCors();
app.MapControllers();
app.Run();