using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Configuration for the Playtime Cashback feature.
    /// </summary>
    [System.Serializable]
    public class PlaytimeCashbackConfig
    {
        [SerializeField] private float exchangeRate;
        [SerializeField] private float maxLimitPerCampaignCoins;
        [SerializeField] private float maxLimitPerCampaignUSD;
        [SerializeField] private string cashbackDescription;
        [SerializeField] private PlaytimeCashbackReward completedRewards;
        [SerializeField] private PlaytimeCashbackReward pendingRewards;

        /// <summary>
        /// The exchange rate between playtime and rewarded coins.
        /// </summary>
        public float? ExchangeRate => exchangeRate;
        
        /// <summary>
        /// The maximum number of coins a user can earn from cashback within a single campaign.
        /// </summary>
        public float? MaxLimitPerCampaignCoins => maxLimitPerCampaignCoins;
        
        /// <summary>
        /// The maximum USD-equivalent limit for cashback rewards in a single campaign.
        /// </summary>
        public float? MaxLimitPerCampaignUsd => maxLimitPerCampaignUSD;
        
        /// <summary>
        /// Description of the cashback reward.
        /// </summary>
        public string? CashbackDescription => cashbackDescription;
        
        /// <summary>
        /// Info on completed cashback rewards.
        /// </summary>
        public PlaytimeCashbackReward? CompletedRewards => completedRewards;
        
        /// <summary>
        /// Info on pending cashback rewards.
        /// </summary>
        public PlaytimeCashbackReward? PendingRewards => pendingRewards;

        public PlaytimeCashbackConfig(float? exchangeRate, float? maxLimitPerCampaignCoins, float? maxLimitPerCampaignUSD,
                                     string? cashbackDescription, PlaytimeCashbackReward? completedRewards, PlaytimeCashbackReward? pendingRewards)
        {
            this.exchangeRate = exchangeRate ?? 0f;
            this.maxLimitPerCampaignCoins = maxLimitPerCampaignCoins ?? 0f;
            this.maxLimitPerCampaignUSD = maxLimitPerCampaignUSD ?? 0f;
            this.cashbackDescription = cashbackDescription;
            this.completedRewards = completedRewards;
            this.pendingRewards = pendingRewards;
        }

        public PlaytimeCashbackConfig(AndroidJavaObject javaObject)
        {
            this.cashbackDescription = javaObject.Call<string?>("getCashbackDescription");

            AndroidJavaObject completedRewardsJava = javaObject.Call<AndroidJavaObject>("getCompletedRewards");
            AndroidJavaObject pendingRewardsJava = javaObject.Call<AndroidJavaObject>("getPendingRewards");
            AndroidJavaObject exchangeRateJava = javaObject.Call<AndroidJavaObject>("getExchangeRate");
            AndroidJavaObject maxLimitPerCampaignCoinsJava = javaObject.Call<AndroidJavaObject>("getMaxLimitPerCampaignCoins");
            AndroidJavaObject maxLimitPerCampaignUsdJava = javaObject.Call<AndroidJavaObject>("getMaxLimitPerCampaignUSD");

            if (completedRewardsJava != null)
            {
                this.completedRewards = new PlaytimeCashbackReward(completedRewardsJava);
            }

            if (pendingRewardsJava != null)
            {
                this.pendingRewards = new PlaytimeCashbackReward(pendingRewardsJava);
            }

            if (exchangeRateJava != null)
            {
                this.exchangeRate = exchangeRateJava.Call<float>("floatValue");
            }

            if (maxLimitPerCampaignCoinsJava != null)
            {
                this.maxLimitPerCampaignCoins = maxLimitPerCampaignCoinsJava.Call<float>("floatValue");
            }

            if (maxLimitPerCampaignUsdJava != null)
            {
                this.maxLimitPerCampaignUSD = maxLimitPerCampaignUsdJava.Call<float>("floatValue");
            }
        }
    }
} 
