using FerozeHub.Services.AuthAPI.Data;
using FerozeHub.Services.AuthAPI.Models;
using FerozeHub.Services.AuthAPI.Models.Dto;
using FerozeHub.Services.AuthAPI.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FerozeHub.Services.AuthAPI.Service.Implementations;

public class AuthService(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwtTokenGenerator):IAuthService
{
      public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = dbContext.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
 
            bool IsValid = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null && IsValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }
            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
 
 
            };
            var token = jwtTokenGenerator.GenerateToken(user);
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token
            };
 
            return loginResponseDto;
        }
 
 
 
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToLower(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };
 
            try
            {
                var result = await userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = dbContext.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);
 
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
 
            }
            catch (Exception ex)
            {
 
            }
            return "Error Encountered";
 
 
        }
    
}