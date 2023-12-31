using Common.Controls.Models;

namespace Common.Controls.Extensions;

public class ColorFormatExtensions
{
    public static ColorFormat ToColorFormat(string value)
    {
        return value switch
        {
            "rgb" => ColorFormat.HexRGB,
            _ => throw new ArgumentException($"Invalid color format: {value}")
        };
    }

    public static string ToString(ColorFormat colorFormat)
    {
        return colorFormat switch
        {
            ColorFormat.HexRGB => "rgb",
            _ => throw new ArgumentException($"Invalid color format: {colorFormat}")
        };
    }
}
