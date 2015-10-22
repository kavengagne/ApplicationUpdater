using System;
using System.Diagnostics;
using System.IO;
using Updater.Config;
using Updater.Helpers;
using Updater.Loggers;

namespace Updater
{
    public class UpdaterProgram
    {
        static void Main(string[] args)
        {
            //Thread.Sleep(TimeSpan.FromSeconds(30));
            if (args == null || args.Length < 4)
            {
                Console.WriteLine("Usage: Updater <processId> \"<sourcePath>\" \"<destinationPath>\" \"<executableFile>\"");
                return;
            }

            ExceptionHelper.SetLogger(new EventLogLogger());
            var updaterConfig = new UpdaterConfig(args);
            
            WaitForCallerExit(updaterConfig.ProcessId);
            DeleteDestinationFolderIfExists(updaterConfig.DestinationPath);

            var copySucceeded = ExceptionHelper.TrySafe<Exception>(
                () => DirectoryHelper.CopyDirectory(updaterConfig.SourcePath, updaterConfig.DestinationPath));
            if (copySucceeded)
            {
                ExceptionHelper.TrySafe<Exception>(() => Process.Start(updaterConfig.ExecutableFile));
            }
        }

        private static void DeleteDestinationFolderIfExists(string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                ExceptionHelper.TrySafe<Exception>(() => Directory.Delete(destinationPath, true));
            }
        }

        private static void WaitForCallerExit(int processId)
        {
            var executableProcess = GetExecutableById(processId);
            if (executableProcess != null)
            {
                ExceptionHelper.TrySafe<Exception>(() => executableProcess.WaitForExit());
            }
        }

        private static Process GetExecutableById(int processId)
        {
            return ExceptionHelper.TrySafe<Exception, Process>(
                () => Process.GetProcessById(processId),
                () => null);
        }
    }
}
