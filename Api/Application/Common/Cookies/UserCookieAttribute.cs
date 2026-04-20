using Microsoft.AspNetCore.Mvc;

public sealed class FromUserCookieAttribute : ModelBinderAttribute
{
    public FromUserCookieAttribute() : base(typeof(UserCookieBinder))
    {
    }
}