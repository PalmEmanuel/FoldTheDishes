using System;
using System.Globalization;
using Xamarin.Forms;

namespace FoldTheDishes.Converters
{
    public class DateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;

            var now = DateTime.Now;

            if (now.Date == date.Date)
            {
                return "Today";
            }
            else if (now.Date.AddDays(1) == date.Date)
            {
                return "Tomorrow";
            }
            else
            {
                return date.ToString("MMMM dd, yyyy");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
