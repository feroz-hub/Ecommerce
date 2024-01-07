using FerozeHub.Services.AuthAPI.Models.Dto;
using FerozeHub.Services.AuthAPI.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FerozeHub.Services.AuthAPI.Controllers;

[Route("api/authAPI")]
[ApiController]
public class AuthAPIController(IAuthService authService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
    {
        var errorMessage=await authService.Register(registrationRequestDto);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            return CreateResponse(null, false, errorMessage);
        }
 
        return CreateResponse(null,true,"Registered SuccessFully");
    }
 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var loginResposne=await authService.Login(loginRequestDto);
 
        if(loginResposne.User==null)
        {
            return CreateResponse(null, false, "UserName or Password is Incorrect");
        }
 
        return CreateResponse(loginResposne, true, "Logged In Successfully");
    }
 
}
