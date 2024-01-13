using FerozeHub.Services.AuthAPI.Models.Dto;

namespace FerozeHub.Services.AuthAPI.Service.Interfaces;

public interface IAuthService
{
    Task<string> Register(RegistrationRequestDto registrationRequestDto);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<bool> AssignRole(string email, string roleName);
}