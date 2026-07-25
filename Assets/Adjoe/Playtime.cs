using UnityEngine;
using System;
using System.Collections;

namespace io.adjoe.sdk
{
    /// <summary>
    /// The main class of the Playtime SDK.
    /// </summary>
    public class Playtime : MonoBehaviour
    {

        /// <summary>
        /// The user sees the Terms of Service of your app.
        /// <para>
        /// Trigger this event every time the user sees the dialog asking for his agreement to the playtime Terms of Service.
        /// </para>
        /// </summary>
        public const int EVENT_TOS_SHOWN = 1;

        /// <summary>
        /// The user has just accepted the Terms of Service of your app.
        /// <para>
        /// Trigger this event when the user has pressed "yes" (or other positive action) on the dialog asking for his agreement to the playtime Terms of Service.
        /// </para>
        /// </summary>
        public const int EVENT_TOS_ACCEPTED = 2;

        /// <summary>
        /// The user has just declined the Terms of Service of your app.
        /// <para>
        /// Trigger this event when the user has pressed "no" (or other negative action) on the dialog asking for his agreement to the playtime Terms of Service.
        /// </para>
        /// </summary>
        public const int EVENT_TOS_DECLINED = 3;

        /// <summary>
        /// The user has just given your app the access to the usage data.
        /// <para>
        /// Trigger this event when the user has returned from his phone settings to your app for the first time with the permission for usage tracking via your app given.
        /// </para>
        /// </summary>
        public const int EVENT_USAGE_PERMISSION_ACCEPTED = 4;

        /// <summary>
        /// The user has denied to give your app the access to the usage data.
        /// </para>
        /// Trigger this event when the user has pressed "no" (or other negative action) on the dialog asking for permission for usage tracking.
        /// </para>
        /// </summary>
        public const int EVENT_USAGE_PERMISSION_DENIED = 5;
        [Obsolete("This event is automatically sent when you call PlaytimePartnerApp.executeClick.")]
        public const int EVENT_INSTALL_CLICKED = 6;

        /// <summary>
        /// The user has started the video for any of the partner apps.
        /// <para>
        /// Trigger this event when the user has started the video of an playtime advertisement.
        /// </para>
        /// </summary>
        public const int EVENT_VIDEO_PLAY = 7;

        /// <summary>
        /// The user has paused the video for any of the partner apps.
        /// <para>
        /// Trigger this event when the user has paused the video of an playtime advertisement.
        /// </para>
        /// </summary>
        public const int EVENT_VIDEO_PAUSE = 8;

        /// <summary>
        /// The video for one of the partner apps has ended.
        /// <para>
        /// Trigger this event when the video of an playtime advertisement has reached its end.
        /// </para>
        /// </summary>
        public const int EVENT_VIDEO_ENDED = 9;

        /// <summary>
        /// You have just displayed the campaigns screen to the user.
        /// <para>
        /// Trigger this event when the Playtime catalog is loaded with either advertised partner apps ir already installed partner apps for signed-up users.
        /// </para>
        /// </summary>
        public const int EVENT_CAMPAIGNS_SHOWN = 10;
        [Obsolete("This event is automatically sent when you call PlaytimePartnerApp.executeView")]
        public const int EVENT_CAMPAIGN_VIEW = 11;

        /// <summary>
        /// The user has opened your app.
        /// <para>
        /// Trigger this event when your app is launched, i.e. a new SDK app session for the user is initiated.
        /// </para>
        /// </summary>
        public const int EVENT_APP_OPEN = 12;

        /// <summary>
        /// The user sees the Playtime content (e.g. the Start method of the Scene displaying the Playtime content is called).
        /// <para>
        /// Trigger this event on the following actions:
        /// <list type="bullet">
        /// <item>(1) the user has opened your app and started a new Playtime-related session.</item>
        /// <item>(2) the user has brought your app from the background to the foreground to continue his Playtime-related session (exception: the user has left your app to allow usage tracking in his phone settings).</item>
        /// </list>
        /// This event should be triggered in both cases regardless of whether the user has already signed up or not.
        /// </para>
        /// </summary>
        public const int EVENT_FIRST_IMPRESSION = 13;

        /// <summary>
        /// The user can see the teaser, e.g. the button via which he can access the Playtime SDK from the SDK app.
        /// <para>
        /// Trigger this event when the teaser has been successfully rendered and would successfully redirect the user to the Playtime SDK. it should be triggered regardless of whether the user has actually clicked the teaser or not. This event is mostly appropriate for uses in which the functionality of the SDK app and SDK are kept separate to a relevant degree.
        /// </para>
        /// </summary>
        public const int EVENT_TEASER_SHOWN = 14;

        /// <summary>
        /// Set this to true to use the legacy callback behaviour, where callbacks are run on the Java main thread rather than on the Unity render thread.
        /// </summary>
        private static bool useLegacyCallbacks = false;

        internal static AndroidJavaObject webViewContainer;

        #if UNITY_EDITOR
            private static AndroidJavaClass playtime = null;
        #else
            private static AndroidJavaClass playtime = new AndroidJavaClass("io.adjoe.sdk.Playtime");
        #endif

        #if UNITY_EDITOR
            private static AndroidJavaClass playtimeCustom = null;
        #else
            private static AndroidJavaClass playtimeCustom = new AndroidJavaClass("io.adjoe.sdk.custom.PlaytimeCustom");
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

