using System;
using System.Globalization;
using System.Windows.Data;

namespace BuzzerWolf.Extensions
{
    public class UtcToLocalDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset dt)
                return dt.ToLocalTime();
            else
                return DateTime.Parse(value?.ToString()).ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
