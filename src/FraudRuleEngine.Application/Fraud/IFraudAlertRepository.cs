using FraudRuleEngine.Domain.Entities;

namespace FraudRuleEngine.Application.Fraud;

public interface IFraudAlertRepository
{
    Task SaveAsync(
        FraudEvaluationResult evaluation,
        CancellationToken cancellationToken = default);

    Task<FraudEvaluationResult?> GetByTransactionIdAsync(
        Guid transactionId,
        CancellationToken cancellationToken = default);
}
