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
        var fallback = ""; // Default fallback CSS
        var fill = true; // Default to fill
        if (parameter is bool)
        {
         
            fill = (bool)parameter;
        }
        if (fill)
        {
            fallback = "* { fill: #000000 !important; }"; // Default fill color
        }
        else
        {
            fallback = "* { stroke: #000000 !important; }"; // Default stroke color
        }

        if (value is List<Color> color)
        {
            if (color.Count != 2)
            {
              
                return fallback;
            }

            var finalColor = Application.Current!.ActualThemeVariant == ThemeVariant.Dark ? color[1] : color[0];

            // Format RGB without alpha channel
            string stringColor = $"#{finalColor.R:X2}{finalColor.G:X2}{finalColor.B:X2}";

            if (fill)
            {
                return $"* {{ fill: {stringColor} !important; }}";
            }
            else
            {
                return $"* {{ stroke: {stringColor} !important; }}";
            }
        }

        return fallback;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
