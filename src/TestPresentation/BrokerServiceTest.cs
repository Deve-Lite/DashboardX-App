using Presentation.Brokers;
using Presentation.Brokers.Interfaces;

namespace PresentationTests;

public class BrokerServiceTest : BaseTest, IAsyncLifetime
{
    private IBrokerService BrokerService { get; set; }

    public  async Task InitializeAsync()
    {
        BrokerService = new BrokerService(FetchBrokerService, ClientManager);
        await Setup();
    }

    public Task DisposeAsync()
    {
        TearDown();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateBrokerTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.NotNull(clients);
        Assert.True(clients!.Data.Count == 2);

        var newBroker = BrokerDtoGenerator.GenerateBrokerDto();
        var result = await BrokerService.CreateBroker(newBroker, BrokerDtoGenerator.GenerateBrokerCredentialsDto());

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClients();

        Assert.True(clients!.Data.Count == 3);
    }

    [Fact]
    public async Task UpdateBrokerTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var brokers = await FetchBrokerService.GetBrokers();
        var broker = brokers.Data[0];
        broker.Server = "google.com";
        var result = await BrokerService.UpdateBroker(broker.Dto(), BrokerDtoGenerator.GenerateBrokerCredentialsDto());

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClients();

        Assert.Equal(2, clients.Data.Count);
        Assert.Contains(clients.Data, x => x.GetBroker().Server == "google.com");
    }

    [Fact]
    public async Task RemoveBrokerTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var brokers = await FetchBrokerService.GetBrokers();
        var brokerToRemove = brokers.Data[0];

        var result = await BrokerService.RemoveBroker(brokerToRemove.Id);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClients();

        Assert.Equal(1, clients.Data.Count);
    }
}
