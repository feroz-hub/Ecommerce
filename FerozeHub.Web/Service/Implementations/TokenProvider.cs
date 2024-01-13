using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;

namespace FerozeHub.Web.Service.Implementations;

public class TokenProvider(HttpContextAccessor httpContextAccessor):ITokenProvider
{
    public void SetToken(string token)
    {
        httpContextAccessor.HttpContext.Response.Cookies.Append(SD.TokenCookie, token);
    }

    public string? GetToken()
    {
       string? token = null;
       bool? valid =httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
       return valid is true ? token : null;
    }

    public void ClearToken()
    {
        httpContextAccessor.HttpContext.Response.Cookies.Delete(SD.TokenCookie);
    }
}