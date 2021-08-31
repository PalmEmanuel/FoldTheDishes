﻿using Android.App;
using Android.Content;
using FoldTheDishes.Services;

namespace FoldTheDishes.Droid
{
    [BroadcastReceiver(Enabled = true, Label = "Fold The Dishes Broadcast Receiver")]
    [IntentFilter(new string[] { "android.intent.action.BOOT_COMPLETED" })]
    public class AlarmHandler : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Extras != null)
            {
                int id = intent.GetIntExtra(AndroidNotificationManager.IdKey, -1);
                string text = intent.GetStringExtra(AndroidNotificationManager.TextKey);

                AndroidNotificationManager manager = AndroidNotificationManager.Instance ?? new AndroidNotificationManager();
                manager.Show(id, text);
            }
        }
    }
}