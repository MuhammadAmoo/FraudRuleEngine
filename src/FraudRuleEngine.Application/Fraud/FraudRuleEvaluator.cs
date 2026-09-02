using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Domain.Models;
using FraudRuleEngine.Domain.Rules;

namespace FraudRuleEngine.Application.Fraud;

public sealed class FraudRuleEvaluator
{
    private readonly IReadOnlyCollection<IFraudRule> _rules;

    public FraudRuleEvaluator(IEnumerable<IFraudRule> rules)
    {
        _rules = rules.ToList();
    }

    public FraudEvaluationResult Evaluate(FraudEvaluationContext context)
    {
        var ruleResults = _rules
            .Select(rule =>
            {
                var triggered = rule.IsTriggered(context);

                return new FraudRuleResult(
                    rule.RuleCode,
                    triggered,
                    triggered ? rule.RiskScore : 0);
            })
            .ToList();

        var totalRiskScore = ruleResults
            .Where(result => result.Triggered)
            .Sum(result => result.RiskScore);

        var riskLevel = DetermineRiskLevel(totalRiskScore);

        return new FraudEvaluationResult(
            context.CurrentTransaction.Id,
            totalRiskScore,
            riskLevel,
            ruleResults);
    }

    private static RiskLevel DetermineRiskLevel(int riskScore)
    {
        return riskScore switch
        {
            >= 80 => RiskLevel.Critical,
            >= 60 => RiskLevel.High,
            >= 30 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };
    }
}