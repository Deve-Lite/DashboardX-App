using Blazored.LocalStorage;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Constraints;
using Shared.Models.Users;

namespace Infrastructure.Services;

public sealed class PreferenceService : IPrefrenceService
{
    private readonly ILogger<PreferenceService> _logger;
    private readonly ILocalStorageService _localStorage;

    public Func<Preferences, Task> OnPreferenceChange { get; set; }

    public PreferenceService(ILogger<PreferenceService> logger, ILocalStorageService localStorage)
    {
        _logger = logger;
        _localStorage = localStorage;

        OnPreferenceChange = (preferences) => { return Task.CompletedTask; };
    }

    public async Task UpdatePreferences(Preferences preferences)
    {
        await _localStorage.SetItemAsync(PreferenceConstraints.Preferences, preferences);
        await OnPreferenceChange(preferences);
    }

    public async Task LoadPreferences()
    {
        Preferences preferences = new();
        try
        {
            preferences = await _localStorage.GetItemAsync<Preferences>(PreferenceConstraints.Preferences);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to load preferences, restoring defaults.");
            await _localStorage.SetItemAsync(PreferenceConstraints.Preferences, preferences);
        }
        finally
        {
            await OnPreferenceChange(preferences);
        }
    }

    public async Task RestroreDefaultPreferences()
    {
        var preferences = new Preferences();
        await _localStorage.SetItemAsync(PreferenceConstraints.Preferences, preferences);
        await OnPreferenceChange(preferences);
    }
}
