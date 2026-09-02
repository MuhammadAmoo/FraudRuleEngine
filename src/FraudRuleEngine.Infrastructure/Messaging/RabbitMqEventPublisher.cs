using System.Text;
using System.Text.Json;
using FraudRuleEngine.Application.Messaging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace FraudRuleEngine.Infrastructure.Messaging;

public sealed class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqEventPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync<T>(
        T eventMessage,
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        await using var connection = await factory.CreateConnectionAsync(
            cancellationToken);

        await using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: "fraud-transactions",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var message = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "fraud-transactions",
            body: body,
            cancellationToken: cancellationToken);
    }
}