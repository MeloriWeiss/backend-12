// using System.Text.Json;
// using Backend_lection_EF.Contracts.Entities;
// using Backend_lection_EF.Models;
// using Backend_lection_EF.Models.Data;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Backend_lection_EF.Controllers;
//
// // делаем с помощью атрибута класс контроллером (для возможности обработки запросов)
// [ApiController]
// // создаём роут (адрес в url-строке), на который будут поступать запросы
// // атрибут [action] означает название метода
// [Route("api/[controller]/[action]")]
// public class WithDiskCacheController : ControllerBase
// {
//     private readonly AppDbContext _dbContext;
//     private readonly ILogger<WithDiskCacheController> _logger;
//
//     private readonly string _cacheFilePath;
//     private readonly TimeSpan _cacheLifetimeInMinutes = TimeSpan.FromMinutes(10);
//
//     // с помощью Dependency Injection получаем экземпляр контекста базы данных
//     public WithDiskCacheController(AppDbContext dbContext,
//         IHostEnvironment environment,
//         ILogger<WithDiskCacheController> logger)
//     {
//         _dbContext = dbContext;
//         _logger = logger;
//
//         // создаём путь файла с кэшем
//         _cacheFilePath = Path.Combine(environment.ContentRootPath, "CacheData", "entities.cache.json");
//
//         var cacheDirectoryPath = Path.GetDirectoryName(_cacheFilePath);
//         if (!Directory.Exists(cacheDirectoryPath))
//         {
//             // создаём директорию для файла с кэшем, если её нет
//             Directory.CreateDirectory(cacheDirectoryPath!);
//         }
//     }
//
//     // осуществляем обработку POST-запроса по адресу api/AddAntity
//     [HttpPost]
//     public async Task<IActionResult> AddEntity([FromBody] CreateEntityRequest request)
//     {
//         // пытаемся выполнить операцию с базой данных
//         try
//         {
//             // создаём новые данные и добавляем их в базу данных через контекст
//             Entity entity = new Entity() { Name = request.Name };
//             
//             await _dbContext.Entities.AddAsync(entity);
//             await _dbContext.SaveChangesAsync();
//
//             // если файл существует
//             if (System.IO.File.Exists(_cacheFilePath))
//             {
//                 try
//                 {
//                     // записываем кэш
//                     await System.IO.File.WriteAllTextAsync(_cacheFilePath, string.Empty);
//                 }
//                 catch (Exception exception)
//                 {
//                     _logger.LogError(exception, "Ошибка очищения кэша");
//                     // инвалидируем кэш, если записать не удалось
//                     System.IO.File.Delete(_cacheFilePath);
//                 }
//             }
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
//         // пытаемся выполнить операцию с базой данных
//         try
//         {
//             // если файл существует
//             if (System.IO.File.Exists(_cacheFilePath))
//             {
//                 try
//                 {
//                     // получаем информацию о файле
//                     var fileInfo = new FileInfo(_cacheFilePath);
//                     // если кэш действителен
//                     if (DateTime.Now - fileInfo.LastWriteTime < _cacheLifetimeInMinutes)
//                     {
//                         // читаем кэш
//                         var jsonEntities = await System.IO.File.ReadAllTextAsync(_cacheFilePath);
//                         // сериализуем из строки в json и отдаём на клиент
//                         return Ok(JsonSerializer.Deserialize<List<Entity>>(jsonEntities) ?? []);
//                     }
//                     // если кэш не действителен, инвалидируем
//                     System.IO.File.Delete(_cacheFilePath);
//                 }
//                 catch (Exception exception)
//                 {
//                     _logger.LogError(exception, "Ошибка чтения кэша");
//                 }
//             }
//
//             var entities = _dbContext.Entities.ToList();
//
//             if (entities.Count == 0)
//                 return Ok(new List<Entity>());
//
//             try
//             {
//                 // конвертируем json в строку
//                 var stringEntities = JsonSerializer.Serialize(entities);
//                 // записываем в файл
//                 await System.IO.File.WriteAllTextAsync(_cacheFilePath, stringEntities);
//             }
//             catch (Exception exception)
//             {
//                 _logger.LogError(exception, "Ошибка записи кэша в файл");
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