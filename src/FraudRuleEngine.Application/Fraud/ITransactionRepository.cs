using FraudRuleEngine.Domain.Entities;

namespace FraudRuleEngine.Application.Fraud;

public interface ITransactionRepository
{
    Task SaveAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Transaction>> GetRecentTransactionsAsync(
        Guid customerId,
        DateTimeOffset occurredAt,
        int limit,
        CancellationToken cancellationToken = default);

    Task<Transaction?> GetPreviousTransactionAsync(
        Guid customerId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default);
}