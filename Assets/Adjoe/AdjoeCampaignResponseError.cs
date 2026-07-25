using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Holds information about errors that occur while requesting partner apps with <c>Adjoe.RequestPartnerApps</c> or <c>Adjoe.RequestInstalledPartnerApps</c>.
    /// </summary>
    public class AdjoeCampaignResponseError
    {
        /// <summary>
        /// Returns the exception which caused the partner apps request to fail.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
