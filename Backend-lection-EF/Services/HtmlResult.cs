namespace Backend_lection_EF.Services;

public class HtmlResult(string htmlCode) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        // устанавливаем заголовок, кодировку и отправляем html
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(htmlCode);
    }
}

static class ResultsExtensions
{
    // создаём метод расширения (для более гибкого управления возможными ответами), который будем использовать при отправке html
    public static IResult Html(this IResultExtensions resultExtensions, string html)
    {
        // создаём исключение, если resultExtensions - null;
        ArgumentNullException.ThrowIfNull(resultExtensions);
        return new HtmlResult(html);
    }
}