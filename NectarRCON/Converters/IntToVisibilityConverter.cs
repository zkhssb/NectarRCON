using System;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace NectarRCON.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int intValue)
            {
                if(parameter is string boolStringValue)
                {
                    bool boolValue = System.Convert.ToBoolean(boolStringValue);
                    // 如果参数为False
                    // <= 0 ? Visible : Hidden
                    // 否则反过来
                    return boolValue 
                        ? (intValue <= 0 ? Visibility.Hidden : Visibility.Visible) 
                        : (intValue <= 0 ? Visibility.Visible : Visibility.Hidden);
                }
                return intValue <= 0 ? Visibility.Visible : Visibility.Hidden;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
