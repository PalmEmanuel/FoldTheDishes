using FoldTheDishes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoldTheDishes.Services
{
    public class MockDataStore : IDataStore<Reminder>
    {
        readonly List<Reminder> reminders;

        public MockDataStore()
        {
            var now = DateTime.Now;
            reminders = new List<Reminder>()
            {
                new Reminder { Id = 1, DueDate = now, DueTime = now.TimeOfDay, Text = "First reminder", Description = "This is an item description." },
                new Reminder { Id = 2, DueDate = now, DueTime = now.TimeOfDay.Add(TimeSpan.FromMinutes(1)), Text = "Second reminder", Description = "This is an item description." },
                new Reminder { Id = 3, DueDate = now, DueTime = now.TimeOfDay.Add(TimeSpan.FromMinutes(2)), Text = "Third reminder", Description = "This is an item description." },
                new Reminder { Id = 4, DueDate = now, DueTime = now.TimeOfDay.Add(TimeSpan.FromMinutes(3)), Text = "Fourth reminder", Description = "This is an item description." },
                new Reminder { Id = 5, DueDate = now, DueTime = now.TimeOfDay.Add(TimeSpan.FromMinutes(4)), Text = "Fifth reminder", Description = "This is an item description." },
                new Reminder { Id = 6, DueDate = now, DueTime = now.TimeOfDay.Add(TimeSpan.FromMinutes(5)), Text = "Sixth reminder", Description = "This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(Reminder reminder)
        {
            reminders.Add(reminder);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Reminder reminder)
        {
            var oldReminder = reminders.Where((Reminder arg) => arg.Id == reminder.Id).FirstOrDefault();
            reminders.Remove(oldReminder);
            reminders.Add(reminder);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldReminder = reminders.Where((Reminder arg) => arg.Id == id).FirstOrDefault();
            reminders.Remove(oldReminder);

            return await Task.FromResult(true);
        }

        public async Task<Reminder> GetItemAsync(int id)
        {
            return await Task.FromResult(reminders.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Reminder>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(reminders);
        }
    }
}