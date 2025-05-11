// using System.Text;
// using System.Text.Json;
// using Backend_lection_EF.Contracts.Entities;
// using Backend_lection_EF.Models;
// using Backend_lection_EF.Models.Data;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Caching.Distributed;
//
// namespace Backend_lection_EF.Controllers;
//
// // делаем с помощью атрибута класс контроллером (для возможности обработки запросов)
// [ApiController]
// // создаём роут (адрес в url-строке), на который будут поступать запросы
// // атрибут [action] означает название метода
// [Route("api/[controller]/[action]")]
// public class SqlServerCacheController : ControllerBase
// {
//     private readonly AppDbContext _dbContext;
//     private readonly IDistributedCache _distributedCache;
//     private readonly ILogger<SqlServerCacheController> _logger;
//
//     private const string _entitiesCacheKey = "entitiesList";
//
//     public SqlServerCacheController(
//         AppDbContext dbContext,
//         IDistributedCache distributedCache,
//         ILogger<SqlServerCacheController> logger)
//     {
//         _dbContext = dbContext;
//         _distributedCache = distributedCache;
//         _logger = logger;
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> AddEntity([FromBody] CreateEntityRequest request)
//     {
//         // пытаемся выполнить операцию с базой данных
//         try
//         {
//             // создаём новые данные и добавляем их в базу данных через контекст
//             Entity entity = new Entity() { Name = request.Name };
//             
//             _dbContext.Entities.Add(entity);
//             await _dbContext.SaveChangesAsync();
//
//             // Инвалидацируем кэш
//             await _distributedCache.RemoveAsync(_entitiesCacheKey);
//             
//             // возвращаем созданный экземпляр со статус-кодом 200 (операция выполнена успешно)
//             return Ok(entity);
//         }
//         catch (Exception exception)
//         {
//             // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
//             _logger.LogError(exception, "Ошибка добавления сущности");
//             return BadRequest("Ошибка");
//         }
//     }
//
//     // осуществляем обработку GET-запроса по адресу api/GetEntities
//     [HttpGet]
//     public async Task<IActionResult> GetEntities()
//     {
//         try
//         {
//             // получаем кэш в байтах
//             var cachedEntitiesBytes = await _distributedCache.GetAsync(_entitiesCacheKey);
//
//             if (cachedEntitiesBytes is not null)
//             {
//                 try
//                 {
//                     // конвертируем полученный кэш в строку
//                     var stringEntities = Encoding.UTF8.GetString(cachedEntitiesBytes);
//                     // преобразуем в json-формат
//                     var returnEntities = JsonSerializer.Deserialize<List<Entity>>(stringEntities);
//                     return Ok(returnEntities);
//                 }
//                 catch (Exception exception)
//                 {
//                     _logger.LogError(exception, "Ошибка получения кэша");
//                     // инвалидируем кэш при ошибке сериализации в json
//                     await _distributedCache.RemoveAsync(_entitiesCacheKey);
//                 }
//             }
//
//             var entities = _dbContext.Entities.ToList();
//
//             // если сущностей нет, то кэш не нужен, инвалидируем и возвращаем пустой список
//             if (entities.Count == 0)
//             {
//                 if (cachedEntitiesBytes is not null)
//                 {
//                     await _distributedCache.RemoveAsync(_entitiesCacheKey);
//                 }
//                 return Ok(new List<Entity>());
//             }
//
//             try
//             {
//                 // конвертируем полученный кэш в строку
//                 var stringEntities = JsonSerializer.Serialize(entities);
//                 // преобразуем в байты
//                 var entitiesBytes = Encoding.UTF8.GetBytes(stringEntities);
//
//                 // конфигурируем кэш, настраиваем время жизни относительно текущего времени
//                 var options = new DistributedCacheEntryOptions()
//                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
//
//                 // устанавливаем кэш
//                 await _distributedCache.SetAsync(_entitiesCacheKey, entitiesBytes, options);
//             }
//             catch (Exception exception)
//             {
//                 _logger.LogError(exception, "Ошибка записи кэша");
//             }
//             
//             // возвращаем сущности со статус-кодом 200
//             return Ok(entities);
//         }
//         catch (Exception exception)
//         {
//             // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
//             _logger.LogError(exception, "Ошибка получения сущностей");
//             return BadRequest("Ошибка получения сущностей");
//         }
//     }
// }