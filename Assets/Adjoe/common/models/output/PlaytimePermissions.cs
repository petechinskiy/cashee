using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// Permissions granted by the user.
    /// </summary>
    [System.Serializable]
    public class PlaytimePermissions
    {
        [SerializeField] private bool isTosAccepted;
        [SerializeField] private bool isUsagePermissionAccepted;

        /// <summary>
        /// Flag indicating whether the terms of service are accepted.
        /// </summary>
        public bool IsTosAccepted => isTosAccepted;
        
        /// <summary>
        /// Usage permissions. Not relevant on iOS.
        /// </summary>
        public bool IsUsagePermissionAccepted => isUsagePermissionAccepted;

        public PlaytimePermissions(bool isTosAccepted, bool isUsagePermissionAccepted)
        {
            this.isTosAccepted = isTosAccepted;
            this.isUsagePermissionAccepted = isUsagePermissionAccepted;
        }

        public PlaytimePermissions(AndroidJavaObject permissions) {
            this.isTosAccepted = permissions.Call<bool>("isTOSAccepted");
            this.isUsagePermissionAccepted = permissions.Call<bool>("isUsagePermissionAccepted");
        }
    }
} 