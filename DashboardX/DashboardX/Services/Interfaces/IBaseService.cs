using System.Text.Json;

namespace DashboardX.Services.Interfaces;

public interface IBaseService
{
    Task<Response<T>> SendAsync<T>(Request request, JsonSerializerOptions options) where T : class, new();
    Task<Response> SendAsync(Request request, JsonSerializerOptions? options = null);
}