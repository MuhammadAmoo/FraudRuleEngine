using FraudRuleEngine.Application.Fraud;
using FraudRuleEngine.Domain.Rules;
using FraudRuleEngine.Infrastructure.Persistence;
using FraudRuleEngine.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("FraudDatabase")
    ?? "Host=localhost;Port=5432;Database=frauddb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<FraudDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Register fraud rules
builder.Services.AddSingleton<IFraudRule, HighRiskCategoryRule>();
builder.Services.AddSingleton<IFraudRule, HighTransactionVelocityRule>();
builder.Services.AddSingleton<IFraudRule, HighValueTransactionRule>();
builder.Services.AddSingleton<IFraudRule, ImpossibleTravelRule>();

// Register fraud evaluator
builder.Services.AddSingleton<FraudRuleEvaluator>();

// Register Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<FraudDbContext>();

    const int maxAttempts = 10;

for (var attempt = 1; attempt <= maxAttempts; attempt++)
{
    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        break;
    }
    catch (Exception) when (attempt < maxAttempts)
    {
        Console.WriteLine(
            $"PostgreSQL is not ready. Attempt {attempt}/{maxAttempts}. Retrying in 3 seconds...");

        await Task.Delay(
            TimeSpan.FromSeconds(3));
    }
}
}

await host.RunAsync();