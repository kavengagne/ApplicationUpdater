using System;
using System.Threading.Tasks;


namespace ApplicationUpdater
{
    public interface IUpdater
    {
        Version CurrentRelease { get; }

        ICheckForUpdateResult CheckForUpdate();
        Task<ICheckForUpdateResult> CheckForUpdateAsync();
        void Update();
    }
}
