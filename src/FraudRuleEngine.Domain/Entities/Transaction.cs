using FraudRuleEngine.Domain.Enums;

namespace FraudRuleEngine.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid AccountId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public TransactionCategory Category { get; private set; }

    public string? Merchant { get; private set; }

    public string Country { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    private Transaction()
    {
    }

    public Transaction(
        Guid id,
        Guid customerId,
        Guid accountId,
        decimal amount,
        string currency,
        TransactionCategory category,
        string? merchant,
        string country,
        DateTimeOffset occurredAt)
    {
        Id = id;
        CustomerId = customerId;
        AccountId = accountId;
        Amount = amount;
        Currency = currency;
        Category = category;
        Merchant = merchant;
        Country = country;
        OccurredAt = occurredAt;
    }
}