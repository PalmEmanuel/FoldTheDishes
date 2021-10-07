using Android.App;
using Android.Content;
using FoldTheDishes.Models;
using FoldTheDishes.Services;
using System;

namespace FoldTheDishes.Droid
{
    [BroadcastReceiver(Enabled = true, DirectBootAware = true, Label = "Fold The Dishes Broadcast Receiver")]
    [IntentFilter(new string[] { Intent.ActionLockedBootCompleted }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class AlarmHandler : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            AndroidNotificationManager manager = AndroidNotificationManager.Instance ?? new AndroidNotificationManager();

            // If user restarted the phone, recreate all notifications
            if (intent.Action == Intent.ActionLockedBootCompleted)
            {
                // Not using dependency service since the ReminderStore has not been registered when the boot event is received
                var dataStore = ReminderStore.Instance.GetAwaiter().GetResult();
                var notDoneItems = dataStore.GetNotExpiredNotDoneItems().GetAwaiter().GetResult();

                foreach (var item in notDoneItems)
                {
                    manager.SendNotification(item.Id, item.Title, item.DueDateTime, item.RepeatInterval);
                }
            }
            else if (intent?.Extras != null)
            {
                int id = intent.GetIntExtra(AndroidNotificationManager.IdKey, -1);
                string text = intent.GetStringExtra(AndroidNotificationManager.TextKey);

                // See if the reminder is repeating
                int repeatingType = intent.GetIntExtra(AndroidNotificationManager.RepeatingIntervalKey, -1);

                manager.Show(id, text);

                // If repeating, schedule the next notification
                if (repeatingType != -1)
                {
                    // Get the time that the current notification was scheduled to, in ticks
                    DateTime currentReminderOccurrence = new DateTime(intent.GetLongExtra(AndroidNotificationManager.NextOccurrenceKey, -1));

                    // Add to that date based on interval
                    switch ((ReminderInterval)repeatingType)
                    {
                        //case ReminderInterval.Minutely:
                        //    manager.SendNotification(id, text, currentReminderOccurrence.AddMinutes(1), ReminderInterval.Minutely);
                        //    break;
                        case ReminderInterval.Daily:
                            manager.SendNotification(id, text, currentReminderOccurrence.AddDays(1), ReminderInterval.Daily);
                            break;
                        
                        case ReminderInterval.Weekly:
                            manager.SendNotification(id, text, currentReminderOccurrence.AddDays(7), ReminderInterval.Weekly);
                            break;
                        
                        case ReminderInterval.Monthly:
                            manager.SendNotification(id, text, currentReminderOccurrence.AddMonths(1), ReminderInterval.Monthly);
                            break;
                        
                        case ReminderInterval.Yearly:
                            manager.SendNotification(id, text, currentReminderOccurrence.AddYears(1), ReminderInterval.Yearly);
                            break;

                        default:
                            throw new Exception("Unexpected repeating schedule!");
                    }
                }
            }
        }
    }
}