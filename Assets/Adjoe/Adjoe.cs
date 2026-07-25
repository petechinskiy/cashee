using UnityEngine;
using System;
using System.Collections;

namespace io.adjoe.sdk
{
    /// <summary>
    /// The main class of the adjoe SDK.
    /// </summary>
    public class Adjoe : MonoBehaviour
    {

        /// <summary>
        /// The user sees the Terms of Service of your app.
        /// <para>
        /// Trigger this event every time the user sees the dialog asking for his agreement to the adjoe Terms of Service.
        /// </para>
        /// </summary>
        public const int EVENT_TOS_SHOWN = 1;

        /// <summary>
        /// The user has just accepted the Terms of Service of your app.
        /// <para>
        /// Trigger this event when the user has pressed "yes" (or other positive action) on the dialog asking for his agreement to the adjoe Terms of Service.
        /// </para>
        /// </summary>
        public const int EVENT_TOS_ACCEPTED = 2;

        /// <summary>
        /// The user has just declined the Terms of Service of your app.
        /// <para>
        /// Trigger this event when the user has pressed "no" (or other negative action) on the dialog asking for his agreement to the adjoe Terms of Service.
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
        [Obsolete("This event is automatically sent when you call AdjoePartnerApp.executeClick.")]
        public const int EVENT_INSTALL_CLICKED = 6;

        /// <summary>
        /// The user has started the video for any of the partner apps.
        /// <para>
        /// Trigger this event when the user has started the video of an adjoe advertisement.
        /// </para>
        /// </summary>
        public const int EVENT_VIDEO_PLAY = 7;

        /// <summary>
        /// The user has paused the video for any of the partner apps.
        /// <para>
        /// Trigger this event when the user has paused the video of an adjoe advertisement.
        /// </para>
        /// </summary>
        public const int EVENT_VIDEO_PAUSE = 8;

        /// <summary>
        /// The video for one of the partner apps has ended.
        /// <para>
        /// Trigger this event when the video of an adjoe advertisement has reached its end.
        /// </para>
        /// </summary>
        public const int EVENT_VIDEO_ENDED = 9;

        /// <summary>
        /// You have just displayed the campaigns screen to the user.
        /// <para>
        /// Trigger this event when the adjoe offerwall is loaded with either advertised partner apps ir already installed partner apps for signed-up users.
        /// </para>
        /// </summary>
        public const int EVENT_CAMPAIGNS_SHOWN = 10;
        [Obsolete("This event is automatically sent when you call AdjoePartnerApp.executeView")]
        public const int EVENT_CAMPAIGN_VIEW = 11;

        /// <summary>
        /// The user has opened your app.
        /// <para>
        /// Trigger this event when your app is launched, i.e. a new SDK app session for the user is initiated.
        /// </para>
        /// </summary>
        public const int EVENT_APP_OPEN = 12;

        /// <summary>
        /// The user sees the adjoe content (e.g. the Start method of the Scene displaying the adjoe content is called).
        /// <para>
        /// Trigger this event on the following actions:
        /// <list type="bullet">
        /// <item>(1) the user has opened your app and started a new adjoe-related session.</item>
        /// <item>(2) the user has brought your app from the background to the foreground to continue his adjoe-related session (exception: the user has left your app to allow usage tracking in his phone settings).</item>
        /// </list>
        /// This event should be triggered in both cases regardless of whether the user has already signed up or not.
        /// </para>
        /// </summary>
        public const int EVENT_FIRST_IMPRESSION = 13;

        /// <summary>
        /// The user can see the teaser, e.g. the button via which he can access the adjoe SDK from the SDK app.
        /// <para>
        /// Trigger this event when the teaser has been successfully rendered and would successfully redirect the user to the adjoe SDK. it should be triggered regardless of whether the user has actually clicked the teaser or not. This event is mostly appropriate for uses in which the functionality of the SDK app and SDK are kept separate to a relevant degree.
        /// </para>
        /// </summary>
        public const int EVENT_TEASER_SHOWN = 14;

        /// <summary>
        /// Set this to true to use the legacy callback behaviour, where callbacks are run on the Java main thread rather than on the Unity render thread.
        /// </summary>
        private static bool useLegacyCallbacks = false;

        internal static AndroidJavaObject webViewContainer;

        #if UNITY_EDITOR
            private static AndroidJavaClass adjoe = null;
        #else
            private static AndroidJavaClass adjoe = new AndroidJavaClass("io.adjoe.sdk.Adjoe");
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
        /// Initializes the adjoe SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the adjoe SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your adjoe SDK hash.</param>
        /// <see cref="Adjoe.Init(string, AdjoeOptions)" />
        /// <see cref="Adjoe.Init(string, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash)
        {
            Init(sdkHash, new AdjoeOptions(), null, null, null, null);
        }

        /// <summary>
        /// Initializes the adjoe SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the adjoe SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your adjoe SDK hash.</param>
        /// <param name="options">An object to pass additional options to the adjoe SDK when initializing.</param>
        /// <see cref="Adjoe.Init(string)" />
        /// <see cref="Adjoe.Init(string, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash, AdjoeOptions options)
        {
            Init(sdkHash, options, null, null, null, null);
        }

