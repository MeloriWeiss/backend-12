using Backend_lection_EF.Contracts.Entities;
using Backend_lection_EF.Models;
using Backend_lection_EF.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend_lection_EF.Controllers;

// делаем с помощью атрибута класс контроллером (для возможности обработки запросов)
[ApiController]
// создаём роут (адрес в url-строке), на который будут поступать запросы
// атрибут [action] означает название метода
[Route("api/Entity")]
public class CrudController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    // с помощью Dependency Injection получаем экземпляр контекста базы данных
    public CrudController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // осуществляем обработку POST-запроса по адресу api/Entity
    [HttpPost]
    public IActionResult AddEntity([FromBody] CreateEntityRequest request)
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // создаём новые данные и добавляем их в базу данных через контекст
            Entity entity = new Entity() {Name = request.Name, Surname = request.Surname};
            _dbContext.Entities.Add(entity);
            _dbContext.SaveChanges();
            // возвращаем созданный экземпляр со статус-кодом 200 (операция выполнена успешно)
            return Ok(entity);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return BadRequest("Ошибка");
        }
    }

    // осуществляем обработку GET-запроса по адресу api/Entity
    [HttpGet]
    public IActionResult GetEntities()
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // получаем все сущности из базы данных
            var entities = _dbContext.Entities.ToList();
            // возвращаем сущности со статус-кодом 200
            return Ok(entities);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return BadRequest("Ошибка");
        }
    }
    
    // осуществляем обработку GET-запроса по адресу api/Entity/{id}
    // если бы в атрибуте HttpGet мы не указывали {id:int}, и оставили бы 'int id' в параметрах метода, то id брался бы из query-параметров
    // так же мы ставим ограничение на то, что id должен быть int, чтобы запрос отработал
    [HttpGet("{id:int}")]
    public IActionResult GetEntity(int id)
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // получаем данные из базы данных
            var entity = _dbContext.Entities.Find(id);
            // если данных не найдено, возвращаем статус-код 404
            if (entity is null)
            {
                return NotFound();
            }
            // возвращаем данные со статус-кодом 200
            return Ok(entity);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return BadRequest("Ошибка");
        }
    }
    
    // осуществляем обработку Put-запроса по адресу api/Entity/{id}
    // ставим ограничение на то, что id должен быть int, чтобы запрос отработал
    [HttpPut("{id:int}")]
    public IActionResult UpdateEntity(int id, [FromBody] UpdateEntityRequest request)
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // получаем данные из базы данных
            var entity = _dbContext.Entities.Find(id);
            // если данных не найдено, возвращаем статус-код 404
            if (entity is null)
            {
                return NotFound();
            }
            // изменяем данные в соответствии с пришедшими из запроса
            if (request.Name is not null)
            {
                entity.Name = request.Name;
            }
            if (request.Surname is not null)
            {
                entity.Surname = request.Surname;
            }

            _dbContext.Entities.Update(entity);
            _dbContext.SaveChanges();
            
            // возвращаем данные со статус-кодом 200
            return Ok(entity);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return BadRequest("Ошибка");
        }
    }
    
    // осуществляем обработку Delete-запроса по адресу api/Entity/{id}
    // ставим ограничение на то, что id должен быть int, чтобы запрос отработал
    [HttpDelete("{id:int}")]
    public IActionResult DeleteEntity(int id)
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // получаем данные из базы данных
            var entity = _dbContext.Entities.Find(id);
            // если данных не найдено, возвращаем статус-код 404
            if (entity is null)
            {
                return NotFound();
            }
            _dbContext.Entities.Remove(entity);
            _dbContext.SaveChanges();
            
            // возвращаем статус-код 200
            return Ok();
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return BadRequest("Ошибка");
        }
    }
}