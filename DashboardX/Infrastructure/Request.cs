namespace Infrastructure;

public class Request
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string Route { get; set; } = string.Empty;

    public static Request Get(string route) => new() { Method = HttpMethod.Get, Route = route };

    public static Request Post(string route) => new() { Method = HttpMethod.Post, Route = route };

    public static Request Put(string route) => new() { Method = HttpMethod.Put, Route = route };

    public static Request Delete(string route) => new() { Method = HttpMethod.Delete, Route = route };
}

public class Request<T> : Request where T : class, new()
{
    public T Data { get; set; } = new();

    public static Request Get(string route, T data) => new Request<T> { Method = HttpMethod.Get, Route = route, Data = data };

    public static Request Post(string route, T data) => new Request<T> { Method = HttpMethod.Post, Route = route, Data = data };

    public static Request Put(string route, T data) => new Request<T> { Method = HttpMethod.Put, Route = route, Data = data };
}