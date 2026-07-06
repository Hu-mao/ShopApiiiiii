namespace Shop.Api.Middlewares;

public class RequestTimerMiddleware(ILogger<RequestTimerMiddleware> _logger, RequestDelegate _next)
{

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // 1. Код ДО наступного компонента
        _logger.LogInformation("Початок запиту: {Path}", context.Request.Path);

        // 2. Передаємо керування далі
        await _next(context);

        // 3. Код ПІСЛЯ того, як відпрацював контролер
        watch.Stop();
        _logger.LogInformation("Запит завершено за {Ms} мс", watch.ElapsedMilliseconds);
    }
}
