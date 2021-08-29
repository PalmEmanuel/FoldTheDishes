using System;

namespace FoldTheDishes.Models
{
    public class Reminder
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}