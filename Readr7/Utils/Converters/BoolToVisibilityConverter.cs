using System;
using System.Windows.Data;
using System.Windows;

namespace Readr7.Utils.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = value as bool?;
            var invert = parameter != null ? parameter.ToString() : String.Empty;

            if(invert.Equals("invert",StringComparison.InvariantCultureIgnoreCase))
                return (b.HasValue && b.Value) ? Visibility.Collapsed : Visibility.Visible;
            else
                return (b.HasValue && b.Value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
