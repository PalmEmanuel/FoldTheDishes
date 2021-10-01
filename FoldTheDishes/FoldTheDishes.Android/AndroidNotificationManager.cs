using System;
using Android.App;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using FoldTheDishes.Droid;
using FoldTheDishes.Models;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(FoldTheDishes.Services.AndroidNotificationManager))]
namespace FoldTheDishes.Services
{
    public class AndroidNotificationManager : INotificationManager
    {
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        public const string IdKey = "id";
        public const string TextKey = "title";
        public const string RepeatingIntervalKey = "interval";
        public const string NextOccurrenceKey = "next";

        bool channelInitialized = false;

        NotificationManager manager;

        public event EventHandler NotificationReceived;

        public static AndroidNotificationManager Instance { get; private set; }

        public AndroidNotificationManager() => Initialize();

        public void Initialize()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                Instance = this;
            }
        }

        public void SendNotification(int id, string text, DateTime? notifyTime = null, ReminderInterval? repeatInterval = null)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            if (notifyTime != null)
            {
                AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;

                Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
                intent.PutExtra(IdKey, id);
                intent.PutExtra(TextKey, text);

                long triggerTime = NotificationScheduleHelper.GetNotifyTime(notifyTime.Value);

                if (repeatInterval != null)
                {
                    intent.PutExtra(RepeatingIntervalKey, repeatInterval.ToString());
                    intent.PutExtra(NextOccurrenceKey, notifyTime.Value.Ticks);
                }

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, id, intent, PendingIntentFlags.UpdateCurrent);

                if (repeatInterval != null)
                {
                    long intervalTime = NotificationScheduleHelper.GetIntervalTime((ReminderInterval)repeatInterval);
                    alarmManager.SetRepeating(AlarmType.RtcWakeup, triggerTime, intervalTime, pendingIntent);
                }
                else
                {
                    alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                }
            }
            else
            {
                Show(id, text);
            }
        }

        public void ReceiveNotification(int id, string text)
        {
            var args = new NotificationEventArgs()
            {
                Id = id,
                Text = text
            };
            NotificationReceived?.Invoke(null, args);
        }

        public void Show(int id, string text)
        {
            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.PutExtra(IdKey, id);
            intent.PutExtra(TextKey, text);

            PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, id, intent, PendingIntentFlags.UpdateCurrent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentText(text)
                .SetSmallIcon(Resource.Mipmap.ic_notification)
                .SetColor(ContextCompat.GetColor(AndroidApp.Context, Resource.Color.colorAccentVeryLight))
                .SetAutoCancel(true)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            Notification notification = builder.Build();
            manager.Notify(id, notification);
        }

        public void DeleteNotification(int id)
        {
            Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, id, intent, PendingIntentFlags.CancelCurrent);
            AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            // removes scheduled notification
            alarmManager.Cancel(pendingIntent);
            // also cancels any active notification in notification bar
            pendingIntent.Cancel();
        }

        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(Context.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }
    }
}