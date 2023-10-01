using Microsoft.AspNetCore.Components;

namespace Presentation;

public class BaseDialog : MudDialog
{
    [CascadingParameter]
    public MudDialogInstance? Dialog { get; set; }

    protected bool isLoading = false;

    public bool IsLoading
    {
        get
        {
            return isLoading;
        }
        set
        {
            if (Dialog is not null)
            {
                var x = Dialog.Options;
                x.DisableBackdropClick = value;
                x.CloseOnEscapeKey = !value;
                Dialog.SetOptions(Dialog.Options);
            }

            isLoading = value;
        }
    }

    public void Cancel() => Dialog!.Cancel();
}
