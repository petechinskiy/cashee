using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Cashback reward info.
    /// </summary>
    [System.Serializable]
    public class PlaytimeCashbackReward
    {
        [SerializeField] private int totalCoins;
        [SerializeField] private PlaytimeCashbackRewardEvent[] events;

        /// <summary>
        /// Total amount of coins for given rewarded events group.
        /// </summary>
        public int? TotalCoins => totalCoins;
        
        /// <summary>
        /// Events in the reward group.
        /// </summary>
        public PlaytimeCashbackRewardEvent[] Events => events;

        public PlaytimeCashbackReward(int? totalCoins, PlaytimeCashbackRewardEvent[] events)
        {
            this.totalCoins = totalCoins ?? 0;
            this.events = events;
        }

        public PlaytimeCashbackReward(AndroidJavaObject javaObject)
        {
            AndroidJavaObject javaTotalCoins = javaObject.Call<AndroidJavaObject>("getTotalCoins");
            AndroidJavaObject javaEvents = javaObject.Call<AndroidJavaObject>("getEvents");
            int size = javaEvents.Call<int>("size");
            PlaytimeCashbackRewardEvent[] events = new PlaytimeCashbackRewardEvent[size];

            if (javaTotalCoins != null)
            {
                this.totalCoins = javaTotalCoins.Call<int>("intValue");
            }

            for (int i = 0; i < size; i++)
            {
                AndroidJavaObject javaEvent = javaEvents.Call<AndroidJavaObject>("get", i);
                PlaytimeCashbackRewardEvent currentEvent = new PlaytimeCashbackRewardEvent(javaEvent);
                events[i] = currentEvent;
            }

            this.events = events;
        }
    }
} 
