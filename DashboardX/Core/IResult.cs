
using System.Net;

namespace Core;

public interface IResult
{
    HttpStatusCode StatusCode { get; set; }
    bool Succeeded { get; set; }
    List<string> Messages { get; set; }
}

public interface IResult<T> : IResult
{
    T Data { get; set; }
}