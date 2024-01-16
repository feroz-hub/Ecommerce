using System.Text;
using FerozeHub.MessageBus.Bus;
using FerozeHub.MessageBus.Commands;
using FerozeHub.MessageBus.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FerozeHub.RabbitMqBus;

public class EventBus : IEventBus
{
    private readonly IMediator _mediator;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;
    private readonly IServiceScopeFactory _scopeFactory;
    private IMqttClient _mqttClient;
    private readonly MqttFactory _mqttFactory;


    public EventBus(IMediator mediator, IServiceScopeFactory scopeFactory,IMqttClient mqttClient)
    {
        _mediator = mediator;
        _scopeFactory = scopeFactory;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
        _mqttClient = mqttClient;
        _mqttFactory=new MqttFactory();


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

    public async Task Publish<T>(T @event, string topic) where T : Event
    {
        var eventName = @event.GetType().Name;
        var message = JsonConvert.SerializeObject(@event);
        ConnectMqttClient();
        _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
            .WithTopic(eventName)
            .WithPayload(message)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag()
            .Build()).Wait();

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

    public void SubscribeToMqtt<T, TH>() where T : Event where TH : IEventHandler<T>
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
        StartMqttSubscription<T>();
        

    }

    private void StartMqttSubscription<T>() where T : Event
    {
        var eventName =typeof(T).Name;
        ConnectMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync+=(async e =>
        {
            if (e.ApplicationMessage.Topic == eventName)
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                await ProcessEvent(eventName, payload);
            }
        });

        _mqttClient.SubscribeAsync(eventName);
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

   

    private void ConnectMqttClient()
    {
        if (_mqttClient == null || !_mqttClient.IsConnected)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost",1883)
                .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithCleanSession()
                .Build();

            // var managedOptions = new ManagedMqttClientOptionsBuilder()
            //     .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            //     .WithClientOptions(options)
            //     .Build();

            _mqttClient = _mqttFactory.CreateMqttClient();
            _mqttClient.ConnectAsync(options).Wait();
        }
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
    
    private async Task ProcessMqttEvent(string eventName, string message)
    {
        using var scope = _scopeFactory.CreateScope();
        var eventType = _mediator.GetType();

        if (eventType != null)
        {
            var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handler = scope.ServiceProvider.GetService(concreteType);

            if (handler != null)
            {
                var @event = JsonConvert.DeserializeObject(message, eventType);
                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
            }
        }
    }

    
   
}