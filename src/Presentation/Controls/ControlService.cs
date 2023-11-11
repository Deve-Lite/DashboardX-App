﻿using Presentation.Controls.Interfaces;

namespace Presentation.Controls;

public class ControlService : IControlService
{
    private readonly IFetchControlService _deviceService;
    private readonly IClientManager _clientManager;
    public ControlService(IClientManager clientManager, IFetchControlService fetchDeviceService)
    {
        _deviceService = fetchDeviceService;
        _clientManager = clientManager;
    }

    public async Task<IResult> RemoveControl(string clientId, Control control)
    {
        var result = await _deviceService.RemoveControl(control.DeviceId, control.Id);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var clientResult = _clientManager.GetClient(clientId);

        if (!clientResult.Succeeded)
            return Result.Warning();

        return await clientResult.Data.RemoveControl(control.Id);
    }

    public async Task<IResult> CreateControl(string clientId, Control control)
    {
        var result = await _deviceService.CreateControl(control);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var clientResult = _clientManager.GetClient(clientId);

        if (!clientResult.Succeeded)
            return Result.Fail();

        return await clientResult.Data.AddControl(control);
    }

    public async Task<IResult> UpdateControl(string clientId, Control control)
    {
        var result = await _deviceService.UpdateControl(control);

        if (!result.Succeeded)
            return Result.Fail(result.Messages, result.StatusCode);

        var clientResult = _clientManager.GetClient(clientId);

        if (!clientResult.Succeeded)
            return Result.Fail();

        return await clientResult.Data.UpdateControl(control);
    }
}
