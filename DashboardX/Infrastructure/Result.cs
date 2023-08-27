
using Core;
using MudBlazor;
using System.Net;

namespace Infrastructure;

public class Result : IResult
{
    public HttpStatusCode StatusCode { get; set; }
    public OperationState OperationState { get; set; }
    public List<string> Messages { get; set; } = new();
    public bool ShowToast => Messages.Any();
    public bool Succeeded => OperationState switch 
    { 
        OperationState.Success or 
        OperationState.Warning => true, 
        _ => false
    };

    public Severity Severity => OperationState switch
    {
        OperationState.Success => Severity.Success,
        OperationState.Warning => Severity.Warning,
        _ => Severity.Error,
    };

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK) => new() 
    { 
        OperationState = OperationState.Success,
        StatusCode = statusCode
    };

    public static Result Warning(HttpStatusCode statusCode = HttpStatusCode.OK, string message = "")  => new()
    {
        OperationState = OperationState.Warning,
        StatusCode = statusCode,
        Messages = new List<string>
        {
            message
        }
    };

    public static Result Fail(HttpStatusCode statusCode = HttpStatusCode.BadRequest, string message = "") => new() 
    { 
        OperationState = OperationState.Error,
        StatusCode = statusCode,
        Messages = new List<string> 
        {
            message 
        }
    };

    public static Result Fail(List<string> messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        OperationState = OperationState.Error,
        StatusCode = statusCode,
        Messages = messages
    };

    public static Result Timeout(string message = "") => new() 
    { 
        OperationState = OperationState.OperationTimedOut, 
        StatusCode = HttpStatusCode.RequestTimeout, 
        Messages = new List<string> 
        {
            message 
        } 
    };
}


public class Result<T> : Result, IResult<T>  
{
    public T Data { get; set; }

    public Result() 
    {
        Data = default!;
    }

    public static Result<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK) => new()
    {
        Data = data, 
        OperationState = OperationState.Success, 
        StatusCode = statusCode 
    };

    public static new Result<T> Warning(HttpStatusCode statusCode = HttpStatusCode.OK, string message = "") => new()
    {
        OperationState = OperationState.Warning,
        StatusCode = statusCode,
        Messages = new List<string>
        {
            message
        }
    };

    public static Result<T> Warning(T data, HttpStatusCode statusCode = HttpStatusCode.OK, string message = "") => new()
    {
        Data = data, 
        OperationState = OperationState.Warning, 
        StatusCode = statusCode,
        Messages = new List<string>
        {
            message
        }
    };

    public static new Result<T> Fail( HttpStatusCode statusCode = HttpStatusCode.BadRequest, string message = "") => new()
    {
        OperationState = OperationState.Error,
        StatusCode = statusCode, 
        Messages = new List<string>
        {
            message 
        } 
    };

    public static new Result<T> Fail(List<string> messages, HttpStatusCode statusCode = HttpStatusCode.BadRequest) => new()
    {
        OperationState = OperationState.Error,
        StatusCode = statusCode,
        Messages = messages
    };

    public static new Result<T> Timeout(string message = "") => new()
    {
        OperationState = OperationState.OperationTimedOut, 
        StatusCode = HttpStatusCode.RequestTimeout, 
        Messages = new List<string>
        {
            message 
        } 
    };
}
