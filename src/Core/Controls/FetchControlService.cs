using Core.App.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Core.Controls;

public class FetchControlService : AuthorizedService, IFetchControlService
{
    public FetchControlService(HttpClient httpClient,
                               ILogger<AuthorizedService> logger,
                               IAuthorizationManager authorizationManager)
        : base(httpClient, logger, authorizationManager) { }

    public async Task<IResult<List<Control>>> GetControls(string deviceId)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices/{deviceId}/controls"
        };

        var response = await SendAsync<List<ControlDto>>(request);

        if (response.Succeeded)
        {
            var brokers = response.Data.Select(x => Control.FromDto(x)).ToList();
            return Result<List<Control>>.Success(brokers, response.StatusCode);
        }

        return Result<List<Control>>.Fail(response.StatusCode, response.Messages[0]);
    }

    public async Task<IResult> RemoveControl(string deviceId, string controlId)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/devices/{deviceId}/controls/{controlId}",
        };

        var response = await SendAsync(request);

        return response;

    }

    public async Task<IResult<Control>> CreateControl(ControlDto dto)
    {
        var request = new Request<ControlDto>
        {
            Method = HttpMethod.Post,
            Route = $"api/v1/devices/{dto.DeviceId}/controls",
            Data = dto
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<BaseModel, ControlDto>(request, options);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.Messages, response.StatusCode);

        var control = new Control(response.Data.Id, dto);

        return Result<Control>.Success(control, response.StatusCode);
    }

    public async Task<IResult<Control>> UpdateControl(ControlDto dto)
    {
        var request = new Request<ControlDto>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/devices/{dto.DeviceId}/controls/{dto.Id}",
            Data = dto
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync(request, options);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.Messages, response.StatusCode);

        var control = new Control(dto.Id, dto);

        return Result<Control>.Success(control, response.StatusCode);
    }
}
