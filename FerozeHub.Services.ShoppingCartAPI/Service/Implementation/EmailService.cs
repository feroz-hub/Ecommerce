using FerozeHub.MessageBus.Bus;
using FerozeHub.Services.ShoppingAPI.Commands;
using FerozeHub.Services.ShoppingAPI.Models.Dto;
using FerozeHub.Services.ShoppingAPI.Service.Interface;

namespace FerozeHub.Services.ShoppingAPI.Service.Implementation;

public class EmailService(IEventBus eventBus):IEmailService
{
    public void EmailCartSend(CartDto cart)
    {
        var createEmailCommand = new CreateEmailCommand(cart);
        
        eventBus.SendCommand(createEmailCommand);
    }
}