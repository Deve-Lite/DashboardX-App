using Core;
using System.Text;
using System.Text.Json;

namespace Infrastructure;

public abstract class BaseService
{
    protected readonly HttpClient _client;

    public BaseService(HttpClient httpClient)
    {
        _client = httpClient;
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

    protected async Task<Result<T>> Run<T>(HttpRequestMessage message)
    {
        try
        {
            var response = await _client.SendAsync(message);

            var payload = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(payload)!;
                return Result<T>.Success(response.StatusCode, data);
            }

            if (!string.IsNullOrEmpty(payload))
            {
                var errors = JsonSerializer.Deserialize<List<string>>(payload)!;
                return Result<T>.Fail(response.StatusCode, errors);
            }

            return Result<T>.Fail(response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return Result<T>.Timeout("Operation timed out.");
        }
    }

    protected async Task<Result> Run(HttpRequestMessage message)
    {
        try
        {
            var response = await _client.SendAsync(message);

            var payload = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Result.Success(response.StatusCode);
            
            if (!string.IsNullOrEmpty(payload))
            {
                var errors = JsonSerializer.Deserialize<List<string>>(payload)!;
                return Result.Fail(response.StatusCode, errors);
            }

            return Result.Fail(response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return Result.Timeout("Operation timed out.");
        }
    }

    protected virtual HttpRequestMessage CreateMessage<T>(Request<T> request, JsonSerializerOptions? options = null) where T : class, new()
    {
        options ??= new();

        HttpRequestMessage message = new(request.Method, request.Route);

        var data = JsonSerializer.Serialize(request.Data, options);
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        message.Content = content;

        return message;
    }
    protected virtual HttpRequestMessage CreateMessage(Request request) => new(request.Method, request.Route);
}
