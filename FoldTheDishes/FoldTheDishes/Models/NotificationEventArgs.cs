using System;

namespace FoldTheDishes.Models
{
    public class NotificationEventArgs : EventArgs
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
