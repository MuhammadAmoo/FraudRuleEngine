namespace FraudRuleEngine.Infrastructure.Persistence.Entities;

public sealed class TransactionEntity
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid AccountId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Merchant { get; set; }

    public string Country { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }
}