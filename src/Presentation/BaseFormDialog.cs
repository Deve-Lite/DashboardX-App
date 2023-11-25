namespace Presentation;

public abstract class BaseFormDialog : BaseDialog
{
    protected abstract string Title();

    protected MudForm Form = new();

    protected override async Task OnAfterRenderAsync(bool firstRender) 
        => await base.OnAfterRenderAsync(firstRender);
}
