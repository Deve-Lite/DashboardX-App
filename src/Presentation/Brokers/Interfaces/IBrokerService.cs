namespace Presentation.Brokers.Interfaces;

public interface IBrokerService
{
    Task<IResult> RemoveBroker(string clientId);
    Task<IResult> CreateBroker(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
    Task<IResult> UpdateBroker(BrokerDTO broker, BrokerCredentialsDTO brokerCredentialsDTO);
}
