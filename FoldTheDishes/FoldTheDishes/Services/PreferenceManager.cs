using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FoldTheDishes.Services
{
    static class PreferenceManager
    {
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
