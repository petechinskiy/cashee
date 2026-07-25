using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Holds information about errors which occur during <c>Adjoe.RequestRewards</c>.
    /// </summary>
    public class AdjoeRewardResponseError
    {
        /// <summary>
        /// The exception which caused the request to fail.
        /// </summary>
        public Exception Exception { get; set; }
    }
}