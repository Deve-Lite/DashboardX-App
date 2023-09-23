using System.Net;

namespace Common;


public interface IResult
{
    HttpStatusCode StatusCode { get; set; }
    OperationState OperationState { get; set; }
    List<string> Messages { get; set; }

    bool Succeeded { get; }
    bool ShowToast { get; }
    MudBlazor.Severity Severity { get; }
}

public interface IResult<T> : IResult
{
    T Data { get; set; }
}