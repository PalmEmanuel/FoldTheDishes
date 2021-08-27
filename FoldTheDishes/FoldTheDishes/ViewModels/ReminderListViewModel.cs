using FoldTheDishes.Models;
using FoldTheDishes.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace FoldTheDishes.ViewModels
{
    public class ReminderListViewModel : BaseViewModel
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;

        public ICommand SendNotificationCommand { get; }
        public ICommand ScheduleNotificationCommand { get; }

        public ReminderListViewModel()
        {
            SendNotificationCommand = new Command(() => OnSendClick());
            ScheduleNotificationCommand = new Command(() => OnScheduleClick());

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message + evtData.Id);
            };
        }

        void OnSendClick()
        {
            notificationNumber++;
            string title = $"Local Notification #{notificationNumber}";
            string message = $"You have now received {notificationNumber} notifications!";
            notificationManager.SendNotification(title, message);
        }

        void OnScheduleClick()
        {
            notificationNumber++;
            string title = $"Local Notification #{notificationNumber}";
            string message = $"You have now received {notificationNumber} notifications!";
            notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                notificationManager.SendNotification(title + "hello", message + "world");
            });
        }
    }
}