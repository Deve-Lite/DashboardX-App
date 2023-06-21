namespace DashboardX;

public class Request<T> where T : class, new()
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public T Data { get; set; } = new();
    public string Route { get; set; } = string.Empty;   
}

public class Request
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string Route { get; set; } = string.Empty;
    public object? Data { get; set; }
}