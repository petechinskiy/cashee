using System.Collections;

namespace io.adjoe.sdk
{
    /// <summary>
    /// This class holds information about the response of <c>Adjoe.RequestPartnerApps</c> or <c>Adjoe.RequestInstalledPartnerApps</c>.
    /// </summary>
    public class AdjoeCampaignResponse
    {
        /// <summary>
        /// Returns the list of requested partner apps.
        /// </summary>
        public ArrayList PartnerApps { get; set; }
    }
}
