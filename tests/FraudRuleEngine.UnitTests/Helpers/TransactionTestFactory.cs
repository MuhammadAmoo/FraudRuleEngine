using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Enums;

namespace FraudRuleEngine.UnitTests.Helpers;

public static class TransactionTestFactory
{
    public static Transaction Create(
        decimal amount = 100m,
        TransactionCategory category = TransactionCategory.Transfer,
        string country = "ZA",
        DateTimeOffset? occurredAt = null)
    {
        return new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount,
            "ZAR",
            category,
            "Test Merchant",
            country,
            occurredAt ?? DateTimeOffset.UtcNow);
    }
}