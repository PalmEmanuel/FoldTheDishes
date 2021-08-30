using System;

namespace FoldTheDishes.Services
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;
        void Initialize();
        void SendNotification(int id, string title, string message, DateTime? notifyTime = null);
        void ReceiveNotification(int id, string title, string message);
    }
}
