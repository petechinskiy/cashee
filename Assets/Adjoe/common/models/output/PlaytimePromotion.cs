using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Promotion representation.
    /// </summary>
    [System.Serializable]
    public class PlaytimePromotion
    {
        [SerializeField] private string name;
        [SerializeField] private string promotionDescription;
        [SerializeField] private float boostFactor;
        [SerializeField] private string startTime;
        [SerializeField] private string endTime;
        [SerializeField] private string targetingType;

        /// <summary>
        /// Promotion name.
        /// </summary>
        public string? Name => name;
        
        /// <summary>
        /// Description of the promotion.
        /// </summary>
        public string? PromotionDescription => promotionDescription;
        
        /// <summary>
        /// Boost multiplier for the promotion.
        /// </summary>
        public float? BoostFactor => boostFactor;
        
        /// <summary>
        /// Promotion start timestamp (ISO 8601).
        /// </summary>
        public string? StartTime => startTime;
        
        /// <summary>
        /// Promotion end timestamp (ISO 8601).
        /// </summary>
        public string? EndTime => endTime;
        
        /// <summary>
        /// Targeting type for promotion.
        /// </summary>
        public string? TargetingType => targetingType;

        public PlaytimePromotion(string? name, string? promotionDescription, float? boostFactor,
                                string? startTime, string? endTime, string? targetingType)
        {
            this.name = name;
            this.promotionDescription = promotionDescription;
            this.boostFactor = boostFactor ?? 0f;
            this.startTime = startTime;
            this.endTime = endTime;
            this.targetingType = targetingType;
        }

        public PlaytimePromotion(AndroidJavaObject javaObject) 
        {
            this.name = javaObject.Call<string?>("getName");
            this.promotionDescription = javaObject.Call<string?>("getPromotionDescription");
            this.startTime = javaObject.Call<string?>("getStartTime");
            this.endTime = javaObject.Call<string?>("getEndTime");
            this.targetingType = javaObject.Call<string?>("getTargetingType");

            AndroidJavaObject boostFactorJava = javaObject.Call<AndroidJavaObject>("getBoostFactor");

            if (boostFactorJava != null) {
                this.boostFactor = boostFactorJava.Call<float>("floatValue");
            }
        }
    }
} 
