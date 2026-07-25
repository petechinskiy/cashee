using System;
namespace io.adjoe.sdk
{
    /// <summary>
    /// Use this class to pass additional options like the user ID to <c>Playtime.init</c>.
    /// </summary>
    public class PlaytimeOptions
    {
        [Obsolete]
        internal bool tosAccepted;

        internal string userId;
        internal string applicationProcessName;
        internal PlaytimeParams playtimeParams;
        internal PlaytimeExtensions playtimeExtensions;
        internal PlaytimeUserProfile playtimeUserProfile;


        [Obsolete("This feature is deprecated. Using this method has no effect.")]
        public PlaytimeOptions SetTosAccepted(bool tosAccepted)
        {
            return this;
        }

        /// <summary>
        /// Sets the unique user ID to be used by playtime.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The instance for chaining.</returns>
        public PlaytimeOptions SetUserId(string userId)
        {
            this.userId = userId;
            return this;
        }

        public PlaytimeOptions SetPlaytimeParams(PlaytimeParams playtimeParams)
        {
            this.playtimeParams = playtimeParams;
            return this;
        }

        public PlaytimeOptions SetPlaytimeExtensions(PlaytimeExtensions playtimeExtensions)
        {
            this.playtimeExtensions = playtimeExtensions;
            return this;
        }

        public PlaytimeOptions SetPlaytimeUserProfile(PlaytimeUserProfile playtimeUserProfile)
        {
            this.playtimeUserProfile = playtimeUserProfile;
            return this;
        }

        public PlaytimeOptions SetApplicationProcessName(string applicationProcessName)
        {
            this.applicationProcessName = applicationProcessName;
            return this;
        }

        public string GetApplicationProcessName()
        {
            return this.applicationProcessName;
        }

    }
}
