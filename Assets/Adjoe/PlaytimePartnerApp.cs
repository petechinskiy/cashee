using System;
using System.Collections;
using UnityEngine;

namespace io.adjoe.sdk
{
    /// <summary>
    /// This class represents an entry from adjoe's offerwall.
    /// </summary>
    public class PlaytimePartnerApp
    {
        private AndroidJavaObject partnerApp;

        internal PlaytimePartnerApp(AndroidJavaObject partnerApp)
        {
            this.partnerApp = partnerApp;
        }

        /// <summary>
        /// Returns the package name (also known as application ID) of this partner app.
        /// </summary>
        /// <returns>The package name.</returns>
        public string GetPackageName()
        {
            return partnerApp.Call<String>("getPackageName");
        }

        /// <summary>
        /// Returns the display name of this partner app.
        /// </summary>
        /// <returns>The display name.</returns>
        public string GetName()
        {
            return partnerApp.Call<string>("getName");
        }

        /// <summary>
        /// Returns a description of this app, similar to that in the Google Play Store. It has a maximum length of 135 characters.
        /// </summary>
        /// <returns>The description.</returns>
        public string GetDescription()
        {
            return partnerApp.Call<string>("getDescription");
        }

        /// <summary>
        /// Returns an URL from where you can load the icon for this app.
        /// </summary>
        /// <returns>The URL for the icon.</returns>
        public string GetIconURL()
        {
            return partnerApp.Call<string>("getIconURL");
        }

        /// <summary>
        /// Returns an URL from where you can load a large preview image (in landscape orientation) of this app.
        /// </summary>
        /// <returns>The URL for the preview image.</returns>
        public string GetLandscapeImageURL()
        {
            return partnerApp.Call<string>("getLandscapeImageURL");
        }

        /// <summary>
        /// Returns the number of coins paid out for this PIR app for campaign events.
        /// </summary>
        /// <returns>the number of coins paid out for this PIR app for campaign events.</returns>
        public int GetPostInstallRewardCoins()
        {
            return partnerApp.Call<int>("getPostInstallEventRewardCoins");
        }

        /// <summary>
        /// Returns an URL from where you can load/stream the preview video of this app.
        /// </summary>
        /// <remarks>
        /// Note that this is optional and might be empty.
        /// </remarks>
        /// <returns>The URL to load the preview video.</returns>
        public string GetVideoURL()
        {
            return partnerApp.Call<string>("getVideoURL");
        }

        /// <summary>
        /// Returns  the date when this app was marked as installed via adjoe or <c>null</c> if it is not installed.
        /// </summary>
        /// <returns>The installation date.</returns>
        public DateTime? GetInstallDate()
        {
            AndroidJavaObject javaInstallData = partnerApp.Call<AndroidJavaObject>("getInstallDate");
            if (javaInstallData == null)
            {
                return null;
            }

            long installDate = javaInstallData.Call<long>("getTime");
            return new DateTime(installDate);
        }

        /// <summary>
        /// Returns a list of <c>RewardLevel</c> entries describing how much the user can earn.
        /// </summary>
        /// <returns>The reward config.</returns>
        public ArrayList GetRewardConfig()
        {
            ArrayList rewardLevels = new ArrayList();
            AndroidJavaObject javaRewardLevelList = partnerApp.Call<AndroidJavaObject>("getRewardConfig");
            if (javaRewardLevelList == null)
            {
                return rewardLevels;
            }

            int size = javaRewardLevelList.Call<int>("size");
            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject javaRewardLevel = javaRewardLevelList.Call<AndroidJavaObject>("get", i);
                RewardLevel rewardLevel = new RewardLevel
                {
                    Level = javaRewardLevel.Call<int>("getLevel"),
                    Seconds = javaRewardLevel.Call<long>("getSeconds"),
                    Value = javaRewardLevel.Call<long>("getValue")
                };
                rewardLevels.Add(rewardLevel);
            }

            return rewardLevels;
        }

        /// <summary>
        /// Returns an object of <c>AppDetails</c> that contains the store values for campaign display. 
        /// </summary>
        /// <returns>The app details.</returns>
        public PlaytimeAppDetails GetAppDetails() {
            AndroidJavaObject javaAppDetails = partnerApp.Call<AndroidJavaObject>("getAppDetails");
            if (javaAppDetails != null)
            {
                return new PlaytimeAppDetails(javaAppDetails);
            }
            return null;
        }

        public int GetAdvanceRewardCoins() {
            return partnerApp.Call<int>("getAdvanceRewardCoins");
        }

        public string GetCampaignType() {
            return partnerApp.Call<string>("getCampaignType");
        }

        public PlaytimePromoEvent? GetPromoEvent()
        {
            AndroidJavaObject javaEvent = partnerApp.Call<AndroidJavaObject>("getEvent");
            if (javaEvent == null)
            {
                return null;
            }
            return new PlaytimePromoEvent(javaEvent);
        }

