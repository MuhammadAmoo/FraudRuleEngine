using FraudRuleEngine.Domain.Models;

namespace FraudRuleEngine.Domain.Rules;

public sealed class ImpossibleTravelRule : IFraudRule
{
    private static readonly TimeSpan MaximumTravelTime = TimeSpan.FromMinutes(60);

    public string RuleCode => "IMPOSSIBLE_TRAVEL";

    public int RiskScore => 50;

    public bool IsTriggered(FraudEvaluationContext context)
    {
        var previousTransaction = context.PreviousTransaction;

        if (previousTransaction is null)
        {
            return false;
        }

        if (string.Equals(
                previousTransaction.Country,
                context.CurrentTransaction.Country,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var timeDifference =
            context.CurrentTransaction.OccurredAt -
            previousTransaction.OccurredAt;

        return timeDifference > TimeSpan.Zero &&
               timeDifference < MaximumTravelTime;
    }
}