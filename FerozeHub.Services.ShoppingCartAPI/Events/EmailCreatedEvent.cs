using FerozeHub.MessageBus.Events;
using FerozeHub.Services.ShoppingAPI.Models.Dto;

namespace FerozeHub.Services.ShoppingAPI.Events;

public class EmailCreatedEvent:Event
{
    public CartDto Cart { get;protected set; }
    public EmailCreatedEvent(CartDto cartDto)
    {
        Cart=cartDto;
    }
}