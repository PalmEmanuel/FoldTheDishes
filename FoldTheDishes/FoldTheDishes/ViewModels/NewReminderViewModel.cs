using FoldTheDishes.Models;
using FoldTheDishes.Services;
using FoldTheDishes.Views;
using System;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    public class NewReminderViewModel : BaseViewModel
    {
        private string title;
        private string notes;
        private DateTime dueDate;
        private TimeSpan dueTime;

        INotificationManager notificationManager;

        public NewReminderViewModel()
        {
            base.Title = "New reminder";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            Today = DateTime.Now.TrimToMinutes();
            dueDate = Today.Date;
            dueTime = Today.TimeOfDay.Add(TimeSpan.FromMinutes(5));

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Id);
            };
        }

        void ShowNotification(int id)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(ReminderDetailViewModel.Id)}={id}");
            });
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(title);
        }

        public string TitleText
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
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
        public DateTime Today { get; }

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
                Title = TitleText,
                Notes = Notes,
                DueDate = DueDate,
                DueTime = DueTime,
                CreatedDate = DateTime.Now,
                Completed = false,
                CompletedDate = DateTime.MinValue
            };

            await DataStore.AddItemAsync(newReminder);
            notificationManager.SendNotification(newReminder.Id, newReminder.Title, newReminder.DueDate.Add(newReminder.DueTime));

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
