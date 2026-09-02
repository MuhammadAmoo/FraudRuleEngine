namespace FraudRuleEngine.Infrastructure.Persistence.Entities;

public sealed class FraudAlertEntity
{
    public Guid TransactionId { get; set; }

    public int RiskScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string RuleResultsJson { get; set; } = "[]";

    public DateTimeOffset CreatedAt { get; set; }
}
