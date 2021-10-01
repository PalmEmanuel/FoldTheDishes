using FoldTheDishes.Models;
using System;

namespace FoldTheDishes.Services
{
    public static class NotificationScheduleHelper
    {
        public static long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }

        public static long GetIntervalTime(ReminderInterval interval)
        {
            var today = DateTime.Now.Date;

            long intervalMilliseconds;
            switch (interval)
            {
                //case ReminderInterval.Minutely:
                //    intervalMilliseconds = 60 * 1000;
                //    break;
                case ReminderInterval.Daily:
                    intervalMilliseconds = (long)TimeSpan.FromDays(1).TotalMilliseconds;
                    break;
                case ReminderInterval.Weekly:
                    intervalMilliseconds = (long)TimeSpan.FromDays(7).TotalMilliseconds;
                    break;
                case ReminderInterval.Monthly:
                    intervalMilliseconds = (long)(today.AddMonths(1) - today).TotalMilliseconds;
                    break;
                case ReminderInterval.Yearly:
                    intervalMilliseconds = DateTime.IsLeapYear(DateTime.Now.Year) ? (long)TimeSpan.FromDays(366).TotalMilliseconds : (long)TimeSpan.FromDays(365).TotalMilliseconds;
                    break;
                default:
                    throw new Exception("Unexpected interval!");
            }
            return intervalMilliseconds;
        }
    }
}
