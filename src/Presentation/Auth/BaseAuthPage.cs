namespace Presentation.Auth;

public class BaseAuthPage : BasePage
{
    protected MudForm Form = new();
    protected bool IsDebug { get; set; }

    protected override Task OnInitializedAsync()
    {
    #if DEBUG
        IsDebug = true;
    #endif

        //TODO: if authorized redirect to suitable page

        return base.OnInitializedAsync();
    }
}

