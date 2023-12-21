namespace Presentation.Devices;

public class DeviceService : IDeviceService
{
    private readonly IFetchDeviceService _deviceService;
    private readonly IClientManager _clientManager;
    private readonly IUnusedDeviceService _unusedDeviceService;

    public DeviceService(IFetchDeviceService deviceService, IClientManager clientManager, IUnusedDeviceService unusedDeviceService)
    {
        _clientManager = clientManager;
        _deviceService = deviceService;
        _unusedDeviceService = unusedDeviceService;
    }

    public async Task<IResult> RemoveDevice(string clientId, string deviceId)
    {
        var result = await _deviceService.RemoveDevice(deviceId);

        if (!result.Succeeded)
            return result;

        if (_unusedDeviceService.ContainsDevice(deviceId))
        {
            _unusedDeviceService.RemoveDevice(deviceId);
            return result;
        }

        var clientResult = _clientManager.GetClient(clientId);

        if (!clientResult.Succeeded)
            return Result.Warning();

        var removeResult = await clientResult.Data.RemoveDevice(deviceId);

        if (removeResult.OperationState != OperationState.Success)
            return removeResult;

        return Result.Success(result.StatusCode);
    }

    public async Task<IResult> CreateDevice(DeviceDTO device)
    {
        var result = await _deviceService.CreateDevice(device);

        if (!result.Succeeded)
            return Result<Device>.Fail(result.Messages, result.StatusCode);

        var clientResult = _clientManager.GetClient(device.BrokerId);

        if (!clientResult.Succeeded)
            return Result.Fail();

        var addResult = clientResult.Data.AddDevice(result.Data);

        return addResult;
    }

    public async Task<IResult> UpdateDevice(DeviceDTO device, string oldClientId)
    {
        var result = await _deviceService.UpdateDevice(device);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        if (_unusedDeviceService.ContainsDevice(device.Id))
            return UpdateUnusedDevice(result.Data);

        if (oldClientId != device.BrokerId)
            return await TransferDeviceFromClient(result.Data, oldClientId);

        var currentClient = _clientManager.GetClient(device.BrokerId);

        if (!currentClient.Succeeded)
            return currentClient;

        return await currentClient.Data.UpdateDevice(result.Data);
    }

    private IResult UpdateUnusedDevice(Device device)
    {
        var currentClient = _clientManager.GetClient(device.BrokerId);

        if (!currentClient.Succeeded)
            return currentClient;

        return currentClient.Data.AddDevice(device);
    } 

    private async Task<IResult> TransferDeviceFromClient(Device device, string oldClientId)
    {
        var currentClient = _clientManager.GetClient(device.BrokerId);
        var oldClient = _clientManager.GetClient(oldClientId);

        if (!currentClient.Succeeded || !oldClient.Succeeded)
            return Result.Fail();

        var deviceControls = oldClient.Data.GetControls(device.Id).ToList();

        await oldClient.Data.RemoveDevice(device.Id);

        return await currentClient.Data.AddDevice(device, deviceControls);
    }
}
