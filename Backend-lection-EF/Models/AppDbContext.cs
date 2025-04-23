using Backend_lection_EF.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_lection_EF.Models;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Entity> Entities { get; set; }

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // реализуем контекст базы данных, с которым будем работать во всём приложении
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var baseConnectionString = _configuration.GetConnectionString("Database");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var connectionString = baseConnectionString?.Replace("password", password);

        optionsBuilder
            // задаём строку подключения
            .UseNpgsql(_configuration.GetConnectionString("Database"));
        // создаём сид (заполнение базы данных демонстрационными данными)
        // .UseSeeding((context, _) =>
        // {
        //     // проверяем, есть ли сущность с полем Name "Мария" в базе данных
        //     var testBlog = context.Set<Entity>().FirstOrDefault(entity => entity.Name == "Мария");
        //     if (testBlog != null) return;
        //     // если сущности нет и функция не завершилась, то добавляем данные в базу
        //     context.Set<Entity>().Add(new Entity { Name = "Мария", Surname = "Волжанова" });
        //     context.SaveChanges();
        // })
        //     // по рекомендациям Microsoft реализуем аналогичную логику, но уже асинхронно
        //     // (при этом для правильного заполнения нужно обязательно использовать синхронный метод UseSeeding)
        // .UseAsyncSeeding(async (context, _, cancellationToken) =>
        // {
        //     var testBlog = await context.Set<Entity>().FirstOrDefaultAsync(b => b.Name == "Мария", cancellationToken);
        //     if (testBlog == null)
        //     {
        //         context.Set<Entity>().Add(new Entity { Name = "Мария", Surname = "Волжанова" });
        //         await context.SaveChangesAsync(cancellationToken);
        //     }
        // });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entity>(e =>
        {
            e.Property(x => x.Name).IsRequired();
            e.HasData(
                new Entity { Id = 1, Name = "Мария", Surname = "Волжанова" },
                new Entity { Id = 2, Name = "Иван", Surname = "Остапов" },
                new Entity { Id = 3, Name = "Владимир", Surname = "Волновой" });
        });
    }
}