using Presentation.Devices;
using Presentation.Devices.Interfaces;

namespace PresentationTests.ServicesTests;

internal class DeviceServiceTests : BaseServiceTest
{
    public IDeviceService DeviceService { get; private set; }

    public DeviceServiceTests() : base()
    {
        DeviceService = new DeviceService(FetchDeviceService, ClientManager);
    }

    [SetUp]
    public override async Task SetUpTest()
    {
        await base.SetUpTest();
    }

    [TearDown]
    public void TearDown()
    {
        base.TearDownTest();
        DeviceService = new DeviceService(FetchDeviceService, ClientManager);
    }

    [Test]
    public async Task CreateDeviceTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var brokers = await FetchBrokerService.GetBrokers();

        var deviceDto = DeviceDtoGenerator.GenerateDeviceDto();
        deviceDto.BrokerId = brokers.Data[0].Id;

        var result = await DeviceService.CreateDevice(deviceDto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        var client = await ClientService.GetClient(brokers.Data[0].Id);

        var totalDevices = clients.Data.SelectMany(x => x.GetDevices())
            .ToList();

        Assert.That(totalDevices, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task UpdateDeviceTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var device = await FetchDeviceService.GetDevices();

        var newPlacing = "NewPlacing";
        var newNickname = "NewNickname";

        var updatedDevice = new DeviceDTO()
        {
            Id = device.Data[0].Id,
            BrokerId = device.Data[0].BrokerId,
            BaseDevicePath = device.Data[0].BaseDevicePath,
            Name = newNickname,
            Icon = device.Data[0].Icon,
            Placing = "NewPlacing"
        };

        var result = await DeviceService.UpdateDevice(updatedDevice);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalDevices = clients.Data.SelectMany(x => x.GetDevices())
            .ToList();

        Assert.That(clients.Data, Has.Count.EqualTo(2));
        Assert.That(totalDevices.Any(x => x.Name==newNickname && x.Placing == newPlacing), Is.True);
    }

    [Test]
    public async Task RemoveDeviceTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.That(clients!.Data, Has.Count.EqualTo(2));

        var devices = await FetchDeviceService.GetDevices();
        var deviceToRemove = devices.Data[0];

        var result = await DeviceService.RemoveDevice(deviceToRemove.BrokerId, deviceToRemove.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalDevices = clients.Data.SelectMany(x => x.GetDevices())
            .ToList();

        Assert.That(totalDevices, Has.Count.EqualTo(1));
    }
}