        public int GetAdvanceDailyLimit() {
            return partnerApp.Call<int>("getAdvanceDailyLimit");
        }

        public int GetAdvanceTotalLimit() {
            return partnerApp.Call<int>("getAdvanceTotalLimit");
        }

        public int GetAdvancePlusCoins() {
            return partnerApp.Call<int>("getAdvancePlusCoins");
        }

        public string GetAdvancePlusRewardedAction() {
            return partnerApp.Call<string>("getAdvancePlusRewardedAction");
        }

        public string GetAdvancePlusActionDescription() {
            return partnerApp.Call<string>("getAdvancePlusActionDescription");
        }

        public PlaytimeAdvancePlusConfig? GetAdvancePlusConfig()
        {
            AndroidJavaObject javaPlusConfig = partnerApp.Call<AndroidJavaObject>("getAdvancePlusConfig");
            if (javaPlusConfig != null)
            {
                return new PlaytimeAdvancePlusConfig(javaPlusConfig);
            }
            return null;
        }

        public bool IsInCoinStreakExperiment()
        {
            return partnerApp.Call<bool>("isInCoinStreakExperiment");
        }

        public PlaytimeStreakInfo? GetStreakInfo() 
        {
            AndroidJavaObject javaStreakInfo = partnerApp.Call<AndroidJavaObject>("getStreakInfo");
            if (javaStreakInfo != null)
            {
                return new PlaytimeStreakInfo(javaStreakInfo);
            }
            return null;
        }

        public int GetCoinStreakMaxCoinAmount() {
            return partnerApp.Call<int>("getCoinStreakMaxCoinAmount");
        }

        [Obsolete("Use ExecuteClick(Action, Action, Action) instead.")]
        public void ExecuteClick(Action successCallback, Action errorCallback)
        {
            ExecuteClick(null, null, successCallback, errorCallback, null);
        }

        [Obsolete("Use ExecuteClick(string, string, Action, Action, Action) instead.")]
        public void ExecuteClick(string subId1, string subId2, Action successCallback, Action errorCallback)
        {
            object[] parameters =
            {
                Playtime.GetCurrentContext(),
                Playtime.webViewContainer,
                subId1,
                subId2,
                new ClickListener(successCallback, errorCallback, null),
            };
            partnerApp.Call("executeClick", parameters);
        }

        /// <summary>
        /// Call this method when the user wants to install this app.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the click is successful and the user is redirected to the Google Play Store.</param>
        /// <param name="errorCallback">The callback to invoke when the click has failed.</param>
        /// <param name="alreadyClickingCallback">The callback to invoke when you call <c>ExecuteClick</c> again before the last call to <c>ExecuteClick</c> has finished.</param>
        public void ExecuteClick(Action successCallback, Action errorCallback, Action alreadyClickingCallback)
        {
            ExecuteClick(null, null, successCallback, errorCallback, alreadyClickingCallback);
        }

        /// <summary>
        /// Call this method when the user wants to install this app.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="subId1">The first Sub-ID (optional).</param>
        /// <param name="subId2">The second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the click is successful and the user is redirected to the Google Play Store.</param>
        /// <param name="errorCallback">The callback to invoke when the click has failed.</param>
        /// <param name="alreadyClickingCallback">The callback to invoke when you call <c>ExecuteClick</c> again before the last call to <c>ExecuteClick</c> has finished.</param>
        public void ExecuteClick(string subId1, string subId2, Action successCallback, Action errorCallback,
            Action alreadyClickingCallback)
        {
            object[] parameters =
            {
                Playtime.GetCurrentContext(),
                Playtime.webViewContainer,
                subId1,
                subId2,
                new ClickListener(successCallback, errorCallback, alreadyClickingCallback),
            };
            partnerApp.Call("executeClick", parameters);
        }

        [Obsolete("Use ExecuteView(Action, Action, Action) instead.")]
        public void ExecuteView(Action successCallback, Action errorCallback)
        {
            ExecuteView(null, null, successCallback, errorCallback, null);
        }

        [Obsolete("Use ExecuteView(string, string, Action, Action, Action) instead.")]
        public void ExecuteView(string subId1, string subId2, Action successCallback, Action errorCallback)
        {
            object[] parameters =
            {
                Playtime.GetCurrentContext(),
                Playtime.webViewContainer,
                subId1,
                subId2,
                new ViewListener(successCallback, errorCallback, null)
            };
            partnerApp.Call("executeView", parameters);
        }

        /// <summary>
        /// Call this method when the user views this app's listing in your app.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="successCallback">The callback to invoke when the view event has been processed by adjoe.</param>
        /// <param name="errorCallback">The callback to invoke when an error occurs while the view event is being processed by adjoe.</param>
        /// <param name="alreadyClickingCallback">The callback to invoke when you call <c>ExecuteView</c> again before the last call to <c>ExecuteView</c> has finished.</param>
        public void ExecuteView(Action successCallback, Action errorCallback, Action alreadyViewingCallback)
        {
            ExecuteView(null, null, successCallback, errorCallback, alreadyViewingCallback);
        }