        /// <summary>
        /// Initializes the adjoe SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the adjoe SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your adjoe SDK hash.</param>
        /// <param name="successCallback">A callback which is invoked when the adjoe SDK was initialized successfully.</param>
        /// <param name="errorCallback">A callback which is invoked when the initialization of the adjoe SDK failed.</param>
        /// <see cref="Adjoe.Init(string)" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions)" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash, Action successCallback, Action<Exception> errorCallback)
        {
            Init(sdkHash, new AdjoeOptions(), null, null, successCallback, errorCallback);
        }

        /// <summary>
        /// Initializes the adjoe SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the adjoe SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your adjoe SDK hash.</param>
        /// <param name="options">An object to pass additional options to the adjoe SDK when initializing.</param>
        /// <param name="successCallback">A callback which is invoked when the adjoe SDK was initialized successfully.</param>
        /// <param name="errorCallback">A callback which is invoked when the initialization of the adjoe SDK failed.</param>
        /// <see cref="Adjoe.Init(string)" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions)" />
        /// <see cref="Adjoe.Init(string, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, string, string, Action, Action{Exception})" />
        public static void Init(string sdkHash, AdjoeOptions adjoeOptions, Action successCallback, Action<Exception> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.Init(sdkHash=" + sdkHash + ", adjoeOptions=" + adjoeOptions + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaAdjoeOptions = new AndroidJavaObject("io.adjoe.sdk.Adjoe$Options");
            if (adjoeOptions != null)
            {
                javaAdjoeOptions.Call<AndroidJavaObject>("setUserId", new object[] { adjoeOptions.userId });
                string wrapper = "unity";
                javaAdjoeOptions.Call("w", new object[] { wrapper });

                AndroidJavaObject adjoeParams = GetJavaAdjoeParams(adjoeOptions.adjoeParams);
                javaAdjoeOptions.Call<AndroidJavaObject>("setParams", new object[] { adjoeParams });

                if (adjoeOptions.adjoeExtensions != null){
                    AndroidJavaObject javaAdjoeExtensions = GetJavaAdjoeExtensions(adjoeOptions.adjoeExtensions);
                    javaAdjoeOptions.Call<AndroidJavaObject>("setExtensions", new object[] { javaAdjoeExtensions });    
                }
                
                if (adjoeOptions.adjoeUserProfile != null){
                    AndroidJavaObject javaAdjoeUserProfile = GetJavaAdjoeUserProfile (adjoeOptions.adjoeUserProfile);
                    javaAdjoeOptions.Call<AndroidJavaObject>("setUserProfile", new object[] { javaAdjoeUserProfile });
                }

                if (adjoeOptions.applicationProcessName != null){
                    javaAdjoeOptions.Call<AndroidJavaObject>("setApplicationProcessName", new object[]{ adjoeOptions.applicationProcessName});
                }
            }

            object[] parameters =
            {
                GetCurrentContext(),
                sdkHash,
                javaAdjoeOptions,
                new InitialisationListener(successCallback, errorCallback)
            };

            adjoe.CallStatic("init", parameters);
        }

        /// <summary>
        /// Initializes the adjoe SDK.
        /// </summary>
        /// <remarks>
        /// You must initialize the adjoe SDK before you can use any of its features.
        /// The initialization will run asynchronously in the background and notify you when it is finished by invoking one of the <c>Action</c> parameters which you pass.
        /// <param name="sdkHash">Your adjoe SDK hash.</param>
        /// <param name="options">An object to pass additional options to the adjoe SDK when initializing.</param>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>.
        /// <param name="successCallback">A callback which is invoked when the adjoe SDK was initialized successfully.</param>
        /// <param name="errorCallback">A callback which is invoked when the initialization of the adjoe SDK failed.</param>
        /// <see cref="Adjoe.Init(string)" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions)" />
        /// <see cref="Adjoe.Init(string, Action, Action{Exception})" />
        /// <see cref="Adjoe.Init(string, AdjoeOptions, Action, Action{Exception})" />
        [Obsolete("Method is Deprecated, use Adjoe.Init(string, AdjoeOptions, Action, Action{Exception})")]
        public static void Init(string sdkHash, AdjoeOptions adjoeOptions, string uaNetwork, string uaChannel, Action successCallback, Action<Exception> errorCallback)
        {
            if (adjoeOptions == null)
            {
                adjoeOptions = new AdjoeOptions();
            }
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            adjoeOptions.SetAdjoeParams(adjoeParams);
            Init(sdkHash, adjoeOptions, successCallback, errorCallback);
        }

        /// <summary>
        /// Checks whether you can display the offerwall.
        /// </summary>
        /// <returns><c>true</c> when you can display the offerwall and <c>false</c> otherwise.</returns>
        /// <seealso cref="Adjoe.ShowOfferwall" />
        /// <seealso cref="Adjoe.ShowOfferwall(string, string)" />
        public static bool CanShowOfferwall()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.CanShowOfferwall()");
                return false;
            #endif

            return adjoe.CallStatic<bool>("canShowOfferwall", GetCurrentContext());
        }

