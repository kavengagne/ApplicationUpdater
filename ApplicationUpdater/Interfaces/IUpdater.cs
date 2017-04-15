using System;
using System.Threading.Tasks;


namespace ApplicationUpdater
{
    public interface IUpdater
    {
        Version CurrentRelease { get; }
        Action BeforeUpdate { get; set; }

        ICheckForUpdateResult CheckForUpdate();
        Task<ICheckForUpdateResult> CheckForUpdateAsync();
        void Update();
    }
}
