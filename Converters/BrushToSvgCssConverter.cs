using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;

namespace ModPane.Converters;

public class BrushToSvgCssConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is List<Color> color)
        {
            if (color.Count != 2)
            {
                Console.WriteLine("BrushToSvgCssConverter: Invalid color list length, expected 2 colors.");
                return "* { fill: #000000 !important; }"; // Fallback
            }

            var finalColor = Application.Current!.ActualThemeVariant == ThemeVariant.Dark ? color[1] : color[0];

            // Format RGB without alpha channel
            string stringColor = $"#{finalColor.R:X2}{finalColor.G:X2}{finalColor.B:X2}";

            return $"* {{ fill: {stringColor} !important; }}";
        }

        Console.WriteLine($"BrushToSvgCssConverter: Invalid brush type {value?.GetType().Name ?? "null"}");
        return "* { fill: #000000 !important; }"; // Fallback
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
