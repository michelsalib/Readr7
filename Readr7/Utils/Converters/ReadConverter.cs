using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Readr7.Utils.Converters
{
    public class ReadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = value as bool?;

            if (b.HasValue && b.Value)
            {
                return App.Current.Resources["PhoneDisabledBrush"];
            }
            else
                return App.Current.Resources["PhoneForegroundBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
