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

                long triggerTime = GetNotifyTime(notifyTime.Value);

                if (repeatInterval != null)
                {
                    intent.PutExtra(RepeatingIntervalKey, repeatInterval.ToString());
                    intent.PutExtra(NextOccurrenceKey, notifyTime.Value.Ticks);
                }

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, id, intent, PendingIntentFlags.UpdateCurrent);

                if (repeatInterval != null)
                {
                    long intervalTime = GetIntervalTime((ReminderInterval)repeatInterval);
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

        public static long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }

        public static long GetIntervalTime(ReminderInterval interval)
        {
            var now = DateTime.Now;
            
            long intervalMilliseconds;
            switch (interval)
            {
                //case ReminderInterval.Minutely:
                //    intervalMilliseconds = 60 * 1000;
                //    break;
                case ReminderInterval.Daily:
                    intervalMilliseconds = AlarmManager.IntervalDay;
                    break;
                case ReminderInterval.Weekly:
                    intervalMilliseconds = AlarmManager.IntervalDay * 7;
                    break;
                case ReminderInterval.Monthly:
                    intervalMilliseconds = (long)(now.AddMonths(1) - now).TotalMilliseconds;
                    break;
                case ReminderInterval.Yearly:
                    intervalMilliseconds = DateTime.IsLeapYear(DateTime.Now.Year) ? AlarmManager.IntervalDay * 366 : AlarmManager.IntervalDay * 365;
                    break;
                default:
                    throw new Exception("Unexpected interval!");
            }
            return intervalMilliseconds;
        }
    }
}