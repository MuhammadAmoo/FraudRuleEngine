using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Domain.Models;

namespace FraudRuleEngine.Domain.Rules;

public sealed class HighRiskCategoryRule : IFraudRule
{
    private const decimal Threshold = 5_000m;

    public string RuleCode => "HIGH_RISK_CATEGORY";

    public int RiskScore => 20;

    public bool IsTriggered(FraudEvaluationContext context)
    {
        return context.CurrentTransaction.Category == TransactionCategory.Gambling
               && context.CurrentTransaction.Amount > Threshold;
    }
}