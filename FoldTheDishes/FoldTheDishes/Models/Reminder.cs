using SQLite;
using System;

namespace FoldTheDishes.Models
{
    [Table("Reminder")]
    public class Reminder
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
        public DateTime DueDateTime { get => DueDate.Add(DueTime); }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Completed { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}