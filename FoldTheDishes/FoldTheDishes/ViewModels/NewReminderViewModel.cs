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
        private DateTime dueDate;
        private TimeSpan dueTime;

        private DateTime today;

        INotificationManager notificationManager;

        public NewReminderViewModel()
        {
            Title = "New reminder";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            today = DateTime.Now;
            dueDate = today.Date;
            dueTime = today.TimeOfDay.Add(TimeSpan.FromMinutes(5));

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                System.Diagnostics.Debug.WriteLine($"Received notification click! {evtData.Text}");
            };
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(text);
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
        public DateTime DueDate
        {
            get => dueDate;
            set => SetProperty(ref dueDate, value);
        }
        public TimeSpan DueTime
        {
            get => dueTime;
            set => SetProperty(ref dueTime, value);
        }
        public DateTime Today
        {
            get => today;
            set => SetProperty(ref today, value);
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
                Id = 0,
                Text = Text,
                DueDate = DueDate,
                DueTime = DueTime,
                Created = DateTime.Now,
                Completed = DateTime.MinValue
            };

            await DataStore.AddItemAsync(newReminder);
            notificationManager.SendNotification(newReminder.Id, newReminder.Text, newReminder.DueDate.Add(newReminder.DueTime));

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
