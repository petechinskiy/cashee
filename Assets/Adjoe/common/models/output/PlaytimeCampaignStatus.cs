using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Represents the current status of a campaign.
    /// [AVAIlABLE] - The campaign is available for installation.
    /// [PENDING] - Waiting for the app installation.
    /// [INSTALLED] - The app associated with the campaign has been installed.
    /// [FAILED] - The campaign installation or processing has failed.
    /// </summary>
    [System.Serializable]
    public enum PlaytimeCampaignStatus
    {
        AVAIlABLE,
        PENDING,
        INSTALLED,
        FAILED
    }
}
