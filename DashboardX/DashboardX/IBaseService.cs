using System.Text.Json;

namespace DashboardX;

public interface IBaseService
{
    Task<Response<T>> SendAuthorizedAsync<T>(Request request, JsonSerializerOptions options) where T : class, new();
    Task<Response> SendAuthorizedAsync(Request request, JsonSerializerOptions options);
    Task<Response<T>> SendAsync<T>(Request request, JsonSerializerOptions options) where T : class, new();
    Task<Response> SendAsync(Request request, JsonSerializerOptions? options = null);
}