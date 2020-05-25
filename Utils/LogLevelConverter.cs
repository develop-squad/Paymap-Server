using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PAYMAP_BACKEND.Utils
{
    public class LogLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            switch ((LogLevel) value)
            {
                case LogLevel.Debug:
                    return 0;
                case LogLevel.Info:
                    return 1;
                case LogLevel.Warn:
                    return 2;
                case LogLevel.Error:
                    return 3;
                case LogLevel.Fatal:
                    return 4;
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}