        /// <summary>
        /// Call this method when the user views this app's listing in your app.
        /// </summary>
        /// <remarks>
        /// You need to call <c>Playtime.InitWebView()</c> before.
        /// </remarks>
        /// <param name="subId1">The first Sub-ID (optional).</param>
        /// <param name="subId2">The second Sub-ID (optional).</param>
        /// <param name="successCallback">The callback to invoke when the view event has been processed by adjoe.</param>
        /// <param name="errorCallback">The callback to invoke when an error occurs while the view event is being processed by adjoe.</param>
        /// <param name="alreadyClickingCallback">The callback to invoke when you call <c>ExecuteView</c> again before the last call to <c>ExecuteView</c> has finished.</param>
        public void ExecuteView(string subId1, string subId2, Action successCallback, Action errorCallback,
            Action alreadyViewingCallback)
        {
            object[] parameters =
            {
                Playtime.GetCurrentContext(),
                Playtime.webViewContainer,
                subId1,
                subId2,
                new ViewListener(successCallback, errorCallback, alreadyViewingCallback)
            };
            partnerApp.Call("executeView", parameters);
        }

        /// <summary>
        /// Returns the time (in milliseconds) until the user will reach the next reward or <c>-1</c> if either an error occurred or the user has already reached the maximum reward for this app.
        /// </summary>
        /// <returns>The remaining time until the user reaches the next reward or <c>-1</c>.</returns>
        public long GetRemainingUntilNextReward()
        {
            return partnerApp.Call<long>("getRemainingUntilNextReward", Playtime.GetCurrentContext());
        }

        /// <summary>
        /// Returns the next reward level which the user will reach or <c>null</c> if either an error occurred or the user has already reached the maximum reward level.
        /// </summary>
        /// <returns>The next reward level which the user will reach or <c>null</c>.</returns>
        public RewardLevel GetNextRewardLevel()
        {
            AndroidJavaObject javaRewardLevel =
                partnerApp.Call<AndroidJavaObject>("getNextRewardLevel", Playtime.GetCurrentContext());
            if (javaRewardLevel == null)
            {
                return null;
            }

            RewardLevel rewardLevel = new RewardLevel
            {
                Level = javaRewardLevel.Call<int>("getLevel"),
                Seconds = javaRewardLevel.Call<long>("getSeconds"),
                Value = javaRewardLevel.Call<long>("getValue")
            };
            return rewardLevel;
        }

        /// <summary>
        /// Represents a level of reward that the user can achieve.
        /// </summary>
        public class RewardLevel
        {
            /// <summary>
            /// The level number of this reward level.
            /// </summary>
            /// <remarks>
            /// This value may be used as a sort key and unique identifier among all reward levels of one <c>PlaytimePartnerApp</c>.
            /// </remarks>
            public int Level { get; set; }

            /// <summary>
            /// The amount of seconds that the user has to play the <c>PlaytimePartnerApp</c> to get this level's reward.
            /// </summary>
            /// <remarks>
            /// This value is not cumulative.
            /// </remarks>
            public long Seconds { get; set; }

            /// <summary>
            /// The reward which the user gets when he reaches this reward level.
            /// </summary>
            public long Value { get; set; }
        }

        private class ClickListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action errorCallback;
            private readonly Action alreadyClickingCallback;

            public ClickListener(Action successCallback, Action errorCallback, Action alreadyClickingCallback) : base(
                "io.adjoe.sdk.PlaytimePartnerApp$ClickListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
                this.alreadyClickingCallback = alreadyClickingCallback;
            }

            public void onFinished()
            {
                if (successCallback != null)
                {
                    successCallback();
                }
            }

            public void onError()
            {
                if (errorCallback != null)
                {
                    errorCallback();
                }
            }

            public void onAlreadyClicking()
            {
                if (alreadyClickingCallback != null)
                {
                    alreadyClickingCallback();
                }
            }
        }

        private class ViewListener : AndroidJavaProxy
        {
            private readonly Action successCallback;
            private readonly Action errorCallback;
            private readonly Action alreadyViewingCallback;

            public ViewListener(Action successCallback, Action errorCallback, Action alreadyViewingCallback) : base(
                "io.adjoe.sdk.PlaytimePartnerApp$ViewListener")
            {
                this.successCallback = successCallback;
                this.errorCallback = errorCallback;
                this.alreadyViewingCallback = alreadyViewingCallback;
            }

            public void onFinished()
            {
                if (successCallback != null)
                {
                    successCallback();
                }
            }

            public void onError()
            {
                if (errorCallback != null)
                {
                    errorCallback();
                }
            }

            public void onAlreadyViewing()
            {
                if (alreadyViewingCallback != null)
                {
                    alreadyViewingCallback();
                }
            }
        }
    }
}