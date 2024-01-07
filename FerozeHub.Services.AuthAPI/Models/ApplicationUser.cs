using Microsoft.AspNetCore.Identity;

namespace FerozeHub.Services.AuthAPI.Models;

public class ApplicationUser:IdentityUser
{
    public string Name { get; set; }
}