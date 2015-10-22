using System;

namespace ApplicationUpdater
{
    internal class Release
    {
        public bool IsValid { get; private set; }
        public Version Version { get; private set; }

        public string FileName { get; private set; }

        public Release(string fileName, string fileVersion)
        {
            FileName = fileName;
            
            Version version;
            if (Version.TryParse(fileVersion, out version))
            {
                Version = version;
            }

            if (FileName != null && Version != null)
            {
                IsValid = true;
            }
        }
    }
}
