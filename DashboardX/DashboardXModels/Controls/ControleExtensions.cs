
namespace DashboardXModels.Controls;

public static class ControlExtensions
{
    public static DateTime CreatedAt(this Control control) => new(control.CreatedAtTicks);

    public static DateTime EditedAt(this Control control) => new(control.EditedAtTicks);
}
