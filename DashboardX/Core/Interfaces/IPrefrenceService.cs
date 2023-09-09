using Shared.Models.Users;

namespace Core.Interfaces;

public interface IPrefrenceService
{
    Func<Preferences, Task> OnPreferenceChange { get; set; }
    Task UpdatePreferences(Preferences preferences);
    Task LoadPreferences();
    Task RestroreDefaultPreferences();
}
