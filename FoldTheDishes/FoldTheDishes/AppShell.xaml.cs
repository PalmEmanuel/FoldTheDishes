using FoldTheDishes.Views;
using Xamarin.Forms;

namespace FoldTheDishes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Device.SetFlags(new string[] { "AppTheme_Experimental" });
            InitializeComponent();
            Routing.RegisterRoute(nameof(RemindersPage), typeof(RemindersPage));
            Routing.RegisterRoute(nameof(ReminderDetailPage), typeof(ReminderDetailPage));
            Routing.RegisterRoute(nameof(NewReminderPage), typeof(NewReminderPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
