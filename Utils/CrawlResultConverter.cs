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
            switch ((CrawlManager.CrawlDataResult) value)
            {
                case CrawlManager.CrawlDataResult.Ready:
                    return 0;
                case CrawlManager.CrawlDataResult.Queue:
                    return 1;
                case CrawlManager.CrawlDataResult.Finish:
                    return 2;
                case CrawlManager.CrawlDataResult.Error:
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