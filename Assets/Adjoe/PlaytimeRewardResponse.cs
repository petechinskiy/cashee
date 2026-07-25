namespace io.adjoe.sdk
{
    /// <summary>
    /// Holds information about the rewards which were requested with <c>PlaytimeCustom.requestRewards</c>.
    /// </summary>
    public class PlaytimeRewardResponse
    {
        /// <summary>
        /// The overall amount of rewards that the user has collected so far (paid out + available for payout).
        /// </summary>
        public int Reward { get; set; }

        /// <summary>
        /// The amount of rewards which are available for payout.
        /// </summary>
        public int AvailablePayoutCoins { get; set;  }

        /// <summary>
        /// The amount of rewards that the user has already paid out.
        /// </summary>
        public int AlreadySpentCoins { get; set; }
    }
}