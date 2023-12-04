using Microsoft.Extensions.Logging;
using Core.App.Interfaces;
using System.Text.Json;

namespace Core.Devices;

public class FetchDeviceService : AuthorizedService, IFetchDeviceService
{
    public FetchDeviceService(HttpClient httpClient, 
                              ILogger<FetchDeviceService> logger,
                              IAuthorizationManager authorizationManager)
        : base(httpClient, logger, authorizationManager)
    {
    }

    public async Task<IResult<List<Device>>> GetDevices(string brokerId)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices?brokerId={brokerId}"
        };

        var response = await SendAsync<List<DeviceDTO>>(request);

        if (response.Succeeded)
        {
            var brokers = response.Data.Select(x => Device.FromDto(x)).ToList();
            return Result<List<Device>>.Success(brokers, response.StatusCode);
        }

        return Result<List<Device>>.Fail(response.StatusCode, response.Messages[0]);
    }

    public async Task<IResult<Device>> GetDevice(string id)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices/{id}"
        };

        var response = await SendAsync<DeviceDTO>(request);

        if (response.Succeeded)
        {
            var broker = Device.FromDto(response.Data);
            return Result<Device>.Success(broker, response.StatusCode);
        }

        return Result<Device>.Fail(response.StatusCode, response.Messages[0]);
    }

    public async Task<IResult<List<Device>>> GetDevices()
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = "api/v1/devices"
        };

        var response = await SendAsync<List<DeviceDTO>>(request);

        if (response.Succeeded)
        {
            var brokers = response.Data.Select(x => Device.FromDto(x)).ToList();
            return Result<List<Device>>.Success(brokers, response.StatusCode);
        }

        return Result<List<Device>>.Fail(response.StatusCode, response.Messages[0]);
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
