using SQLite;
using System;

namespace FoldTheDishes.Models
{
    public class Reminder
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }
    }
}