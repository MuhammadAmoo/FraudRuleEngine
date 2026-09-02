using FraudRuleEngine.Domain.Models;

namespace FraudRuleEngine.Domain.Rules;

public sealed class HighTransactionVelocityRule : IFraudRule
{
    private const int MaximumTransactions = 5;

    public string RuleCode => "HIGH_TRANSACTION_VELOCITY";

    public int RiskScore => 40;

    public bool IsTriggered(FraudEvaluationContext context)
    {
        return context.RecentTransactions.Count > MaximumTransactions;
    }
}