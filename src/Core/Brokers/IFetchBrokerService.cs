namespace Core.Brokers;

public interface IFetchBrokerService
{
    Task<IResult<Broker>> GetBroker(string id);
    Task<IResult<List<Broker>>> GetBrokers();
    Task<IResult<Broker>> CreateBroker(BrokerDTO broker);
    Task<IResult<Broker>> UpdateBroker(BrokerDTO broker);
    Task<IResult> RemoveBroker(string id);

    Task<IResult> UpdateBrokerCredentials(string brokerId, BrokerCredentialsDTO id);
    Task<IResult<BrokerCredentialsDTO>> GetBrokerCredentials(string id);
}
