namespace Core.Brokers;

public interface IBrokerService
{
    Task<IResult<Broker>> GetBroker(string id);
    Task<IResult<List<Broker>>> GetBrokers();
    Task<IResult<BrokerCredentials>> GetBrokerCredentials(string id);
    Task<IResult<Broker>> CreateBroker(BrokerDTO broker);
    Task<IResult<Broker>> UpdateBroker(BrokerDTO broker);
    Task<IResult> RemoveBroker(string id);
}
