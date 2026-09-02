using FraudRuleEngine.Domain.Models;

namespace FraudRuleEngine.Domain.Rules;

public sealed class HighValueTransactionRule : IFraudRule
{
    private const decimal Threshold = 20_000m;

    public string RuleCode => "HIGH_VALUE_TRANSACTION";

    public int RiskScore => 30;

    public bool IsTriggered(FraudEvaluationContext context)
    {
        return context.CurrentTransaction.Amount > Threshold;
    }
}