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
        private Reminder selectedReminder;

        public ObservableCollection<Reminder> UncompletedReminders { get; }
        public ObservableCollection<Reminder> CompletedReminders { get; }
        public Command LoadRemindersCommand { get; }
        public Command AddReminderCommand { get; }
        public Command<Reminder> CheckedChangedCommand { get; }
        public Command<Reminder> ReminderTapped { get; }

        INotificationManager notificationManager;

        public ICommand SendNotificationCommand { get; }
        public ICommand ScheduleNotificationCommand { get; }

        public RemindersViewModel()
        {
            Title = "Reminders";
            UncompletedReminders = new ObservableCollection<Reminder>();
            CompletedReminders = new ObservableCollection<Reminder>();
            LoadRemindersCommand = new Command(async () => await ExecuteLoadRemindersCommand());

            ReminderTapped = new Command<Reminder>(OnRemindersSelected);

            AddReminderCommand = new Command(OnAddReminder);
            CheckedChangedCommand = new Command<Reminder>(async (r) => await CheckedChanged(r));

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

        async Task ExecuteLoadRemindersCommand()
        {
            IsBusy = true;

            try
            {
                UncompletedReminders.Clear();
                CompletedReminders.Clear();
                var reminders = (await DataStore.GetItemsAsync()).OrderBy(r => r.DueDate.Add(r.DueTime));
                foreach (var reminder in reminders)
                {
                    switch (reminder.Completed)
                    {
                        case false:
                            if (!UncompletedReminders.Any(r => r.Id == reminder.Id)) { UncompletedReminders.Add(reminder); }
                            break;
                        case true:
                            if (!CompletedReminders.Any(r => r.Id == reminder.Id)) { CompletedReminders.Add(reminder); }
                            break;
                    }
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
            get => selectedReminder;
            set
            {
                SetProperty(ref selectedReminder, value);
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
            await Shell.Current.GoToAsync($"{nameof(ReminderDetailPage)}?{nameof(ReminderDetailViewModel.Id)}={reminder.Id}");
        }

        private async Task CheckedChanged(Reminder reminder)
        {
            // Value has already changed because of the two-way binding
            if (reminder.Completed)
            {
                reminder.CompletedDate = DateTime.Now;
            }
            else
            {
                reminder.CompletedDate = DateTime.MinValue;
            }

            await DataStore.UpdateItemAsync(reminder);

            // This needs some work, currently the list updates from db when tab is changed
            if (reminder.Completed)
            {
                //// If the list does not contain the reminder
                //if (!CompletedReminders.Any(r => r.Id == reminder.Id))
                //{
                //    try
                //    {
                //        // Find the closest date in list, insert at that index
                //        // This ensures a sorted list without sorting it, which is more complex due to bindings and two pages in tabbedpagev
                //        var orderedList = CompletedReminders.OrderBy(r => reminder.DueDate.Add(reminder.DueTime) - r.DueDate.Add(r.DueTime));
                //        int index = CompletedReminders.IndexOf(orderedList.First(r => r.DueDate.Add(r.DueTime) < reminder.DueDate.Add(reminder.DueTime)));
                //        CompletedReminders.Insert(index, reminder);
                //    }
                //    catch (Exception)
                //    {
                //        CompletedReminders.Add(reminder);
                //    }
                //}
                notificationManager.DeleteNotification(reminder.Id);
                UncompletedReminders.Remove(reminder);
            }
            else
            {
                //if (!UncompletedReminders.Any(r => r.Id == reminder.Id))
                //{
                //    try
                //    {
                //        // Find the closest date in list, insert at that index
                //        // This ensures a sorted list without sorting it, which is more complex due to bindings and two pages in tabbedpagev
                //        var orderedList = UncompletedReminders.OrderBy(r => reminder.DueDate.Add(reminder.DueTime) - r.DueDate.Add(r.DueTime));
                //        int index = UncompletedReminders.IndexOf(orderedList.First(r => r.DueDate.Add(r.DueTime) < reminder.DueDate.Add(reminder.DueTime)));
                //        UncompletedReminders.Insert(index, reminder);
                //    }
                //    catch (Exception)
                //    {
                //        UncompletedReminders.Add(reminder);
                //    }
                //}
                notificationManager.SendNotification(reminder.Id, reminder.Title, reminder.DueDate.Add(reminder.DueTime));
                CompletedReminders.Remove(reminder);
            }

            // Avoid an infinite loop of the two pages in the tabbedpage invoking each other's updates
            // ie sort update when it's in view
            // CurrentPage is set when navigating between pages
            //if ((CurrentPage == "Completed" && reminder.Completed) ||
            //    (CurrentPage == "Uncompleted" && !reminder.Completed))
            //{
            //    CompletedReminders.Sort((a, b) => a.DueDate.Add(a.DueTime).CompareTo(b.DueDate.Add(b.DueTime)));
            //    UncompletedReminders.Sort((a, b) => a.DueDate.Add(a.DueTime).CompareTo(b.DueDate.Add(b.DueTime)));
            //}
        }
    }
}