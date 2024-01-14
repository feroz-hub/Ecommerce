using FerozeHub.Services.ShoppingAPI.Models.Dto;

namespace FerozeHub.Services.ShoppingAPI.Service.Interface;

public interface IEmailService
{
    void EmailCartSend(CartDto cart);
}