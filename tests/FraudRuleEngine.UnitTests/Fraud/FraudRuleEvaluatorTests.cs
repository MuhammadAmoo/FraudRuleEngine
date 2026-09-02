using FraudRuleEngine.Application.Fraud;
using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Domain.Rules;
using FraudRuleEngine.UnitTests.Helpers;

namespace FraudRuleEngine.UnitTests.Fraud;

public class FraudRuleEvaluatorTests
{
    [Fact]
    public void Evaluate_WhenMultipleRulesTrigger_ReturnsCorrectRiskScoreAndLevel()
    {
        var previousTransaction = TransactionTestFactory.Create(
            country: "ZA",
            occurredAt: DateTimeOffset.UtcNow.AddMinutes(-20));

        var currentTransaction = TransactionTestFactory.Create(
            amount: 25_000m,
            category: TransactionCategory.Gambling,
            country: "GB",
            occurredAt: DateTimeOffset.UtcNow);

        var recentTransactions = Enumerable
            .Range(1, 6)
            .Select(_ => TransactionTestFactory.Create())
            .ToList();

        var context = new FraudEvaluationContext(
            currentTransaction,
            recentTransactions,
            previousTransaction);

        var rules = new IFraudRule[]
        {
            new HighValueTransactionRule(),
            new HighRiskCategoryRule(),
            new HighTransactionVelocityRule(),
            new ImpossibleTravelRule()
        };

        var evaluator = new FraudRuleEvaluator(rules);

        var result = evaluator.Evaluate(context);

        Assert.Equal(140, result.RiskScore);
        Assert.Equal(RiskLevel.Critical, result.RiskLevel);
        Assert.Equal(4, result.RuleResults.Count);
        Assert.All(result.RuleResults, rule => Assert.True(rule.Triggered));
    }

    [Fact]
    public void Evaluate_WhenNoRulesTrigger_ReturnsLowRisk()
    {
        var transaction = TransactionTestFactory.Create(
            amount: 100m,
            category: TransactionCategory.Transfer,
            country: "ZA");

        var context = new FraudEvaluationContext(
            transaction,
            Array.Empty<Transaction>(),
            null);

        var rules = new IFraudRule[]
        {
            new HighValueTransactionRule(),
            new HighRiskCategoryRule(),
            new HighTransactionVelocityRule(),
            new ImpossibleTravelRule()
        };

        var evaluator = new FraudRuleEvaluator(rules);

        var result = evaluator.Evaluate(context);

        Assert.Equal(0, result.RiskScore);
        Assert.Equal(RiskLevel.Low, result.RiskLevel);
        Assert.Equal(4, result.RuleResults.Count);
        Assert.All(result.RuleResults, rule => Assert.False(rule.Triggered));
    }
}