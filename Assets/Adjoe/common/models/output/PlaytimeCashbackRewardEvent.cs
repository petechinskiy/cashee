using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Cashback event.
    /// </summary>
    [System.Serializable]
    public class PlaytimeCashbackRewardEvent
    {
        [SerializeField] private int coins;
        [SerializeField] private string processAt;
        [SerializeField] private string receivedAt;

        /// <summary>
        /// Amount of coins granted for the event.
        /// </summary>
        public int? Coins => coins;
        
        /// <summary>
        /// Timestamp (ISO 8601) when the event is processed.
        /// </summary>
        public string? ProcessAt => processAt;
        
        /// <summary>
        /// Timestamp (ISO 8601) when the event is received.
        /// </summary>
        public string? ReceivedAt => receivedAt;

        public PlaytimeCashbackRewardEvent(int? coins, string? processAt, string? receivedAt)
        {
            this.coins = coins ?? 0;
            this.processAt = processAt;
            this.receivedAt = receivedAt;
        }

        public PlaytimeCashbackRewardEvent(AndroidJavaObject javaObject)
        {
            this.processAt = javaObject.Call<string?>("getProcessAt");
            this.receivedAt = javaObject.Call<string?>("getReceivedAt");

            AndroidJavaObject javaCoins = javaObject.Call<AndroidJavaObject>("getCoins");

            if (javaCoins != null) {
                this.coins = javaCoins.Call<int>("intValue");
            }
        }
    }
} 
