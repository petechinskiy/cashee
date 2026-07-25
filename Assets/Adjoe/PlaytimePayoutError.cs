using System;

namespace io.adjoe.sdk
{
    /// <summary>
    /// Holds information about errors which occur during <c>PlaytimeCustom.doPayout</c>.
    /// </summary>
    public class PlaytimePayoutError
    {
        /// <summary>
        /// An unspecified error.
        /// </summary>
        public const int UNKNOWN = 0;

        /// <summary>
        /// The user has not accepted the Terms of Service (TOS).
        /// </summary>
        public const int TOS_NOT_ACCEPTED = 1;

        /// <summary>
        /// The user has not collected enough rewards for a payout.
        /// </summary>
        public const int NOT_ENOUGH_COINS = 400;

        /// <summary>
        /// The reason with which the payout failed.
        /// </summary>
        public int Reason { get; set; }

        /// <summary>
        /// The exception that caused this payout to fail.
        /// </summary>
        /// <value></value>
        public Exception Exception { get; set; }

    }
}