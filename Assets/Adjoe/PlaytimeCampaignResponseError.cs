using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Holds information about errors that occur while requesting partner apps with <c>PlaytimeCustom.RequestPartnerApps</c> or <c>PlaytimeCustom.RequestInstalledPartnerApps</c>.
    /// </summary>
    public class PlaytimeCampaignResponseError
    {
        /// <summary>
        /// Returns the exception which caused the partner apps request to fail.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
