using System;
using UnityEngine;

namespace io.adjoe.sdk
{
    /// <summary>
    /// The model that represents the status details of SDK
    /// </summary>
    [System.Serializable]
    public class PlaytimeStatusDetails
    {
        [SerializeField] private bool isFraud;
        [SerializeField] private bool campaignsAvailable;
        [SerializeField] private string[] campaignsState;
        [SerializeField] private int testGroup;

        /// <summary>
        /// Flag that shows if user is frauded or not
        /// </summary>
        public bool IsFraud => isFraud;

        /// <summary>
        /// Indicates whether the user is eligible to request campaigns.
        /// </summary>
        public bool CampaignsAvailable => campaignsAvailable;

        /// <summary>
        /// Provides optional context explaining the eligibility state.
        /// </summary>
        public PlaytimeCampaignsState[] CampaignsState
        {
            get
            {
                if (campaignsState == null) return null;
                var result = new PlaytimeCampaignsState[campaignsState.Length];
                for (int i = 0; i < campaignsState.Length; i++)
                {
                    result[i] = (PlaytimeCampaignsState)Enum.Parse(typeof(PlaytimeCampaignsState), campaignsState[i]);
                }
                return result;
            }
        }

        /// <summary>
        /// The test group assigned to the user by the backend, if any.
        /// </summary>
        public int? TestGroup => testGroup == -1 ? (int?)null : testGroup;

        public PlaytimeStatusDetails(bool isFraud, int? testGroup, bool campaignsAvailable = false, PlaytimeCampaignsState[] campaignsState = null)
        {
            this.isFraud = isFraud;
            this.campaignsAvailable = campaignsAvailable;
            this.testGroup = testGroup ?? -1;
            if (campaignsState != null)
            {
                this.campaignsState = Array.ConvertAll(campaignsState, s => s.ToString());
            }
        }

        #if UNITY_ANDROID
        internal PlaytimeStatusDetails(AndroidJavaObject statusDetails) {
            this.isFraud = statusDetails.Call<bool>("isFraud");
            this.campaignsAvailable = statusDetails.Call<bool>("getCampaignsAvailable");

            AndroidJavaObject testGroupOjb = statusDetails.Call<AndroidJavaObject>("getTestGroup");
            this.testGroup = testGroupOjb == null ? (int)-1 : testGroupOjb.Call<int>("intValue");

            AndroidJavaObject javaCampaignsState = statusDetails.Call<AndroidJavaObject>("getCampaignsState");
            if (javaCampaignsState != null)
            {
                int size = javaCampaignsState.Call<int>("size");
                this.campaignsState = new string[size];
                for (int i = 0; i < size; i++)
                {
                    AndroidJavaObject stateEnum = javaCampaignsState.Call<AndroidJavaObject>("get", i);
                    this.campaignsState[i] = stateEnum.Call<string>("name");
                }
            }
        }
        #endif
    }
}
