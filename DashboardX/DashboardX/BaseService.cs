using System.Net;
using System.Text;
using System.Text.Json;

namespace DashboardX;

public class BaseService : IBaseService
{
    protected readonly HttpClient client;
    protected readonly Auth.Services.IAuthorizationService authorizationService;

    public BaseService(HttpClient httpClient, 
                           Auth.Services.IAuthorizationService authorizationService)
    {
        client = httpClient;
        this.authorizationService = authorizationService;
    }

    public async Task<Response<T>> SendAuthorizedAsync<T>(Request request, JsonSerializerOptions? options = null) where T : class, new()
    {
        HttpRequestMessage message = CreateMessage(request, options);
        await authorizationService.AuthorizeMessage(message);

        return await Run<T>(message);
    }

    public async Task<Response<T>> SendAsync<T>(Request request, JsonSerializerOptions? options = null) where T : class, new()
    {
        HttpRequestMessage message = CreateMessage(request);

        return await Run<T>(message);
    }

    public async Task<Response> SendAsync(Request request, JsonSerializerOptions? options = null)
    {
        HttpRequestMessage message = CreateMessage(request);

        return await Run(message);
    }

    public async Task<Response> SendAuthorizedAsync(Request request, JsonSerializerOptions? options = null)
    {
        HttpRequestMessage message = CreateMessage(request, options);

        await authorizationService.AuthorizeMessage(message);

        return await Run(message);
    }

    private HttpRequestMessage CreateMessage(Request request, JsonSerializerOptions? options = null)
    {
        options ??= new();

        HttpRequestMessage message = new HttpRequestMessage(request.Method, request.Route);

        var data = JsonSerializer.Serialize(request.Data, options);
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        message.Content = content;

        return message;
    }

    private async Task<Response> Run(HttpRequestMessage message)
    {
        try
        {
            var response = await client.SendAsync(message);

            if (response.IsSuccessStatusCode)
                return new Response
                {
                    StatusCode = response.StatusCode,
                };
            

            var errors = JsonSerializer.Deserialize<List<string>>(await response.Content.ReadAsStringAsync())!;

            return new Response
            {
                StatusCode = response.StatusCode,
                Errors = errors
            };
        }
        catch (TaskCanceledException e)
        {
            return new Response
            {
                StatusCode = HttpStatusCode.RequestTimeout,
                Errors = new List<string> { e.Message }
            };
        }
    }

    private async Task<Response<T>> Run<T>(HttpRequestMessage message) where T : class, new()
    {
        try
        {
            var response = await client.SendAsync(message);

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync())!;

                return new Response<T>
                {
                    StatusCode = response.StatusCode,
                    Data = data,
                };
            }

            var errors = JsonSerializer.Deserialize<List<string>>(await response.Content.ReadAsStringAsync())!;

            return new Response<T>
            {
                StatusCode = response.StatusCode,
                Errors = errors
            };
        }
        catch (TaskCanceledException e)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.RequestTimeout,
                Errors = new List<string> { e.Message }
            };
        }
    }
}
