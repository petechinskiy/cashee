using System.Collections;

namespace io.adjoe.sdk
{
    /// <summary>
    /// This class holds information about the response of <c>PlaytimeCustom.RequestPartnerApps</c> or <c>PlaytimeCustom.RequestInstalledPartnerApps</c>.
    /// </summary>
    public class PlaytimeCampaignResponse
    {
        /// <summary>
        /// Returns the list of requested partner apps.
        /// </summary>
        public ArrayList PartnerApps { get; set; }
    }
}
