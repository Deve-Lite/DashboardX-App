﻿using MudBlazor;

namespace Presentation.Utils;

public static class ThemeColors
{
    public static MudTheme AppTheme => new MudTheme()
    {
        Palette = Palette,
        PaletteDark = PaletteDark
    };

    private static PaletteLight Palette => new PaletteLight
    {
        Primary = new("9C27B0"),
        TextPrimary = new("121212"),
        PrimaryContrastText = new("F5F5F5"),

        Secondary = new("E0E0E0"),
        TextSecondary = new("9C27B0"),
        SecondaryContrastText = new(18, 18, 18, 0.4),

        Tertiary = new("c9a0dc"),

        Background = new("F5F5F5"),

        Success = new("00C853"),
        Warning = new("FFAB40"),
        Error = new("C62828")
    };

    private static PaletteDark PaletteDark => new PaletteDark
    {

    };
}