using FerozeHub.MessageBus.Bus;
using FerozeHub.Services.ShoppingAPI.Commands;
using FerozeHub.Services.ShoppingAPI.Events;
using MediatR;

namespace FerozeHub.Services.ShoppingAPI.CommandHandlers;

public class EmailCommandHandler(IEventBus eventBus) : IRequestHandler<CreateEmailCommand, bool>
{
    public Task<bool> Handle(CreateEmailCommand request, CancellationToken cancellationToken)
    {
        eventBus.Publish(new EmailCreatedEvent(request.cartDto));
        return Task.FromResult(true);
    }
}
