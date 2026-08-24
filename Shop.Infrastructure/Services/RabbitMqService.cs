using Microsoft.Extensions.Options;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
namespace Shop.Infrastructure.Services;

public class RabbitMqService : IQueueService
{
    private readonly RabbitMqSettings _rabbitMqSettings;


    public RabbitMqService(IOptions<RabbitMqSettings> options)
    {
        _rabbitMqSettings = options.Value;
    }

    public async Task PublishAsync<T>(string queue, T message)
    {
        // Створюємо фабрику підключення до
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqSettings.Host,

            Port = _rabbitMqSettings.Port
        };

        using var connection = await factory.CreateConnectionAsync();

        await using var channel = await connection.CreateChannelAsync();

       await channel.QueueDeclareAsync(
            queue: queue, durable: true, false, autoDelete: false, arguments: null );

       var json = JsonSerializer.Serialize(message);

       var body = Encoding.UTF8.GetBytes(json);

       var properties = new BasicProperties { Persistent = true        };

        await channel.BasicPublishAsync(
             exchange: "", queue, mandatory: false, basicProperties: properties,
             body: body );
}
}