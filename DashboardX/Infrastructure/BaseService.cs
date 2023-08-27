
using System.Net;
using System.Text;
using System.Text.Json;

namespace Infrastructure;

public abstract class BaseService
{
    protected const int RequestDebugDelay = 0;
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
#endif

            var response = await _client.SendAsync(message);

            var payload = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    //todo log warning that no content was returned
                    return Result<T>.Success(new(), HttpStatusCode.NoContent);
                }

                var data = JsonSerializer.Deserialize<T>(payload)!;
                return Result<T>.Success(data, response.StatusCode);
            }

            if (!string.IsNullOrEmpty(payload))
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorMessage>(payload)!;
                return Result<T>.Fail(statusCode:response.StatusCode, errorResponse.Message);
            }

            return Result<T>.Fail(response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return Result<T>.Timeout("Request timed out.");
        }
        catch (Exception e)
        {
            return Result<T>.Fail(message:$"Unknown error occured. {e.Message}");
        }
    }

    protected async Task<Result> Run(HttpRequestMessage message)
    {
        try
        {
#if DEBUG
            await Task.Delay(RequestDebugDelay);
#endif

            var response = await _client.SendAsync(message);

            var payload = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Result.Success(response.StatusCode);
            
            if (!string.IsNullOrEmpty(payload))
            {
                var errors = JsonSerializer.Deserialize<List<string>>(payload)!;
                return Result.Fail(errors, response.StatusCode);
            }

            return Result.Fail(response.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return Result.Timeout("Operation timed out.");
        }
        catch (Exception e)
        {
            var errors = new List<string>()
            {
                $"Unknown error occured. {e.Message}" 
            };

            return Result.Fail(errors);
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
