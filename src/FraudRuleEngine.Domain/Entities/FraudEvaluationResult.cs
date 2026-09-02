using FraudRuleEngine.Domain.Enums;

namespace FraudRuleEngine.Domain.Entities;

public sealed class FraudEvaluationResult
{
    public Guid TransactionId { get; private set; }

    public int RiskScore { get; private set; }

    public RiskLevel RiskLevel { get; private set; }

    public IReadOnlyCollection<FraudRuleResult> RuleResults { get; private set; }

    private FraudEvaluationResult()
    {
        RuleResults = Array.Empty<FraudRuleResult>();
    }

    public FraudEvaluationResult(
        Guid transactionId,
        int riskScore,
        RiskLevel riskLevel,
        IReadOnlyCollection<FraudRuleResult> ruleResults)
    {
        TransactionId = transactionId;
        RiskScore = riskScore;
        RiskLevel = riskLevel;
        RuleResults = ruleResults;
    }
}