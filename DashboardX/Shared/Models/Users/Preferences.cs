namespace Shared.Models.Users;

public class Preferences
{
    [JsonPropertyName("theme")]
    public string StringTheme { get; set; } = "inherit";
    [JsonIgnore]
    public Theme Theme
    {
        get => StringTheme switch
        {
            "light" => Theme.Light,
            "dark" => Theme.Dark,
            _ => Theme.Inherit
        };

        set => StringTheme = value switch
        {
            Theme.Light => "light",
            Theme.Dark => "dark",
            _ => "inherit"
        };

    }


    [JsonPropertyName("language")]
    public string StringLanguage { get; set; } = "en";
    [JsonIgnore]
    public Language Language
    {
        get => StringLanguage switch
        {
            "pl" => Language.Polish,
            _ => Language.English
        };

        set => StringLanguage = value switch
        {
            Language.Polish => "pl",
            _ => "en"
        };
    }
}
