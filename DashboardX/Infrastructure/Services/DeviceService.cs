﻿using Blazored.LocalStorage;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Constraints;
using Shared.Models.Devices;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Shared.Models.Controls;
using Shared.Models.Brokers;

namespace Infrastructure.Services;

public class DeviceService : AuthorizedService, IDeviceService
{
    public DeviceService(HttpClient httpClient, 
                         ILocalStorageService localStorageService, 
                         NavigationManager navigationManager, 
                         AuthenticationStateProvider authenticationState)
        : base(httpClient, localStorageService, navigationManager, authenticationState)
    {
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

    public async Task<IResult<Device>> CreateDevice(Device device)
    {
        var request = new Request<Device>
        {
            Method = HttpMethod.Post,
            Route = $"api/v1/devices",
            Data = device
        };

        var response = await SendAsync<CreateResponse, Device>(request);

        if (!response.Succeeded)
            return Result<Device>.Fail(response.StatusCode, response.Messages);

        var itemResponse = await GetDevice(response.Data.Id);

        if (!itemResponse.Succeeded)
            return Result<Device>.Fail(itemResponse.StatusCode, itemResponse.Messages + " Pleace refresh page.");

        device = itemResponse.Data;

        await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, device);

        return Result<Device>.Success(response.StatusCode, device);
    }

    public async Task<IResult<Device>> UpdateDevice(Device device)
    {
        var request = new Request<Device>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/devices/{device.Id}",
            Data = device
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Device>(request, options);

        if (!response.Succeeded)
            return Result<Device>.Fail(response.StatusCode, response.Messages);

        var itemResponse = await GetDevice(device.Id);

        if (!itemResponse.Succeeded)
            return Result<Device>.Fail(itemResponse.StatusCode, itemResponse.Messages + " Pleace refresh page.");

        device = itemResponse.Data;

        await _localStorage.UpsertItemToList(DeviceConstants.DevicesListName, device);


        return Result<Device>.Success(response.StatusCode, device);
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

    public async Task<IResult> RemoveDeviceControls(string deviceId, List<string> controlIds)
    {
        var request = new Request
        {
            Method = HttpMethod.Delete,
            Route = $"api/v1/devices/{deviceId}/controls",
        };

        var response = await SendAsync(request);

        if (response.Succeeded)
            await _localStorage.RemoveItemsFromList<Device>(ControlStoragePath(deviceId), controlIds);

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

        var response = await SendAsync<CreateResponse, Control>(request);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.StatusCode, response.Messages);

        control.Id = response.Data.Id;

        await _localStorage.UpsertItemToList(ControlStoragePath(control.DeviceId), control);

        return Result<Control>.Success(response.StatusCode, control);
    }

    public async Task<IResult<Control>> UpdateDeviceControl(Control control)
    {
        var request = new Request<Control>
        {
            Method = HttpMethod.Patch,
            Route = $"api/v1/devices/{control.DeviceId}/controls",
            Data = control
        };

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var response = await SendAsync<Control>(request, options);

        if (!response.Succeeded)
            return Result<Control>.Fail(response.StatusCode, response.Messages);

        await _localStorage.UpsertItemToList(ControlStoragePath(control.DeviceId), control);

        return Result<Control>.Success(response.StatusCode, control);
    }

    #endregion

    #region Privates

    public string ControlStoragePath(string deviceId) => $"{deviceId}{DeviceConstants.ControlsListName}";

    #endregion
}
