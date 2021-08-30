using FoldTheDishes.Models;
using FoldTheDishes.Services;
using System;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class ReminderDetailViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private int id;
        private DateTime dueDate;
        private TimeSpan dueTime;

        public int Id
        {
            get => id;
            set
            {
                if (value != id)
                {
                    LoadItemId(value);
                }
                SetProperty(ref id, value);
            }
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

        public DateTime Created { get; set; }

        INotificationManager notificationManager;

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command DeleteCommand { get; }

        public ReminderDetailViewModel()
        {
            CancelCommand = new Command(OnCancel);
            DeleteCommand = new Command(OnDelete);
            SaveCommand = new Command(OnSave, ValidateSave);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                System.Diagnostics.Debug.WriteLine($"Received notification click! {evtData.Title} - {evtData.Message}");
            };
        }

        private bool ValidateSave()
        {
            var now = DateTime.Now;
            return !string.IsNullOrWhiteSpace(text) && DueDate.Add(DueTime) >= now;
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnDelete()
        {
            await DataStore.DeleteItemAsync(new Reminder { Id = Id });

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Reminder newReminder = new Reminder()
            {
                Id = Id,
                Text = Text,
                Description = Description,
                DueDate = DueDate,
                DueTime = DueTime
            };

            await DataStore.UpdateItemAsync(newReminder);
            notificationManager.SendNotification(Id, Text, Description, DueDate.Add(DueTime));
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        public async void LoadItemId(int itemId)
        {
            try
            {
                var reminder = await DataStore.GetItemAsync(itemId);
                Id = reminder.Id;
                Text = reminder.Text;
                Description = reminder.Description;
                DueDate = reminder.DueDate;
                DueTime = reminder.DueTime;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
