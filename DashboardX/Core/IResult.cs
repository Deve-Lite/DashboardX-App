
using MudBlazor;
using System.Net;

namespace Core;

public interface IResult
{
    HttpStatusCode StatusCode { get; set; }
    OperationState OperationState { get; set; }
    List<string> Messages { get; set; }

    bool Succeeded { get; }
    bool ShowToast { get; }
    Severity Severity { get; }
}

public interface IResult<T> : IResult
{
    T Data { get; set; }
}