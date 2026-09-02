using FraudRuleEngine.Application.Fraud;
using FraudRuleEngine.Application.Messaging;
using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Domain.Events;
using FraudRuleEngine.Infrastructure.Messaging;
using FraudRuleEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

var connectionString =
    builder.Configuration.GetConnectionString("FraudDatabase")
    ?? "Host=localhost;Port=5432;Database=frauddb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/transactions", async (
    TransactionRequest request,
    IEventPublisher eventPublisher,
    CancellationToken cancellationToken) =>
{
    var transactionId = Guid.NewGuid();
    var eventId = Guid.NewGuid();

    var transactionEvent = new TransactionCategorized(
        eventId,
        transactionId,
        request.CustomerId,
        request.Amount,
        request.Currency,
        request.Category,
        request.Country,
        DateTimeOffset.UtcNow);

    await eventPublisher.PublishAsync(
        transactionEvent,
        cancellationToken);

    return Results.Accepted(
        $"/transactions/{transactionId}",
        new
        {
            TransactionId = transactionId,
            Message = "Transaction published for fraud evaluation."
        });
});

app.MapGet("/api/fraud-alerts/{transactionId:guid}", async (
    Guid transactionId,
    IFraudAlertRepository repository,
    CancellationToken cancellationToken) =>
{
    var alert = await repository.GetByTransactionIdAsync(
        transactionId,
        cancellationToken);

    return alert is null
        ? Results.NotFound()
        : Results.Ok(alert);
});

app.Run();

public sealed record TransactionRequest(
    Guid CustomerId,
    decimal Amount,
    string Currency,
    TransactionCategory Category,
    string Country);