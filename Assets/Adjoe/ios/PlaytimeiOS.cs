using System;
using System.Runtime.InteropServices;

namespace io.adjoe.sdk
{
    #if UNITY_IOS
    /// <summary>
    /// Native iOS interface for Playtime SDK.
    /// This class provides DllImport declarations for the native iOS functions.
    /// </summary>
    public static class PlaytimeiOS
    {
        // Delegate types for callbacks
        public delegate void OnSuccessCallback();
        public delegate void OnSuccessWithStringCallback(IntPtr responseJson);
        public delegate void OnErrorCallback(IntPtr errorMessage);

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_getCampaigns(
            IntPtr optionsJSON,
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_getCampaignsWithTokens(
            IntPtr tokensJSON,
            IntPtr optionsJSON,
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_getInstalledCampaigns(
            IntPtr optionsJSON,
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_openInStore(
            IntPtr campaignJSON,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_openChatbot(
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_openChatbotWithCampaign(
            IntPtr openInStoreCampaignPtr,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_openInstalledCampaign(
            IntPtr campaignJSON,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_getPermissions(
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_resetRewardsConnect(
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_registerRewardsConnect(
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_showPermissionsPrompt(
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void Playtime_showCatalogWithOptions(
            IntPtr optionsJSON,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void Playtime_setPlaytimeOptions(
            IntPtr optionsJSON,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void Playtime_getStatus(
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_showInstalledApps(
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_showAppDetails(
            IntPtr campaignJSON,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_showAppDetailsWithToken(
            IntPtr token,
            IntPtr appId,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void Playtime_getUserId(
            OnSuccessWithStringCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_executeEngagement(
            IntPtr campaignJSON,
            IntPtr engagementType,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void Playtime_teardown(
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );

        [DllImport("__Internal")]
        public static extern void PlaytimeStudio_executeEngagementWithToken(
            IntPtr appID,
            IntPtr token,
            OnSuccessCallback onSuccess,
            OnErrorCallback onError
        );
    }
    #endif
} 