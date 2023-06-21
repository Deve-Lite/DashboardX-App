using System.Net;

namespace DashboardX;

public class Response<T> where T : class, new()
{
    public HttpStatusCode StatusCode { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
    public T Data { get; set; } = new();
    public bool Success => (int) StatusCode >= 200 && (int) StatusCode <= 299;
    public bool Timeout => StatusCode == HttpStatusCode.RequestTimeout;
}


public class Response
{
    public HttpStatusCode StatusCode { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
    public bool Success => (int)StatusCode >= 200 && (int)StatusCode <= 299;
    public bool Timeout => StatusCode == HttpStatusCode.RequestTimeout;
}