using System;

namespace io.adjoe.sdk
{
    [System.Serializable]
    public enum PlaytimeCampaignsState
    {
        READY,
        BLOCKED,
        VPN_DETECTED,
        GEO_MISMATCH
    }
}
