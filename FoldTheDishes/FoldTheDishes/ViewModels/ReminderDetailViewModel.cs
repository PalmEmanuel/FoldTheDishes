using FoldTheDishes.Models;
using FoldTheDishes.Services;
using FoldTheDishes.Views;
using System;
using System.Collections.ObjectModel;
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
        private bool isRepeating;

        private Reminder reminder;

        private string originalTitle;
        private string originalNotes;
        private DateTime originalDueDate;
        private TimeSpan originalDueTime;
        private bool originalCompleted;
        private bool originalIsRepeating;
        private ReminderInterval? originalRepeatInterval;

        public ObservableCollection<string> RepeatIntervals { get; }
        private string selectedRepeatInterval;

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
            bool wasChanged = title != originalTitle || // Title changed
                notes != originalNotes || // Notes changed
                (dueDate.Year > 1900 && dueDate != originalDueDate) || // DueDate changed
                dueTime != originalDueTime || // DueTime changed
                completed != originalCompleted || // Completed changed
                isRepeating != originalIsRepeating || // Repeating changed
                (isRepeating && originalRepeatInterval != (ReminderInterval)Enum.Parse(typeof(ReminderInterval), selectedRepeatInterval)); // Repeating true and interval changed

            CanSave = !string.IsNullOrWhiteSpace(title) && (wasChanged && dueDate.Add(dueTime) > DateTime.Now ||
                (completed && !originalCompleted));
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
            }
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
                await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(Id)}={id}");
            });
        }

        private bool ValidateSave()
        {
            return CanSave;
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
            DateTime now = DateTime.Now;
            reminder.Title = TitleText;
            reminder.Notes = Notes;
            reminder.DueDate = DueDate;
            reminder.DueTime = DueTime;
            reminder.Completed = Completed;
            reminder.CompletedDate = Completed ? now : DateTime.MinValue;
            reminder.IsRepeating = IsRepeating;
            if (IsRepeating)
            {
                reminder.RepeatInterval = (ReminderInterval)Enum.Parse(typeof(ReminderInterval), SelectedRepeatInterval);
            }
            
            bool proceed = true;
            bool inFuture = DueDate.Add(DueTime) > now;

            if (Completed && inFuture)
            {
                //proceed = await Shell.Current.DisplayAlert("You will not be reminded!", "The reminder is marked as completed, and you will not be reminded when the date is reached.",
                //    Constants.CONFIRM_BUTTON_TEXT,
                //    Constants.CANCEL_BUTTON_TEXT);
            }
            else if (inFuture)
            {
                // Replaces the old scheduled notification
                notificationManager.SendNotification(Id, TitleText, DueDate.Add(DueTime), reminder.RepeatInterval);
            }

            if (proceed)
            {
                if (Completed)
                {
                    notificationManager.DeleteNotification(Id);
                }
                await DataStore.UpdateItemAsync(reminder);
                // This will pop the current page off the navigation stack
                await Shell.Current.GoToAsync("..", true);
            }
        }

        public async void LoadItemId(int itemId)
        {
            try
            {
                reminder = await DataStore.GetItemAsync(itemId);
                Id = reminder.Id;
                TitleText = originalTitle = reminder.Title;
                Notes = originalNotes = reminder.Notes;
                DueDate = originalDueDate = reminder.DueDate;
                DueTime = originalDueTime = reminder.DueTime;
                Completed = originalCompleted = reminder.Completed;
                IsRepeating = originalIsRepeating = reminder.IsRepeating;
                originalRepeatInterval = reminder.RepeatInterval;

                if (isRepeating)
                {
                    SelectedRepeatInterval = originalRepeatInterval.ToString();
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load reminder");
            }

            CanSave = false;
        }
    }
}
