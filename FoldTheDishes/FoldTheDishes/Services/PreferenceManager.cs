using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FoldTheDishes.Services
{
    static class PreferenceManager
    {
        public static void ApplyReminderRetention(ReminderStore store)
        {
            string retention = "Forever";
            // If user has saved a preference before, retreive it
            if (Preferences.ContainsKey(Constants.CONFIG_REMINDER_RETENTION_TEXT))
            {
                retention = Preferences.Get(Constants.CONFIG_REMINDER_RETENTION_TEXT, "Forever");
            }

            DateTime date;
            DateTime today = DateTime.Now.Date;
            switch (retention)
            {
                case "Forever":
                    return;
                case "1 Year":
                    date = today.AddYears(-1);
                    break;
                case "6 Months":
                    date = today.AddMonths(-6);
                    break;
                case "3 months":
                    date = today.AddMonths(-3);
                    break;
                case "1 Month":
                    date = today.AddMonths(-1);
                    break;
                case "1 Week":
                    date = today.AddDays(-7);
                    break;
                case "1 Day":
                    date = today.AddDays(-1);
                    break;
                default:
                    return;
            }

            var itemsToDelete = store.GetDoneItemsThatAreOlderThanDateAsync(date).GetAwaiter().GetResult();

            foreach (var item in itemsToDelete)
            {
                store.DeleteItemAsync(item);
            }
        }

        public static void SetTheme()
        {
            bool themeLoaded = false;
            OSAppTheme currentTheme = Application.Current.RequestedTheme;

            // If user has saved a preference before, retreive it
            if (Preferences.ContainsKey(Constants.CONFIG_THEME_TEXT))
            {
                themeLoaded = Enum.TryParse(Preferences.Get(Constants.CONFIG_THEME_TEXT, "Light"), out currentTheme);
            }

            if (!themeLoaded)
            {
                switch (currentTheme)
                {
                    case OSAppTheme.Unspecified:
                        Preferences.Set(Constants.CONFIG_THEME_TEXT, "Light");
                        break;
                    case OSAppTheme.Light:
                        Preferences.Set(Constants.CONFIG_THEME_TEXT, "Light");
                        break;
                    case OSAppTheme.Dark:
                        Preferences.Set(Constants.CONFIG_THEME_TEXT, "Dark");
                        break;
                }
            }

            Application.Current.UserAppTheme = currentTheme;
        }
    }
}
