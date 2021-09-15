using System;
using Xamarin.Forms;

namespace FoldTheDishes.Behaviors
{
    static class ValidationHelper
    {
        public static void ValidateDateTimeAndSetLabelColor(DateTime date, TimeSpan time, Label dateLabel, Label timeLabel)
        {
            // Remove seconds for comparison
            var now = DateTime.Now.TrimToMinutes();

            // Date becomes invalid if yesterday or before
            if (date.Date < now.Date)
            {
                dateLabel.TextColor = GetErrorColor();
            }
            else
            {
                dateLabel.TextColor = GetTextColor();
            }

            // Time becomes invalid if date + time is exactly now or before
            if (date.Add(time).TrimToMinutes() <= now)
            {
                timeLabel.TextColor = GetErrorColor();
            }
            else
            {
                timeLabel.TextColor = GetTextColor();
            }
        }

        internal static void ValidateEntryAndSetPlaceholderColor(Entry entry, string newTextValue)
        {
            if (string.IsNullOrWhiteSpace(newTextValue))
            {
                entry.PlaceholderColor = GetErrorColor();
            }
            else
            {
                entry.PlaceholderColor = GetTextColor();
            }
        }

        private static Color GetErrorColor()
        {
            switch (Application.Current.RequestedTheme)
            {
                case OSAppTheme.Unspecified:
                    return (Color)Application.Current.Resources["ColorErrorDark"];
                case OSAppTheme.Light:
                    return (Color)Application.Current.Resources["ColorErrorDark"];
                case OSAppTheme.Dark:
                    return (Color)Application.Current.Resources["ColorErrorLight"];
                default:
                    throw new Exception("Undefined color theme.");
            }
        }

        private static Color GetTextColor()
        {
            switch (Application.Current.RequestedTheme)
            {
                case OSAppTheme.Unspecified:
                    return (Color)Application.Current.Resources["PrimaryTextLight"];
                case OSAppTheme.Light:
                    return (Color)Application.Current.Resources["PrimaryTextDark"];
                case OSAppTheme.Dark:
                    return (Color)Application.Current.Resources["PrimaryTextLight"];
                default:
                    throw new Exception("Undefined color theme.");
            }
        }
    }
}
