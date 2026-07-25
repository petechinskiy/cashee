using I2.Loc;
using System;
using Unity.Notifications.Android;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;
#endif

public static class NotificationSender
{
    public const string ChannelId = "default_test_channel";

    public static void RegisterChannel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var channel = new AndroidNotificationChannel
        {
            Id = ChannelId,
            Name = "Test notifications",
            Importance = Importance.Default,
            Description = "Channel for local test notifications"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

    public static void ScheduleNotifications()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        RegisterChannel();

        for (int day = 2; day <= 7; day++)
        {
            DateTime fireTime = DateTime.Now.Date
                .AddDays(day)
                .AddHours(12);

            var notification = new AndroidNotification
            {
                Title = $"Notifications/DailyStreak/Day{day}/Title",
                Text = $"Notifications/DailyStreak/Day{day}/Desc",
                FireTime = fireTime,
                ShouldAutoCancel = true
            };

            AndroidNotificationCenter.SendNotification(notification, ChannelId);
        }
#endif
    }

    public static void CancelAll()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif
    }
}