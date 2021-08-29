using FoldTheDishes.Models;
using FoldTheDishes.Services;
using System;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    public class NewReminderViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private DateTime date;
        private TimeSpan time;

        INotificationManager notificationManager;

        public NewReminderViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            date = DateTime.Now.Date;
            time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5));

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                System.Diagnostics.Debug.WriteLine($"Received notification click! {evtData.Title} - {evtData.Message}");
                //ShowNotification(evtData.Title, evtData.Message);
            };
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(text) &&
                !string.IsNullOrWhiteSpace(description);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }
        public TimeSpan Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Reminder newReminder = new Reminder()
            {
                Id = Guid.NewGuid().ToString(),
                Text = Text,
                Description = Description,
                Date = Date,
                Time = Time
            };

            await DataStore.AddItemAsync(newReminder);
            notificationManager.SendNotification(newReminder.Text, newReminder.Description, newReminder.Date.Add(newReminder.Time));

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
