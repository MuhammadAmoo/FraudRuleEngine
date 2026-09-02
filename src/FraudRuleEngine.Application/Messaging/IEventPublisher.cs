namespace FraudRuleEngine.Application.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventMessage, CancellationToken cancellationToken = default);
}