using FraudRuleEngine.Domain.Enums;

namespace FraudRuleEngine.Domain.Events;

public sealed record TransactionCategorized(
    Guid EventId,
    Guid TransactionId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    TransactionCategory Category,
    string Country,
    DateTimeOffset OccurredAt);