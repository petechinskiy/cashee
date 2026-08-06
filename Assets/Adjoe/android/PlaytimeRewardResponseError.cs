using System;

namespace io.adjoe.sdk
{
    #if UNITY_ANDROID
    /// <summary>
    /// Holds information about errors which occur during <c>PlaytimeCustom.requestRewards</c>.
    /// </summary>
    public class PlaytimeRewardResponseError
    {
        /// <summary>
        /// The exception which caused the request to fail.
        /// </summary>
        public Exception Exception { get; set; }
    }
    #endif
}