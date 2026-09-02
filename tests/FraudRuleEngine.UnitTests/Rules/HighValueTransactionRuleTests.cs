using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Domain.Rules;
using FraudRuleEngine.UnitTests.Helpers;

namespace FraudRuleEngine.UnitTests.Rules;

public class HighValueTransactionRuleTests
{
    private readonly HighValueTransactionRule _rule = new();

    [Fact]
    public void IsTriggered_WhenAmountExceedsThreshold_ReturnsTrue()
    {
        // Arrange
        var transaction = TransactionTestFactory.Create(
            amount: 25_000m);

        var context = new FraudEvaluationContext(
            transaction,
            Array.Empty<FraudRuleEngine.Domain.Entities.Transaction>(),
            null);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTriggered_WhenAmountIsBelowThreshold_ReturnsFalse()
    {
        // Arrange
        var transaction = TransactionTestFactory.Create(
            amount: 10_000m);

        var context = new FraudEvaluationContext(
            transaction,
            Array.Empty<FraudRuleEngine.Domain.Entities.Transaction>(),
            null);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.False(result);
    }
}