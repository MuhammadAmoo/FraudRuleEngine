namespace FraudRuleEngine.Domain.Entities;

public sealed class FraudRuleResult
{
    public string RuleCode { get; private set; } = string.Empty;

    public bool Triggered { get; private set; }

    public int RiskScore { get; private set; }

    private FraudRuleResult()
    {
    }

    public FraudRuleResult
    (   string ruleCode,
        bool triggered,
        int riskScore)
    {
        RuleCode = ruleCode;
        Triggered = triggered;
        RiskScore = riskScore;
    }
}