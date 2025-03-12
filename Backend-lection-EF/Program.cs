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
        // указываем адреса, с которых мы сможем выполнять запросы к нашему источнику
        policy.WithOrigins("http://localhost:4200");
        policy.WithOrigins("http://localhost:5173");
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

// чтобы не создавать контроллеры, мы можем создать роуты через Map, на которые будут приходить запросы
// при этом можем так же писать логику, которая будет выполняться при обращении
// некоторые примеры роутов:
app.Map("/about", () => "About page");
app.MapPost("/user", () => "Name");
// можем указывать параметры
app.MapPost("/user/{id}", (string id) => $"Name of user {id}");
// можем создавать ограничения на параметры (при неправильных параметрах будет выдаваться либо 404 ошибка, либо выполняться роут по умолчанию)
app.MapPost("/car/{id:range(10, 15)}", (int id) => $"Name of user {id}");
// можем делать параметры необязательными
app.MapPost("/admin/{id?}", (string? id) => id is not null ? $"Name of user {id}" : "");

app.UseHttpsRedirection();

// добавляем политику Cors в приложение
app.UseCors();
app.MapControllers();
app.Run();