namespace Presentation.Devices;

public class DeviceService : IDeviceService
{
    private readonly IFetchDeviceService _deviceService;
    private readonly IClientManager _clientManager;

    public DeviceService(IClientManager clientManager, IFetchDeviceService deviceService)
    {
        _clientManager = clientManager;
        _deviceService = deviceService;
    }

    public async Task<IResult> RemoveDevice(string clientId, string deviceId)
    {
        var result = await _deviceService.RemoveDevice(deviceId);

        if (!result.Succeeded)
            return result;

        var clientResult = _clientManager.GetClient(clientId);

        if(!clientResult.Succeeded)
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

    public async Task<IResult> UpdateDevice(DeviceDTO device)
    {
        var result = await _deviceService.UpdateDevice(device);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var clientResult = _clientManager.GetClient(device.BrokerId);

        if (!clientResult.Succeeded)
            return Result.Fail();

        var updateResult = await clientResult.Data.UpdateDevice(result.Data);
        
        return updateResult;
    }
}
