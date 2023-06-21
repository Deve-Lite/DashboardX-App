
namespace DashboardXModels.Controls;


/// Json On Payload Template:
/// {
///   "value": @OnValue
/// }
/// 
/// Json Off Payload Template:
/// {
///   "value": @OffValue
/// } 
public class SwitchControl : Control
{
    public string PayloadTemplate { get; set; } = string.Empty;
    public string OnValue { get; set; } = string.Empty;
    public string OffValue { get; set; } = string.Empty;
    public bool State { get; set; }
}
