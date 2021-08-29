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
            var now = DateTime.UtcNow;
            reminders = new List<Reminder>()
            {
                new Reminder { Id = Guid.NewGuid().ToString(), Date = now, Time = now.TimeOfDay, Text = "First item", Description="This is an item description." },
                new Reminder { Id = Guid.NewGuid().ToString(), Date = now, Time = now.TimeOfDay, Text = "Second item", Description="This is an item description." },
                new Reminder { Id = Guid.NewGuid().ToString(), Date = now, Time = now.TimeOfDay, Text = "Third item", Description="This is an item description." },
                new Reminder { Id = Guid.NewGuid().ToString(), Date = now, Time = now.TimeOfDay, Text = "Fourth item", Description="This is an item description." },
                new Reminder { Id = Guid.NewGuid().ToString(), Date = now, Time = now.TimeOfDay, Text = "Fifth item", Description="This is an item description." },
                new Reminder { Id = Guid.NewGuid().ToString(), Date = now, Time = now.TimeOfDay, Text = "Sixth item", Description="This is an item description." }
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

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldReminder = reminders.Where((Reminder arg) => arg.Id == id).FirstOrDefault();
            reminders.Remove(oldReminder);

            return await Task.FromResult(true);
        }

        public async Task<Reminder> GetItemAsync(string id)
        {
            return await Task.FromResult(reminders.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Reminder>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(reminders);
        }
    }
}