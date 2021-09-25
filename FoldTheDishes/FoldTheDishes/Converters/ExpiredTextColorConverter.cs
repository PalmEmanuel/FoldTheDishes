using FoldTheDishes.Services;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace FoldTheDishes.Converters
{
    public class ExpiredTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var now = DateTime.Now;
            var reminderDue = (DateTime)value;
            if (now > reminderDue)
            {
                return ValidationHelper.GetErrorColor();
            }
            else
            {
                return ValidationHelper.GetTextColor();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
