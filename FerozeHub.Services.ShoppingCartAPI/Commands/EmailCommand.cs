using FerozeHub.MessageBus.Commands;
using FerozeHub.Services.ShoppingAPI.Models.Dto;

namespace FerozeHub.Services.ShoppingAPI.Commands;

public class EmailCommand:Command
{
    public CartDto cartDto { get; protected set; }
}