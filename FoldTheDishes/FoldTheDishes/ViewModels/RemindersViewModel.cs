using FoldTheDishes.Models;
using FoldTheDishes.Services;
using FoldTheDishes.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    public class RemindersViewModel : BaseViewModel
    {
        private Reminder _selectedReminder;

        public ObservableCollection<Reminder> Reminders { get; }
        public Command LoadRemindersCommand { get; }
        public Command AddReminderCommand { get; }
        public Command<Reminder> ReminderTapped { get; }

        INotificationManager notificationManager;

        public ICommand SendNotificationCommand { get; }
        public ICommand ScheduleNotificationCommand { get; }

        public RemindersViewModel()
        {
            Title = "Reminders";
            Reminders = new ObservableCollection<Reminder>();
            LoadRemindersCommand = new Command(async () => await ExecuteLoadRemindersCommand());

            ReminderTapped = new Command<Reminder>(OnRemindersSelected);

            AddReminderCommand = new Command(OnAddReminder);

            //SendNotificationCommand = new Command(() => OnSendClick());
            //ScheduleNotificationCommand = new Command(() => OnScheduleClick());

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
        }

        //void OnSendClick()
        //{
        //    notificationNumber++;
        //    string title = $"Local Notification #{notificationNumber}";
        //    string message = $"You have now received {notificationNumber} notifications!";
        //    notificationManager.SendNotification(title, message);
        //}

        //void OnScheduleClick()
        //{
        //    notificationNumber++;
        //    string title = $"Local Notification #{notificationNumber}";
        //    string message = $"You have now received {notificationNumber} notifications!";
        //    notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
        //}

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //notificationManager.SendNotification(title, message);
                // When user clicks notification, what do?
            });
        }

        async Task ExecuteLoadRemindersCommand()
        {
            IsBusy = true;

            try
            {
                Reminders.Clear();
                var reminders = (await DataStore.GetItemsAsync()).OrderBy(r => r.DueDate.Add(r.DueTime));
                foreach (var item in reminders)
                {
                    Reminders.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedReminder = null;
        }

        public Reminder SelectedReminder
        {
            get => _selectedReminder;
            set
            {
                SetProperty(ref _selectedReminder, value);
                OnRemindersSelected(value);
            }
        }

        private async void OnAddReminder(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewReminderPage));
        }

        async void OnRemindersSelected(Reminder reminder)
        {
            if (reminder == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(ReminderDetailViewModel.ItemId)}={reminder.Id}");
        }
    }
}