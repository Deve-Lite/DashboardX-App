namespace PresentationTests;

public class ClientServiceTests : BaseTest, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Setup();
    }

    public Task DisposeAsync()
    {
        TearDown();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetClientsTest()
    {
        var clients = await ClientService.GetClients();

        Assert.Equal(2, clients!.Data.Count);

        var brokers = await FetchBrokerService.GetBrokers();
        var brokersInClients = clients.Data.Select(x => x.GetBroker()).ToList();

        foreach (var broker in brokers.Data)
            Assert.Contains(broker, brokersInClients);
    }

    [Fact]
    public async Task GetClientTest()
    {
        var clients = await ClientService.GetClients();

        Assert.Equal(2, clients.Data.Count);

        var brokers = await FetchBrokerService.GetBrokers();

        foreach (var broker in brokers.Data)
        {
            var result = await ClientService.GetClient(broker.Id);

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(broker.Id, result.Data.GetBroker().Id);
        }
    }

    [Fact]
    public async Task GetFullClientsTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var brokers = await FetchBrokerService.GetBrokers();
        var brokersInClients = clients.Data.Select(x => x.GetBroker()).ToList();

        Assert.Contains(brokers.Data[0], brokersInClients);
        Assert.Contains(brokers.Data[1], brokersInClients);

        var devices = await FetchDeviceService.GetDevices();
        var devicesInClients = clients.Data.SelectMany(x => x.GetDevices()).ToList();

        Assert.Equal(2, devicesInClients.Count);
        Assert.Contains(devices.Data[0], devicesInClients);
        Assert.Contains(devices.Data[1], devicesInClients);
    }

    [Fact]
    public async Task NewClientAddedInBackgroundTest()
    {
        var clients = await ClientService.GetClients();

        Assert.Equal(2, clients.Data.Count);

        var result = await FetchBrokerService.CreateBroker(BrokerDtoGenerator.GenerateBrokerDto());

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        var resultClients = await ClientService.GetClients();
        var brokers = await FetchBrokerService.GetBrokers();

        Assert.Equal(3, resultClients.Data.Count);

        var brokersInClients = clients.Data.Select(x => x.GetBroker()).ToList();

        foreach (var broker in brokers.Data)
            Assert.Contains(broker, brokersInClients);
    }

    [Fact]
    public async Task RemovedClientInBackgroundTest()
    {
        var brokers = await FetchBrokerService.GetBrokers();

        var firstToRemove = brokers.Data[0];
        var secondToRemove = brokers.Data[1];

        var clients = await ClientService.GetClients();
        Assert.Equal(2, clients.Data.Count);

        var result = await FetchBrokerService.RemoveBroker(firstToRemove.Id);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        var resultClients = await ClientService.GetClients();

        Assert.Equal(1, resultClients.Data.Count);

        result = await FetchBrokerService.RemoveBroker(secondToRemove.Id);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        resultClients = await ClientService.GetClients();

        Assert.Empty(resultClients.Data);
    }

    [Fact]
    public async Task UpdatedClientInBackgroundTest()
    {
        var clients = await ClientService.GetClients();

        Assert.Equal(2, clients.Data.Count);

        var result = await FetchBrokerService.GetBroker(clients.Data[0].Id);

        Assert.True(result.Succeeded);

        var updatedDto = BrokerDtoGenerator.GenerateBrokerDto();

        updatedDto.Id = result.Data.Id;
        updatedDto.Port = result.Data.Port;
        updatedDto.KeepAlive = result.Data.KeepAlive;

        var updateResult = await FetchBrokerService.UpdateBroker(updatedDto);

        Assert.True(updateResult.Succeeded);

        clients = await ClientService.GetClients();

        Assert.Equal(2, clients.Data.Count);

        var updatedClient = clients.Data
            .Where(x => x.GetBroker().Id == result.Data.Id)
            .FirstOrDefault();

        Assert.NotNull(updatedClient);
        Assert.Equal(updateResult.Data.Id, updatedClient.GetBroker().Id);
        Assert.Equal(updateResult.Data.Port, updatedClient.GetBroker().Port);
        Assert.Equal(updateResult.Data.KeepAlive, updatedClient.GetBroker().KeepAlive);
        Assert.Equal(updateResult.Data.Server, updatedClient.GetBroker().Server);
    }

    [Fact]
    public async Task LogoutTest()
    {
        var clients = await ClientService.GetClients();

        Assert.Equal(2, clients!.Data.Count);

        await ClientService.Logout();

        var emptyClients = ClientManager.GetClients();

        Assert.Empty(emptyClients.Data);

        clients = await ClientService.GetClients();

        Assert.Equal(2, clients!.Data.Count);
    }
}