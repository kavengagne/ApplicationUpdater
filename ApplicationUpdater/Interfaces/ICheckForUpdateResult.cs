using System;

namespace ApplicationUpdater
{
    public interface ICheckForUpdateResult
    {
        Version CurrentRelease { get; }
        Version LatestRelease { get; }

        bool IsUpdateAvailable { get; }

        IResponse Response { get; }
    }
}
