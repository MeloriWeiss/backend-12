using System.Net;
using System.Text;
using Backend_lection_EF.Models;
using Backend_lection_EF.Models.Data;
using Backend_lection_EF.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend_lection_EF.Controllers;

[ApiController]
[Route("api/[action]")]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    // с помощью Dependency Injection получаем экземпляр контекста базы данных
    public FilesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{id:int}")]
    public IActionResult GetJson(int id)
    {
        try
        {
            // получаем данные из базы данных
            var entity = _dbContext.Entities.Find(id);
            // если данных не найдено, возвращаем статус-код 404
            if (entity is null)
            {
                return NotFound();
            }

            // возвращаем данные json со статус-кодом 200
            return Ok(entity);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return BadRequest("Ошибка");
        }
    }

    [HttpGet]
    public IResult GetHtml()
    {
        // с помощью созданного класса HtmlResult возвращаем html
        const string htmlContent = "<h1>Имя сотрудника:</h1><span>Иван</span>";
        return Results.Extensions.Html(@$"<!doctype html> <html><body>{htmlContent}</body></html>");
    }

    [HttpGet]
    public async Task<IResult> GetFile()
    {
        try
        {
            // путь к существующему файлу
            const string path = "Public/vk.jpg";
            // считываем файл в массив байтов
            var fileContent = await System.IO.File.ReadAllBytesAsync(path);
            // устанавливаем тип контента
            const string contentType = "image/jpeg";
            // задаём название скачиваемому файлу (может отличаться от "базового" названия)
            const string downloadName = "vk.jpg";
            // возвращаем файл
            return Results.File(fileContent, contentType, downloadName);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return Results.BadRequest(e);
        }
    }

    [HttpGet]
    public IResult GetFileWithStream()
    {
        try
        {
            // путь к существующему файлу
            const string path = "Public/vk.jpg";
            // создаём поток данных, чтобы не забивать память единоразово, если файл большой
            var fileStream = new FileStream(path, FileMode.Open);
            // устанавливаем тип контента
            const string contentType = "image/jpeg";
            // задаём название скачиваемому файлу (может отличаться от "базового" названия)
            const string downloadName = "vk.jpg";
            // возвращаем файл
            return Results.File(fileStream, contentType, downloadName);
        }
        catch (Exception e)
        {
            // в случае ошибки возвращаем статус-код 400 с сообщением "Ошибка"
            Console.WriteLine(e);
            return Results.BadRequest(e);
        }
    }
}