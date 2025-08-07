using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ModPane.Converters
{
    public class BoolToIndexConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 0 : 1; // true = 1 stop bit (index 0), false = 2 stop bits (index 1)
            }
            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index == 0; // index 0 = true (1 stop bit), index 1 = false (2 stop bits)
            }
            return true;
        }
    }
}
