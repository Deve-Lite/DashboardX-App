using Presentation.Brokers;
using Presentation.Devices;
using Presentation.Devices.Interfaces;

namespace PresentationTests;

public class DeviceServiceTests : BaseTest, IAsyncLifetime
{
    public IDeviceService? DeviceService { get; private set; }

    public async Task InitializeAsync()
    {
        DeviceService = new DeviceService(FetchDeviceService, ClientManager);
        await Setup();
    }

    public Task DisposeAsync()
    {
        TearDown();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateDeviceTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var brokers = await FetchBrokerService.GetBrokers();

        var deviceDto = DeviceDtoGenerator.GenerateDeviceDto();
        deviceDto.BrokerId = brokers.Data[0].Id;

        var result = await DeviceService!.CreateDevice(deviceDto);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        var client = await ClientService.GetClient(brokers.Data[0].Id);

        var totalDevices = clients.Data.SelectMany(x => x.GetDevices()).ToList();

        Assert.Equal(3, totalDevices.Count);
    }

    [Fact]
    public async Task UpdateDeviceTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

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

        var result = await DeviceService!.UpdateDevice(updatedDevice);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalDevices = clients.Data.SelectMany(x => x.GetDevices()).ToList();

        Assert.Equal(2, clients.Data.Count);
        Assert.Contains(totalDevices, x => x.Name == newNickname && x.Placing == newPlacing);
    }

    [Fact]
    public async Task RemoveDeviceTest()
    {
        var clients = await ClientService.GetClientsWithDevices();

        Assert.Equal(2, clients!.Data.Count);

        var devices = await FetchDeviceService.GetDevices();
        var deviceToRemove = devices.Data[0];

        var result = await DeviceService!.RemoveDevice(deviceToRemove.BrokerId, deviceToRemove.Id);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        clients = await ClientService.GetClientsWithDevices();

        var totalDevices = clients.Data.SelectMany(x => x.GetDevices()).ToList();

        Assert.Single(totalDevices);
    }
}
