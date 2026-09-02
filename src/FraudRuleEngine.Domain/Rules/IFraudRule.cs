using FraudRuleEngine.Domain.Models;

namespace FraudRuleEngine.Domain.Rules;

public interface IFraudRule
{
    string RuleCode { get; }

    int RiskScore { get; }

    bool IsTriggered(FraudEvaluationContext context);
}