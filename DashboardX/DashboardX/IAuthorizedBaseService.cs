using System.Text.Json;

namespace DashboardX;

public interface IAuthorizedBaseService
{
    Task<Response<T>> SendAuthorizedAsync<T>(Request request, JsonSerializerOptions options) where T : class, new();
    Task<Response> SendAuthorizedAsync(Request request, JsonSerializerOptions options);
}
