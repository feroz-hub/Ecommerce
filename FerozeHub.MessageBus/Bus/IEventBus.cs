using FerozeHub.MessageBus.Commands;
using FerozeHub.MessageBus.Events;

namespace FerozeHub.MessageBus.Bus;

public interface IEventBus
{
    Task SendCommand<T>(T command) where T : Command;

    void Publish<T>(T @event) where T : Event;

    void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>;
}