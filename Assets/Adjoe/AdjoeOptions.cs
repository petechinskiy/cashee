using System;
namespace io.adjoe.sdk
{
    /// <summary>
    /// Use this class to pass additional options like the user ID to <c>Adjoe.inheritdoc</c>.
    /// </summary>
    public class AdjoeOptions
    {
        [Obsolete]
        internal bool tosAccepted;

        internal string userId;
        internal string applicationProcessName;
        internal AdjoeParams adjoeParams;
        internal AdjoeExtensions adjoeExtensions;
        internal AdjoeUserProfile adjoeUserProfile;


        [Obsolete("This feature is deprecated. Using this method has no effect.")]
        public AdjoeOptions SetTosAccepted(bool tosAccepted)
        {
            return this;
        }

        /// <summary>
        /// Sets the unique user ID to be used by adjoe.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The instance for chaining.</returns>
        public AdjoeOptions SetUserId(string userId)
        {
            this.userId = userId;
            return this;
        }

        public AdjoeOptions SetAdjoeParams(AdjoeParams adjoeParams)
        {
            this.adjoeParams = adjoeParams;
            return this;
        }

        public AdjoeOptions SetAdjoeExtensions(AdjoeExtensions adjoeExtensions)
        {
            this.adjoeExtensions = adjoeExtensions;
            return this;
        }

        public AdjoeOptions SetAdjoeUserProfile(AdjoeUserProfile adjoeUserProfile)
        {
            this.adjoeUserProfile = adjoeUserProfile;
            return this;
        }

        public AdjoeOptions SetApplicationProcessName(string applicationProcessName)
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
