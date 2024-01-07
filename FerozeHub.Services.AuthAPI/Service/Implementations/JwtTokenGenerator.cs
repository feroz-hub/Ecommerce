using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FerozeHub.Services.AuthAPI.Models;
using FerozeHub.Services.AuthAPI.Service.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FerozeHub.Services.AuthAPI.Service.Implementations;

public class JwtTokenGenerator(IOptions<JwtOptions> jwtOptions):IJwtTokenGenerator
{
    public string GenerateToken(ApplicationUser applicationUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
 
        var key = Encoding.ASCII.GetBytes(jwtOptions.Value.Secret);
 
        var claimList = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Email, applicationUser.Email),
            new (JwtRegisteredClaimNames.Sub,applicationUser.Id),
            new (JwtRegisteredClaimNames.Name, applicationUser.Name.ToString()),
        };
 
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = jwtOptions.Value.Audience,
            Issuer = jwtOptions.Value.Issuer,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token=tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
 
    }
}