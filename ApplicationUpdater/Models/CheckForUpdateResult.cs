using System;

namespace ApplicationUpdater
{
    internal class CheckForUpdateResult : ICheckForUpdateResult
    {
        #region ICheckForUpdateResult Implementation
        public Version CurrentRelease { get; private set; }
        public Version LatestRelease { get; private set; }
        public bool IsUpdateAvailable { get; private set; }
        public IResponse Response { get; private set; }
        #endregion ICheckForUpdateResult Implementation

        #region Constructors
        public CheckForUpdateResult(Version currentRelease, Version latestRelease, IResponse response)
        {
            CurrentRelease = currentRelease;
            LatestRelease = latestRelease ?? currentRelease;

            if (LatestRelease > CurrentRelease)
            {
                IsUpdateAvailable = true;
            }

            Response = response;
        }
        #endregion Constructors
    }
}
