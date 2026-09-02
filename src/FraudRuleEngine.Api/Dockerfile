FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["src/FraudRuleEngine.Api/FraudRuleEngine.Api.csproj", "src/FraudRuleEngine.Api/"]
COPY ["src/FraudRuleEngine.Infrastructure/FraudRuleEngine.Infrastructure.csproj", "src/FraudRuleEngine.Infrastructure/"]
COPY ["src/FraudRuleEngine.Application/FraudRuleEngine.Application.csproj", "src/FraudRuleEngine.Application/"]
COPY ["src/FraudRuleEngine.Domain/FraudRuleEngine.Domain.csproj", "src/FraudRuleEngine.Domain/"]

RUN dotnet restore "src/FraudRuleEngine.Api/FraudRuleEngine.Api.csproj"

COPY . .

RUN dotnet publish "src/FraudRuleEngine.Api/FraudRuleEngine.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FraudRuleEngine.Api.dll"]