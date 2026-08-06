using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Rewarded action representation.
    /// </summary>
    [System.Serializable]
    public class PlaytimeRewardAction
    {
        [SerializeField] private string name;
        [SerializeField] private string taskDescription;
        [SerializeField] private string taskType;
        [SerializeField] private int playDuration; // android only
        [SerializeField] private int amount;
        [SerializeField] private string rewardedAt;
        [SerializeField] private int rewardsCount;
        [SerializeField] private int completedRewards;
        [SerializeField] private int timedCoinsDuration;
        [SerializeField] private int timedCoins;
        [SerializeField] private int originalCoins;
        [SerializeField] private bool isTimed;
        [SerializeField] private bool isRewardedForPromotion;
        [SerializeField] private string boosterExpiresAt;
        [SerializeField] private int level;

        /// <summary>
        /// Reward action name.
        /// </summary>
        public string? Name => name;
        
        /// <summary>
        /// Description of the task.
        /// </summary>
        public string? TaskDescription => taskDescription;
        
        /// <summary>
        /// Type of task to complete.
        /// Possible values: sequential, bonus, playtime.
        /// </summary>
        public string TaskType => taskType;
        
        /// <summary>
        /// Duration (in seconds) of play time required to reward a time-based event. Currently not supported on iOS.
        /// </summary>
        public int? PlayDuration => playDuration;
        
        /// <summary>
        /// The level number of the time-based event. Currently not supported on iOS.
        /// </summary>
        public int? Level => level;
        
        /// <summary>
        /// The amount of coins or rewards the user will receive upon completing the event.
        /// </summary>
        public int Amount => amount;
        
        /// <summary>
        /// Timestamp when a time-based reward was granted (ISO 8601).
        /// </summary>
        public string? RewardedAt => rewardedAt;
        
        /// <summary>
        /// Number of times that the action can be rewarded, applicable to repetitive events.
        /// </summary>
        public int? RewardsCount => rewardsCount;
        
        /// <summary>
        /// Number of times that the action has been rewarded, applicable to repetitive events.
        /// </summary>
        public int? CompletedRewards => completedRewards;
        
        /// <summary>
        /// Time window (in minutes) during which the Booster reward is applicable.
        /// </summary>
        public int? TimedCoinsDuration => timedCoinsDuration;
        
        /// <summary>
        /// Amount of coins rewarded during the booster period.
        /// </summary>
        public int? TimedCoins => timedCoins;
        
        /// <summary>
        /// The amount of coins or rewards the user would receive if there was no promotion.
        /// </summary>
        public int? OriginalCoins => originalCoins;
        
        /// <summary>
        /// Flag indicating whether the event is a booster event.
        /// </summary>
        public bool? IsTimed => isTimed;
        
        /// <summary>
        /// Flag indicating whether the event has been rewarded with promotion.
        /// </summary>
        public bool? IsRewardedForPromotion => isRewardedForPromotion;
        
        /// <summary>
        /// The timestamp (ISO 8601) indicating when the booster reward expires.
        /// Use it to determine user eligibility for booster rewards and support features such as booster countdown.
        /// </summary>
        public string? BoosterExpiresAt => boosterExpiresAt;

        public PlaytimeRewardAction(string? name, string? taskDescription, string taskType, int? playDuration, int? level, int amount,
                                   string? rewardedAt, int? rewardsCount, int? completedRewards, int? timedCoinsDuration,
                                   int? timedCoins, int? originalCoins, bool? isTimed, bool? isRewardedForPromotion, string? boosterExpiresAt)
        {
            this.name = name;
            this.taskDescription = taskDescription;
            this.taskType = taskType;
            this.playDuration = playDuration ?? 0;
            this.amount = amount;
            this.level = level ?? 0;
            this.rewardedAt = rewardedAt;
            this.rewardsCount = rewardsCount ?? 0;
            this.completedRewards = completedRewards ?? 0;
            this.timedCoinsDuration = timedCoinsDuration ?? 0;
            this.timedCoins = timedCoins ?? 0;
            this.originalCoins = originalCoins ?? 0;
            this.isTimed = isTimed ?? false;
            this.isRewardedForPromotion = isRewardedForPromotion ?? false;
            this.boosterExpiresAt = boosterExpiresAt;
        }

        public PlaytimeRewardAction(AndroidJavaObject rewardAction) 
        {
            this.name = rewardAction.Call<string>("getName");
            this.taskDescription = rewardAction.Call<string>("getTaskDescription");
            this.taskType = rewardAction.Call<string>("getTaskType");
            this.rewardedAt = rewardAction.Call<string>("getRewardedAt");
            this.boosterExpiresAt = rewardAction.Call<string>("getBoosterExpiresAt");
            this.amount = rewardAction.Call<int>("getAmount");

            AndroidJavaObject levelJava = rewardAction.Call<AndroidJavaObject>("getLevel");
            AndroidJavaObject playDurationJava = rewardAction.Call<AndroidJavaObject>("getPlayDuration");
            AndroidJavaObject rewardsCountJava = rewardAction.Call<AndroidJavaObject>("getRewardsCount");
            AndroidJavaObject completedRewardsJava = rewardAction.Call<AndroidJavaObject>("getCompletedRewards");
            AndroidJavaObject timedCoinsDurationJava = rewardAction.Call<AndroidJavaObject>("getTimedCoinsDuration");
            AndroidJavaObject timedCoinsJava = rewardAction.Call<AndroidJavaObject>("getTimedCoins");
            AndroidJavaObject originalCoinsJava = rewardAction.Call<AndroidJavaObject>("getOriginalCoins");
            AndroidJavaObject isTimedJava = rewardAction.Call<AndroidJavaObject>("isTimed");
            AndroidJavaObject isRewardedForPromotionJava = rewardAction.Call<AndroidJavaObject>("isRewardedForPromotion");

            if (levelJava != null) 
            {
                this.level = levelJava.Call<int>("intValue");
            }

            if (playDurationJava != null) 
            {
                this.playDuration = playDurationJava.Call<int>("intValue");
            }

            if (rewardsCountJava != null) 
            {
                this.rewardsCount = rewardsCountJava.Call<int>("intValue");
            }

            if (completedRewardsJava != null) 
            {
                this.completedRewards = completedRewardsJava.Call<int>("intValue");
            }

            if (timedCoinsDurationJava != null) 
            {
                this.timedCoinsDuration = timedCoinsDurationJava.Call<int>("intValue");
            }

            if (timedCoinsJava != null)
            {
                this.timedCoins = timedCoinsJava.Call<int>("intValue");
            }

            if (originalCoinsJava != null)
            {
                this.originalCoins = originalCoinsJava.Call<int>("intValue");
            }

            if (isTimedJava != null)
            {
                this.isTimed = isTimedJava.Call<bool>("booleanValue");
            }

            if (isRewardedForPromotionJava != null)
            {
                this.isRewardedForPromotion = isRewardedForPromotionJava.Call<bool>("booleanValue");
            }
        }
    }
} 
