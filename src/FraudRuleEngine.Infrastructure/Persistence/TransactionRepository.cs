using FraudRuleEngine.Application.Fraud;
using FraudRuleEngine.Domain.Entities;
using FraudRuleEngine.Domain.Enums;
using FraudRuleEngine.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Persistence;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly FraudDbContext _dbContext;

    public TransactionRepository(FraudDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        var entity = new TransactionEntity
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Category = transaction.Category.ToString(),
            Merchant = transaction.Merchant,
            Country = transaction.Country,
            OccurredAt = transaction.OccurredAt
        };

        _dbContext.Transactions.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Transaction>> GetRecentTransactionsAsync(
        Guid customerId,
        DateTimeOffset occurredAt,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Transactions
            .AsNoTracking()
            .Where(x =>
                x.CustomerId == customerId &&
                x.OccurredAt < occurredAt)
            .OrderByDescending(x => x.OccurredAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return entities
            .Select(MapToDomain)
            .ToList();
    }

    public async Task<Transaction?> GetPreviousTransactionAsync(
        Guid customerId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Transactions
            .AsNoTracking()
            .Where(x =>
                x.CustomerId == customerId &&
                x.OccurredAt < occurredAt)
            .OrderByDescending(x => x.OccurredAt)
            .FirstOrDefaultAsync(cancellationToken);

        return entity is null
            ? null
            : MapToDomain(entity);
    }

    private static Transaction MapToDomain(TransactionEntity entity)
    {
        return new Transaction(
            entity.Id,
            entity.CustomerId,
            entity.AccountId,
            entity.Amount,
            entity.Currency,
            Enum.Parse<TransactionCategory>(entity.Category),
            entity.Merchant,
            entity.Country,
            entity.OccurredAt);
    }
}