using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;

namespace FerozeHub.Web.Service.Implementations;

public class AuthService(IBaseService baseService):IAuthService
{
    public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.POST,
            Data = loginRequestDto,
            Url = SD.AuthAPI+"/api/authAPI/login"
        },withBearer:false);

    }

    public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registerRequestDto)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.POST,
            Data = registerRequestDto,
            Url = SD.AuthAPI + "/api/authAPI/register"
        },withBearer:false);
    }

    public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registerRequestDto)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.POST,
            Data = registerRequestDto,
            Url = SD.AuthAPI + "/api/authAPI/assignRole"
        });
    }
}