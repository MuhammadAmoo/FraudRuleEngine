using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Rules;
using FraudRuleEngine.UnitTests.Helpers;

namespace FraudRuleEngine.UnitTests.Rules;

public class HighRiskCategoryRuleTests
{
    private readonly HighRiskCategoryRule _rule = new();

    [Fact]
    public void IsTriggered_WhenGamblingAmountExceedsThreshold_ReturnsTrue()
    {
        // Arrange
        var transaction = TransactionTestFactory.Create(
            amount: 6_000m,
            category: TransactionCategory.Gambling);

        var context = new FraudEvaluationContext(
            transaction,
            Array.Empty<Transaction>(),
            null);

        // Act and implement
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTriggered_WhenGamblingAmountIsBelowThreshold_ReturnsFalse()
    {
        // Arrange
        var transaction = TransactionTestFactory.Create(
            amount: 4_000m,
            category: TransactionCategory.Gambling);

        var context = new FraudEvaluationContext(
            transaction,
            Array.Empty<Transaction>(),
            null);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTriggered_WhenHighValueButNotGambling_ReturnsFalse()
    {
        // Arrange
        var transaction = TransactionTestFactory.Create(
            amount: 25_000m,
            category: TransactionCategory.Transfer);

        var context = new FraudEvaluationContext(
            transaction,
            Array.Empty<Transaction>(),
            null);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.False(result);
    }
}