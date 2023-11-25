namespace PresentationTests;

public class ClientServiceTests : BaseServiceTest
{

    [SetUp]
    public override async Task SetUpTest()
    {
        await base.SetUpTest();
    }

    [TearDown]
    public override void TearDownTest()
    {
        base.TearDownTest();
    }

    [Test]
    public async Task GetClientsTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var brokers = await FetchBrokerService.GetBrokers();
        var brokersInClients = clients.Data
                                     .Select(x => x.GetBroker())
                                     .ToList();

        foreach (var broker in brokers.Data)
            Assert.That(brokersInClients, Does.Contain(broker));
    }

    [Test]
    public async Task GetClientTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients.Data, Has.Count.EqualTo(2));

        var brokers = await FetchBrokerService.GetBrokers();

        foreach (var broker in brokers.Data)
        {
            var result = await ClientService.GetClient(broker.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.GetBroker().Id, Is.EqualTo(broker.Id));
        }
    }

    [Test]
    public async Task GetFullClientsTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var brokers = await FetchBrokerService.GetBrokers();
        var brokersInClients = clients.Data
                                      .Select(x => x.GetBroker())
                                      .ToList();

        Assert.That(brokersInClients, Does.Contain(brokers.Data[0]));
        Assert.That(brokersInClients, Does.Contain(brokers.Data[1]));

        var devices = await FetchDeviceService.GetDevices();
        var devicesInClients = clients.Data
                                      .SelectMany(x => x.GetDevices())
                                      .ToList();

        Assert.That(devicesInClients, Has.Count.EqualTo(2));
        Assert.That(devicesInClients, Does.Contain(devices.Data[0]));
        Assert.That(devicesInClients, Does.Contain(devices.Data[1]));
    }

    [Test]
    public async Task NewClientAddedInBackgroundTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var result = await FetchBrokerService.CreateBroker(BrokerDtoGenerator.GenerateBrokerDto());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);

        var resultClients = await ClientService.GetClients();
        var brokers = await FetchBrokerService.GetBrokers();

        Assert.That(resultClients.Data.Count, Is.EqualTo(3));

        var brokersInClients = clients.Data
                             .Select(x => x.GetBroker())
                             .ToList();

        foreach (var broker in brokers.Data)
            Assert.That(brokersInClients, Does.Contain(broker));
    }

    [Test]
    public async Task RemovedClientInBackgroundTest()
    {
        var brokers = await FetchBrokerService.GetBrokers();

        var firstToRemove = brokers.Data[0];
        var secondToRemove = brokers.Data[1];

        var clients = await ClientService.GetClients();
        Assert.That(clients.Data, Has.Count.EqualTo(2));

        var result = await FetchBrokerService.RemoveBroker(firstToRemove.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);

        var resultClients = await ClientService.GetClients();

        Assert.That(resultClients.Data, Has.Count.EqualTo(1));

        result = await FetchBrokerService.RemoveBroker(secondToRemove.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded, Is.True);

        resultClients = await ClientService.GetClients();

        Assert.That(resultClients.Data, Is.Empty);
    }

    [Test]
    public async Task UpdatedClientInBackgroundTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var result = await FetchBrokerService.GetBroker(clients.Data[0].Id);

        Assert.That(result.Succeeded, Is.True);

        var updatedDto = BrokerDtoGenerator.GenerateBrokerDto();

        updatedDto.Id = result.Data.Id;
        updatedDto.Port = result.Data.Port;
        updatedDto.KeepAlive = result.Data.KeepAlive;

        var updateResult = await FetchBrokerService.UpdateBroker(updatedDto);

        Assert.That(updateResult.Succeeded, Is.True);

        clients = await ClientService.GetClients();

        Assert.That(clients.Data.Count, Is.EqualTo(2));

        var updatedClient = clients.Data
            .Where(x => x.GetBroker().Id == result.Data.Id)
            .FirstOrDefault();

        Assert.That(updatedClient, Is.Not.Null);
        Assert.That(updatedClient.GetBroker().Id, Is.EqualTo(updateResult.Data.Id));
        Assert.That(updatedClient.GetBroker().Port, Is.EqualTo(updateResult.Data.Port));
        Assert.That(updatedClient.GetBroker().KeepAlive, Is.EqualTo(updateResult.Data.KeepAlive));
        Assert.That(updatedClient.GetBroker().Server, Is.EqualTo(updateResult.Data.Server));
    }

    [Test]
    public async Task LogoutTest()
    {
        var clients = await ClientService.GetClients();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        await ClientService.Logout();

        var emptyClients = ClientManager.GetClients();

        Assert.That(emptyClients!.Data, Is.Empty);

        clients = await ClientService.GetClients();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));
    }
}