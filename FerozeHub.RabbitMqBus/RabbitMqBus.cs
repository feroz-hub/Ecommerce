using System.Text;
using FerozeHub.MessageBus.Bus;
using FerozeHub.MessageBus.Commands;
using FerozeHub.MessageBus.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FerozeHub.RabbitMqBus;

public class RabbitMqBus : IEventBus
{
    private readonly IMediator _mediator;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;
    private readonly IServiceScopeFactory _scopeFactory;


    public RabbitMqBus(IMediator mediator, IServiceScopeFactory scopeFactory)
    {
        _mediator = mediator;
        _scopeFactory = scopeFactory;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();


    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _mediator.Send(command);
    }

    public void Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
        };
        using var connection = factory.CreateConnection();
        using (var channel = connection.CreateChannel())
        {
            var eventName = @event.GetType().Name;
            channel.QueueDeclare(eventName, false, false, false, null);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("", eventName, body);
        }

    }

    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s.GetType() == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already is registered for {eventName}",
                nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsume<T>();
    }

    private void StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            DispatchConsumersAsync = true
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateChannel();

        var eventname = typeof(T).Name;

        channel.QueueDeclare(eventname, false, false, false, null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += Consumer_Recieved;

        channel.BasicConsume(eventname, true, consumer);
    }

    private async Task Consumer_Recieved(object sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);
        }
        catch (Exception ex)
        {


        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.ContainsKey(eventName))
        {
            var scope = _scopeFactory.CreateScope();
            var subscriptions = _handlers[eventName];
            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription);
                if (handler == null) continue;
                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                
                var conreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                var handleMethod = conreteType.GetMethod("Handle");
               
                if (handleMethod == null)
                {
                    continue; // Handle method not found, skip this subscription
                }


                var @event = JsonConvert.DeserializeObject(message,eventType);
                
                await (Task)conreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
            }

        }

    }
}