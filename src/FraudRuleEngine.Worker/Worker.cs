using System.Text;
using System.Text.Json;
using FraudRuleEngine.Application.Fraud;
using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Events;
using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FraudRuleEngine.Worker;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FraudRuleEvaluator _fraudRuleEvaluator;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    private IConnection? _connection;
    private IChannel? _channel;

    public Worker(
        ILogger<Worker> logger,
        FraudRuleEvaluator fraudRuleEvaluator,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _fraudRuleEvaluator = fraudRuleEvaluator;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(
            stoppingToken);

        _channel = await _connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: "fraud-transactions",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                var transactionEvent =
                    JsonSerializer.Deserialize<TransactionCategorized>(
                        message);

                if (transactionEvent is null)
                {
                    _logger.LogWarning(
                        "Received an invalid transaction event.");

                    await _channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken);

                    return;
                }

                var transaction = new Transaction(
                    transactionEvent.TransactionId,
                    transactionEvent.CustomerId,
                    Guid.NewGuid(),
                    transactionEvent.Amount,
                    transactionEvent.Currency,
                    transactionEvent.Category,
                    null,
                    transactionEvent.Country,
                    transactionEvent.OccurredAt);

                await using var scope =
                    _scopeFactory.CreateAsyncScope();

                var transactionRepository =
                    scope.ServiceProvider
                        .GetRequiredService<ITransactionRepository>();

                var fraudAlertRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IFraudAlertRepository>();

                var recentTransactions =
                    await transactionRepository
                        .GetRecentTransactionsAsync(
                            transaction.CustomerId,
                            transaction.OccurredAt,
                            10,
                            stoppingToken);

                var previousTransaction =
                    await transactionRepository
                        .GetPreviousTransactionAsync(
                            transaction.CustomerId,
                            transaction.OccurredAt,
                            stoppingToken);

                var context = new FraudEvaluationContext(
                    transaction,
                    recentTransactions,
                    previousTransaction);

                var evaluation =
                    _fraudRuleEvaluator.Evaluate(context);

                await transactionRepository.SaveAsync(
                    transaction,
                    stoppingToken);

                await fraudAlertRepository.SaveAsync(
                    evaluation,
                    stoppingToken);

                _logger.LogInformation(
                    "Fraud evaluation completed for transaction {TransactionId}. RiskScore={RiskScore}, RiskLevel={RiskLevel}",
                    evaluation.TransactionId,
                    evaluation.RiskScore,
                    evaluation.RiskLevel);

                await _channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing fraud transaction event.");

                await _channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "fraud-transactions",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "Fraud Rule Engine Worker started and listening for transactions.");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}

