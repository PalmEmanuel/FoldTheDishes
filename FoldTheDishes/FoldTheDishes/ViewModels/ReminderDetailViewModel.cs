using FoldTheDishes.Models;
using FoldTheDishes.Services;
using FoldTheDishes.Views;
using System;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class ReminderDetailViewModel : BaseViewModel
    {
        private string title;
        private string notes;
        private int id;
        private DateTime dueDate;
        private TimeSpan dueTime;
        private bool completed;

        private string originalTitle;
        private string originalNotes;
        private DateTime originalDueDate;
        private TimeSpan originalDueTime;
        private bool originalCompleted;

        private bool canSave;
        public bool CanSave
        {
            get
            {
                WasChanged();
                return canSave;
            }
            set => SetProperty(ref canSave, value);
        }

        private void WasChanged()
        {
            CanSave = title != originalTitle ||
                notes != originalNotes ||
                dueDate != originalDueDate ||
                dueTime != originalDueTime ||
                completed != originalCompleted ||
                (completed && !originalCompleted);
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

        public bool Completed
        {
            get => completed;
            set
            {
                SetProperty(ref completed, value);
                WasChanged();
            }
        }

        INotificationManager notificationManager;

        public Command SaveCommand { get; }
        public Command DeleteCommand { get; }
        public Command CompleteCommand { get; }

        public ReminderDetailViewModel()
        {
            DeleteCommand = new Command(OnDelete);
            SaveCommand = new Command(OnSave, ValidateSave);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            CompleteCommand = new Command(OnComplete);

            Title = "Edit reminder";
            
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Id);
            };

            CustomBackButtonAction = async () =>
            {
                if (CanSave)
                {
                    var action = await Shell.Current.DisplayAlert("Lose changes?", "Going back without saving will discard your changes to the reminder.",
                        Constants.CONFIRM_BUTTON_TEXT,
                        Constants.CANCEL_BUTTON_TEXT);
                    if (action)
                    {
                        // Replace navigation stack with the "root" list page RemindersPage
                        await Shell.Current.GoToAsync("//RemindersPage", true);
                    }
                }
                else
                {
                    await Shell.Current.GoToAsync("//RemindersPage", true);
                }
            };
        }

        void ShowNotification(int id)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(Id)}={id}");
            });
        }

        private bool ValidateSave()
        {
            var now = DateTime.Now;
            return !string.IsNullOrWhiteSpace(title) && dueDate.Add(dueTime) >= now && CanSave;
        }

        private void OnComplete()
        {
            Completed = !Completed;
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
                await Shell.Current.GoToAsync("..", true);
            }
        }

        private async void OnSave()
        {
            Reminder newReminder = new Reminder()
            {
                Id = Id,
                Title = TitleText,
                Notes = Notes,
                DueDate = DueDate,
                DueTime = DueTime,
                Completed = Completed
            };

            bool proceed = true;
            bool inFuture = DueDate.Add(DueTime) > DateTime.Now;

            if (Completed && inFuture)
            {
                //proceed = await Shell.Current.DisplayAlert("You will not be reminded!", "The reminder is marked as completed, and you will not be reminded when the date is reached.",
                //    Constants.CONFIRM_BUTTON_TEXT,
                //    Constants.CANCEL_BUTTON_TEXT);
            }
            else if (inFuture)
            {
                // Replaces the old scheduled notification
                notificationManager.SendNotification(Id, TitleText, DueDate.Add(DueTime));
            }

            if (proceed)
            {
                if (Completed)
                {
                    notificationManager.DeleteNotification(Id);
                }
                await DataStore.UpdateItemAsync(newReminder);
                // This will pop the current page off the navigation stack
                await Shell.Current.GoToAsync("..", true);
            }
        }

        public async void LoadItemId(int itemId)
        {
            try
            {
                var reminder = await DataStore.GetItemAsync(itemId);
                Id = reminder.Id;
                TitleText = originalTitle = reminder.Title;
                Notes = originalNotes = reminder.Notes;
                DueDate = originalDueDate = reminder.DueDate;
                DueTime = originalDueTime = reminder.DueTime;
                Completed = originalCompleted = reminder.Completed;
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load reminder");
            }
        }
    }
}
