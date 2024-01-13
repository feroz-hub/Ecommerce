using FerozeHub.Services.AuthAPI.Models;

namespace FerozeHub.Services.AuthAPI.Service.Interfaces;

public interface IJwtTokenGenerator
{
    public string GenerateToken(ApplicationUser applicationUser,IEnumerable<string>roles);
}