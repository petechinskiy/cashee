namespace io.adjoe.sdk
{
    /// <summary>
    /// Holds information about how many coins have been paid out by <c>PlaytimeCustom.doPayout</c>.
    /// </summary>
    public class PlaytimePayoutResponse
    {
        /// <summary>
        /// The amount of coins that have been paid out.
        /// </summary>
        public int Coins { get; set; }
    }
}