        /// <summary>
        /// Initializes the Playtime SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the Playtime SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your Playtime SDK hash.</param>
        /// <see cref="Playtime.Init(string, PlaytimeOptions)" />
        /// <see cref="Playtime.Init(string, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash)
        {
            Init(sdkHash, new PlaytimeOptions(), null, null, null, null);
        }

        /// <summary>
        /// Initializes the Playtime SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the Playtime SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your Playtime SDK hash.</param>
        /// <param name="options">An object to pass additional options to the Playtime SDK when initializing.</param>
        /// <see cref="Playtime.Init(string)" />
        /// <see cref="Playtime.Init(string, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash, PlaytimeOptions options)
        {
            Init(sdkHash, options, null, null, null, null);
        }

        /// <summary>
        /// Initializes the Playtime SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the Playtime SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your Playtime SDK hash.</param>
        /// <param name="successCallback">A callback which is invoked when the Playtime SDK was initialized successfully.</param>
        /// <param name="errorCallback">A callback which is invoked when the initialization of the Playtime SDK failed.</param>
        /// <see cref="Playtime.Init(string)" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions)" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash, Action successCallback, Action<Exception> errorCallback)
        {
            Init(sdkHash, new PlaytimeOptions(), null, null, successCallback, errorCallback);
        }

        /// <summary>
        /// Initializes the Playtime SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the Playtime SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your Playtime SDK hash.</param>
        /// <param name="options">An object to pass additional options to the Playtime SDK when initializing.</param>
        /// <param name="successCallback">A callback which is invoked when the Playtime SDK was initialized successfully.</param>
        /// <param name="errorCallback">A callback which is invoked when the initialization of the Playtime SDK failed.</param>
        /// <see cref="Playtime.Init(string)" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions)" />
        /// <see cref="Playtime.Init(string, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash, PlaytimeOptions playtimeOptions, Action successCallback, Action<Exception> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.Init(sdkHash=" + sdkHash + ", playtimeOptions=" + playtimeOptions + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaPlaytimeOptions = new AndroidJavaObject("io.adjoe.sdk.PlaytimeOptions");
            if (playtimeOptions != null)
            {
                javaPlaytimeOptions.Call<AndroidJavaObject>("setUserId", new object[] { playtimeOptions.userId });
                string wrapper = "unity";
                javaPlaytimeOptions.Call("w", new object[] { wrapper });

                AndroidJavaObject playtimeParams = GetJavaPlaytimeParams(playtimeOptions.playtimeParams);
                javaPlaytimeOptions.Call<AndroidJavaObject>("setParams", new object[] { playtimeParams });

                if (playtimeOptions.playtimeExtensions != null){
                    AndroidJavaObject javaPlaytimeExtensions = GetJavaPlaytimeExtensions(playtimeOptions.playtimeExtensions);
                    javaPlaytimeOptions.Call<AndroidJavaObject>("setExtensions", new object[] { javaPlaytimeExtensions });    
                }
                
                if (playtimeOptions.playtimeUserProfile != null){
                    AndroidJavaObject javaPlaytimeUserProfile = GetJavaPlaytimeUserProfile (playtimeOptions.playtimeUserProfile);
                    javaPlaytimeOptions.Call<AndroidJavaObject>("setUserProfile", new object[] { javaPlaytimeUserProfile });
                }

                if (playtimeOptions.applicationProcessName != null){
                    javaPlaytimeOptions.Call<AndroidJavaObject>("setApplicationProcessName", new object[]{ playtimeOptions.applicationProcessName});
                }
            }

            object[] parameters =
            {
                GetCurrentContext(),
                sdkHash,
                javaPlaytimeOptions,
                new InitialisationListener(successCallback, errorCallback)
            };

            playtime.CallStatic("init", parameters);
        }

