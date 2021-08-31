using System;

namespace FoldTheDishes.Models
{
    public class NotificationEventArgs : EventArgs
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
