
namespace DashboardXModels.Controls;

public class ColorControl : Control
{
    public string PayloadTemplate { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public ColorFormat ColorFormat { get; set; }
}
