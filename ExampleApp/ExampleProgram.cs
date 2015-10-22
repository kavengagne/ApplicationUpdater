using System;
using System.Net.Configuration;
using ApplicationUpdater;

namespace ExampleApp
{
    public class ExampleProgram
    {
        static void Main()
        {
            IUpdater updater = UpdaterFactory.Create("http://updater.staging.s3.amazonws.com", "releases.dat");
            
            Console.WriteLine(@"Version is " + updater.CurrentRelease);
            
            ICheckForUpdateResult checkForUpdateResult = updater.CheckForUpdate();
            if (checkForUpdateResult.IsUpdateAvailable)
            {
                updater.Update();
            }

            Console.Read();
        }
    }
}
