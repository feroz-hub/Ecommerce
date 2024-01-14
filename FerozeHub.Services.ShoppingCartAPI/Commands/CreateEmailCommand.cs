using FerozeHub.Services.ShoppingAPI.Models.Dto;
using MediatR;

namespace FerozeHub.Services.ShoppingAPI.Commands;

public class CreateEmailCommand:EmailCommand
{
    public CreateEmailCommand(CartDto cart)
    {
        cartDto = cart;
    }
}