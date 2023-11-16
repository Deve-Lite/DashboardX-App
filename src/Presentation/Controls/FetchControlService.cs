﻿using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Presentation.Application.Interfaces;

namespace Presentation.Controls;


public class FetchControlService : AuthorizedService, IFetchControlService
{
    public FetchControlService(HttpClient httpClient,
        ILoadingService loadingService,
        ILogger<AuthorizedService> logger,
        NavigationManager navigationManager,
        AuthenticationStateProvider authenticationState) 
        : base(httpClient, loadingService, logger, navigationManager, authenticationState)
    {
    }

    public async Task<IResult<List<Control>>> GetControls(string deviceId)
    {
        var request = new Request
        {
            Method = HttpMethod.Get,
            Route = $"api/v1/devices/{deviceId}/controls"
        };

        var response = await SendAsync<List<Control>>(request);

        return response;
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

    public async Task<IResult<Control>> CreateControl(ControlDTO dto)
    {
        var request = new Request<ControlDTO>
        {
            Method = HttpMethod.Post,
            Route = $"api/v1/devices/{dto.DeviceId}/controls",
            Data = dto
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<BaseModel, ControlDTO>(request, options);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.Messages, response.StatusCode);

        var control = new Control(response.Data.Id, dto);

        return Result<Control>.Success(control, response.StatusCode);
    }

    public async Task<IResult<Control>> UpdateControl(ControlDTO dto)
    {
        var request = new Request<ControlDTO>
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