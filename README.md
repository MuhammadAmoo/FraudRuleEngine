# Fraud Rule Engine

A production-oriented, asynchronous fraud detection service built with .NET 8. The system accepts transactions through a REST API, publishes transaction events to RabbitMQ, evaluates fraud rules in a background worker, and persists fraud results in PostgreSQL.

## Architecture

```text
Client
  |
  v
ASP.NET Core Minimal API
  |
  v
RabbitMQ
  |
  v
Fraud Rule Worker
  |
  v
Fraud Rules
  |
  v
PostgreSQL
```

The API returns `202 Accepted` after publishing the transaction event. Fraud evaluation happens asynchronously in the Worker.

## Fraud Rules

| Rule | Condition | Score |
|---|---|---:|
| `HIGH_VALUE_TRANSACTION` | Amount > R20,000 | +30 |
| `HIGH_RISK_CATEGORY` | Gambling and amount > R5,000 | +20 |
| `HIGH_TRANSACTION_VELOCITY` | More than 5 previous transactions | +40 |
| `IMPOSSIBLE_TRAVEL` | Different countries within 60 minutes | +50 |

### Risk Levels

- **Low:** < 30
- **Medium:** 30–59
- **High:** 60–79
- **Critical:** 80+

Multiple triggered rules are combined to produce the final risk score.

## Technology Stack

- C# / .NET 8
- ASP.NET Core Minimal API
- RabbitMQ
- PostgreSQL
- Entity Framework Core
- Docker / Docker Compose
- xUnit

## Running with Docker

### Prerequisites

- Docker Desktop

### Start the application

From the repository root:

```powershell
docker compose up --build
```

The services are:

| Service | Address |
|---|---|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| RabbitMQ Management | http://localhost:15673 |
| PostgreSQL | localhost:5432 |

RabbitMQ credentials for the local Docker environment:

```text
Username: guest
Password: guest
```

The application includes separate runnable Dockerfiles for the API and Worker, and `docker-compose.yml` starts the complete system.

To stop the application:

```powershell
docker compose down
```

## API

### Submit a transaction

`POST /transactions`

Example request:

```json
{
  "customerId": "11111111-1111-1111-1111-111111111111",
  "amount": 1000,
  "currency": "ZAR",
  "category": "OnlinePayment",
  "country": "ZA"
}
```

The API returns `202 Accepted` with a transaction ID.

### Retrieve a fraud alert

`GET /api/fraud-alerts/{transactionId}`

Use the transaction ID returned by the POST request.

### PowerShell example

```powershell
$body = @{
    customerId = "11111111-1111-1111-1111-111111111111"
    amount     = 25000
    currency   = "ZAR"
    category   = "OnlinePayment"
    country    = "ZA"
} | ConvertTo-Json

$response = Invoke-RestMethod `
    -Uri "http://localhost:8080/transactions" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body

$response
```

Then retrieve the result:

```powershell
Invoke-RestMethod `
    -Uri "http://localhost:8080/api/fraud-alerts/$($response.transactionId)" `
    -Method Get
```

The R25,000 transaction should trigger `HIGH_VALUE_TRANSACTION` and produce a **Medium** risk level with a score of **30**.

## Testing the Fraud Rules

The following scenarios can be used to demonstrate the rule engine:

### 1. High-value transaction

```json
{
  "customerId": "22222222-2222-2222-2222-222222222222",
  "amount": 25000,
  "currency": "ZAR",
  "category": "OnlinePayment",
  "country": "ZA"
}
```

Expected: `HIGH_VALUE_TRANSACTION` → +30 → Medium.

### 2. High-risk category

```json
{
  "customerId": "33333333-3333-3333-3333-333333333333",
  "amount": 6000,
  "currency": "ZAR",
  "category": "Gambling",
  "country": "ZA"
}
```

Expected: `HIGH_RISK_CATEGORY` → +20 → Low.

### 3. Transaction velocity

Submit more than five transactions for the same customer. The subsequent transaction should trigger `HIGH_TRANSACTION_VELOCITY` → +40.

### 4. Impossible travel

Submit transactions for the same customer from different countries less than 60 minutes apart. The subsequent transaction should trigger `IMPOSSIBLE_TRAVEL` → +50.

## Running Tests

From the repository root:

```powershell
dotnet test
```

The unit test suite covers the fraud evaluator and individual fraud rules, including boundary conditions.

## Design Notes

- The asynchronous architecture decouples transaction ingestion from fraud evaluation.
- RabbitMQ provides reliable event-based communication between the API and Worker.
- Fraud rules implement `IFraudRule`, allowing additional rules to be added without changing the evaluator.
- PostgreSQL stores transactions and fraud evaluations for later retrieval and historical analysis.
- Transaction history is indexed by customer and transaction time to support velocity and previous-transaction checks.
- Scoped database services are created per Worker message to maintain correct Entity Framework Core `DbContext` lifetimes.
- Docker Compose provides a reproducible local environment for the complete solution.

## Production Considerations

For a production deployment, the solution could be extended with EF Core migrations, RabbitMQ retry policies and dead-letter queues, idempotent event processing, health checks, centralized configuration/secrets management, observability, and horizontal Worker scaling.
