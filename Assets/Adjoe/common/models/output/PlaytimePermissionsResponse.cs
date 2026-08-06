using System;
using UnityEngine;

namespace io.adjoe.sdk.studio
{
    /// <summary>
    /// SDK response containing permissions.
    /// </summary>
    [System.Serializable]
    public class PlaytimePermissionsResponse
    {
        [SerializeField] private PlaytimePermissions permissions;

        /// <summary>
        /// Permissions granted by the user.
        /// </summary>
        public PlaytimePermissions Permissions => permissions;

        public PlaytimePermissionsResponse(PlaytimePermissions permissions)
        {
            this.permissions = permissions;
        }
    }
} 