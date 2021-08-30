using FoldTheDishes.Services;
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
            MainPage = new AppShell();
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
