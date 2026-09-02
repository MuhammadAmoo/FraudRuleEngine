using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Domain.Rules;
using FraudRuleEngine.UnitTests.Helpers;

namespace FraudRuleEngine.UnitTests.Rules;

public class HighTransactionVelocityRuleTests
{
    private readonly HighTransactionVelocityRule _rule = new();

    [Fact]
    public void IsTriggered_WhenMoreThanFiveRecentTransactionsExist_ReturnsTrue()
    {
        var currentTransaction = TransactionTestFactory.Create();

        var recentTransactions = Enumerable
            .Range(1, 6)
            .Select(_ => TransactionTestFactory.Create())
            .ToList();

        var context = new FraudEvaluationContext(
            currentTransaction,
            recentTransactions,
            null);

   
        var result = _rule.IsTriggered(context);

         Assert.True(result);
    }

    [Fact]
    public void IsTriggered_WhenFiveOrFewerRecentTransactionsExist_ReturnsFalse()
    {
         
        var currentTransaction = TransactionTestFactory.Create();

        var recentTransactions = Enumerable
            .Range(1, 5)
            .Select(_ => TransactionTestFactory.Create())
            .ToList();

        var context = new FraudEvaluationContext(
            currentTransaction,
            recentTransactions,
            null);

        
        var result = _rule.IsTriggered(context);

        Assert.False(result);
    }
}