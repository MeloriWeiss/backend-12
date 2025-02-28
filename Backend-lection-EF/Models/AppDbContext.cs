using Backend_lection_EF.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_lection_EF.Models;

public class AppDbContext : DbContext
{
    public DbSet<Entity> Entities { get; set; }

    // реализуем контекст базы данных, с которым будем работать во всём приложении
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
                // задаём строку подключения
            .UseNpgsql("Host=localhost;Port=5432;Database=TestBackend;Username=postgres;Password=postgres;")
                // создаём сид (заполнение базы данных демонстрационными данными)
            .UseSeeding((context, _) =>
            {
                // проверяем, есть ли сущность с полем Name "Мария" в базе данных
                var testBlog = context.Set<Entity>().FirstOrDefault(entity => entity.Name == "Мария");
                if (testBlog != null) return;
                // если сущности нет и функция не завершилась, то добавляем данные в базу
                context.Set<Entity>().Add(new Entity { Name = "Мария" });
                context.SaveChanges();
            })
                // по рекомендациям Microsoft реализуем аналогичную логику, но уже асинхронно
                // (при этом для правильного заполнения нужно обязательно использовать синхронный метод UseSeeding)
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var testBlog = await context.Set<Entity>().FirstOrDefaultAsync(b => b.Name == "Мария", cancellationToken);
                if (testBlog == null)
                {
                    context.Set<Entity>().Add(new Entity { Name = "Мария" });
                    await context.SaveChangesAsync(cancellationToken);
                }
            });
    }

    // protected override void OnModelCreating()
    // {
    //     
    // }
}