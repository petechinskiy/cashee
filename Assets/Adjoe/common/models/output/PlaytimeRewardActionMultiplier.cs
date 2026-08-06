using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Configuration for a multiplier.
    /// </summary>
    [System.Serializable]
    public class PlaytimeRewardActionMultiplier
    {
        [SerializeField] private string eventName;
        [SerializeField] private string eventDescription;
        [SerializeField] private int multiplierFactorPercentage;
        [SerializeField] private int multiplierLevels;
        [SerializeField] private string status;
        [SerializeField] private int usedLevels;

        /// <summary>
        /// Name of the event
        /// </summary>
        public string? EventName => eventName;
        
        /// <summary>
        /// Description of the event
        /// </summary>
        public string? EventDescription => eventDescription;
        
        /// <summary>
        /// Multiplication factor of the coin for the multiplier event. value: 0-100
        /// </summary>
        public int? MultiplierFactorPercentage => multiplierFactorPercentage;
        
        /// <summary>
        /// Maximum number of levels that can be multiplied by the event
        /// </summary>
        public int? MultiplierLevels => multiplierLevels;
        
        /// <summary>
        /// Status of the multiplied event, possible values: Pending, Active, Finished
        /// </summary>
        public string? Status => status;

        public PlaytimeRewardActionMultiplier(string? eventName, string? eventDescription, int? multiplierFactorPercentage,
                                   int? multiplierLevels, string? status)
        {
            this.eventName = eventName;
            this.eventDescription = eventDescription;
            this.multiplierFactorPercentage = multiplierFactorPercentage ?? 0;
            this.multiplierLevels = multiplierLevels ?? 0;
            this.status = status;
        }

        public PlaytimeRewardActionMultiplier(AndroidJavaObject actionMultiplier) 
        {
            this.eventName = actionMultiplier.Call<string?>("getEventName");
            this.eventDescription = actionMultiplier.Call<string?>("getEventDescription");
            this.status = actionMultiplier.Call<string?>("getStatus");

            AndroidJavaObject javaMultiplierFactorPercentage 
                = actionMultiplier.Call<AndroidJavaObject>("getMultiplierFactorPercentage");
            AndroidJavaObject javaMultiplierLevels
                = actionMultiplier.Call<AndroidJavaObject>("getMultiplierLevels");

            if (javaMultiplierFactorPercentage != null)
            {
                this.multiplierFactorPercentage = javaMultiplierFactorPercentage.Call<int>("intValue");
            }

            if (javaMultiplierLevels != null)
            {
                this.multiplierLevels = javaMultiplierLevels.Call<int>("intValue");
            }
        }
    }
} 
