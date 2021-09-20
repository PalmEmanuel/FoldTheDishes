using Android.App;
using Android.Content;
using FoldTheDishes.Services;

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
                    manager.SendNotification(item.Id, item.Title, item.DueDate.Add(item.DueTime));
                }
            }
            else if (intent?.Extras != null)
            {
                int id = intent.GetIntExtra(AndroidNotificationManager.IdKey, -1);
                string text = intent.GetStringExtra(AndroidNotificationManager.TextKey);

                manager.Show(id, text);
            }
        }
    }
}