using FraudRuleEngine.Domain.Entities;

namespace FraudRuleEngine.Domain.Models;

public sealed class FraudEvaluationContext
{
    public Transaction CurrentTransaction { get; }
    public IReadOnlyCollection<Transaction> RecentTransactions { get; }
    public Transaction? PreviousTransaction { get; }

    public FraudEvaluationContext
    (
        Transaction currentTransaction,
        IReadOnlyCollection<Transaction> recentTransactions,
        Transaction? previousTransaction)
    {
        CurrentTransaction = currentTransaction;
        RecentTransactions = recentTransactions;
        PreviousTransaction = previousTransaction;
    }
}