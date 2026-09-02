using FraudRuleEngine.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudRuleEngine.Infrastructure.Persistence;

public sealed class FraudDbContext : DbContext
{
    public FraudDbContext(DbContextOptions<FraudDbContext> options)
        : base(options)
    {
    }

    public DbSet<FraudAlertEntity> FraudAlerts => Set<FraudAlertEntity>();

    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudAlertEntity>(entity =>
        {
            entity.ToTable("fraud_alerts");

            entity.HasKey(x => x.TransactionId);

            entity.Property(x => x.TransactionId)
                .HasColumnName("transaction_id");

            entity.Property(x => x.RiskScore)
                .HasColumnName("risk_score");

            entity.Property(x => x.RiskLevel)
                .HasColumnName("risk_level")
                .HasMaxLength(20);

            entity.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(x => x.RuleResultsJson)
                .HasColumnName("rule_results")
                .HasColumnType("jsonb");
        });

        modelBuilder.Entity<TransactionEntity>(entity =>
        {
            entity.ToTable("transactions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("id");

            entity.Property(x => x.CustomerId)
                .HasColumnName("customer_id");

            entity.Property(x => x.AccountId)
                .HasColumnName("account_id");

            entity.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)");

            entity.Property(x => x.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3);

            entity.Property(x => x.Category)
                .HasColumnName("category")
                .HasMaxLength(50);

            entity.Property(x => x.Merchant)
                .HasColumnName("merchant")
                .HasMaxLength(200);

            entity.Property(x => x.Country)
                .HasColumnName("country")
                .HasMaxLength(2);

            entity.Property(x => x.OccurredAt)
                .HasColumnName("occurred_at");

            entity.HasIndex(x => new { x.CustomerId, x.OccurredAt });
        });
    }
}