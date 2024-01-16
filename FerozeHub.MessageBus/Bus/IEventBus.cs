using FerozeHub.MessageBus.Commands;
using FerozeHub.MessageBus.Events;

namespace FerozeHub.MessageBus.Bus;

public interface IEventBus
{
    Task SendCommand<T>(T command) where T : Command;

    void Publish<T>(T @event) where T : Event;
    Task Publish<T>(T @event, string topic) where T : Event;

    

    void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>;
    
    void SubscribeToMqtt<T, TH>() where T : Event where TH : IEventHandler<T>;

}