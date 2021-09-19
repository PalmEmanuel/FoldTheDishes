using Xamarin.Forms;
using System;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

namespace FoldTheDishes.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private OSAppTheme currentTheme;
        private bool darkMode;
        private string selectedRetentionOption;

        public ObservableCollection<string> RetentionOptions { get; }

        public bool DarkMode
        {
            get => darkMode;
            set
            {
                SetProperty(ref darkMode, value);
                UpdateTheme();
            }
        }
        public OSAppTheme CurrentTheme
        {
            get => currentTheme;
            set => SetProperty(ref currentTheme, value);
        }
        public string SelectedRetentionOption
        {
            get => selectedRetentionOption;
            set => SetProperty(ref selectedRetentionOption, value);
        }

        public SettingsViewModel()
        {
            Title = "Settings";

            CurrentTheme = Application.Current.UserAppTheme;

            DarkMode = CurrentTheme == OSAppTheme.Dark;

            RetentionOptions = new ObservableCollection<string>();
            RetentionOptions.Add("Forever");
            RetentionOptions.Add("1 Day");
            RetentionOptions.Add("1 Week");
            RetentionOptions.Add("1 Month");
            RetentionOptions.Add("3 Months");
            RetentionOptions.Add("6 Months");
            RetentionOptions.Add("1 Year");

            SelectedRetentionOption = "Forever";
        }

        private void UpdateTheme()
        {
            // Convert from 1 or 2 to Light or Dark
            CurrentTheme = Application.Current.UserAppTheme = (OSAppTheme)(Convert.ToInt32(DarkMode) + 1);

            Preferences.Set(Constants.CONFIG_THEME_TEXT, CurrentTheme.ToString());
        }
    }
}
