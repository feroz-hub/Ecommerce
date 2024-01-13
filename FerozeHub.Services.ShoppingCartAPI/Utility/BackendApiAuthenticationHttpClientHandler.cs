using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace FerozeHub.Services.ShoppingAPI.Utility;

public class BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor):DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await httpContextAccessor.HttpContext.GetTokenAsync("access-token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
    
}