using System;
using System.Diagnostics;
using System.IO;
using Updater.Config;
using Updater.Helpers;
using Updater.Loggers;

namespace Updater
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Thread.Sleep(TimeSpan.FromSeconds(30));
            if (args == null || args.Length < 4)
            {
                Console.WriteLine("Usage: Updater <processId> \"<sourcePath>\" \"<destinationPath>\" \"<executableFile>\"");
                Debug.WriteLine("Usage: Updater <processId> \"<sourcePath>\" \"<destinationPath>\" \"<executableFile>\"");
                return;
            }

            ExceptionHelper.SetLogger(new EventLogLogger());
            var updaterConfig = new UpdaterConfig(args);
            
            WaitForCallerExit(updaterConfig.ProcessId);

            // TODO: KG - Pourrait être plus conservateur et effacer seulement les fichiers installés par nous.
            // TODO: KG - Faire un backup de l'installation courante avant d'updater.
            DeleteDestinationFolderIfExists(updaterConfig.DestinationPath);

            var copySucceeded = ExceptionHelper.TrySafe<Exception>(
                () => DirectoryHelper.CopyDirectory(updaterConfig.SourcePath, updaterConfig.DestinationPath));
            if (copySucceeded)
            {
                // TODO: KG - Here we do nothing on error. Should log to somewhere at least.
                ExceptionHelper.TrySafe<Exception>(() => Process.Start(updaterConfig.ExecutableFile));
            }
            else
            {
                // TODO: KG - Do not fail in silence. Report to EventLog and show message.
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
            // TODO: KG - Ne pas attendre éternellement pour le process.
            // TODO: KG - Si le process ne termine pas après X secondes, Rollback la mise a jour.
            // TODO: KG - Lors du rollback, effacer tout (backup zip, release folder)
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
