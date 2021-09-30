using FoldTheDishes.Models;
using FoldTheDishes.Services;
using FoldTheDishes.Views;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    public class NewReminderViewModel : BaseViewModel
    {
        private string title;
        private string notes;
        private DateTime dueDate;
        private TimeSpan dueTime;
        private bool isRepeating;

        public ObservableCollection<string> RepeatIntervals { get; }
        private string selectedRepeatInterval;

        INotificationManager notificationManager;

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

        public bool IsRepeating
        {
            get => isRepeating;
            set => SetProperty(ref isRepeating, value);
        }

        public string SelectedRepeatInterval
        {
            get => selectedRepeatInterval;
            set => SetProperty(ref selectedRepeatInterval, value);
        }

        public DateTime Today { get; }

        public Command SaveCommand { get; }

        public NewReminderViewModel()
        {
            base.Title = "New reminder";
            SaveCommand = new Command(OnSave, ValidateSave);
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

            RepeatIntervals = new ObservableCollection<string>();
            foreach (var item in Enum.GetNames(typeof(ReminderInterval)))
            {
               RepeatIntervals.Add(item);
            }
            SelectedRepeatInterval = RepeatIntervals[0];
        }

        void ShowNotification(int id)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(ReminderDetailViewModel.Id)}={id}", true);
            });
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(title);
        }

        private async void OnSave()
        {
            ReminderInterval? repeatInterval = null;
            if (IsRepeating)
            {
                repeatInterval = (ReminderInterval)Enum.Parse(typeof(ReminderInterval), SelectedRepeatInterval);
            }
            Reminder newReminder = new Reminder()
            {
                Id = 0,
                Title = TitleText,
                Notes = Notes,
                DueDate = DueDate,
                DueTime = DueTime,
                CreatedDate = DateTime.Now,
                Completed = false,
                CompletedDate = DateTime.MinValue,
                IsRepeating = IsRepeating,
                RepeatInterval = repeatInterval
            };

            await DataStore.AddItemAsync(newReminder);
            notificationManager.SendNotification(newReminder.Id, newReminder.Title, newReminder.DueDateTime, repeatInterval);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..", true);
        }
    }
}
