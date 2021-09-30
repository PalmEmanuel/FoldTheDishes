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
                var notDoneItems = dataStore.GetNotDoneItems().Result;

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
                string repeatingType = intent.GetStringExtra(AndroidNotificationManager.RepeatingIntervalKey);

                manager.Show(id, text);

                if (repeatingType != null)
                {
                    // If the repeating type is monthly we need to calculate the difference between months in days manually
                    if (repeatingType == "monthly")
                    {
                        // Get the time that the current notification was scheduled to
                        DateTime currentReminderOccurrence = new DateTime(intent.GetLongExtra(AndroidNotificationManager.NextOccurrenceKey, AlarmManager.IntervalDay * 30));
                        // Set the next notification to be one month in the future (adjusting properly for days in month)
                        manager.SendNotification(id, text, currentReminderOccurrence.AddMonths(1), ReminderInterval.Monthly);
                    }
                }
            }
        }
    }
}