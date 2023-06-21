
namespace DashboardXModels.Controls;

//TODO Now it's not considered
public class MultiValueButtonControl : Control
{
    public string Payload { get; set; } = string.Empty;
    public Dictionary<string, string> NameToValues { get; set; } = new();
}
