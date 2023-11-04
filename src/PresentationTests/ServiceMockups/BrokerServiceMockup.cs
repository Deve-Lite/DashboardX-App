namespace PresentationTests.ServiceMockups;

public class BrokerServiceMockup : IBrokerService
{
    private List<Broker> Brokers { get; set; } = new();
    private BrokerCredentialsDTO BrokerCredentials = new BrokerCredentialsDTO
    {
        Password = "Password",
        Username = "Username",
    };

    public Task<IResult<Broker>> CreateBroker(BrokerDTO dto)
    {
        var broker = new Broker
        {
            Id = Guid.NewGuid().ToString(),
            Server = dto.Server,
            IsSSL = dto.IsSSL,
            ClientId = dto.ClientId,
            Icon = dto.Icon.Copy(),
            KeepAlive = dto.KeepAlive,
            Name = dto.Name,
            Port = dto.Port,
            EditedAt = DateTime.Now
        };

        Brokers.Add(broker);

        return Task.FromResult((IResult<Broker>) Result<Broker>.Success(broker));
    }

    public Task<IResult<Broker>> GetBroker(string id)
    {
        try
        {
            var broker = Brokers.First(x => x.Id == id);
            return Task.FromResult((IResult<Broker>) Result<Broker>.Success(broker));
        }
        catch 
        {
            return Task.FromResult((IResult<Broker>) Result<Broker>.Fail());
        }
    }

    public Task<IResult<List<Broker>>> GetBrokers()
    {
        return Task.FromResult((IResult<List<Broker>>) Result<List<Broker>>.Success(Brokers));
    }

    public Task<IResult> RemoveBroker(string id)
    {
        Brokers.RemoveAll(x => x.Id == id);
        return Task.FromResult((IResult) Result.Success());
    }

    public Task<IResult<Broker>> UpdateBroker(BrokerDTO dto)
    {
        try
        {
            var broker = Brokers.First(x => x.Id == dto.Id);

            broker.Server = dto.Server;
            broker.Port = dto.Port;
            broker.KeepAlive = dto.KeepAlive;
            broker.ClientId = dto.ClientId;
            broker.Icon = dto.Icon;
            broker.IsSSL = dto.IsSSL;
            broker.Name = dto.Name;

            broker.EditedAt = DateTime.Now;

            return Task.FromResult((IResult<Broker>)Result<Broker>.Success(broker));
        }
        catch
        {
            return Task.FromResult((IResult<Broker>)Result<Broker>.Fail());
        }
    }

    public Task<IResult> UpdateBrokerCredentials(string brokerId, BrokerCredentialsDTO bcd)
    {
        BrokerCredentials = bcd;
        return Task.FromResult((IResult)Result.Success());
    }

    public Task<IResult<BrokerCredentialsDTO>> GetBrokerCredentials(string id)
    {
        return Task.FromResult((IResult<BrokerCredentialsDTO>)Result<BrokerCredentialsDTO>.Success(BrokerCredentials));
    }
}
