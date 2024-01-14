using FerozeHub.Services.EmailAPI.Models.Dto;

namespace FerozeHub.Services.EmailAPI.Interface;

public interface IEmailService
{
    public Task SendAsync(CartDto cartDto);
}