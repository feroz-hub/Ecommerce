using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace FerozeHub.Web.Controllers;

public class AuthController(IAuthService authService,ITokenProvider tokenProvider) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDto loginRequest = new();
        return View(loginRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
    {
            var response = await authService.LoginAsync(loginRequestDto);
            if (response != null && response.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                await SignInUserAsync(loginResponseDto);
                tokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                return View(loginRequestDto);
            }
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationRequestDto registrationRequest)
    {
        if (ModelState.IsValid)
        {
            ResponseDto response = await authService.RegisterAsync(registrationRequest);
            ResponseDto assignRole;
            if (response != null && response.IsSuccess)
            {
                registrationRequest.Role = Role.CUSTOMER.ToString();
                assignRole = await authService.AssignRoleAsync(registrationRequest);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(registrationRequest);
        }

        return View(registrationRequest);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        tokenProvider.ClearToken();
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(LoginResponseDto loginResponseDto)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt=handler.ReadJwtToken(loginResponseDto.Token);
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,jwt.Claims.FirstOrDefault(u=>u.Type==JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,jwt.Claims.FirstOrDefault(u=>u.Type==JwtRegisteredClaimNames.Sub).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,jwt.Claims.FirstOrDefault(u=>u.Type==JwtRegisteredClaimNames.Name).Value));

        identity.AddClaim(new Claim(ClaimTypes.Name,jwt.Claims.FirstOrDefault(u=>u.Type==JwtRegisteredClaimNames.Name).Value));
        identity.AddClaim(new Claim(ClaimTypes.Role,jwt.Claims.FirstOrDefault(u=>u.Type=="role").Value));

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    }
}
