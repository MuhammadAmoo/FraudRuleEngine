using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Domain.Rules;
using FraudRuleEngine.UnitTests.Helpers;

namespace FraudRuleEngine.UnitTests.Rules;

public class ImpossibleTravelRuleTests
{
    private readonly ImpossibleTravelRule _rule = new();

    [Fact]
    public void IsTriggered_WhenCountriesDifferWithinSixtyMinutes_ReturnsTrue()
    {
        // Arrange
        var previousTime = DateTimeOffset.UtcNow.AddMinutes(-20);

        var previousTransaction = TransactionTestFactory.Create(
            country: "ZA",
            occurredAt: previousTime);

        var currentTransaction = TransactionTestFactory.Create(
            country: "GB",
            occurredAt: DateTimeOffset.UtcNow);

        var context = new FraudEvaluationContext(
            currentTransaction,
            Array.Empty<FraudRuleEngine.Domain.Entities.Transaction>(),
            previousTransaction);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTriggered_WhenCountriesAreTheSame_ReturnsFalse()
    {
        // Arrange
        var previousTransaction = TransactionTestFactory.Create(
            country: "ZA",
            occurredAt: DateTimeOffset.UtcNow.AddMinutes(-20));

        var currentTransaction = TransactionTestFactory.Create(
            country: "ZA",
            occurredAt: DateTimeOffset.UtcNow);

        var context = new FraudEvaluationContext(
            currentTransaction,
            Array.Empty<FraudRuleEngine.Domain.Entities.Transaction>(),
            previousTransaction);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTriggered_WhenTravelTimeExceedsSixtyMinutes_ReturnsFalse()
    {
        // Arrange
        var previousTransaction = TransactionTestFactory.Create(
            country: "ZA",
            occurredAt: DateTimeOffset.UtcNow.AddMinutes(-90));

        var currentTransaction = TransactionTestFactory.Create(
            country: "GB",
            occurredAt: DateTimeOffset.UtcNow);

        var context = new FraudEvaluationContext(
            currentTransaction,
            Array.Empty<FraudRuleEngine.Domain.Entities.Transaction>(),
            previousTransaction);

        // Act
        var result = _rule.IsTriggered(context);

        // Assert
        Assert.False(result);
    }
}