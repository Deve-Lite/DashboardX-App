
using Core;
using System.Net;

namespace Infrastructure;

public class Result : IResult
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = new();

    public Result() { }

    public static Result Fail(HttpStatusCode statusCode, List<string> messages) => new() { Succeeded = false, Messages = messages, StatusCode = statusCode };
    public static Result Success(HttpStatusCode statusCode) => new() { Succeeded = true, StatusCode = statusCode };
    public static Result Timeout(string message) => new() { Succeeded = false, StatusCode = HttpStatusCode.RequestTimeout, Messages = new List<string> { message } };
    public static Result Fail(HttpStatusCode statusCode) => new() { Succeeded = false, StatusCode = statusCode };
    public static Result Fail(string messages) => new() { Succeeded = false, Messages = new List<string> { messages }, };
}


public class Result<T> : Result, IResult<T>  
{
    public T Data { get; set; }

    public Result() 
    {
        Data = default!;
    }

    public static Result<T> Success(HttpStatusCode statusCode, T data) => new() { Succeeded = true, Data = data, StatusCode = statusCode };
    public static Result<T> Fail(HttpStatusCode statusCode, string messages) => new() { Succeeded = false, Messages = new List<string> { messages }, StatusCode = statusCode };
    public static new Result<T> Fail(string messages) => new() { Succeeded = false, Messages = new List<string> { messages }, };
    public static new Result<T> Fail(HttpStatusCode statusCode, List<string> messages) => new() { Succeeded = false, Messages = messages, StatusCode = statusCode };
    public static new Result<T> Fail(HttpStatusCode statusCode) => new() { Succeeded = false, StatusCode = statusCode };
    public static new Result<T> Timeout(string message) => new() { Succeeded = false, StatusCode = HttpStatusCode.RequestTimeout, Messages = new List<string> { message } };
}
