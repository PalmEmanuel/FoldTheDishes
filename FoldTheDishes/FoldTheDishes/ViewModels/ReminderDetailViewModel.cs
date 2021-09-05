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
        private int id;
        private DateTime dueDate;
        private TimeSpan dueTime;

        private string originalText;
        private DateTime originalDueDate;
        private TimeSpan originalDueTime;

        private bool isChanged;
        public bool IsChanged
        {
            get
            {
                IsChanged = text != originalText || dueDate != originalDueDate || dueTime != originalDueTime;
                return isChanged;
            }
            set => SetProperty(ref isChanged, value);
        }

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
            set
            {
                SetProperty(ref text, value);
            }
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
        public Command DeleteCommand { get; }

        public ReminderDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            SaveCommand = new Command(OnSave, ValidateSave);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            Title = "Edit reminder";

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                System.Diagnostics.Debug.WriteLine($"Received notification click! {evtData.Text}");
            };

            CustomBackButtonAction = async () =>
            {
                if (IsChanged)
                {
                    var action = await Shell.Current.DisplayAlert("Lose changes?", "Going back without saving will discard your changes to the reminder.",
                        Constants.CONFIRM_BUTTON_TEXT,
                        Constants.CANCEL_BUTTON_TEXT);
                    if (action)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync("..");
                }
            };
        }

        private bool ValidateSave()
        {
            var now = DateTime.Now;
            return !string.IsNullOrWhiteSpace(text) && dueDate.Add(dueTime) >= now && IsChanged;
        }

        private async void OnDelete()
        {
            var action = await Shell.Current.DisplayAlert("Delete reminder?", "This will permanently remove the reminder from your list." ,
                Constants.CONFIRM_BUTTON_TEXT,
                Constants.CANCEL_BUTTON_TEXT);
            if (action)
            {
                notificationManager.DeleteNotification(id);
                await DataStore.DeleteItemAsync(new Reminder { Id = Id });

                // This will pop the current page off the navigation stack
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void OnSave()
        {
            Reminder newReminder = new Reminder()
            {
                Id = Id,
                Text = Text,
                DueDate = DueDate,
                DueTime = DueTime
            };

            await DataStore.UpdateItemAsync(newReminder);
            // Replaces the old scheduled notification
            notificationManager.SendNotification(Id, Text, DueDate.Add(DueTime));
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        public async void LoadItemId(int itemId)
        {
            try
            {
                var reminder = await DataStore.GetItemAsync(itemId);
                Id = reminder.Id;
                Text = originalText = reminder.Text;
                DueDate = originalDueDate = reminder.DueDate;
                DueTime = originalDueTime = reminder.DueTime;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
