FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NuGet.Config ./

COPY src/FraudRuleEngine.Domain/FraudRuleEngine.Domain.csproj src/FraudRuleEngine.Domain/
COPY src/FraudRuleEngine.Application/FraudRuleEngine.Application.csproj src/FraudRuleEngine.Application/
COPY src/FraudRuleEngine.Infrastructure/FraudRuleEngine.Infrastructure.csproj src/FraudRuleEngine.Infrastructure/
COPY src/FraudRuleEngine.Worker/FraudRuleEngine.Worker.csproj src/FraudRuleEngine.Worker/

RUN dotnet restore src/FraudRuleEngine.Worker/FraudRuleEngine.Worker.csproj --configfile /src/NuGet.Config

COPY src/ src/

RUN dotnet publish src/FraudRuleEngine.Worker/FraudRuleEngine.Worker.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FraudRuleEngine.Worker.dll"]