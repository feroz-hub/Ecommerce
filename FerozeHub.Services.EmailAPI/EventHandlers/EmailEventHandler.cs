using FerozeHub.MessageBus.Bus;
using FerozeHub.Services.EmailAPI.Events;
using FerozeHub.Services.EmailAPI.Interface;
using FerozeHub.Services.EmailAPI.Models.Dto;

namespace FerozeHub.Services.EmailAPI.EventHandlers;

public class EmailEventHandler(IEmailService emailService):IEventHandler<EmailCreatedEvent>
{
    public  Task Handle(EmailCreatedEvent @event)
    {
        CartDto cartDto = @event.Cart;
        var email = emailService.SendAsync(cartDto);
        return Task.CompletedTask;

    }
}