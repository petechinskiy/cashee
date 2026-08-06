using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// The response of the SDK containing campaigns.
    /// </summary>
    [System.Serializable]
    public class PlaytimeCampaignsResponse
    {
        [SerializeField] private PlaytimeCampaign[] campaigns;

        /// <summary>
        /// The requested selection of campaigns.
        /// </summary>
        public PlaytimeCampaign[] Campaigns => campaigns;

        public PlaytimeCampaignsResponse(PlaytimeCampaign[] campaigns)
        {
            this.campaigns = campaigns;
        }
    }
} 