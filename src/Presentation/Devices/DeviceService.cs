using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Presentation.Devices;

public class DeviceService : AuthorizedService, IDeviceService
{
    public DeviceService(HttpClient httpClient, 
                         ILocalStorageService localStorageService, 
                         ILogger<DeviceService> logger,
                         NavigationManager navigationManager, 
                         AuthenticationStateProvider authenticationState)
        : base(httpClient, localStorageService, logger, navigationManager, authenticationState)
    {
    }

    public async Task<IResult<List<Device>>> GetDevices(string brokerId)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices?brokerId={brokerId}"
        };

        var response = await SendAsync<List<Device>>(request);

        return response;
    }

    public async Task<IResult<Device>> GetDevice(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices/{id}"
        };

        var response = await SendAsync<Device>(request);

        return response;
    }

    public async Task<IResult<List<Device>>> GetDevices()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/devices"
        };

        var response = await SendAsync<List<Device>>(request);

        return response;
    }

    public async Task<IResult<Device>> CreateDevice(DeviceDTO dto)
    {
        var request = new Request<DeviceDTO>
        {
            Method = HttpMethod.Post,
            Route = $"api/v1/devices",
            Data = dto
        };

        var response = await SendAsync<BaseModel, DeviceDTO>(request);

        if (!response.Succeeded)
            return Result<Device>.Fail( response.Messages, response.StatusCode);

        var itemResponse = await GetDevice(response.Data.Id);

        if (!itemResponse.Succeeded)
            return Result<Device>.Fail(itemResponse.Messages, itemResponse.StatusCode);

        var device = itemResponse.Data;

        return Result<Device>.Success(device, response.StatusCode);
    }

    public async Task<IResult<Device>> UpdateDevice(DeviceDTO dto)
    {

        var request = new Request<DeviceDTO>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/devices/{dto.Id}",
            Data = dto
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<DeviceDTO>(request, options);

        if (!response.Succeeded)
            return Result<Device>.Fail(response.Messages, response.StatusCode);

        var itemResponse = await GetDevice(dto.Id);

        if (!itemResponse.Succeeded)
            return Result<Device>.Fail(itemResponse.Messages, itemResponse.StatusCode);

        var device = itemResponse.Data;

        return Result<Device>.Success(device, response.StatusCode);
    }

    public async Task<IResult> RemoveDevice(string deviceId)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/devices/{deviceId}"
        };

        var response = await SendAsync(request);

        return response;
    }

    #region DeviceControls

    public async Task<IResult<List<Control>>> GetDeviceControls(string deviceId)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices/{deviceId}/controls"
        };

        var response = await SendAsync<List<Control>>(request);

        return response;
    }

    public async Task<IResult> RemoveDeviceControls(string deviceId, string controlId)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/devices/{deviceId}/controls/{controlId}",
        };

        var response = await SendAsync(request);

        return response;
    }

    public async Task<IResult<Control>> CreateDeviceControl(Control control)
    {
        var request = new Request<Control>
        {
            Method = HttpMethod.Post,
            Route = $"api/v1/devices/{control.DeviceId}/controls",
            Data = control
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<BaseModel, Control>(request, options);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.Messages, response.StatusCode);

        control.Id = response.Data.Id;

        return Result<Control>.Success(control, response.StatusCode);
    }

    public async Task<IResult<Control>> UpdateDeviceControl(Control control)
    {
        var request = new Request<Control>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/devices/{control.DeviceId}/controls/{control.Id}",
            Data = control
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync(request, options);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.Messages, response.StatusCode);

        return Result<Control>.Success(control, response.StatusCode);
    }

    #endregion

    #region Privates

    public static string ControlStoragePath(string deviceId) => $"{deviceId}{DeviceConstants.ControlsListName}";

    #endregion
}
