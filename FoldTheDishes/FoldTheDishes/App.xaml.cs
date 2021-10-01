using FoldTheDishes.Models;
using FoldTheDishes.Services;
using FoldTheDishes.ViewModels;
using FoldTheDishes.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FoldTheDishes
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            var reminderStore = ReminderStore.Instance.GetAwaiter().GetResult();
            DependencyService.RegisterSingleton(reminderStore);

            PreferenceManager.SetTheme();
            PreferenceManager.ApplyReminderRetention(reminderStore);

            reminderStore.AdjustRepeatingDueDateTimes().GetAwaiter().GetResult();

            DependencyService.Get<INotificationManager>().NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Id);
            };

            MainPage = new AppShell();
        }
        void ShowNotification(int id)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(ReminderDetailViewModel.Id)}={id}", true);
            });
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
