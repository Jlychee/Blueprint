using Client.Models.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public sealed class UserCookieBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var cookies = context.HttpContext.Request.Cookies;

        var model = new UserCookie
        {
            MetricUserId = Parse(cookies["metric_user_id"]),
            FilterSessionId = Parse(cookies["filter_session_id"])
        };

        context.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }

    private static Guid Parse(string? raw) =>
        Guid.TryParse(raw, out var value) ? value : Guid.Empty;
}