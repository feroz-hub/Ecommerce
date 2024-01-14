using FerozeHub.MessageBus.Events;
using FerozeHub.Services.EmailAPI.Models.Dto;

namespace FerozeHub.Services.EmailAPI.Events;

public class EmailCreatedEvent:Event
{
    public CartDto Cart { get; set; }
    public EmailCreatedEvent(CartDto cartDto)
    {
        Cart=cartDto;
    }
    
}