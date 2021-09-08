using FoldTheDishes.Models;
using FontAwesome;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace FoldTheDishes.Converters
{
    public class CompletedIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var completed = (bool)value;

            return completed ? FontAwesomeIcons.CheckSquare : FontAwesomeIcons.Square;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