        /// <summary>
        /// Initializes the playtime SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the playtime SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your playtime SDK hash.</param>
        /// <param name="options">An object to pass additional options to the playtime SDK when initializing.</param>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>.
        /// <param name="successCallback">A callback which is invoked when the playtime SDK was initialized successfully.</param>
        /// <param name="errorCallback">A callback which is invoked when the initialization of the playtime SDK failed.</param>
        /// <see cref="Playtime.Init(string)" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions)" />
        /// <see cref="Playtime.Init(string, Action, Action{Exception})" />
        /// <see cref="Playtime.Init(string, PlaytimeOptions, Action, Action{Exception})" />
        [Obsolete("Method is Deprecated, use Playtime.Init(string, PlaytimeOptions, Action, Action{Exception})")]
        public static void Init(string sdkHash, PlaytimeOptions playtimeOptions, string uaNetwork, string uaChannel, Action successCallback, Action<Exception> errorCallback)
        {
            if (playtimeOptions == null)
            {
                playtimeOptions = new PlaytimeOptions();
            }
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);
            playtimeOptions.SetPlaytimeParams(playtimeParams);
            Init(sdkHash, playtimeOptions, successCallback, errorCallback);
        }


        /// <summary>
        /// Displays the playtime offerwal.
        /// </summary>
        /// The catalog contains the UI to show the user the rewarded apps which he can install and use.
        public static void ShowCatalog()
        {
            ShowCatalog(null, null);
        }

        /// <summary>
        /// Displays the playtime offerwal.
        /// </summary>
        /// The catalog contains the UI to show the user the rewarded apps which he can install and use.
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        public static void ShowCatalog(string uaNetwork, string uaChannel)
        {
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);

            ShowCatalog(playtimeParams);
        }

        /// <summary>
        /// Displays the playtime offerwal.
        /// </summary>
        /// The catalog contains the UI to show the user the rewarded apps which he can install and use.
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        public static void ShowCatalog(PlaytimeParams playtimeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.ShowCatalog(playtimeParams=" + playtimeParams);
                return;
            #endif
            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);
            AndroidJavaObject playtimeCatalogIntent = playtime.CallStatic<AndroidJavaObject>("getCatalogIntent", GetCurrentContext(), javaParams);
            GetCurrentContext().Call("startActivity", playtimeCatalogIntent);
        }

        /// <summary>
        /// Registers two listeners which notify you when the catalog is opened or closed.
        /// </summary>
        /// <param name="onCatalogOpened">The callback to invoke when the catalog is opened.</param>
        /// <param name="onCatalogClosed">The callback to invoke when the catalog is closed.</param>
        public static void SetCatalogListener(Action<string> onCatalogOpened, Action<string> onCatalogClosed)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.SetCatalogListener(onCatalogOpened=" + onCatalogOpened + ", onCatalogClosed=" + onCatalogClosed + ")");
                return;
            #endif

            playtime.CallStatic("setCatalogListener", new CatalogListener(onCatalogOpened, onCatalogClosed));
        }

        public static void SetUAParams(PlaytimeParams playtimeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.SetUAParams(PlaytimeParams=" + playtimeParams + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);
            object[] parameters = {
                GetCurrentContext(),
                javaParams
            };
            playtime.CallStatic("setUAParams", parameters);
        }

        /// <summary>
        /// Requests the user's current rewards, including how many of them are available for payout and how many have already been paid out.
        /// </summary>
        /// <remarks>
        /// This method will invoke one of the callbacks, depending on whether the request was successful or not.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the request was successful.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestRewards(Action<PlaytimeRewardResponse> successCallback, Action<PlaytimeRewardResponseError> errorCallback)
        {
            RequestRewards(null, null, successCallback, errorCallback);
        }


        /// <summary>
        /// Requests the user's current rewards, including how many of them are available for payout and how many have already been paid out.
        /// </summary>
        /// <remarks>
        /// This method will invoke one of the callbacks, depending on whether the request was successful or not.
        /// </remarks>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request was successful.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestRewards(string uaNetwork, string uaChannel, Action<PlaytimeRewardResponse> successCallback, Action<PlaytimeRewardResponseError> errorCallback)
        {
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);
            RequestRewards(playtimeParams, successCallback, errorCallback);
        }

        /// <summary>
        /// Requests the user's current rewards, including how many of them are available for payout and how many have already been paid out.
        /// </summary>
        /// <remarks>
        /// This method will invoke one of the callbacks, depending on whether the request was successful or not.
        /// </remarks>
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request was successful.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestRewards(PlaytimeParams playtimeParams, Action<PlaytimeRewardResponse> successCallback, Action<PlaytimeRewardResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeCustom.RequestRewards(playtimeParams=" + playtimeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);
            object[] parameters = {
                GetCurrentContext(),
                javaParams,
                new RewardListener(successCallback, errorCallback)
            };
            playtimeCustom.CallStatic("requestRewards", parameters);
        }

        [Obsolete("DoPayout with coins parameter is deprecated, please use DoPayout without coins parameter instead.")]
        public static void DoPayout(int coins, Action<PlaytimePayoutResponse> successCallback, Action<PlaytimePayoutError> errorCallback)
        {
            DoPayout(successCallback, errorCallback);
        }

        /// <summary>
        /// Pays out the user's collected rewards.
        /// </summary>
        /// <remarks>
        /// When finished, one of the callbacks will be invoked, depending on the result of the payout.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the payout is successful.</param>
        /// <param name="errorCallback">The callback to invoke when the payout has failed.</param>
        public static void DoPayout(Action<PlaytimePayoutResponse> successCallback, Action<PlaytimePayoutError> errorCallback)
        {
            DoPayout(null, null, successCallback, errorCallback);
        }

        /// <summary>
        /// Pays out the user's collected rewards.
        /// </summary>
        /// <remarks>
        /// When finished, one of the callbacks will be invoked, depending on the result of the payout.
        /// </remarks>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the payout is successful.</param>
        /// <param name="errorCallback">The callback to invoke when the payout has failed.</param>
        public static void DoPayout(string uaNetwork, string uaChannel, Action<PlaytimePayoutResponse> successCallback, Action<PlaytimePayoutError> errorCallback)
        {
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);
            DoPayout(playtimeParams, successCallback, errorCallback);
        }

        /// <summary>
        /// Pays out the user's collected rewards.
        /// </summary>
        /// <remarks>
        /// When finished, one of the callbacks will be invoked, depending on the result of the payout.
        /// </remarks>
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the payout is successful.</param>
        /// <param name="errorCallback">The callback to invoke when the payout has failed.</param>
        public static void DoPayout(PlaytimeParams playtimeParams, Action<PlaytimePayoutResponse> successCallback, Action<PlaytimePayoutError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeCustom.DoPayout(playtimeParams=" + playtimeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);

            object[] parameters = {
                GetCurrentContext(),
                javaParams,
                new PayoutListener(successCallback, errorCallback)
            };
            playtimeCustom.CallStatic("doPayout", parameters);
        }


        /* ----------------------------------
              ADVANCED IMPLEMENTATION METHODS
           ---------------------------------- */

        public static void InitWebView()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.InitWebView()");
                return;
            #endif

            AndroidJavaObject activity = GetCurrentContext();

            // create the FrameLayout
            object[] parameters = {activity};
            webViewContainer = new AndroidJavaObject("android.widget.FrameLayout", parameters);

            // create the ViewGroup.LayoutParams
            parameters = new object[] {1, 1};
            AndroidJavaObject layoutParams = new AndroidJavaObject("android.view.ViewGroup$LayoutParams", parameters);

            // add the FrameLayout to the content view
            // needs to run on Android's UI thread in order to work (UI may only be modified form the UI thread)
            activity.Call("runOnUiThread", new AndroidJavaRunnable(delegate() {
                parameters = new object[] {webViewContainer, layoutParams};
                activity.Call("addContentView", parameters);
            }));
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the catalog.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestPartnerApps(Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            RequestPartnerApps(null, null, successCallback, errorCallback);
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the catalog.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestPartnerApps(string uaNetwork, string uaChannel, Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);
            RequestPartnerApps(playtimeParams, successCallback, errorCallback);
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the catalog.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestPartnerApps(PlaytimeParams playtimeParams, Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeCustom.RequestPartnerApps(playtimeParams=" + playtimeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);
            object[] parameters = {
                GetCurrentContext(),
                webViewContainer,
                javaParams,
                new CampaignListener(successCallback, errorCallback)
            };
            playtimeCustom.CallStatic("requestPartnerApps", parameters);
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the Advance/PIR catalog.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        [Obsolete("Use RequestPartnerApps(PlaytimeParams, Action, Action) instead.")]
        public static void RequestAdvancePartnerApps(PlaytimeParams playtimeParams, Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.RequestAdvancePartnerApps(playtimeParams=" + playtimeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);
            object[] parameters = {
                GetCurrentContext(),
                webViewContainer,
                javaParams,
                new CampaignListener(successCallback, errorCallback)
            };
            playtime.CallStatic("requestPostInstallRewardPartnerApps", parameters);
        }

        /// <summary>
        /// Requests a list of partner apps which the user has installed.
        /// </summary>
        /// <param name="successCallback">The callback to invoke when the request was successful. This will also give you the list of installed partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestInstalledPartnerApps(Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            RequestInstalledPartnerApps(null, null, successCallback, errorCallback);
        }

        /// <summary>
        /// Requests a list of partner apps which the user has installed.
        /// </summary>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request was successful. This will also give you the list of installed partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestInstalledPartnerApps(string uaNetwork, string uaChannel, Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);
            RequestInstalledPartnerApps(playtimeParams, successCallback, errorCallback);
        }

        /// <summary>
        /// Requests a list of partner apps which the user has installed.
        /// </summary>
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request was successful. This will also give you the list of installed partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestInstalledPartnerApps(PlaytimeParams playtimeParams, Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeCustom.RequestInstalledPartnerApps(playtimeParams=" + playtimeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif
            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);

            object[] parameters =
            {
                GetCurrentContext(),
                javaParams,
                new CampaignListener(successCallback, errorCallback)
            };
            playtimeCustom.CallStatic("requestInstalledPartnerApps", parameters);
        }



        /// <summary>
        /// Opens the usage statistics system settings page so that the user can give access to it.
        /// </summary>
        /// <remarks>
        /// When the user has given access, it automatically will return back to your current activity
        /// </remarks>
        public static void ShowUsagePermissionScreen()
        {
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeCustom.ShowUsagePermissionScreen()");
                return;
            #endif

            object[] parameters = {
                GetCurrentContext()
            };
            playtimeCustom.CallStatic("showUsagePermissionScreen", parameters);
        }

        /// <summary>
        /// Accepts the playtime Terms of Service (TOS).
        /// </summary>
        /// <remarks>
        /// When the user accepts the playtime TOS, call this method to notify playtime about it.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when playtime has successfully processed this information.</param>
        /// <param name="errorCallback">The callback to invoke when playtime couldn't process this information.</param>
        public static void SetTosAccepted(Action successCallback, Action<Exception> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called PlaytimeCustom.SetTosAccepted(successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            object[] parameters = {
                GetCurrentContext(),
                new InitialisationListener(successCallback, errorCallback)
            };
            playtimeCustom.CallStatic("setTosAccepted", parameters);
        }

        /// <summary>
        /// Sends a uer event to playtime.
        /// </summary>
        /// <remarks>
        /// These events help to improve the accuracy of the app recommendations for the user.
        /// </remarks>
        /// <param name="eventId">The ID of the event.</param>
        public static void SendUserEvent(int eventId)
        {
            SendUserEvent(eventId, null, null, null);
        }

        /// <summary>
        /// Sends a uer event to playtime.
        /// </summary>
        /// <remarks>
        /// These events help to improve the accuracy of the app recommendations for the user.
        /// </remarks>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="extra">For <c>EVENT_VIDEO_PLAY</c>, <c>EVENT_VIDEO_PAUSE</c> and <c>EVENT_VIDEO_ENDED</c> this must be the application ID of the app to which the video belongs, otherwise <c>null</c>.
        public static void SendUserEvent(int eventId, string extra)
        {
            SendUserEvent(eventId, extra, null, null);
        }

        /// <summary>
        /// Sends a uer event to playtime.
        /// </summary>
        /// <remarks>
        /// These events help to improve the accuracy of the app recommendations for the user.
        /// </remarks>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        public static void SendUserEvent(int eventId, string uaNetwork, string uaChannel)
        {
            SendUserEvent(eventId, null, uaNetwork, uaChannel);
        }

        /// <summary>
        /// Sends a uer event to playtime.
        /// </summary>
        /// <remarks>
        /// These events help to improve the accuracy of the app recommendations for the user.
        /// </remarks>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="extra">For <c>EVENT_VIDEO_PLAY</c>, <c>EVENT_VIDEO_PAUSE</c> and <c>EVENT_VIDEO_ENDED</c> this must be the application ID of the app to which the video belongs, otherwise <c>null</c>.
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        public static void SendUserEvent(int eventId, string extra, string uaNetwork, string uaChannel)
        {
            PlaytimeParams playtimeParams = new PlaytimeParams();
            playtimeParams.SetUaNetwork(uaNetwork);
            playtimeParams.SetUaChannel(uaChannel);
            SendUserEvent(eventId, extra, playtimeParams);
        }

        /// <summary>
        /// Sends a uer event to playtime.
        /// </summary>
        /// <remarks>
        /// These events help to improve the accuracy of the app recommendations for the user.
        /// </remarks>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="extra">For <c>EVENT_VIDEO_PLAY</c>, <c>EVENT_VIDEO_PAUSE</c> and <c>EVENT_VIDEO_ENDED</c> this must be the application ID of the app to which the video belongs, otherwise <c>null</c>.
        /// <param name="playtimeParams">The PlaytimeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        public static void SendUserEvent(int eventId, string extra, PlaytimeParams playtimeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.SendUserEvent(eventId=" + eventId + ", extra=" + extra + ", playtimeParams=" + playtimeParams + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaPlaytimeParams(playtimeParams);

            object[] parameters = {
                GetCurrentContext(),
                eventId,
                extra,
                javaParams
            };
            playtime.CallStatic("sendUserEvent", parameters);
        }

        /* ----------------------------------
                    UTILITY METHODS
           ---------------------------------- */

        /// <summary>
        /// Returns the version of the playtime SDK.
        /// </summary>
        /// <returns>The version of the playtime SDK.</returns>
        public static int GetVersion()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.GetVersion()");
                return 0;
            #endif

            return playtime.CallStatic<int>("getVersion");
        }

        /// <summary>
        /// Returns the version name of the playtime SDK.
        /// </summary>
        /// <returns>The version name of the playtime SDK.</returns>
        public static string GetVersionName()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.GetVersionNAme()");
                return "";
            #endif

            return playtime.CallStatic<string>("getVersionName");
        }

        /// <summary>
        /// Checks whether the playtime SDK is initialized.
        /// </summary>
        /// <returns><c>true</c> when it is initialized, <c>false</c> otherwise.</returns>
        public static bool IsInitialized()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.IsInitialized()");
                return false;
            #endif

            return playtime.CallStatic<bool>("isInitialized");
        }

        /// <summary>
        /// Checks whether the user has accepted the playtime Terms of Service (TOS).
        /// </summary>
        /// <returns><c>true</c> when the user has accepted the TOS, <c>false</c> otherwise.</returns>
        public static bool HasAcceptedTOS()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.HasAcceptedTOS()");
                return false;
            #endif

            return playtime.CallStatic<bool>("hasAcceptedTOS", GetCurrentContext());
        }

        /// <summary>
        /// Checks whether the user has given access to the usage statistics.
        /// </summary>
        /// <returns><c>true</c> when the user has given access, <c>false</c> otherwise.</returns>
        public static bool HasAcceptedUsagePermission()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.HasAcceptedUsagePermission()");
                return false;
            #endif

            return playtime.CallStatic<bool>("hasAcceptedUsagePermission", GetCurrentContext());
        }

        /// <summary>
        /// Returns the unique ID of the user by which he is identified within the playtime infrastructure.
        /// </summary>
        /// <returns>The user's unique ID.</returns>
        public static string GetUserId()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.GetUserId()");
                return null;
            #endif

            return playtime.CallStatic<string>("getUserId", GetCurrentContext());
        }

        public static void a(bool whether)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Playtime.a(whether=" + whether + ")");
                return;
            #endif

            object[] parameters =
            {
                GetCurrentContext(),
                whether
            };
            playtime.CallStatic("a", parameters);
        }

        /* ----------------------------------
                    OTHER METHODS
           ---------------------------------- */

        public static void FaceVerificationStatus(
            Action verifiedCallback,
            Action notVerifiedCallback,
            Action notInitializedCallback,
            Action tosIsNotAcceptedCallback,
            Action pendingReviewCallback,
            Action maxAttemptsReachedCallback,
            Action<Exception> errorCallback)
        {
            object[] parameters = {
                GetCurrentContext(),
                new FaceVerificationStatusListener(
                    verifiedCallback,
                    notVerifiedCallback,
                    notInitializedCallback,
                    tosIsNotAcceptedCallback,
                    pendingReviewCallback,
                    maxAttemptsReachedCallback,
                    errorCallback
                )
            };
            playtimeCustom.CallStatic("faceVerificationStatus", parameters);
        }

        public static void FaceVerification(
            Action successCallback,
            Action alreadyVerifiedCallback,
            Action cancelCallback,
            Action notInitializedCallback,
            Action tosIsNotAcceptedCallback,
            Action livenessCheckFailedCallback,
            Action pendingReviewCallback,
            Action maxAttemptsReachedCallback,
            Action<Exception> errorCallback)
        {
            object[] parameters = {
                GetCurrentContext(),
                new FaceVerificationListener(
                    successCallback,
                    alreadyVerifiedCallback,
                    cancelCallback,
                    notInitializedCallback,
                    tosIsNotAcceptedCallback,
                    livenessCheckFailedCallback,
                    pendingReviewCallback,
                    maxAttemptsReachedCallback,
                    errorCallback
                )
            };
            playtime.CallStatic("faceVerification", parameters);
        }

        /* ----------------------------------
                    PRIVATE METHODS
           ---------------------------------- */

        internal static AndroidJavaObject GetCurrentContext()
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }

        internal static AndroidJavaObject GetJavaPlaytimeParams(PlaytimeParams playtimeParams)
        {
            AndroidJavaObject builder = new AndroidJavaObject("io.adjoe.sdk.PlaytimeParams$Builder");
                if (playtimeParams != null)
                {
                    builder.Call<AndroidJavaObject>("setUaNetwork", new object[] {playtimeParams.uaNetwork});
                    builder.Call<AndroidJavaObject>("setUaChannel", new object[] {playtimeParams.uaChannel});
                    builder.Call<AndroidJavaObject>("setUaSubPublisherCleartext", new object[] {playtimeParams.uaSubPublisherCleartext});
                    builder.Call<AndroidJavaObject>("setUaSubPublisherEncrypted", new object[] {playtimeParams.uaSubPublisherEncrypted});
                    builder.Call<AndroidJavaObject>("setPlacement", new object[] {playtimeParams.placement});
                }
            AndroidJavaObject javaPlaytimeParams = builder.Call<AndroidJavaObject>("build");
            return javaPlaytimeParams;

        }

        internal static AndroidJavaObject GetJavaPlaytimeExtensions(PlaytimeExtensions playtimeExtensions)
        {
            AndroidJavaObject builder = new AndroidJavaObject("io.adjoe.sdk.PlaytimeExtensions$Builder");
            if (playtimeExtensions != null){
                builder.Call<AndroidJavaObject>("setSubId1", new object[] {playtimeExtensions.subId1});
                builder.Call<AndroidJavaObject>("setSubId2", new object[] {playtimeExtensions.subId2});
                builder.Call<AndroidJavaObject>("setSubId3", new object[] {playtimeExtensions.subId3});
                builder.Call<AndroidJavaObject>("setSubId4", new object[] {playtimeExtensions.subId4});
                builder.Call<AndroidJavaObject>("setSubId5", new object[] {playtimeExtensions.subId5});
            }
            AndroidJavaObject javaPlaytimeExtensions = builder.Call<AndroidJavaObject>("build");
            return javaPlaytimeExtensions;

        }

        internal static AndroidJavaObject GetJavaPlaytimeUserProfile (PlaytimeUserProfile profile){

            long timestamp = (long) profile.birthday.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            AndroidJavaObject javaBirthday = new AndroidJavaObject("java.util.Date", timestamp);
            AndroidJavaObject javaGender = null;
            switch (profile.gender)
            {
                case PlaytimeGender.MALE:
                    javaGender = new AndroidJavaClass("io.adjoe.sdk.PlaytimeGender").GetStatic<AndroidJavaObject>("MALE");
                    break;

                case PlaytimeGender.FEMALE:
                    javaGender = new AndroidJavaClass("io.adjoe.sdk.PlaytimeGender").GetStatic<AndroidJavaObject>("FEMALE");
                    break;

                case PlaytimeGender.UNKNOWN:
                default:
                    javaGender = new AndroidJavaClass("io.adjoe.sdk.PlaytimeGender").GetStatic<AndroidJavaObject>("UNKNOWN");
                    break;
            }

            AndroidJavaObject playtimeUserProfile = new AndroidJavaObject("io.adjoe.sdk.PlaytimeUserProfile", new object[] {javaGender, javaBirthday});

            return playtimeUserProfile;
        }

        /* ----------------------------------
                       LISTENERS
           ---------------------------------- */
        private class InitialisationListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public InitialisationListener(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.PlaytimeInitialisationListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onInitialisationFinished()
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
                    Dispatcher.RunOnMainThread(successCallback);
                }
            }

            public void onInitialisationError(AndroidJavaObject excetpion)
            {
                if (errorCallback == null)
                {
                    return;
                }
                if (useLegacyCallbacks)
                {
                    errorCallback(new Exception(excetpion.Call<string>("getMessage")));
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(new Exception(excetpion.Call<string>("getMessage")));
                    });
                }
            }
        }

        private class RewardListener : AndroidJavaProxy
        {
            private readonly Action<PlaytimeRewardResponse> successCallback;
            private readonly Action<PlaytimeRewardResponseError> errorCallback;

            public RewardListener(Action<PlaytimeRewardResponse> successCallback, Action<PlaytimeRewardResponseError> errorCallback) : base("io.adjoe.sdk.custom.PlaytimeRewardListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onUserReceivesReward(AndroidJavaObject data) // data has class io.adjoe.sdk.PlaytimeRewardResponse
            {
                if (successCallback == null || data == null)
                {
                    return;
                }

                PlaytimeRewardResponse response = new PlaytimeRewardResponse();

                // use getters because the fields might be obfuscated
                response.Reward = data.Get<int>("reward");
                response.AvailablePayoutCoins = data.Get<int>("availablePayoutCoins");
                response.AlreadySpentCoins = data.Get<int>("alreadySpentCoins");

                if (useLegacyCallbacks)
                {
                    successCallback(response);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback(response);
                    });
                }
            }

            public void onUserReceivesRewardError(AndroidJavaObject data) // data has class io.adjoe.sdk.PlaytimeRewardResponseError
            {
                if (errorCallback == null || data == null)
                {
                    return;
                }

                PlaytimeRewardResponseError error = new PlaytimeRewardResponseError();
                if (data.Get<AndroidJavaObject>("exception") != null)
                {
                    error.Exception = new Exception(data.Get<AndroidJavaObject>("exception").Call<string>("getMessage"));
                }

                if (useLegacyCallbacks)
                {
                    errorCallback(error);
                }
                else {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(error);
                    });
                }
            }
        }

        private class PayoutListener : AndroidJavaProxy
        {
            private readonly Action<PlaytimePayoutResponse> successCallback;
            private readonly Action<PlaytimePayoutError> errorCallback;

            public PayoutListener(Action<PlaytimePayoutResponse> successCallback, Action<PlaytimePayoutError> errorCallback) : base("io.adjoe.sdk.custom.PlaytimePayoutListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onPayoutExecuted(int coins)
            {
                if (successCallback == null)
                {
                    return;
                }

                PlaytimePayoutResponse response = new PlaytimePayoutResponse();
                response.Coins = coins;

                if (useLegacyCallbacks)
                {
                    successCallback(response);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        successCallback(response);
                    });
                }
            }

            public void onPayoutError(AndroidJavaObject data) // data has class io.adjoe.sdk.PlaytimePayoutError
            {
                if (errorCallback == null || data == null)
                {
                    return;
                }

                PlaytimePayoutError error = new PlaytimePayoutError();
                error.Reason = data.Call<int>("getReason");
                if (data.Call<AndroidJavaObject>("getException") != null)
                {
                    error.Exception = new Exception(data.Call<AndroidJavaObject>("getException").Call<string>("getMessage"));
                }

                if (useLegacyCallbacks)
                {
                    errorCallback(error);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(error);
                    });
                }
            }

        }


        private class CampaignListener : AndroidJavaProxy
        {
            private readonly Action<PlaytimeCampaignResponse> successCallback;
            private readonly Action<PlaytimeCampaignResponseError> errorCallback;

            public CampaignListener(Action<PlaytimeCampaignResponse> successCallback, Action<PlaytimeCampaignResponseError> errorCallback) : base("io.adjoe.sdk.custom.PlaytimeCampaignListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onCampaignsReceived(AndroidJavaObject response)
            {
                if (successCallback == null)
                {
                    return;
                }

                AndroidJavaObject javaPartnerApps = response.Get<AndroidJavaObject>("partnerApps");
                int size = javaPartnerApps.Call<int>("size");
                ArrayList partnerApps = new ArrayList(size);
                for (int i = 0; i < size; i++)
                {
                    AndroidJavaObject javaPartnerApp = javaPartnerApps.Call<AndroidJavaObject>("get", i);
                    PlaytimePartnerApp partnerApp = new PlaytimePartnerApp(javaPartnerApp);
                    partnerApps.Add(partnerApp);
                }
                PlaytimeCampaignResponse campaignResponse = new PlaytimeCampaignResponse
                {
                    PartnerApps = partnerApps
                };

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

            public void onCampaignsReceivedError(AndroidJavaObject response)
            {
                if (errorCallback == null || response == null)
                {
                    return;
                }
                AndroidJavaObject exception = response.Get<AndroidJavaObject>("exception");
                PlaytimeCampaignResponseError campaignsError = new PlaytimeCampaignResponseError();
                campaignsError.Exception = new Exception(exception.Call<string>("getMessage"));

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

        private class CatalogListener : AndroidJavaProxy
        {
            private readonly Action<string> openedCallback;
            private readonly Action<string> closedCallback;

            public CatalogListener(Action<string> openedCallback, Action<string> closedCallback) : base("io.adjoe.sdk.PlaytimeCatalogListener")
            {
                this.openedCallback = openedCallback;
                this.closedCallback = closedCallback;
            }

            public void onCatalogOpened(string type)
            {
                if (openedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    openedCallback(type);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        openedCallback(type);
                    });
                }
            }

            public void onCatalogClosed(string type)
            {
                if (closedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    closedCallback(type);
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        closedCallback(type);
                    });
                }
            }
        }

        private class FaceVerificationListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action alreadyVerifiedCallback;
            private readonly Action cancelCallback;
            private readonly Action notInitializedCallback;
            private readonly Action tosIsNotAcceptedCallback;
            private readonly Action livenessCheckFailedCallback;
            private readonly Action pendingReviewCallback;
            private readonly Action maxAttemptsReachedCallback;
            private readonly Action<Exception> errorCallback;

            public FaceVerificationListener(
                Action successCallback,
                Action alreadyVerifiedCallback,
                Action cancelCallback,
                Action notInitializedCallback,
                Action tosIsNotAcceptedCallback,
                Action livenessCheckFailedCallback,
                Action pendingReviewCallback,
                Action maxAttemptsReachedCallback,
                Action<Exception> errorCallback) : base("io.adjoe.sdk.Playtime$FaceVerificationCallback")
            {
                this.successCallback = successCallback;
                this.alreadyVerifiedCallback = alreadyVerifiedCallback;
                this.cancelCallback = cancelCallback;
                this.notInitializedCallback = notInitializedCallback;
                this.tosIsNotAcceptedCallback = tosIsNotAcceptedCallback;
                this.livenessCheckFailedCallback = livenessCheckFailedCallback;
                this.pendingReviewCallback = pendingReviewCallback;
                this.maxAttemptsReachedCallback = maxAttemptsReachedCallback;
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
                    Dispatcher.RunOnMainThread(successCallback);
                }
            }

            public void onAlreadyVerified()
            {
                if (alreadyVerifiedCallback == null)
                {
                    return;
                }


                if (useLegacyCallbacks)
                {
                    alreadyVerifiedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(alreadyVerifiedCallback);
                }
            }

            public void onCancel()
            {
                if (cancelCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    cancelCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(cancelCallback);
                }
            }

            public void onNotInitialized()
            {
                if (notInitializedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    notInitializedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(notInitializedCallback);
                }
            }

            public void onTosIsNotAccepted()
            {
                if (tosIsNotAcceptedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    tosIsNotAcceptedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(tosIsNotAcceptedCallback);
                }
            }

            public void onLivenessCheckFailed()
            {
                if (livenessCheckFailedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    livenessCheckFailedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(livenessCheckFailedCallback);
                }
            }

            public void onPendingReview()
            {
                if (pendingReviewCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    pendingReviewCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(pendingReviewCallback);
                }
            }

            public void onMaxAttemptsReached()
            {
                if (maxAttemptsReachedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    maxAttemptsReachedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(maxAttemptsReachedCallback);
                }
            }

            public void onError(AndroidJavaObject exception)
            {
                if (errorCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    errorCallback(new Exception(exception.Call<string>("getMessage")));
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(new Exception(exception.Call<string>("getMessage")));
                    });
                }
            }
        }

        private class FaceVerificationStatusListener : AndroidJavaProxy
        {
            private readonly Action verifiedCallback;
            private readonly Action notVerifiedCallback;
            private readonly Action notInitializedCallback;
            private readonly Action tosIsNotAcceptedCallback;
            private readonly Action pendingReviewCallback;
            private readonly Action maxAttemptsReachedCallback;
            private readonly Action<Exception> errorCallback;

            public FaceVerificationStatusListener(
                Action verifiedCallback,
                Action notVerifiedCallback,
                Action notInitializedCallback,
                Action tosIsNotAcceptedCallback,
                Action pendingReviewCallback,
                Action maxAttemptsReachedCallback,
                Action<Exception> errorCallback) : base("io.adjoe.sdk.custom.PlaytimeCustom$FaceVerificationStatusCallback")
            {
                this.verifiedCallback = verifiedCallback;
                this.notVerifiedCallback = notVerifiedCallback;
                this.notInitializedCallback = notInitializedCallback;
                this.tosIsNotAcceptedCallback = tosIsNotAcceptedCallback;
                this.pendingReviewCallback = pendingReviewCallback;
                this.maxAttemptsReachedCallback = maxAttemptsReachedCallback;
                this.errorCallback = errorCallback;
            }

            public void onVerified()
            {
                if (verifiedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    verifiedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(verifiedCallback);
                }
            }

            public void onNotVerified()
            {
                if (notVerifiedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    notVerifiedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(notVerifiedCallback);
                }
            }

            public void onNotInitialized()
            {
                if (notInitializedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    notInitializedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(notInitializedCallback);
                }
            }

            public void onTosIsNotAccepted()
            {
                if (tosIsNotAcceptedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    tosIsNotAcceptedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(tosIsNotAcceptedCallback);
                }
            }

            public void onPendingReview()
            {
                if (pendingReviewCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    pendingReviewCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(pendingReviewCallback);
                }
            }

            public void onMaxAttemptsReached()
            {
                if (maxAttemptsReachedCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    maxAttemptsReachedCallback();
                }
                else
                {
                    Dispatcher.RunOnMainThread(maxAttemptsReachedCallback);
                }
            }

            public void onError(AndroidJavaObject exception)
            {
                if (errorCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    errorCallback(new Exception(exception.Call<string>("getMessage")));
                }
                else
                {
                    Dispatcher.RunOnMainThread(() => {
                        errorCallback(new Exception(exception.Call<string>("getMessage")));
                    });
                }
            }
        }
    }
}
