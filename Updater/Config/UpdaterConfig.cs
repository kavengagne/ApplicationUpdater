using System;
using System.Collections.Generic;

namespace Updater.Config
{
    public class UpdaterConfig
    {
        public string SourcePath;
        public string DestinationPath;
        public string ExecutableFile;
        public int ProcessId;

        public UpdaterConfig(IReadOnlyList<string> args)
        {
            ProcessId = Convert.ToInt32(args[0]);
            SourcePath = args[1];
            DestinationPath = args[2];
            ExecutableFile = args[3];
        }
    }
}