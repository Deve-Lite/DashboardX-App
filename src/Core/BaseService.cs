using Core.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Core;

public abstract class BaseService
{
    protected const int RequestDebugDelay = 200;
    protected readonly HttpClient _client;
    protected readonly ILogger<BaseService> _logger;

    public BaseService(HttpClient httpClient, ILogger<BaseService> logger)
    {
        _client = httpClient;
        _logger = logger;
    }

    protected virtual async Task<Result<T>> SendAsync<T>(Request request, JsonSerializerOptions? options = null) where T : class, new()
    {
        var message = CreateMessage(request);
        var results = await Run<T>(message);
        return results;
    }

    protected virtual async Task<Result> SendAsync<T>(Request<T> request, JsonSerializerOptions? options = null) where T : class, new()
    {
        var message = CreateMessage(request);
        var results = await Run(message);
        return results;
    }

    protected virtual async Task<Result<T>> SendAsync<T, T1>(Request<T1> request, JsonSerializerOptions? options = null) where T1 : class, new() where T : class, new()
    {
        var message = CreateMessage(request);
        var results = await Run<T>(message);
        return results;
    }

    protected async Task<Result<T>> Run<T>(HttpRequestMessage message) where T : class, new()
    {
        try
        {
#if DEBUG
            await Task.Delay(RequestDebugDelay);
            _logger.LogInformation("Sending request to: {uri} wih {method}, {version}", message.RequestUri, message.Method, message.Version);
#endif

            var response = await _client.SendAsync(message);

            var payload = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogWarning("Response should have content but received no content status code.");
                    return Result<T>.Success(new(), HttpStatusCode.NoContent);
                }

                try
                {
                    //TODO: Chandle Invalid serialziation
                    var data = JsonSerializer.Deserialize<T>(payload)!;

                    return Result<T>.Success(data, response.StatusCode);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
                return Result<T>.Fail();
            }

            if (!string.IsNullOrEmpty(payload))
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorMessage>(payload)!;
                return Result<T>.Fail(statusCode: response.StatusCode, errorResponse.Message);
            }

            return Result<T>.Fail(response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Request timed out.");
            return Result<T>.Timeout("Request timed out.");
        }
        catch (Exception e)
        {
            _logger.LogError("Unexpected error occured.", e.Message);
            return Result<T>.Fail(message: "Failed to fetch data");
        }
    }

    protected async Task<Result> Run(HttpRequestMessage message)
    {
        try
        {
#if DEBUG
            await Task.Delay(RequestDebugDelay);
            _logger.LogInformation($"Sending request to: {message.RequestUri} with {message.Method}, {message.Version}");
#endif

            var response = await _client.SendAsync(message);

            var payload = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Result.Success(response.StatusCode);

            if (!string.IsNullOrEmpty(payload))
            {
                var error = JsonSerializer.Deserialize<ErrorMessage>(payload)!;
                return Result.Fail(response.StatusCode, error.Message);
            }

            return Result.Fail(response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Request timed out.");
            return Result.Timeout("Operation timed out.");
        }
        catch (Exception e)
        {
            _logger.LogError($"Unexpected error occured.", e.Message);
            return Result.Fail(message: "Failed to fetch data");
        }
    }

    protected virtual HttpRequestMessage CreateMessage<T>(Request<T> request, JsonSerializerOptions? options = null) where T : class, new()
    {
        options ??= new();

        HttpRequestMessage message = new(request.Method, request.Route);

        var data = JsonSerializer.Serialize(request.Data, options);
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        message.Content = content;

#if DEBUG
        _logger.LogInformation($"New payload for Request:", data);
#endif

        return message;
    }
    protected virtual HttpRequestMessage CreateMessage(Request request) => new(request.Method, request.Route);
}
