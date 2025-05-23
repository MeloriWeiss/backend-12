﻿using Backend_lection_EF.Contracts.Entities;
using Backend_lection_EF.Models;
using Backend_lection_EF.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Exception = System.Exception;

namespace Backend_lection_EF.Controllers;

// делаем с помощью атрибута класс контроллером (для возможности обработки запросов)
[ApiController]
// создаём роут (адрес в url-строке), на который будут поступать запросы
// атрибут [action] означает название метода
[Route("api/[action]")]
public class AppController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private string _entitiesCacheKey = "EntitiesList";

    // с помощью Dependency Injection получаем экземпляр контекста базы данных
    public AppController(AppDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    // осуществляем обработку POST-запроса по адресу api/AddAntity
    [HttpPost]
    public IActionResult AddEntity([FromBody] CreateEntityRequest request)
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // создаём новые данные и добавляем их в базу данных через контекст
            Entity entity = new Entity() { Name = request.Name };
            _dbContext.Entities.Add(entity);
            _dbContext.SaveChanges();
            
            // инвалидируем данные кэша при их изменении
            _memoryCache.Remove(_entitiesCacheKey);
            
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

    // осуществляем обработку GET-запроса по адресу api/GetEntities
    [HttpGet]
    public IActionResult GetEntities()
    {
        // пытаемся выполнить операцию с базой данных
        try
        {
            // проверяем, есть ли данные в кэше, и если есть, возвращаем их оттуда
            if (_memoryCache.TryGetValue(_entitiesCacheKey, out List<Entity>? cacheEntities))
            {
                return Ok(cacheEntities);
            }
            
            // иначе получаем все сущности из базы данных
            var entities = _dbContext.Entities.ToList();

            // конфигурируем настройки кэша
            var cacheOptions = new MemoryCacheEntryOptions()
                // устанавливаем время истечения
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                // устанавливаем приоритет при очистке при нехватке памяти
                .SetPriority(CacheItemPriority.Normal);

            // устанавливам кэш
            _memoryCache.Set(_entitiesCacheKey, entities, cacheOptions);
            
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

    [HttpGet("{fileName}")]
    public IActionResult GetCss(string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", fileName);
        if (!System.IO.File.Exists(path))
            return NotFound();
        return PhysicalFile(path, "text/css");
    }

    // осуществляем обработку GET-запроса по адресу api/GetEntity/{id}
    // если бы в атрибуте HttpGet мы не указывали {id:int}, и оставили бы 'int id' в параметрах метода, то id брался бы из query-параметров
    // так же мы ставим ограничение на то, что id должен быть int, чтобы запрос отработал
    // [HttpGet("{id:int}")]
    // public IActionResult GetEntity(int id)
    // {
    //     // пытаемся выполнить операцию с базой данных
    //     try
    //     {
    //         // получаем данные из базы данных
    //         var entity = _dbContext.Entities.Find(id);
    //         // если данных не найдено, возвращаем статус-код 404
    //         if (entity is null)
    //         {
    //             return NotFound();
    //         }
    //
    //         // возвращаем данные со статус-кодом 200
    //         return Ok(entity);
    //     }
    //     catch (Exception e)
    //     {
    //         // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
    //         Console.WriteLine(e);
    //         return BadRequest("Ошибка");
    //     }
    // }
}