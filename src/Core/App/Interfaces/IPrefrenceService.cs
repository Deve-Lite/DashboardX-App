namespace Core.App.Interfaces;

public interface IPrefrenceService
{
    Func<Preferences, Task> OnPreferenceChange { get; set; }
    Task UpdatePreferences(Preferences preferences);
    Task LoadPreferences();
    Task RestroreDefaultPreferences();
}
