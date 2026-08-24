using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shop.Application.DTOs.OrderDTOs;
using System.Text;
using System.Text.Json;

namespace Shop.Infrastructure.RabbitMQ;

public class RabbitMQProducer
{
    private readonly IConfiguration _configuration;

    public RabbitMQProducer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOrderAsync(OrderMessage order)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "Orders",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var json = JsonSerializer.Serialize(order);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "Orders",
            body: body
        );
    }
}