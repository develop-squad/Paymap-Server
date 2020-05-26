using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PAYMAP_BACKEND.Utils
{
    public class CrawlResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            switch ((CrawlDataResult) value)
            {
                case CrawlDataResult.Ready:
                    return 0;
                case CrawlDataResult.Queue:
                    return 1;
                case CrawlDataResult.Finish:
                    return 2;
                case CrawlDataResult.Error:
                    return 3;
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}