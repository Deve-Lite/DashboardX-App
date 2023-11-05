using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;
using System.Text.Json;
using Presentation.Application.Interfaces;

namespace Presentation.Devices;

public class FetchDeviceService : AuthorizedService, IFetchDeviceService
{
    public FetchDeviceService(HttpClient httpClient, 
                         ILogger<FetchDeviceService> logger,
                         ILoadingService loadingService,
                         NavigationManager navigationManager, 
                         AuthenticationStateProvider authenticationState)
        : base(httpClient, loadingService, logger, navigationManager, authenticationState)
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
            return Result<Device>.Fail(response.Messages, response.StatusCode);

        var itemResponse = await GetDevice(response.Data.Id);

        //TODO: Fail to get however added

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
}
