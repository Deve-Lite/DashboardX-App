using DashboardX.Services.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;

namespace DashboardX.Services;

public abstract class BaseService : IBaseService
{
    protected readonly HttpClient _client;

    protected readonly string _baseUrl;
    protected readonly string _apiVersion;

    public BaseService(HttpClient httpClient, IConfiguration configuration)
    {
        _client = httpClient;

        _baseUrl = configuration.GetValue<string>("Api:Url")!;
        _apiVersion = configuration.GetValue<string>("Api:Version")!;
    }

    public async Task<Response<T>> SendAsync<T>(Request request, JsonSerializerOptions? options = null) where T : class, new()
    {
        HttpRequestMessage message = CreateMessage(request);

        #if DEBUG
        await Task.Delay(1000);
        #endif

        return await Run<T>(message);
    }

    public async Task<Response> SendAsync(Request request, JsonSerializerOptions? options = null)
    {
        HttpRequestMessage message = CreateMessage(request);

        #if DEBUG
        await Task.Delay(1000);
        #endif

        return await Run(message);
    }

    protected virtual HttpRequestMessage CreateMessage(Request request, JsonSerializerOptions? options = null)
    {
        options ??= new();

        HttpRequestMessage message = new HttpRequestMessage(request.Method, CombineResourcePath(request.Route));

        var data = JsonSerializer.Serialize(request.Data, options);
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        message.Content = content;

        return message;
    }

    protected virtual Task OnUnauthorised(HttpResponseMessage response) => Task.CompletedTask;

    protected async Task<Response> Run(HttpRequestMessage message)
    {
        try
        {
            var response = await _client.SendAsync(message);

            if (response.IsSuccessStatusCode)
                return new Response
                {
                    StatusCode = response.StatusCode,
                };

            //TODO: Wrapper -> data propably will be wrapped in some object

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                await OnUnauthorised(response);

            var payload = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(payload))
            {
                var errors = JsonSerializer.Deserialize<List<string>>(payload)!;
                return new Response
                {
                    StatusCode = response.StatusCode,
                    Errors = errors
                };
            }

            return new Response
            {
                StatusCode = response.StatusCode,
            };
        }
        catch (TaskCanceledException)
        {
            return new Response
            {
                StatusCode = HttpStatusCode.RequestTimeout,
                Errors = new List<string> { "Operation timed out." }
            };
        }
    }

    protected async Task<Response<T>> Run<T>(HttpRequestMessage message) where T : class, new()
    {
        try
        {
            var response = await _client.SendAsync(message);

            if (response.IsSuccessStatusCode)
            {
                //TODO: Wrapper -> data propably will be wrapped in some object
                var data = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync())!;

                return new Response<T>
                {
                    StatusCode = response.StatusCode,
                    Data = data,
                };
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                await OnUnauthorised(response);

            //TODO: Wrapper -> data propably will be wrapped in some object

            var payload = await response.Content.ReadAsStringAsync();

            if(!string.IsNullOrEmpty(payload))
            {
                var errors = JsonSerializer.Deserialize<List<string>>(payload)!;
                return new Response<T>
                {
                    StatusCode = response.StatusCode,
                    Errors = errors
                };
            }

            return new Response<T>
            {
                StatusCode = response.StatusCode,
            };
        }
        catch (TaskCanceledException)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.RequestTimeout,
                Errors = new List<string> { "Operation timed out." }
            };
        }
    }

    protected string CombineResourcePath(string resourcePath) => $"{_baseUrl}/{_apiVersion}/{resourcePath}";
}
