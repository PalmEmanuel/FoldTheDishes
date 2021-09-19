using Android.App;
using Android.Content;
using FoldTheDishes.Services;
using Xamarin.Forms;

namespace FoldTheDishes.Droid
{
    [BroadcastReceiver(Enabled = true, Label = "Fold The Dishes Broadcast Receiver", Permission = "android.permission.RECEIVE_BOOT_COMPLETED")]
    [IntentFilter(new string[] { Intent.ActionBootCompleted }, Priority = 1337)]
    public class AlarmHandler : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            AndroidNotificationManager manager = AndroidNotificationManager.Instance ?? new AndroidNotificationManager();

            // If user restarted the phone, recreate all notifications
            if (intent.Action == Intent.ActionBootCompleted)
            {
                ReminderStore dataStore = DependencyService.Get<ReminderStore>(DependencyFetchTarget.GlobalInstance);

                foreach (var item in dataStore.GetNotDoneItems().Result)
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