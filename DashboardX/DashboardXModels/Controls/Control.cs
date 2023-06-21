
namespace DashboardXModels.Controls;

public class Control
{
    [Key]
    public string ControlId { get; set; } = string.Empty;
    public long EditedAtTicks { get; set; }
    public long CreatedAtTicks { get; set; }

    public string Icon { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
    public bool IsConfiramtionRequired { get; set; }

    public QualityOfService QualityOfService { get; set; }

    public string Topic { get; set; } = string.Empty;
}
