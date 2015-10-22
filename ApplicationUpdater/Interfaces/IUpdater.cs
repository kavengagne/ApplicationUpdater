using System;

namespace ApplicationUpdater
{
    public interface IUpdater
    {
        Version CurrentRelease { get; }

        ICheckForUpdateResult CheckForUpdate();
        void Update();
    }
}
