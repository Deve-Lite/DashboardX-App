using Microsoft.AspNetCore.Components;

namespace Presentation;

public class BasePage : ComponentBase
{
    public List<BreadcrumbItem>? BreadcrumbItems { get; set; }

    public bool LoadedSuccessfully { get; set; } = true;
}
