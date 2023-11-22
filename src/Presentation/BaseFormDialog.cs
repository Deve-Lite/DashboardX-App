namespace Presentation;

public class BaseFormDialog : BaseDialog
{
    protected MudForm Form = new();

    protected override async Task OnAfterRenderAsync(bool firstRender) 
        => await base.OnAfterRenderAsync(firstRender);
}