        /// <summary>
        /// Checks whether you can display the Advance offerwall.
        /// </summary>
        /// <returns><c>true</c> when you can display the offerwall and <c>false</c> otherwise.</returns>
        /// <seealso cref="Adjoe.ShowOfferwall" />
        /// <seealso cref="Adjoe.ShowOfferwall(string, string)" />
        public static bool CanShowAdvanceOfferwall()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.CanShowAdvanceOfferwall()");
                return false;
            #endif

            return adjoe.CallStatic<bool>("canShowPostInstallRewardOfferwall", GetCurrentContext());
        }

        /// <summary>
        /// Displays the adjoe offerwal.
        /// </summary>
        /// The offerwall contains the UI to show the user the rewarded apps which he can install and use.
        /// <seealso cref="Adjoe.CanShowOfferwall" />
        public static void ShowOfferwall()
        {
            ShowOfferwall(null, null);
        }

        /// <summary>
        /// Displays the adjoe offerwal.
        /// </summary>
        /// The offerwall contains the UI to show the user the rewarded apps which he can install and use.
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        public static void ShowOfferwall(string uaNetwork, string uaChannel)
        {
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);

            ShowOfferwall(adjoeParams);
        }

        /// <summary>
        /// Displays the adjoe offerwal.
        /// </summary>
        /// The offerwall contains the UI to show the user the rewarded apps which he can install and use.
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        public static void ShowOfferwall(AdjoeParams adjoeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.ShowOfferwall(adjoeParams=" + adjoeParams);
                return;
            #endif
            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);
            AndroidJavaObject adjoeOfferwallIntent = adjoe.CallStatic<AndroidJavaObject>("getOfferwallIntent", GetCurrentContext(), javaParams);
            GetCurrentContext().Call("startActivity", adjoeOfferwallIntent);
        }

        /// <summary>
        /// Registers two listeners which notify you when the offerwall is opened or closed.
        /// </summary>
        /// <param name="onOfferwallOpened">The callback to invoke when the offerwall is opened.</param>
        /// <param name="onOfferwallClosed">The callback to invoke when the offerwall is closed.</param>
        public static void SetOfferwallListener(Action<string> onOfferwallOpened, Action<string> onOfferwallClosed)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.SetOfferwallListener(onOfferwallOpened=" + onOfferwallOpened + ", onOfferwallClosed=" + onOfferwallClosed + ")");
                return;
            #endif

            adjoe.CallStatic("setOfferwallListener", new OfferwallListener(onOfferwallOpened, onOfferwallClosed));
        }

        public static void SetUAParams(AdjoeParams adjoeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.SetUAParams(AdjoeParams=" + adjoeParams + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);
            object[] parameters = {
                GetCurrentContext(),
                javaParams
            };
            adjoe.CallStatic("setUAParams", parameters);
        }

        /// <summary>
        /// Requests the user's current rewards, including how many of them are available for payout and how many have already been paid out.
        /// </summary>
        /// <remarks>
        /// This method will invoke one of the callbacks, depending on whether the request was successful or not.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the request was successful.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestRewards(Action<AdjoeRewardResponse> successCallback, Action<AdjoeRewardResponseError> errorCallback)
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
        public static void RequestRewards(string uaNetwork, string uaChannel, Action<AdjoeRewardResponse> successCallback, Action<AdjoeRewardResponseError> errorCallback)
        {
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            RequestRewards(adjoeParams, successCallback, errorCallback);
        }

        /// <summary>
        /// Requests the user's current rewards, including how many of them are available for payout and how many have already been paid out.
        /// </summary>
        /// <remarks>
        /// This method will invoke one of the callbacks, depending on whether the request was successful or not.
        /// </remarks>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request was successful.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestRewards(AdjoeParams adjoeParams, Action<AdjoeRewardResponse> successCallback, Action<AdjoeRewardResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.RequestRewards(adjoeParams=" + adjoeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);
            object[] parameters = {
                GetCurrentContext(),
                javaParams,
                new RewardListener(successCallback, errorCallback)
            };
            adjoe.CallStatic("requestRewards", parameters);
        }

        [Obsolete("DoPayout with coins parameter is deprecated, please use DoPayout without coins parameter instead.")]
        public static void DoPayout(int coins, Action<AdjoePayoutResponse> successCallback, Action<AdjoePayoutError> errorCallback)
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
        public static void DoPayout(Action<AdjoePayoutResponse> successCallback, Action<AdjoePayoutError> errorCallback)
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
        public static void DoPayout(string uaNetwork, string uaChannel, Action<AdjoePayoutResponse> successCallback, Action<AdjoePayoutError> errorCallback)
        {
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            DoPayout(adjoeParams, successCallback, errorCallback);
        }

        /// <summary>
        /// Pays out the user's collected rewards.
        /// </summary>
        /// <remarks>
        /// When finished, one of the callbacks will be invoked, depending on the result of the payout.
        /// </remarks>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the payout is successful.</param>
        /// <param name="errorCallback">The callback to invoke when the payout has failed.</param>
        public static void DoPayout(AdjoeParams adjoeParams, Action<AdjoePayoutResponse> successCallback, Action<AdjoePayoutError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.DoPayout(adjoeParams=" + adjoeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);

            object[] parameters = {
                GetCurrentContext(),
                javaParams,
                new PayoutListener(successCallback, errorCallback)
            };
            adjoe.CallStatic("doPayout", parameters);
        }

        /// <summary>
        /// Provides adjoe with the information about the user's profile.
        /// </summary>
        /// <remarks>
        /// With this information adjoe can suggest better matching apps to the user.
        /// All arguments are optional, but the more information you can provide, the more accurately adjoe can suggest apps.
        /// </remarks>
        /// <param name="source">Describes how or where you obtained the information (e.g. "facebook", "google", "user_input", ...).</param>
        /// <param name="gender">The user's gender.</param>
        /// <param name="birthday">The user's birthday.</param>
        [Obsolete("Use the options parameter in init methods to pass the profile data instead.")]
        public static void SetProfile(string source, AdjoeGender gender, DateTime birthday)
        {
            SetProfile(source, gender, birthday, null, null);
        }

        /// <summary>
        /// Provides adjoe with the information about the user's profile.
        /// </summary>
        /// <remarks>
        /// With this information adjoe can suggest better matching apps to the user.
        /// All arguments are optional, but the more information you can provide, the more accurately adjoe can suggest apps.
        /// </remarks>
        /// <param name="source">Describes how or where you obtained the information (e.g. "facebook", "google", "user_input", ...).</param>
        /// <param name="gender">The user's gender.</param>
        /// <param name="birthday">The user's birthday.</param>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        [Obsolete("Use the options parameter in init methods to pass the profile data instead.")]
        public static void SetProfile(string source, AdjoeGender gender, DateTime birthday, string uaNetwork, string uaChannel)
        {
            AdjoeUserProfile profile = new AdjoeUserProfile();
            profile.SetGender(gender);
            profile.SetBirthday(birthday);

            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            SetProfile(source, profile, adjoeParams);
        }

        /// <summary>
        /// Provides adjoe with the information about the user's profile.
        /// </summary>
        /// <remarks>
        /// With this information adjoe can suggest better matching apps to the user.
        /// All arguments are optional, but the more information you can provide, the more accurately adjoe can suggest apps.
        /// </remarks>
        /// <param name="source">Describes how or where you obtained the information (e.g. "facebook", "google", "user_input", ...).</param>
        /// <param name="profile">The user's profile (contains the birthday and gender).</param>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        [Obsolete("Use the options parameter in init methods to pass the profile data instead.")]
        public static void SetProfile(string source, AdjoeUserProfile profile, AdjoeParams adjoeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.SetProfile(source=" + source + "profile=" + profile + ", adjoeParams=" + adjoeParams);
                return;
            #endif

            AndroidJavaObject adjoeUserProfile = GetJavaAdjoeUserProfile(profile);
            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);

            object[] parameters = {
                GetCurrentContext(),
                source,
                adjoeUserProfile,
                javaParams
            };
            adjoe.CallStatic("setProfile", parameters);
        }

        /* ----------------------------------
              ADVANCED IMPLEMENTATION METHODS
           ---------------------------------- */

        public static void InitWebView()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.InitWebView()");
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
        ///  Requests a list of partner apps which the user would see in the offerwall.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Adjoe.InitWebView()</c> before.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestPartnerApps(Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
        {
            RequestPartnerApps(null, null, successCallback, errorCallback);
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the offerwall.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Adjoe.InitWebView()</c> before.
        /// </remarks>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestPartnerApps(string uaNetwork, string uaChannel, Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
        {
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            RequestPartnerApps(adjoeParams, successCallback, errorCallback);
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the offerwall.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Adjoe.InitWebView()</c> before.
        /// </remarks>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestPartnerApps(AdjoeParams adjoeParams, Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.RequestPartnerApps(adjoeParams=" + adjoeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);
            object[] parameters = {
                GetCurrentContext(),
                webViewContainer,
                javaParams,
                new CampaignListener(successCallback, errorCallback)
            };
            adjoe.CallStatic("requestPartnerApps", parameters);
        }

        /// <summary>
        ///  Requests a list of partner apps which the user would see in the Advance/PIR offerwall.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Adjoe.InitWebView()</c> before.
        /// </remarks>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request is successful. This will also give you the list of partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        [Obsolete("Use RequestPartnerApps(AdjoeParams, Action, Action) instead.")]
        public static void RequestAdvancePartnerApps(AdjoeParams adjoeParams, Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.RequestAdvancePartnerApps(adjoeParams=" + adjoeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);
            object[] parameters = {
                GetCurrentContext(),
                webViewContainer,
                javaParams,
                new CampaignListener(successCallback, errorCallback)
            };
            adjoe.CallStatic("requestPostInstallRewardPartnerApps", parameters);
        }

        /// <summary>
        /// Requests a list of partner apps which the user has installed.
        /// </summary>
        /// <param name="successCallback">The callback to invoke when the request was successful. This will also give you the list of installed partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestInstalledPartnerApps(Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
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
        public static void RequestInstalledPartnerApps(string uaNetwork, string uaChannel, Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
        {
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            RequestInstalledPartnerApps(adjoeParams, successCallback, errorCallback);
        }

        /// <summary>
        /// Requests a list of partner apps which the user has installed.
        /// </summary>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="successCallback">The callback to invoke when the request was successful. This will also give you the list of installed partner apps.</param>
        /// <param name="errorCallback">The callback to invoke when the request has failed.</param>
        public static void RequestInstalledPartnerApps(AdjoeParams adjoeParams, Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.RequestInstalledPartnerApps(adjoeParams=" + adjoeParams + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif
            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);

            object[] parameters =
            {
                GetCurrentContext(),
                javaParams,
                new CampaignListener(successCallback, errorCallback)
            };
            adjoe.CallStatic("requestInstalledPartnerApps", parameters);
        }

        /// <summary>
        /// Opens the usage statistics system settings page so that the user can give access to it.
        /// </summary>
        /// <remarks>
        /// When the user hsd given access and adjoe has processed that information, <c>successCallback</c> will be invoked.
        /// If the user hasn't given access after a certain amount of time, <c>errorCallback</c> will be invoked.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke after the usr has given the permission.</param>
        /// <param name="errorCallback">The callback to invoke if the user has not given the permission after some time.</param>
        public static void RequestUsagePermission(Action successCallback, Action<Exception> errorCallback)
        {
            RequestUsagePermission(null, null, false, 0, successCallback, errorCallback);
        }

        /// <summary>
        /// Opens the usage statistics system settings page so that the user can give access to it.
        /// </summary>
        /// <remarks>
        /// When the user hsd given access and adjoe has processed that information, <c>successCallback</c> will be invoked.
        /// If the user hasn't given access after a certain amount of time, <c>errorCallback</c> will be invoked.
        /// </remarks>
        /// <param name="bringBackAfterAccept">Whether to bring your app to the foreground again after the user has given access.</param>
        /// <param name="successCallback">The callback to invoke after the usr has given the permission.</param>
        /// <param name="errorCallback">The callback to invoke if the user has not given the permission after some time.</param>
        public static void RequestUsagePermission(bool bringBackAfterAccept, Action successCallback, Action<Exception> errorCallback)
        {
            RequestUsagePermission(null, null, bringBackAfterAccept, 0, successCallback, errorCallback);
        }

        /// <summary>
        /// Opens the usage statistics system settings page so that the user can give access to it.
        /// </summary>
        /// <remarks>
        /// When the user hsd given access and adjoe has processed that information, <c>successCallback</c> will be invoked.
        /// If the user hasn't given access after a certain amount of time, <c>errorCallback</c> will be invoked.
        /// </remarks>
        /// <param name="bringBackAfterAccept">Whether to bring your app to the foreground again after the user has given access.</param>
        /// <param name="ticks">How long to wait before invoking the error callback (1 tick = 500ms).</param>
        /// <param name="successCallback">The callback to invoke after the usr has given the permission.</param>
        /// <param name="errorCallback">The callback to invoke if the user has not given the permission after some time.</param>
        public static void RequestUsagePermission(bool bringBackAfterAccept, int ticks, Action successCallback, Action<Exception> errorCallback)
        {
            RequestUsagePermission(null, null, bringBackAfterAccept, ticks, successCallback, errorCallback);
        }

        /// <summary>
        /// Opens the usage statistics system settings page so that the user can give access to it.
        /// </summary>
        /// <remarks>
        /// When the user hsd given access and adjoe has processed that information, <c>successCallback</c> will be invoked.
        /// If the user hasn't given access after a certain amount of time, <c>errorCallback</c> will be invoked.
        /// </remarks>
        /// <param name="uaNetwork">The user acquisition (UA) network formerly known as the first Sub-ID (optional).</param>
        /// <param name="uaChannel">The user acquisition (UA) channel formerly known as the second Sub-ID (optional).</param>
        /// <param name="bringBackAfterAccept">Whether to bring your app to the foreground again after the user has given access.</param>
        /// <param name="ticks">How long to wait before invoking the error callback (1 tick = 500ms).</param>
        /// <param name="successCallback">The callback to invoke after the usr has given the permission.</param>
        /// <param name="errorCallback">The callback to invoke if the user has not given the permission after some time.</param>
        public static void RequestUsagePermission(string uaNetwork, string uaChannel, bool bringBackAfterAccept, int ticks, Action successCallback, Action<Exception> errorCallback)
        {
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            RequestUsagePermission(adjoeParams, bringBackAfterAccept, ticks, successCallback, errorCallback);
        }

        /// <summary>
        /// Opens the usage statistics system settings page so that the user can give access to it.
        /// </summary>
        /// <remarks>
        /// When the user hsd given access and adjoe has processed that information, <c>successCallback</c> will be invoked.
        /// If the user hasn't given access after a certain amount of time, <c>errorCallback</c> will be invoked.
        /// </remarks>
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        /// <param name="bringBackAfterAccept">Whether to bring your app to the foreground again after the user has given access.</param>
        /// <param name="ticks">How long to wait before invoking the error callback (1 tick = 500ms).</param>
        /// <param name="successCallback">The callback to invoke after the usr has given the permission.</param>
        /// <param name="errorCallback">The callback to invoke if the user has not given the permission after some time.</param>
        public static void RequestUsagePermission(AdjoeParams adjoeParams, bool bringBackAfterAccept, int ticks, Action successCallback, Action<Exception> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.RequestUsagePermission(adjoeParams=" + adjoeParams + ", bringBackAfterAccept=" + bringBackAfterAccept + ", ticks=" + ticks + ", successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif
            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);

            object[] parameters = {
                GetCurrentContext(),
                javaParams,
                new UsageManagerCallback(successCallback, errorCallback),
                ticks,
                bringBackAfterAccept
            };
            adjoe.CallStatic("requestUsagePermission", parameters);
        }

        /// <summary>
        /// Accepts the adjoe Terms of Service (TOS).
        /// </summary>
        /// <remarks>
        /// When the user accepts the adjoe TOS, call this method to notify adjoe about it.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when adjoe has successfully processed this information.</param>
        /// <param name="errorCallback">The callback to invoke when adjoe couldn't process this information.</param>
        public static void SetTosAccepted(Action successCallback, Action<Exception> errorCallback)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.SetTosAccepted(successCallback=" + successCallback + ", errorCallback=" + errorCallback + ")");
                return;
            #endif

            object[] parameters = {
                GetCurrentContext(),
                new InitialisationListener(successCallback, errorCallback)
            };
            adjoe.CallStatic("setTosAccepted", parameters);
        }

        /// <summary>
        /// Sends a uer event to adjoe.
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
        /// Sends a uer event to adjoe.
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
        /// Sends a uer event to adjoe.
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
        /// Sends a uer event to adjoe.
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
            AdjoeParams adjoeParams = new AdjoeParams();
            adjoeParams.SetUaNetwork(uaNetwork);
            adjoeParams.SetUaChannel(uaChannel);
            SendUserEvent(eventId, extra, adjoeParams);
        }

        /// <summary>
        /// Sends a uer event to adjoe.
        /// </summary>
        /// <remarks>
        /// These events help to improve the accuracy of the app recommendations for the user.
        /// </remarks>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="extra">For <c>EVENT_VIDEO_PLAY</c>, <c>EVENT_VIDEO_PAUSE</c> and <c>EVENT_VIDEO_ENDED</c> this must be the application ID of the app to which the video belongs, otherwise <c>null</c>.
        /// <param name="adjoeParams">The AdjoeParams that holds the user acquisition (UA) paramaters and placement (optional).</param>
        public static void SendUserEvent(int eventId, string extra, AdjoeParams adjoeParams)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.SendUserEvent(eventId=" + eventId + ", extra=" + extra + ", adjoeParams=" + adjoeParams + ")");
                return;
            #endif

            AndroidJavaObject javaParams = GetJavaAdjoeParams(adjoeParams);

            object[] parameters = {
                GetCurrentContext(),
                eventId,
                extra,
                javaParams
            };
            adjoe.CallStatic("sendUserEvent", parameters);
        }

        /* ----------------------------------
                    UTILITY METHODS
           ---------------------------------- */

        /// <summary>
        /// Returns the version of the adjoe SDK.
        /// </summary>
        /// <returns>The version of the adjoe SDK.</returns>
        public static int GetVersion()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.GetVersion()");
                return 0;
            #endif

            return adjoe.CallStatic<int>("getVersion");
        }

        /// <summary>
        /// Returns the version name of the adjoe SDK.
        /// </summary>
        /// <returns>The version name of the adjoe SDK.</returns>
        public static string GetVersionName()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.GetVersionNAme()");
                return "";
            #endif

            return adjoe.CallStatic<string>("getVersionName");
        }

        /// <summary>
        /// Checks whether the adjoe SDK is initialized.
        /// </summary>
        /// <returns><c>true</c> when it is initialized, <c>false</c> otherwise.</returns>
        public static bool IsInitialized()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.IsInitialized()");
                return false;
            #endif

            return adjoe.CallStatic<bool>("isInitialized");
        }

        /// <summary>
        /// Checks whether the user has accepted the adjoe Terms of Service (TOS).
        /// </summary>
        /// <returns><c>true</c> when the user has accepted the TOS, <c>false</c> otherwise.</returns>
        public static bool HasAcceptedTOS()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.HasAcceptedTOS()");
                return false;
            #endif

            return adjoe.CallStatic<bool>("hasAcceptedTOS", GetCurrentContext());
        }

        /// <summary>
        /// Checks whether the user has given access to the usage statistics.
        /// </summary>
        /// <returns><c>true</c> when the user has given access, <c>false</c> otherwise.</returns>
        public static bool HasAcceptedUsagePermission()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.HasAcceptedUsagePermission()");
                return false;
            #endif

            return adjoe.CallStatic<bool>("hasAcceptedUsagePermission", GetCurrentContext());
        }

        /// <summary>
        /// Returns the unique ID of the user by which he is identified within the adjoe infrastructure.
        /// </summary>
        /// <returns>The user's unique ID.</returns>
        public static string GetUserId()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.GetUserId()");
                return null;
            #endif

            return adjoe.CallStatic<string>("getUserId", GetCurrentContext());
        }

        [Obsolete]
        public static bool CanUseOfferwallFeatures()
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.CanUseOfferwallFeatures()");
                return true;
            #endif

            return adjoe.CallStatic<bool>("canUseOfferwallFeatures", GetCurrentContext());
        }

        public static void a(bool whether)
        {
            #if UNITY_EDITOR
                Debug.Log("Called Adjoe.a(whether=" + whether + ")");
                return;
            #endif

            object[] parameters =
            {
                GetCurrentContext(),
                whether
            };
            adjoe.CallStatic("a", parameters);
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
            adjoe.CallStatic("faceVerificationStatus", parameters);
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
            adjoe.CallStatic("faceVerification", parameters);
        }

        /* ----------------------------------
                    PRIVATE METHODS
           ---------------------------------- */

        internal static AndroidJavaObject GetCurrentContext()
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }

        internal static AndroidJavaObject GetJavaAdjoeParams(AdjoeParams adjoeParams)
        {
            AndroidJavaObject builder = new AndroidJavaObject("io.adjoe.sdk.AdjoeParams$Builder");
                if (adjoeParams != null)
                {
                    builder.Call<AndroidJavaObject>("setUaNetwork", new object[] {adjoeParams.uaNetwork});
                    builder.Call<AndroidJavaObject>("setUaChannel", new object[] {adjoeParams.uaChannel});
                    builder.Call<AndroidJavaObject>("setUaSubPublisherCleartext", new object[] {adjoeParams.uaSubPublisherCleartext});
                    builder.Call<AndroidJavaObject>("setUaSubPublisherEncrypted", new object[] {adjoeParams.uaSubPublisherEncrypted});
                    builder.Call<AndroidJavaObject>("setPlacement", new object[] {adjoeParams.placement});
                }
            AndroidJavaObject javaAdjoeParams = builder.Call<AndroidJavaObject>("build");
            return javaAdjoeParams;

        }

        internal static AndroidJavaObject GetJavaAdjoeExtensions(AdjoeExtensions adjoeExtensions)
        {
            AndroidJavaObject builder = new AndroidJavaObject("io.adjoe.sdk.AdjoeExtensions$Builder");
            if (adjoeExtensions != null){
                builder.Call<AndroidJavaObject>("setSubId1", new object[] {adjoeExtensions.subId1});
                builder.Call<AndroidJavaObject>("setSubId2", new object[] {adjoeExtensions.subId2});
                builder.Call<AndroidJavaObject>("setSubId3", new object[] {adjoeExtensions.subId3});
                builder.Call<AndroidJavaObject>("setSubId4", new object[] {adjoeExtensions.subId4});
                builder.Call<AndroidJavaObject>("setSubId5", new object[] {adjoeExtensions.subId5});
            }
            AndroidJavaObject javaAdjoeExtensions = builder.Call<AndroidJavaObject>("build");
            return javaAdjoeExtensions;

        }

        internal static AndroidJavaObject GetJavaAdjoeUserProfile (AdjoeUserProfile profile){

            long timestamp = (long) profile.birthday.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            AndroidJavaObject javaBirthday = new AndroidJavaObject("java.util.Date", timestamp);
            AndroidJavaObject javaGender = null;
            switch (profile.gender)
            {
                case AdjoeGender.MALE:
                    javaGender = new AndroidJavaClass("io.adjoe.sdk.AdjoeGender").GetStatic<AndroidJavaObject>("MALE");
                    break;

                case AdjoeGender.FEMALE:
                    javaGender = new AndroidJavaClass("io.adjoe.sdk.AdjoeGender").GetStatic<AndroidJavaObject>("FEMALE");
                    break;

                case AdjoeGender.UNKNOWN:
                default:
                    javaGender = new AndroidJavaClass("io.adjoe.sdk.AdjoeGender").GetStatic<AndroidJavaObject>("UNKNOWN");
                    break;
            }

            AndroidJavaObject adjoeUserProfile = new AndroidJavaObject("io.adjoe.sdk.AdjoeUserProfile", new object[] {javaGender, javaBirthday});

            return adjoeUserProfile;
        }

        /* ----------------------------------
                       LISTENERS
           ---------------------------------- */
        private class InitialisationListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public InitialisationListener(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.AdjoeInitialisationListener")
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
            private readonly Action<AdjoeRewardResponse> successCallback;
            private readonly Action<AdjoeRewardResponseError> errorCallback;

            public RewardListener(Action<AdjoeRewardResponse> successCallback, Action<AdjoeRewardResponseError> errorCallback) : base("io.adjoe.sdk.AdjoeRewardListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onUserReceivesReward(AndroidJavaObject data) // data has class io.adjoe.sdk.AdjoeRewardResponse
            {
                if (successCallback == null || data == null)
                {
                    return;
                }

                AdjoeRewardResponse response = new AdjoeRewardResponse();

                // use getters because the fields might be obfuscated
                response.Reward = data.Call<int>("getReward");
                response.AvailablePayoutCoins = data.Call<int>("getAvailablePayoutCoins");
                response.AlreadySpentCoins = data.Call<int>("getAlreadySpentCoins");

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

            public void onUserReceivesRewardError(AndroidJavaObject data) // data has class io.adjoe.sdk.AdjoeRewardResponseError
            {
                if (errorCallback == null || data == null)
                {
                    return;
                }

                AdjoeRewardResponseError error = new AdjoeRewardResponseError();
                if (data.Call<AndroidJavaObject>("getException") != null)
                {
                    error.Exception = new Exception(data.Call<AndroidJavaObject>("getException").Call<string>("getMessage"));
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
            private readonly Action<AdjoePayoutResponse> successCallback;
            private readonly Action<AdjoePayoutError> errorCallback;

            public PayoutListener(Action<AdjoePayoutResponse> successCallback, Action<AdjoePayoutError> errorCallback) : base("io.adjoe.sdk.AdjoePayoutListener")
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

                AdjoePayoutResponse response = new AdjoePayoutResponse();
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

            public void onPayoutError(AndroidJavaObject data) // data has class io.adjoe.sdk.AdjoePayoutError
            {
                if (errorCallback == null || data == null)
                {
                    return;
                }

                AdjoePayoutError error = new AdjoePayoutError();
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

        private class UsageManagerCallback : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action<Exception> errorCallback;

            public UsageManagerCallback(Action successCallback, Action<Exception> errorCallback) : base("io.adjoe.sdk.AdjoeUsageManagerCallback")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
            }

            public void onUsagePermissionAccepted()
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

            public void onUsagePermissionError(AndroidJavaObject excetpion)
            {
                if (errorCallback == null)
                {
                    return;
                }

                if (useLegacyCallbacks)
                {
                    if (excetpion != null)
                    {
                        errorCallback(new Exception(excetpion.Call<string>("getMessage")));
                    }
                    else
                    {
                        errorCallback(new Exception("Could not request usage permission"));
                    }
                }
                else
                {
                    if (excetpion != null)
                    {
                        Dispatcher.RunOnMainThread(() => {
                            errorCallback(new Exception(excetpion.Call<string>("getMessage")));
                        });
                    }
                    else
                    {
                        Dispatcher.RunOnMainThread(() => {
                            errorCallback(new Exception("Could not request usage permission"));
                        });
                    }
                }

            }
        }

        private class CampaignListener : AndroidJavaProxy
        {
            private readonly Action<AdjoeCampaignResponse> successCallback;
            private readonly Action<AdjoeCampaignResponseError> errorCallback;

            public CampaignListener(Action<AdjoeCampaignResponse> successCallback, Action<AdjoeCampaignResponseError> errorCallback) : base("io.adjoe.sdk.AdjoeCampaignListener")
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

                AndroidJavaObject javaPartnerApps = response.Call<AndroidJavaObject>("getPartnerApps");
                int size = javaPartnerApps.Call<int>("size");
                ArrayList partnerApps = new ArrayList(size);
                for (int i = 0; i < size; i++)
                {
                    AndroidJavaObject javaPartnerApp = javaPartnerApps.Call<AndroidJavaObject>("get", i);
                    AdjoePartnerApp partnerApp = new AdjoePartnerApp(javaPartnerApp);
                    partnerApps.Add(partnerApp);
                }
                AdjoeCampaignResponse campaignResponse = new AdjoeCampaignResponse
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
                AndroidJavaObject exception = response.Call<AndroidJavaObject>("getException");
                AdjoeCampaignResponseError campaignsError = new AdjoeCampaignResponseError();
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

        private class OfferwallListener : AndroidJavaProxy
        {
            private readonly Action<string> openedCallback;
            private readonly Action<string> closedCallback;

            public OfferwallListener(Action<string> openedCallback, Action<string> closedCallback) : base("io.adjoe.sdk.AdjoeOfferwallListener")
            {
                this.openedCallback = openedCallback;
                this.closedCallback = closedCallback;
            }

            public void onOfferwallOpened(string type)
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

            public void onOfferwallClosed(string type)
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
                Action<Exception> errorCallback) : base("io.adjoe.sdk.Adjoe$FaceVerificationCallback")
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
                Action<Exception> errorCallback) : base("io.adjoe.sdk.Adjoe$FaceVerificationStatusCallback")
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
