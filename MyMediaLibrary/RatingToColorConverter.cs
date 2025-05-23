using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyMediaLibrary
{
    public class RatingToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double rating && parameter is string strParam && int.TryParse(strParam, out int starValue))
            {
                return rating >= starValue ? Brushes.Gold : Brushes.LightGray;
            }
            return Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}