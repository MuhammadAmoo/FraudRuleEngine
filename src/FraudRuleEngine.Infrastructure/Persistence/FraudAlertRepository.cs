using System.Text.Json;
using FraudRuleEngine.Application.Fraud;
using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Persistence;

public sealed class FraudAlertRepository : IFraudAlertRepository
{
    private readonly FraudDbContext _dbContext;

    public FraudAlertRepository(FraudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(
        FraudEvaluationResult evaluation,
        CancellationToken cancellationToken = default)
    {
        var existingAlert = await _dbContext.FraudAlerts
            .FirstOrDefaultAsync(
                x => x.TransactionId == evaluation.TransactionId,
                cancellationToken);

        var ruleResultsJson = JsonSerializer.Serialize(evaluation.RuleResults);

        if (existingAlert is null)
        {
            _dbContext.FraudAlerts.Add(
                new FraudAlertEntity
                {
                    TransactionId = evaluation.TransactionId,
                    RiskScore = evaluation.RiskScore,
                    RiskLevel = evaluation.RiskLevel.ToString(),
                    RuleResultsJson = ruleResultsJson,
                    CreatedAt = DateTimeOffset.UtcNow
                });
        }
        else
        {
            existingAlert.RiskScore = evaluation.RiskScore;
            existingAlert.RiskLevel = evaluation.RiskLevel.ToString();
            existingAlert.RuleResultsJson = ruleResultsJson;
            existingAlert.CreatedAt = DateTimeOffset.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<FraudEvaluationResult?> GetByTransactionIdAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.FraudAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TransactionId == transactionId,
                cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var ruleResults =
            JsonSerializer.Deserialize<List<FraudRuleResult>>(
                entity.RuleResultsJson) ?? [];

        return new FraudEvaluationResult(
            entity.TransactionId,
            entity.RiskScore,
            Enum.Parse<RiskLevel>(entity.RiskLevel),
            ruleResults);
    }
}
