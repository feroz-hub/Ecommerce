using System.Net;
using System.Text;
using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;
using Newtonsoft.Json;

namespace FerozeHub.Web.Service.Implementations;

public class BaseService(IHttpClientFactory clientFactory,ITokenProvider tokenProvider) : IBaseService
{
    public async Task<ResponseDto?> SendAsync(RequestDto request,bool withBearur=true)
    {
        try
        {
            HttpClient client = clientFactory.CreateClient("FerozeHub");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            if (withBearur)
            {
                var token = tokenProvider.GetToken();
                message.Headers.Add("Authorization", $"Bearer {token}");
            }
            //token
            message.RequestUri = new Uri(request.Url);
            if (request.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8,
                    "application/json");
            }

            HttpResponseMessage apiresponse = null;

            switch (request.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiresponse = await client.SendAsync(message);

            switch (apiresponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Access Denied" };
                case HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case HttpStatusCode.InternalServerError:
                    return new() { IsSuccess = false, Message = "Internal Server Error" };
                default:
                    var apiContent = await apiresponse.Content.ReadAsStringAsync();
                    var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return apiResponseDto;
            }
        }
        catch (Exception exception)
        {
            var response = new ResponseDto()
            {
                IsSuccess = false,
                Message = exception.Message.ToString(),
            };
            return response;
        }

    }
}

