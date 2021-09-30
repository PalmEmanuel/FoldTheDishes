using FoldTheDishes.Models;
using System;

namespace FoldTheDishes.Services
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;
        void Initialize();
        void SendNotification(int id, string text, DateTime? notifyTime = null, ReminderInterval? repeatInterval = null);
        void ReceiveNotification(int id, string text);
        void DeleteNotification(int id);
    }
}
