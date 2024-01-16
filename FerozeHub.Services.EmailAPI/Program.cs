using FerozeHub.MessageBus.Bus;
using FerozeHub.RabbitMqBus;
using FerozeHub.Services.EmailAPI.Data;
using FerozeHub.Services.EmailAPI.EventHandlers;
using FerozeHub.Services.EmailAPI.Events;
using FerozeHub.Services.EmailAPI.Interface;
using FerozeHub.Services.EmailAPI.Services;
using FerozeHub.Services.EmailAPI.Utility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
EmailSD.EmailHost = builder.Configuration["EmailConfiguration:EmailHost"];
EmailSD.EmailUsername=builder.Configuration["EmailConfiguration:EmailUsername"];
EmailSD.EmailPassword = builder.Configuration["EmailConfiguration:EmailPassword"];
builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEventBus, EventBus>(sp =>
{
    var scopefactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new EventBus(sp.GetService<IMediator>(), scopefactory,sp.GetService<IMqttClient>());
});
builder.Services.AddMediatR(cfg=>cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddSingleton(typeof(Dictionary<string, List<Type>>));
//builder.Services.AddSingleton(typeof(List<Type>));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<EmailEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
ConfigureEventBus(app);

app.MapControllers();
app.Run();


void ConfigureEventBus(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var service = scope.ServiceProvider;
    var eventBus = service.GetRequiredService<IEventBus>();
    eventBus.SubscribeToMqtt<EmailCreatedEvent,EmailEventHandler>();
}

