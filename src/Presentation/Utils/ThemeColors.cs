namespace Presentation.Utils;

public static class ThemeColors
{
    private const string colorPrimary = "9C27B0";
    public const string ColorPrimary = $"#{colorPrimary}";

    public static MudTheme AppTheme => new()
    {
        Palette = Palette,
        PaletteDark = PaletteDark
    };

    private static PaletteLight Palette => new()
    {
        Primary = new(colorPrimary),

        TextPrimary = new("121212"),
        PrimaryContrastText = new("F5F5F5"),

        Secondary = new("E0E0E0"),
        TextSecondary = new("9C27B0"),
        SecondaryContrastText = new(18, 18, 18, 0.4),

        Tertiary = new($"{colorPrimary}aa"),

        Background = new("F5F5F5"),

        Success = new("00C853"),
        Warning = new("FFAB40"),
        Error = new("C62828")
    };

    private static PaletteDark PaletteDark => new()
    {
        Primary = new("#1ec7a5"),

        Secondary = new("212529"),

        Tertiary = new("#1ec7a5aa")
    };
}
