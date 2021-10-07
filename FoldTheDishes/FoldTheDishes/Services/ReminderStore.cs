using FoldTheDishes.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoldTheDishes.Services
{
    public class ReminderStore
    {
        public const string DatabaseFilename = "Reminders.db3";

        public const SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLiteOpenFlags.SharedCache;

        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<ReminderStore> Instance = new AsyncLazy<ReminderStore>(async () =>
        {
            var instance = new ReminderStore();
            CreateTableResult result = await Database.CreateTableAsync<Reminder>(); 
            return instance;
        });

        public ReminderStore()
        {
            Database = new SQLiteAsyncConnection(DatabasePath, Flags);
        }

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

        public Task<List<Reminder>> GetItemsAsync()
        {
            return Database.Table<Reminder>().ToListAsync();
        }

        public Task<List<Reminder>> GetDoneItemsThatAreOlderThanDateAsync(DateTime date)
        {
            return Database.Table<Reminder>().Where(r => r.Completed && r.DueDate < date).ToListAsync();
        }

        public Task<List<Reminder>> GetNotExpiredNotDoneItems()
        {
            var now = DateTime.Now.TrimToMinutes();
            // Either duedate is tomorrow or onwards, or it's today and the time is later
            return Database.Table<Reminder>().Where(r => !r.Completed && (r.DueDate > now.Date || (r.DueDate == now.Date && r.DueTime > now.TimeOfDay))).ToListAsync();
        }

        public Task<List<Reminder>> GetNotDoneItems()
        {
            return Database.Table<Reminder>().Where(r => !r.Completed).ToListAsync();
        }

        public Task<Reminder> GetItemAsync(int id)
        {
            return Database.Table<Reminder>().Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> AddItemAsync(Reminder reminder)
        {
            return UpdateItemAsync(reminder);
        }

        public Task<int> UpdateItemAsync(Reminder reminder)
        {
            if (reminder.Id != 0)
            {
                return Database.UpdateAsync(reminder);
            }
            else
            {
                return Database.InsertAsync(reminder);
            }
        }

        public Task AdjustRepeatingDueDateTimes()
        {
            return Task.Run(() =>
            {
                var reminders = GetItemsAsync().GetAwaiter().GetResult();

                var repeatingReminders = reminders.Where(r => !r.Completed && r.IsRepeating);

                foreach (var reminder in repeatingReminders)
                {
                    var dueDateTime = reminder.DueDateTime;
                    while (DateTime.Now > dueDateTime)
                    {
                        dueDateTime = dueDateTime.AddMilliseconds(NotificationScheduleHelper.GetIntervalTime((ReminderInterval)reminder.RepeatInterval));
                    }
                    reminder.DueDate = dueDateTime.Date;
                    reminder.DueTime = dueDateTime.TimeOfDay;
                    UpdateItemAsync(reminder);
                }
            });
        }

        public Task<int> DeleteItemAsync(Reminder item)
        {
            return Database.DeleteAsync(item);
        }
    }
}