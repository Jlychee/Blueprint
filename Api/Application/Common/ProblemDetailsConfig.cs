namespace Api.Application.Common;

public static class ProblemDetailsConfig
{
    public static void Configure(ProblemDetailsContext ctx)
    {
        var ex = ctx.Exception;
        while (ex.InnerException != null &&
               (ex is AggregateException or System.Reflection.TargetInvocationException))
        {
            ex = ex.InnerException;
        }

        var statusCode = ex switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        ctx.HttpContext.Response.StatusCode = statusCode;
        ctx.ProblemDetails.Status = statusCode;

        ctx.ProblemDetails.Title = ctx.ProblemDetails.Status switch
        {
            404 => "Ресурс не найден",
            _ => "Внутренняя ошибка сервера"
        };

        ctx.ProblemDetails.Detail = null;
    }
}