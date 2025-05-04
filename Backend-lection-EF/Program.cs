using Backend_lection_EF.Models;
using Backend_lection_EF.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Добавляем использование внутреннего кэша
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>();

// конфигурируем sql server для кэша
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("CacheDatabase");
    options.SchemaName = "dbo";
    options.TableName = "CacheTable";
    // время истечения кэша по умолчанию (скользящего)
    options.DefaultSlidingExpiration = TimeSpan.FromMinutes(5);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Configuration
        // указываем путь к файлу json
    .SetBasePath(Directory.GetCurrentDirectory())
        // добавляем файл, делая его обязательным
        // (если не найдётся, то будет выброшено исключение) и чувствительным к изменениям
        // (приложение будет реагировать на изменения без перезапуска)
    .AddJsonFile("config.json", false, true)
        // добавим поддержку переменных среды для получения через IConfiguration
    .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    // настриваем минимальный уровень логирования
    .MinimumLevel.Debug()
    // логируем в консоль
    .WriteTo.Console()
    // одновременно делаем запись в файл, при этом каждый день будет создаваться новый файл с датой в названии
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    // создаём логгер
    .CreateLogger();

// используем serilog в качестве логгера
builder.Host.UseSerilog();

builder.Services.AddSwaggerGen();

var MainPolicy = "_mainPolicy";
builder.Services.AddCors(options =>
{
    // добавляем настройки Cors для приложения по умолчанию
    options.AddDefaultPolicy(policy =>
    {
        // указываем адреса, с которых мы сможем выполнять запросы к нашему источнику
        policy.WithOrigins("http://localhost:4200");
        // policy.WithOrigins("http://localhost:5173");
        // разрешаем использовать все заголовки запросов
        policy.AllowAnyHeader();
        // разрешаем использовать все методы запросов
        policy.AllowAnyMethod();
    });
    
    // добавляем настройки Cors для приложения с именем
    options.AddPolicy(name: MainPolicy,
        policy =>
        {
            // указываем адреса, с которых мы сможем выполнять запросы к нашему источнику
            policy.WithOrigins("http://localhost:4200");
            policy.WithOrigins("http://localhost:5173");
            // разрешаем использовать все заголовки запросов (можно указать конкретные через WithHeaders)
            policy.AllowAnyHeader();
            // разрешаем использовать все методы запросов (можно указать конкретные через WithMethods)
            policy.AllowAnyMethod();
        });
});
// также перед методом или контроллером можно использовать атрибут [EnableCors("{имя политики}")],
// чтобы активировать cors для конкретного метода или контроллера,
// однако не рекомендуется использовать политику, применённую и черзе middleware, и через атрибуты, потому что будут
// применены сразу обе политики
// отключать cors для метода или контроллера можно при помощи атрибута [DisableCors]

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
}

// чтобы не создавать контроллеры, мы можем создать роуты через Map, на которые будут приходить запросы
// при этом можем так же писать логику, которая будет выполняться при обращении
// некоторые примеры роутов:
// app.Map("/about", () => "About page");
// app.MapPost("/user", () => "Name");
// // можем указывать параметры
// app.MapPost("/user/{id}", (string id) => $"Name of user {id}");
// // можем создавать ограничения на параметры (при неправильных параметрах будет выдаваться либо 404 ошибка, либо выполняться роут по умолчанию)
// app.MapPost("/car/{id:range(10, 15)}", (int id) => $"Name of user {id}");
// // можем делать параметры необязательными
// app.MapPost("/admin/{id?}", (string? id) => id is not null ? $"Name of user {id}" : "");

app.UseHttpsRedirection();

// добавляем политику Cors в приложение с указанием названия, тем самым активируя cors middleware
// если ничего не передавать в параметры UseCors, то будет использоваться указанная политика по умолчанию
app.UseCors(MainPolicy);
app.MapControllers();
app.Run();