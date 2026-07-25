using System;
using System.Collections;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Unity.Notifications.Android;
#endif

public static class NotificationPermissionService
{
    private const string PostNotificationsPermission = "android.permission.POST_NOTIFICATIONS";

    public enum NotificationPermissionState
    {
        Allowed,
        Denied,
        NotDetermined
    }

    public static bool IsAndroid13OrHigher()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using var version = new AndroidJavaClass("android.os.Build$VERSION");
        int sdkInt = version.GetStatic<int>("SDK_INT");
        return sdkInt >= 33;
#else
        return false;
#endif
    }

    public static bool HasPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IsAndroid13OrHigher())
        {
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(PostNotificationsPermission);
        }

        return AreNotificationsEnabledInSystem();
#else
        return true;
#endif
    }

    public static NotificationPermissionState GetState()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (HasPermission())
            return NotificationPermissionState.Allowed;

        if (IsAndroid13OrHigher())
            return NotificationPermissionState.Denied;

        return AreNotificationsEnabledInSystem()
            ? NotificationPermissionState.Allowed
            : NotificationPermissionState.Denied;
#else
        return NotificationPermissionState.Allowed;
#endif
    }

    public static IEnumerator RequestPermission(Action<bool> onCompleted = null)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!IsAndroid13OrHigher())
        {
            onCompleted?.Invoke(AreNotificationsEnabledInSystem());
            yield break;
        }

        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(PostNotificationsPermission))
        {
            onCompleted?.Invoke(true);
            yield break;
        }

        var request = new PermissionRequest();

        while (request.Status == PermissionStatus.RequestPending)
            yield return null;

        bool granted = request.Status == PermissionStatus.Allowed;
        onCompleted?.Invoke(granted);
#else
        onCompleted?.Invoke(true);
        yield break;
#endif
    }

    public static bool AreNotificationsEnabledInSystem()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
            using var managerClass = new AndroidJavaClass("androidx.core.app.NotificationManagerCompat");
            using var manager = managerClass.CallStatic<AndroidJavaObject>("from", context);

            return manager.Call<bool>("areNotificationsEnabled");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to check notification settings: " + e.Message);
            return false;
        }
#else
        return true;
#endif
    }

    public static void OpenNotificationSettings()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var intent = new AndroidJavaObject("android.content.Intent");
            using var buildVersion = new AndroidJavaClass("android.os.Build$VERSION");

            int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
            string packageName = activity.Call<string>("getPackageName");

            if (sdkInt >= 26)
            {
                intent.Call<AndroidJavaObject>("setAction", "android.settings.APP_NOTIFICATION_SETTINGS");
                intent.Call<AndroidJavaObject>("putExtra", "android.provider.extra.APP_PACKAGE", packageName);
            }
            else
            {
                intent.Call<AndroidJavaObject>("setAction", "android.settings.APPLICATION_DETAILS_SETTINGS");
                using var uriClass = new AndroidJavaClass("android.net.Uri");
                using var uri = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null);
                intent.Call<AndroidJavaObject>("setData", uri);
            }

            activity.Call("startActivity", intent);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to open notification settings: " + e.Message);
        }
#endif
    }
}