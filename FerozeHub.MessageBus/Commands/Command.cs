using FerozeHub.MessageBus.Events;

namespace FerozeHub.MessageBus.Commands;


public abstract class Command:Message
{
    public DateTime Timestamp {get; protected set; }

    protected Command()
    {
        Timestamp = DateTime.Now;
    }
}