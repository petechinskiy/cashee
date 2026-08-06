using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

namespace io.adjoe.sdk.studio
{
/// <summary>
/// The entry point of Adjoe Playtime SDK in Studio version.
/// This class provides methods to fetch campaigns and installed apps to show them in your custom UI.
/// Note: The SDK is functional starting from Unity 2020.3 LTS.
/// </summary>
public static class PlaytimeStudio
{
        #if UNITY_IOS

        // Delegates and static fields for GetCampaigns
        private delegate void GetCampaignsOnSuccessDelegate(IntPtr responseJsonPtr);
        private delegate void GetCampaignsOnErrorDelegate(IntPtr errorPtr);

        private static Action<PlaytimeCampaignsResponse> getCampaignsSuccessCallback;
        private static Action<Exception> getCampaignsErrorCallback;
        private static IntPtr getCampaignsOptionsPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(GetCampaignsOnSuccessDelegate))]
        private static void OnGetCampaignsSuccess(IntPtr responseJsonPtr)
        {
            var responseJson = Marshal.PtrToStringAnsi(responseJsonPtr);
            var response = GenericJsonConverter.JsonToObject<PlaytimeCampaignsResponse>(responseJson);
            
            if (getCampaignsSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getCampaignsSuccessCallback(response));
            }
            
            // Free allocated memory after callback
            if (getCampaignsOptionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getCampaignsOptionsPtr);
                getCampaignsOptionsPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GetCampaignsOnErrorDelegate))]
        private static void OnGetCampaignsError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (getCampaignsErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getCampaignsErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (getCampaignsOptionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getCampaignsOptionsPtr);
                getCampaignsOptionsPtr = IntPtr.Zero;
            }
        }

        // Delegates and static fields for GetCampaigns with tokens
        private delegate void GetCampaignsWithTokensOnSuccessDelegate(IntPtr responseJsonPtr);
        private delegate void GetCampaignsWithTokensOnErrorDelegate(IntPtr errorPtr);

        private static Action<PlaytimeCampaignsResponse> getCampaignsWithTokensSuccessCallback;
        private static Action<Exception> getCampaignsWithTokensErrorCallback;
        private static IntPtr getCampaignsWithTokensTokensPtr = IntPtr.Zero;
        private static IntPtr getCampaignsWithTokensOptionsPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(GetCampaignsWithTokensOnSuccessDelegate))]
        private static void OnGetCampaignsWithTokensSuccess(IntPtr responseJsonPtr)
        {
            var responseJson = Marshal.PtrToStringAnsi(responseJsonPtr);
            var response = GenericJsonConverter.JsonToObject<PlaytimeCampaignsResponse>(responseJson);
            
            if (getCampaignsWithTokensSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getCampaignsWithTokensSuccessCallback(response));
            }
            
            // Free allocated memory after callback
            if (getCampaignsWithTokensTokensPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getCampaignsWithTokensTokensPtr);
                getCampaignsWithTokensTokensPtr = IntPtr.Zero;
            }
            if (getCampaignsWithTokensOptionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getCampaignsWithTokensOptionsPtr);
                getCampaignsWithTokensOptionsPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GetCampaignsWithTokensOnErrorDelegate))]
        private static void OnGetCampaignsWithTokensError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (getCampaignsWithTokensErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getCampaignsWithTokensErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (getCampaignsWithTokensTokensPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getCampaignsWithTokensTokensPtr);
                getCampaignsWithTokensTokensPtr = IntPtr.Zero;
            }
            if (getCampaignsWithTokensOptionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getCampaignsWithTokensOptionsPtr);
                getCampaignsWithTokensOptionsPtr = IntPtr.Zero;
            }
        }

        // Delegates and static fields for GetInstalledCampaigns
        private delegate void GetInstalledCampaignsOnSuccessDelegate(IntPtr responseJsonPtr);
        private delegate void GetInstalledCampaignsOnErrorDelegate(IntPtr errorPtr);

        private static Action<PlaytimeCampaignsResponse> getInstalledCampaignsSuccessCallback;
        private static Action<Exception> getInstalledCampaignsErrorCallback;
        private static IntPtr getInstalledCampaignsOptionsPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(GetInstalledCampaignsOnSuccessDelegate))]
        private static void OnGetInstalledCampaignsSuccess(IntPtr responseJsonPtr)
        {
            var responseJson = Marshal.PtrToStringAnsi(responseJsonPtr);
            var response = GenericJsonConverter.JsonToObject<PlaytimeCampaignsResponse>(responseJson);
            
            if (getInstalledCampaignsSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getInstalledCampaignsSuccessCallback(response));
            }
            
            // Free allocated memory after callback
            if (getInstalledCampaignsOptionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getInstalledCampaignsOptionsPtr);
                getInstalledCampaignsOptionsPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GetInstalledCampaignsOnErrorDelegate))]
        private static void OnGetInstalledCampaignsError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (getInstalledCampaignsErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getInstalledCampaignsErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (getInstalledCampaignsOptionsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(getInstalledCampaignsOptionsPtr);
                getInstalledCampaignsOptionsPtr = IntPtr.Zero;
            }
        }

        // Delegates and static fields for OpenInStore
        private delegate void OpenInStoreOnSuccessDelegate();
        private delegate void OpenInStoreOnErrorDelegate(IntPtr errorPtr);

        private static Action openInStoreSuccessCallback;
        private static Action<Exception> openInStoreErrorCallback;
        private static IntPtr openInStoreCampaignPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(OpenInStoreOnSuccessDelegate))]
        private static void OnOpenInStoreSuccess()
        {
            if (openInStoreSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(openInStoreSuccessCallback);
            }
            
            // Free allocated memory after callback
            if (openInStoreCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(openInStoreCampaignPtr);
                openInStoreCampaignPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(OpenInStoreOnErrorDelegate))]
        private static void OnOpenInStoreError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (openInStoreErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => openInStoreErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (openInStoreCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(openInStoreCampaignPtr);
                openInStoreCampaignPtr = IntPtr.Zero;
            }
        }

        // Delegates and static fields for OpenChatbot
        private delegate void OpenChatbotOnSuccessDelegate();
        private delegate void OpenChatbotOnErrorDelegate(IntPtr errorPtr);

        private static Action openChatbotSuccessCallback;
        private static Action<Exception> openChatbotErrorCallback;
        private static IntPtr openChatbotCampaignPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(OpenChatbotOnSuccessDelegate))]
        private static void OnOpenChatbotSuccess()
        {
            if (openChatbotSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(openChatbotSuccessCallback);
            }
            
            // Free allocated memory after callback
            if (openChatbotCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(openChatbotCampaignPtr);
                openChatbotCampaignPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(OpenChatbotOnErrorDelegate))]
        private static void OnOpenChatbotError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (openChatbotErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => openChatbotErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (openChatbotCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(openChatbotCampaignPtr);
                openChatbotCampaignPtr = IntPtr.Zero;
            }
        }

        // Delegates and static fields for OpenInstalledCampaign
        private delegate void OpenInstalledCampaignOnSuccessDelegate();
        private delegate void OpenInstalledCampaignOnErrorDelegate(IntPtr errorPtr);

        private static Action openInstalledCampaignSuccessCallback;
        private static Action<Exception> openInstalledCampaignErrorCallback;
        private static IntPtr openInstalledCampaignCampaignPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(OpenInstalledCampaignOnSuccessDelegate))]
        private static void OnOpenInstalledCampaignSuccess()
        {
            if (openInstalledCampaignSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(openInstalledCampaignSuccessCallback);
            }
            
            // Free allocated memory after callback
            if (openInstalledCampaignCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(openInstalledCampaignCampaignPtr);
                openInstalledCampaignCampaignPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(OpenInstalledCampaignOnErrorDelegate))]
        private static void OnOpenInstalledCampaignError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (openInstalledCampaignErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => openInstalledCampaignErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (openInstalledCampaignCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(openInstalledCampaignCampaignPtr);
                openInstalledCampaignCampaignPtr = IntPtr.Zero;
            }
        }

        // Delegates and static fields for GetPermissions
        private delegate void GetPermissionsOnSuccessDelegate(IntPtr responseJsonPtr);
        private delegate void GetPermissionsOnErrorDelegate(IntPtr errorPtr);

        private static Action<PlaytimePermissionsResponse> getPermissionsSuccessCallback;
        private static Action<Exception> getPermissionsErrorCallback;

        [AOT.MonoPInvokeCallback(typeof(GetPermissionsOnSuccessDelegate))]
        private static void OnGetPermissionsSuccess(IntPtr responseJsonPtr)
        {
            var responseJson = Marshal.PtrToStringAnsi(responseJsonPtr);
            var response = GenericJsonConverter.JsonToObject<PlaytimePermissionsResponse>(responseJson);
            
            if (getPermissionsSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getPermissionsSuccessCallback(response));
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GetPermissionsOnErrorDelegate))]
        private static void OnGetPermissionsError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (getPermissionsErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => getPermissionsErrorCallback(new Exception(errorMessage)));
            }
        }

        // Delegates and static fields for ShowInstalledApps
        private delegate void ShowInstalledAppsOnSuccessDelegate();
        private delegate void ShowInstalledAppsOnErrorDelegate(IntPtr errorPtr);

        private static Action showInstalledAppsSuccessCallback;
        private static Action<Exception> showInstalledAppsErrorCallback;

        [AOT.MonoPInvokeCallback(typeof(ShowInstalledAppsOnSuccessDelegate))]
        private static void OnShowInstalledAppsSuccess()
        {
            if (showInstalledAppsSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(showInstalledAppsSuccessCallback);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(ShowInstalledAppsOnErrorDelegate))]
        private static void OnShowInstalledAppsError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (showInstalledAppsErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => showInstalledAppsErrorCallback(new Exception(errorMessage)));
            }
        }

        // Delegates and static fields for ShowAppDetails
        private delegate void ShowAppDetailsOnSuccessDelegate();
        private delegate void ShowAppDetailsOnErrorDelegate(IntPtr errorPtr);

        private static Action showAppDetailsSuccessCallback;
        private static Action<Exception> showAppDetailsErrorCallback;
        private static IntPtr showAppDetailsCampaignPtr = IntPtr.Zero;
        private static IntPtr showAppDetailsTokenPtr = IntPtr.Zero;
        private static IntPtr showAppDetailsAppIdPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(ShowAppDetailsOnSuccessDelegate))]
        private static void OnShowAppDetailsSuccess()
        {
            if (showAppDetailsSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(showAppDetailsSuccessCallback);
            }
            
            // Free allocated memory after callback
            if (showAppDetailsCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(showAppDetailsCampaignPtr);
                showAppDetailsCampaignPtr = IntPtr.Zero;
            }

            if (showAppDetailsTokenPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(showAppDetailsTokenPtr);
                showAppDetailsTokenPtr = IntPtr.Zero;
            }

            if (showAppDetailsAppIdPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(showAppDetailsAppIdPtr);
                showAppDetailsAppIdPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(ShowAppDetailsOnErrorDelegate))]
        private static void OnShowAppDetailsError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (showAppDetailsErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => showAppDetailsErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (showAppDetailsCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(showAppDetailsCampaignPtr);
                showAppDetailsCampaignPtr = IntPtr.Zero;
            }

            if (showAppDetailsTokenPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(showAppDetailsTokenPtr);
                showAppDetailsTokenPtr = IntPtr.Zero;
            }

            if (showAppDetailsAppIdPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(showAppDetailsAppIdPtr);
                showAppDetailsAppIdPtr = IntPtr.Zero;
            }
        } 
        
        // Delegates and static fields for ResetRewardsConnect
        private delegate void ResetRewardsConnectOnSuccessDelegate();
        private delegate void ResetRewardsConnectOnErrorDelegate(IntPtr errorPtr);

        private static Action resetRewardsConnectSuccessCallback;
        private static Action<Exception> resetRewardsConnectErrorCallback;
        private static IntPtr resetRewardsConnectCampaignPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(ResetRewardsConnectOnSuccessDelegate))]
        private static void OnResetRewardsConnectSuccess()
        {
            if (resetRewardsConnectSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(resetRewardsConnectSuccessCallback);
            }
            
            // Free allocated memory after callback
            if (resetRewardsConnectCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(resetRewardsConnectCampaignPtr);
                resetRewardsConnectCampaignPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(ResetRewardsConnectOnErrorDelegate))]
        private static void OnResetRewardsConnectError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (resetRewardsConnectErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => resetRewardsConnectErrorCallback(new Exception(errorMessage)));
            }
        }

        // Delegates and static fields for RegisterRewardsConnect
        private delegate void RegisterRewardsConnectOnSuccessDelegate(IntPtr responseJsonPtr);
        private delegate void RegisterRewardsConnectOnErrorDelegate(IntPtr errorPtr);

        private static Action registerRewardsConnectSuccessCallback;
        private static Action<Exception> registerRewardsConnectErrorCallback;

        [AOT.MonoPInvokeCallback(typeof(RegisterRewardsConnectOnSuccessDelegate))]
        private static void OnRegisterRewardsConnectSuccess()
        {
            if (registerRewardsConnectSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(registerRewardsConnectSuccessCallback);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(RegisterRewardsConnectOnErrorDelegate))]
        private static void OnRegisterRewardsConnectError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (registerRewardsConnectErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => registerRewardsConnectErrorCallback(new Exception(errorMessage)));
            }
        }

        // Delegates and static fields for ShowPermissionsPrompt
        private delegate void ShowPermissionsPromptOnSuccessDelegate(IntPtr responseJsonPtr);
        private delegate void ShowPermissionsPromptOnErrorDelegate(IntPtr errorPtr);

        private static Action<PlaytimePermissionsResponse> showPermissionsPromptSuccessCallback;
        private static Action<Exception> showPermissionsPromptErrorCallback;

        [AOT.MonoPInvokeCallback(typeof(ShowPermissionsPromptOnSuccessDelegate))]
        private static void OnShowPermissionsPromptSuccess(IntPtr responseJsonPtr)
        {
            var responseJson = Marshal.PtrToStringAnsi(responseJsonPtr);
            var response = GenericJsonConverter.JsonToObject<PlaytimePermissionsResponse>(responseJson);
            
            if (showPermissionsPromptSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => showPermissionsPromptSuccessCallback(response));
            }
        }

        [AOT.MonoPInvokeCallback(typeof(ShowPermissionsPromptOnErrorDelegate))]
        private static void OnShowPermissionsPromptError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (showPermissionsPromptErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => showPermissionsPromptErrorCallback(new Exception(errorMessage)));
            }
        }

        // Delegates and static fields for ExecuteEngagement
        private delegate void ExecuteEngagementOnSuccessDelegate();
        private delegate void ExecuteEngagementOnErrorDelegate(IntPtr errorPtr);

        private static Action executeEngagementSuccessCallback;
        private static Action<Exception> executeEngagementErrorCallback;
        private static IntPtr executeEngagementCampaignPtr = IntPtr.Zero;
        private static IntPtr executeEngagementEngagementTypePtr = IntPtr.Zero;
        private static IntPtr executeEngagementTokenPtr = IntPtr.Zero;
        private static IntPtr executeEngagementAppIDPtr = IntPtr.Zero;

        [AOT.MonoPInvokeCallback(typeof(ExecuteEngagementOnSuccessDelegate))]
        private static void OnExecuteEngagementSuccess()
        {
            if (executeEngagementSuccessCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(executeEngagementSuccessCallback);
            }
            
            // Free allocated memory after callback
            if (executeEngagementCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementCampaignPtr);
                executeEngagementCampaignPtr = IntPtr.Zero;
            }
            if (executeEngagementEngagementTypePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementEngagementTypePtr);
                executeEngagementEngagementTypePtr = IntPtr.Zero;
            }
            if (executeEngagementTokenPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementTokenPtr);
                executeEngagementTokenPtr = IntPtr.Zero;
            }
            if (executeEngagementAppIDPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementAppIDPtr);
                executeEngagementAppIDPtr = IntPtr.Zero;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(ExecuteEngagementOnErrorDelegate))]
        private static void OnExecuteEngagementError(IntPtr errorPtr)
        {
            var errorMessage = Marshal.PtrToStringAnsi(errorPtr);
            if (executeEngagementErrorCallback != null)
            {
                // Ensure callback runs on main thread
                Dispatcher.RunOnMainThread(() => executeEngagementErrorCallback(new Exception(errorMessage)));
            }
            
            // Free allocated memory after callback
            if (executeEngagementCampaignPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementCampaignPtr);
                executeEngagementCampaignPtr = IntPtr.Zero;
            }
            if (executeEngagementEngagementTypePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementEngagementTypePtr);
                executeEngagementEngagementTypePtr = IntPtr.Zero;
            }
            if (executeEngagementTokenPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementTokenPtr);
                executeEngagementTokenPtr = IntPtr.Zero;
            }
            if (executeEngagementAppIDPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(executeEngagementAppIDPtr);
                executeEngagementAppIDPtr = IntPtr.Zero;
            }
        }

        #endif

        #if UNITY_ANDROID
        private static bool useLegacyCallbacks = false;
        private static Dictionary<string, AndroidJavaObject> cache = new Dictionary<string, AndroidJavaObject>();
        #if UNITY_EDITOR
            private static AndroidJavaClass playtimeStudioAndroid = null;
        #else
            private static AndroidJavaClass playtimeStudioAndroid = new AndroidJavaClass("io.adjoe.sdk.studio.PlaytimeStudio");
        #endif 
        /* ----------------------------------
                    GENERAL METHODS
        ---------------------------------- */

        /// <summary>
        /// Use this method to enable or disable the legacy callback behaviour, where callbacks are run on the Java main thread rather than on the Unity render thread.
        /// </summary>
        /// <param name="useLegacy">the value to set.</param>
        public static void SetUseLegacyCallbacks(bool useLegacy) {
            useLegacyCallbacks = useLegacy;
        }
        #endif

        /// <summary>
        /// Get the list of offers that a user can install.
        /// </summary>
        /// <param name="options">The options for fetching campaigns.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void GetCampaigns(PlaytimeOptions options, Action<PlaytimeCampaignsResponse> onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var optionsJson = GenericJsonConverter.ObjectToJson(options);
            getCampaignsOptionsPtr = Marshal.StringToHGlobalAnsi(optionsJson);

            getCampaignsSuccessCallback = onSuccess;
            getCampaignsErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_getCampaigns(getCampaignsOptionsPtr, OnGetCampaignsSuccess, OnGetCampaignsError);

            #elif UNITY_ANDROID
            AndroidJavaObject javaOptions = ToJavaOptionsConverter.PlaytimeOptionsToJavaObject(options);
            object[] parameters = {
                Playtime.GetCurrentContext(),
                javaOptions,
                new PlaytimeCampaignsListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("getCampaigns", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.GetCampaigns(options=" + options + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Get campaigns by the specified tokens.
        /// </summary>
        /// <param name="tokens">Array of campaign tokens to fetch.</param>
        /// <param name="options">The options for fetching campaigns.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void GetCampaigns(string[] tokens, PlaytimeOptions options, Action<PlaytimeCampaignsResponse> onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var tokensString = string.Join(",", tokens);
            var optionsJson = GenericJsonConverter.ObjectToJson(options);
            getCampaignsWithTokensTokensPtr = Marshal.StringToHGlobalAnsi(tokensString);
            getCampaignsWithTokensOptionsPtr = Marshal.StringToHGlobalAnsi(optionsJson);

            getCampaignsWithTokensSuccessCallback = onSuccess;
            getCampaignsWithTokensErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_getCampaignsWithTokens(getCampaignsWithTokensTokensPtr, getCampaignsWithTokensOptionsPtr, OnGetCampaignsWithTokensSuccess, OnGetCampaignsWithTokensError);

            #elif UNITY_ANDROID
            AndroidJavaObject javaOptions = ToJavaOptionsConverter.PlaytimeOptionsToJavaObject(options);
            object[] parameters = {
                Playtime.GetCurrentContext(),
                GetJavaList(tokens),
                javaOptions,
                new PlaytimeCampaignsListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("getCampaigns", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.GetCampaigns(tokens = " + tokens + ", options=" + options + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Get the list of apps the user has already installed and that will contain the progress the user has made already.
        /// </summary>
        /// <param name="options">The options for fetching installed campaigns.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void GetInstalledCampaigns(PlaytimeOptions options, Action<PlaytimeCampaignsResponse> onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var optionsJson = GenericJsonConverter.ObjectToJson(options);
            getInstalledCampaignsOptionsPtr = Marshal.StringToHGlobalAnsi(optionsJson);

            getInstalledCampaignsSuccessCallback = onSuccess;
            getInstalledCampaignsErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_getInstalledCampaigns(getInstalledCampaignsOptionsPtr, OnGetInstalledCampaignsSuccess, OnGetInstalledCampaignsError);

            #elif UNITY_ANDROID
            AndroidJavaObject javaOptions = ToJavaOptionsConverter.PlaytimeOptionsToJavaObject(options);
            object[] parameters = {
                Playtime.GetCurrentContext(),
                javaOptions,
                new PlaytimeCampaignsListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("getInstalledCampaigns", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.GetInstalledCampaigns(options=" + options + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Use this method to forward the user to the store for an uninstalled app.
        /// </summary>
        /// <param name="campaign">The campaign to open in the store.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void OpenInStore(PlaytimeCampaign campaign, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var campaignJson = GenericJsonConverter.ObjectToJson(campaign);
            openInStoreCampaignPtr = Marshal.StringToHGlobalAnsi(campaignJson);

            openInStoreSuccessCallback = onSuccess;
            openInStoreErrorCallback = onError;

            Console.WriteLine("OpenInStore: " + campaignJson);

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_openInStore(openInStoreCampaignPtr, OnOpenInStoreSuccess, OnOpenInStoreError);

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                cache[campaign.CampaignUUID],
                new PlaytimeOpenStoreListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("openInStore", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.OpenInStore(campaign=" + campaign + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Use this method to open the chatbot
        /// </summary>
        /// <param name="campaign">The campaign to open in the store. (nullable)</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void OpenChatbot(PlaytimeCampaign campaign, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            openChatbotSuccessCallback = onSuccess;
            openChatbotErrorCallback = onError;

            if (campaign == null) {
                PlaytimeiOS.PlaytimeStudio_openChatbot(OnOpenChatbotSuccess, OnOpenChatbotError);
            } else {
                var campaignJson = GenericJsonConverter.ObjectToJson(campaign);
                openChatbotCampaignPtr = Marshal.StringToHGlobalAnsi(campaignJson);
                Console.WriteLine("OpenChatbot: " + campaignJson);

                // Use static callbacks instead of lambdas
                PlaytimeiOS.PlaytimeStudio_openChatbotWithCampaign(openChatbotCampaignPtr, OnOpenChatbotSuccess, OnOpenChatbotError);
            }

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                campaign == null ? null : cache[campaign.CampaignUUID],
                new PlaytimeDeeplinkListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("openChatbot", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.OpenChatbot(campaign=" + campaign + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Use this method to open installed application.
        /// </summary>
        /// <param name="campaign">The installed campaign to open.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void OpenInstalledCampaign(PlaytimeCampaign campaign, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var campaignJson = GenericJsonConverter.ObjectToJson(campaign);
            openInstalledCampaignCampaignPtr = Marshal.StringToHGlobalAnsi(campaignJson);

            openInstalledCampaignSuccessCallback = onSuccess;
            openInstalledCampaignErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_openInstalledCampaign(openInstalledCampaignCampaignPtr, OnOpenInstalledCampaignSuccess, OnOpenInstalledCampaignError);

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                cache[campaign.CampaignUUID],
                new PlaytimeOpenInstalledCampaignListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("openInstalledCampaign", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.OpenInstalledCampaign(campaign=" + campaign + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Get user's permissions.
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void GetPermissions(Action<PlaytimePermissionsResponse> onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            getPermissionsSuccessCallback = onSuccess;
            getPermissionsErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_getPermissions(OnGetPermissionsSuccess, OnGetPermissionsError);

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                new PlaytimePermissionsListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("getPermissions", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.GetPermissions(onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Reset rewards connect
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ResetRewardsConnect(Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            resetRewardsConnectSuccessCallback = onSuccess;
            resetRewardsConnectErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_resetRewardsConnect(OnResetRewardsConnectSuccess, OnResetRewardsConnectError);

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                new RewardsConnectResetListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("resetRewardsConnect", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.ResetRewardsConnect(onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Register rewards connect
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void RegisterRewardsConnect(Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            registerRewardsConnectSuccessCallback = onSuccess;
            registerRewardsConnectErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_registerRewardsConnect(OnRegisterRewardsConnectSuccess, OnRegisterRewardsConnectError);

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                new RewardsConnectRegistrationListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("registerRewardsConnect", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.RegisterRewardsConnect(onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Show the prompt requesting user's permissions.
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ShowPermissionsPrompt(Action<PlaytimePermissionsResponse> onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            showPermissionsPromptSuccessCallback = onSuccess;
            showPermissionsPromptErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_showPermissionsPrompt(OnShowPermissionsPromptSuccess, OnShowPermissionsPromptError);

            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                new PlaytimePermissionsListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("showPermissionsPrompt", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.ShowPermissionsPrompt(onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Show installed apps.
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ShowInstalledApps(Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            showInstalledAppsSuccessCallback = onSuccess;
            showInstalledAppsErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_showInstalledApps(OnShowInstalledAppsSuccess, OnShowInstalledAppsError);
            
            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                new PlaytimeDeeplinkListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("showInstalledApps", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.ShowInstalledApps(onSuccess=" + onSuccess + ", onError=" + onError + ")");
            #endif
        }

        /// <summary>
        /// Show app details.
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ShowAppDetails(PlaytimeCampaign campaign, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var campaignJson = GenericJsonConverter.ObjectToJson(campaign);
            showAppDetailsCampaignPtr = Marshal.StringToHGlobalAnsi(campaignJson);

            showAppDetailsSuccessCallback = onSuccess;
            showAppDetailsErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_showAppDetails(showAppDetailsCampaignPtr, OnShowAppDetailsSuccess, OnShowAppDetailsError);

            #elif UNITY_ANDROID
                Debug.Log("Called PlaytimeStudio.showAppDetails(onSuccess=" + onSuccess + ", onError=" + onError + ", campaign = " + campaign + ")");
            object[] parameters = {
                Playtime.GetCurrentContext(),
                cache[campaign.CampaignUUID],
                new PlaytimeDeeplinkListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("showAppDetails", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.showAppDetails(onSuccess=" + onSuccess + ", onError=" + onError + ", campaign = " + campaign + ")");
            #endif
        }

        /// <summary>
        /// Show app details with token.
        /// </summary>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ShowAppDetails(String token, String campaignAppId, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            showAppDetailsTokenPtr = Marshal.StringToHGlobalAnsi(token);
            showAppDetailsAppIdPtr = Marshal.StringToHGlobalAnsi(campaignAppId);

            showAppDetailsSuccessCallback = onSuccess;
            showAppDetailsErrorCallback = onError;

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_showAppDetailsWithToken(showAppDetailsTokenPtr, showAppDetailsAppIdPtr, OnShowAppDetailsSuccess, OnShowAppDetailsError);
            
            #elif UNITY_ANDROID
            object[] parameters = {
                Playtime.GetCurrentContext(),
                token,
                campaignAppId,
                new PlaytimeDeeplinkListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("showAppDetails", parameters);
            #elif UNITY_EDITOR
            Debug.Log("Called PlaytimeStudio.showAppDetails(onSuccess=" + onSuccess + ", onError=" + onError + ", token = " + token + ", campaignAppId = " + campaignAppId + ")");
            #endif
        }

        /// <summary>
        /// Execute a engagement request for the given campaign.
        /// This method tracks view execution locally and ensures only one view tracking request
        /// is sent to the backend per campaign within a 30-minute window.
        /// </summary>
        /// <param name="campaign">The campaign which view should be executed.</param>
        /// <param name="engagementType">The type of engagement you want to execute.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ExecuteEngagement(PlaytimeCampaign campaign, PlaytimeEngagementType engagementType, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            var campaignJson = GenericJsonConverter.ObjectToJson(campaign);

            var engagementTypeString = "default";

            if (engagementType == PlaytimeEngagementType.ENGAGED) {
                engagementTypeString = "engaged";
            }

            executeEngagementCampaignPtr = Marshal.StringToHGlobalAnsi(campaignJson);
            executeEngagementEngagementTypePtr = Marshal.StringToHGlobalAnsi(engagementTypeString);

            executeEngagementSuccessCallback = onSuccess;
            executeEngagementErrorCallback = onError;

            Console.WriteLine("ExecuteEngagement: " + campaignJson + engagementType);

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_executeEngagement(executeEngagementCampaignPtr, executeEngagementEngagementTypePtr, OnExecuteEngagementSuccess, OnExecuteEngagementError);

            #elif UNITY_ANDROID
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeStudio.ExecuteEngagement(campaign=" + campaign + ", engagementType=" + engagementType + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
                return;
            #endif
            object[] parameters = {
                Playtime.GetCurrentContext(),
                cache[campaign.CampaignUUID],
                GetJavaPlaytimeEngagementType(engagementType),
                new PlaytimeExecuteEngagementListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("executeEngagement", parameters);
            #endif
        }

        /// <summary>
        /// Execute a engagement request for the given campaign.
        /// This method tracks view execution locally and ensures only one view tracking request
        /// is sent to the backend per campaign within a 30-minute window.
        /// </summary>
        /// <param name="appID">The campaign which view should be executed.</param>
        /// <param name="token">Token of the campaign.</param>
        /// <param name="onSuccess">Callback called when the operation succeeds.</param>
        /// <param name="onError">Callback called when the operation fails.</param>
        public static void ExecuteEngagement(string appID, string token, Action onSuccess, Action<Exception> onError)
        {
            #if UNITY_IOS
            executeEngagementTokenPtr = Marshal.StringToHGlobalAnsi(token);
            executeEngagementAppIDPtr = Marshal.StringToHGlobalAnsi(appID);

            executeEngagementSuccessCallback = onSuccess;
            executeEngagementErrorCallback = onError;

            Console.WriteLine("ExecuteEngagement: " + appID + token);

            // Use static callbacks instead of lambdas
            PlaytimeiOS.PlaytimeStudio_executeEngagementWithToken(executeEngagementAppIDPtr, executeEngagementTokenPtr, OnExecuteEngagementSuccess, OnExecuteEngagementError);

            #elif UNITY_ANDROID
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeStudio.ExecuteEngagement(appID=" + appID + ", token=" + token + ", onSuccess=" + onSuccess + ", onError=" + onError + ")");
                return;
            #endif
            object[] parameters = {
                Playtime.GetCurrentContext(),
                appID,
                token,
                new PlaytimeExecuteEngagementListener(onSuccess, onError)
            };
            playtimeStudioAndroid.CallStatic("executeEngagement", parameters);
            #endif
        }

        #if UNITY_ANDROID
        /* ----------------------------------
                       LISTENERS
           ---------------------------------- */
        private class PlaytimeCampaignsListener : AndroidJavaProxy
        {
            private readonly Action<PlaytimeCampaignsResponse> successCallback;
            private readonly Action<Exception> errorCallback;

            public PlaytimeCampaignsListener(Action<PlaytimeCampaignsResponse> successCallback, Action<Exception> errorCallback) 
                    : base("io.adjoe.sdk.studio.PlaytimeCampaignsListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onReceived(AndroidJavaObject response)
            {
                if (successCallback == null)
                {
                    return;
                }

                AndroidJavaObject javaCampaigns = response.Call<AndroidJavaObject>("getCampaigns");
                int size = javaCampaigns.Call<int>("size");
                PlaytimeCampaign[] campaigns = new PlaytimeCampaign[size];

                for (int i = 0; i < size; i++)
                {
                    AndroidJavaObject javaCampaign = javaCampaigns.Call<AndroidJavaObject>("get", i);
                    PlaytimeCampaign campaign = new PlaytimeCampaign(javaCampaign);
                    campaigns[i] = campaign;
                    if (cache.ContainsKey(campaign.CampaignUUID)) {
                        cache.Remove(campaign.CampaignUUID);
                    }
                    cache.Add(campaign.CampaignUUID, javaCampaign);
                }

                PlaytimeCampaignsResponse campaignResponse = new PlaytimeCampaignsResponse(campaigns);

                if (useLegacyCallbacks)
                {
                    successCallback(campaignResponse);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback(campaignResponse);
                    });
                }
            }

            public void onError(AndroidJavaObject excetpion)
            {
                if (errorCallback == null || excetpion == null)
                {
                    return;
                }

                AndroidJavaObject error = excetpion.Call<AndroidJavaObject>("getError");
                Exception campaignsError = new Exception(error.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }

        private class RewardsConnectRegistrationListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public RewardsConnectRegistrationListener(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.connect.RewardsConnectRegistrationListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onSuccess()
            {
                if (successCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    successCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback();
                    });
                }
            }

            public void onFailure(AndroidJavaObject exception)
            {
                if (errorCallback == null || exception == null)
                {
                    return;
                }

                Exception campaignsError = new Exception(exception.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }


        private class RewardsConnectResetListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public RewardsConnectResetListener(Action successCallback, Action<Exception> errorCallback) 
                    : base("io.adjoe.sdk.connect.RewardsConnectResetListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onSuccess()
            {
                if (successCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    successCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback();
                    });
                }
            }

            public void onFailure(AndroidJavaObject exception)
            {
                if (errorCallback == null || exception == null)
                {
                    return;
                }

                Exception campaignsError = new Exception(exception.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }

        private class PlaytimePermissionsListener : AndroidJavaProxy
        {
            private readonly Action<PlaytimePermissionsResponse> successCallback;
            private readonly Action<Exception> errorCallback;

            public PlaytimePermissionsListener(Action<PlaytimePermissionsResponse> successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.studio.PlaytimePermissionsListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onReceived(AndroidJavaObject response)
            {
                if (successCallback == null)
                {
                    return;
                }

                AndroidJavaObject javaPermissions = response.Call<AndroidJavaObject>("getPermissions");
                PlaytimePermissions permissions = new PlaytimePermissions(javaPermissions);
                PlaytimePermissionsResponse permissionsResponse = new PlaytimePermissionsResponse(permissions);

                if (useLegacyCallbacks)
                {
                    successCallback(permissionsResponse);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback(permissionsResponse);
                    });
                }
            }

            public void onError(AndroidJavaObject excetpion)
            {
                if (errorCallback == null || excetpion == null)
                {
                    return;
                }

                AndroidJavaObject error = excetpion.Call<AndroidJavaObject>("getError");
                Exception campaignsError = new Exception(error.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }

        private class PlaytimeDeeplinkListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public PlaytimeDeeplinkListener(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.studio.PlaytimeDeeplinkListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onOpened()
            {
                if (successCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    successCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback();
                    });
                }
            }

            public void onError(AndroidJavaObject error)
            {
                if (errorCallback == null || error == null)
                {
                    return;
                }

                AndroidJavaObject exception = error.Call<AndroidJavaObject>("getError");
                Exception showDetailsError = new Exception(exception.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(showDetailsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(showDetailsError);
                    });
                }
            }
        }

        private class PlaytimeOpenStoreListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public PlaytimeOpenStoreListener(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.studio.PlaytimeOpenStoreListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onFinished()
            {
                if (successCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    successCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback();
                    });
                }
            }

            public void onAlreadyClicking()
            {
                if (errorCallback == null)
                {
                    return;
                }

                Exception campaignsError = new Exception("Already clicking campaign");

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }

            public void onError(AndroidJavaObject exception)
            {
                if (errorCallback == null || exception == null)
                {
                    return;
                }

                AndroidJavaObject error = exception.Call<AndroidJavaObject>("getError");
                Exception campaignsError = new Exception(error.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }

        private class PlaytimeExecuteEngagementListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public PlaytimeExecuteEngagementListener(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.studio.PlaytimeExecuteEngagementListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onFinished()
            {
                if (successCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    successCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback();
                    });
                }
            }

            public void onAlreadyEngaging()
            {
                if (errorCallback == null)
                {
                    return;
                }

                Exception campaignsError = new Exception("Already engaging with campaign");

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }

            public void onError(AndroidJavaObject excetpion)
            {
                if (errorCallback == null || excetpion == null)
                {
                    return;
                }

                AndroidJavaObject error = excetpion.Call<AndroidJavaObject>("getError");
                Exception campaignsError = new Exception(error.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }

        private class PlaytimeOpenInstalledCampaignListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public PlaytimeOpenInstalledCampaignListener(Action successCallback, Action<Exception> errorCallback) 
                : base("io.adjoe.sdk.studio.PlaytimeOpenInstalledCampaignListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onOpened()
            {
                if (successCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    successCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback();
                    });
                }
            }

            public void onError(AndroidJavaObject excetpion)
            {
                if (errorCallback == null || excetpion == null)
                {
                    return;
                }

                Exception campaignsError = new Exception(excetpion.Call<string>("getMessage"));

                if (useLegacyCallbacks)
                {
                    errorCallback(campaignsError);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(campaignsError);
                    });
                }
            }
        }

        /* ----------------------------------
                    PRIVATE METHODS
           ---------------------------------- */
        internal static AndroidJavaObject GetJavaList(string[] list) 
        {
            AndroidJavaObject javaList = new AndroidJavaObject("java.util.ArrayList");
            for (int i = 0; i < list.Length; i++) 
            {
                javaList.Call<bool>("add", list[i]);
            }
            return javaList;
        }

        internal static AndroidJavaObject GetJavaPlaytimeEngagementType(PlaytimeEngagementType engagementType){
            AndroidJavaObject javaPlaytimeEngagementType = null;
            
            switch (engagementType)
            {
                case PlaytimeEngagementType.ENGAGED:
                    javaPlaytimeEngagementType = new AndroidJavaClass("io.adjoe.sdk.studio.PlaytimeEngagementType").GetStatic<AndroidJavaObject>("ENGAGED");
                    break;

                case PlaytimeEngagementType.DEFAULT:
                default:
                    javaPlaytimeEngagementType = new AndroidJavaClass("io.adjoe.sdk.studio.PlaytimeEngagementType").GetStatic<AndroidJavaObject>("DEFAULT");
                    break;
            }

            return javaPlaytimeEngagementType;
        }
        #endif
    }
}
