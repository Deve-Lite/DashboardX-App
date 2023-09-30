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

        if (response.StatusCode == HttpStatusCode.OK)
        {

            try
            {
                var list = await _localStorage.GetItemAsync<List<Device>>(DeviceConstants.DevicesListName) ?? new List<Device>();
                foreach (var device in response.Data)
                {
                    if (list.Any())
                    {
                        int index = list.FindIndex(x => x.BrokerId == device.BrokerId);

                        if (index != -1)
                            list[index] = device;
                        else
                            list.Add(device);
                    }
                    else
                        list.Add(device);
                }

                string data = JsonSerializer.Serialize(list);
                await _localStorage.SetItemAsync(DeviceConstants.DevicesListName, data);
            }
            catch (Exception e)
            {
                //TODO: Investigate error.
                Console.WriteLine(e);
                _logger.LogError($"Failed to update cache.{e.Message}");
            }

        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Device>>(DeviceConstants.DevicesListName);
            response.Data = list.Where(x => x.BrokerId == brokerId).ToList();
        }

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

        if (response.StatusCode == HttpStatusCode.OK)
        {
            await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, response.Data);
        }

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            var list = await _localStorage.GetItemAsync<List<Device>>(DeviceConstants.DevicesListName);
            response.Data = list.SingleOrDefault(b => b.BrokerId == id)!;
        }

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

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(DeviceConstants.DevicesListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<List<Device>>(DeviceConstants.DevicesListName);

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

        await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, device);

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

        await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, device);


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

        if (response.Succeeded)
            await _localStorage.RemoveItemFromList<Device>(DeviceConstants.DevicesListName, deviceId);
        
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

        if (response.StatusCode == HttpStatusCode.OK)
            await _localStorage.SetItemAsync(DeviceConstants.DevicesListName, response.Data);

        if (response.StatusCode == HttpStatusCode.NotModified)
            response.Data = await _localStorage.GetItemAsync<List<Control>>(ControlStoragePath(deviceId));

        return response;
    }

    public async Task<IResult> RemoveDeviceControls(string deviceId, string controlId)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/devices/{deviceId}/controls/controlId",
        };

        var response = await SendAsync(request);

        if (response.Succeeded)
            await _localStorage.RemoveItemFromList<Device>(ControlStoragePath(deviceId), controlId);

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

        await _localStorage.UpsertItemToList(ControlStoragePath(control.DeviceId), control);

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

        await _localStorage.UpsertItemToList(ControlStoragePath(control.DeviceId), control);

        return Result<Control>.Success(control, response.StatusCode);
    }

    #endregion

    #region Privates

    public static string ControlStoragePath(string deviceId) => $"{deviceId}{DeviceConstants.ControlsListName}";

    #endregion
}
