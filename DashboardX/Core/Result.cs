﻿
using System.Net;

namespace Core;

public class Result
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Succeeded { get; set; }
    public List<string> Messages { get; set; } = new();

    public Result() { }

    public static Result Fail(HttpStatusCode statusCode, List<string> messages) => new() { Succeeded = false, Messages = messages, StatusCode = statusCode };
    public static Result Success(HttpStatusCode statusCode) => new() { Succeeded = true, StatusCode = statusCode };
    public static Result Success(HttpStatusCode statusCode, List<string> messages) => new() { Succeeded = true, Messages = messages, StatusCode = statusCode };
    public static Result Timeout(string message) => new() { Succeeded = false, StatusCode = HttpStatusCode.RequestTimeout, Messages = new List<string> { message } };
    public static Result Fail(HttpStatusCode statusCode) => new() { Succeeded = false, StatusCode = statusCode };
}


public class Result<T> : Result
{
    public T? Data { get; set; }

    public Result() { }

    public static Result<T> Success(HttpStatusCode statusCode, T data) => new() { Succeeded = true, Data = data, StatusCode = statusCode };
    public static new Result<T> Fail(HttpStatusCode statusCode, List<string> messages) => new() { Succeeded = false, Messages = messages, StatusCode = statusCode };
    public static new Result<T> Fail(HttpStatusCode statusCode) => new() { Succeeded = false, StatusCode = statusCode };
    public static new Result<T> Timeout(string message) => new() { Succeeded = false, StatusCode = HttpStatusCode.RequestTimeout, Messages = new List<string> { message } };
}
