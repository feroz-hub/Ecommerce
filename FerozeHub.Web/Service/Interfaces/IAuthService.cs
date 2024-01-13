using FerozeHub.Web.Models.Dto;

namespace FerozeHub.Web.Service.Interfaces;

public interface IAuthService
{
    Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
    Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registerRequestDto);
    Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registerRequestDto);
}