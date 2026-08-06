using System;
using UnityEngine;

namespace io.adjoe.sdk
{
    /// <summary>
    /// The model that represents the status of SDK
    /// </summary>
    [System.Serializable]
    public class PlaytimeStatus
    {
        [SerializeField] private bool isInitialized;
        [SerializeField] private PlaytimeStatusDetails details;

        /// <summary>
        /// Flag that shows if SDK is initialized or not
        /// </summary>
        public bool IsInitialized => isInitialized;

        /// <summary>
        /// Shows details of the SDK
        /// </summary>
        public PlaytimeStatusDetails Details => details;

        public PlaytimeStatus(bool isInitialized, PlaytimeStatusDetails details)
        {
            this.isInitialized = isInitialized;
            this.details = details;
        }

        #if UNITY_ANDROID
        internal PlaytimeStatus(AndroidJavaObject status) {
            this.isInitialized = status.Call<bool>("isInitialized");

            AndroidJavaObject statusDetails = status.Call<AndroidJavaObject>("getDetails");
            this.details = new PlaytimeStatusDetails(statusDetails);
        }
        #endif
    }
} 