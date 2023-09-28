namespace Core.Brokers;

public interface IBrokerService
{
    Task<IResult<Broker>> GetBroker(string id);
    Task<IResult<List<Broker>>> GetBrokers();
    Task<IResult<Broker>> CreateBroker(Broker broker);
    Task<IResult<Broker>> UpdateBroker(Broker broker);
    Task<IResult> RemoveBroker(string id);
}
