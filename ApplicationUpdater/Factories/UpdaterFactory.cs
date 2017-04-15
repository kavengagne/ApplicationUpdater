namespace ApplicationUpdater
{
    public static class UpdaterFactory
    {
        public static IUpdater Create(string httpAddress, string releaseFileName)
        {
            return new Updater(httpAddress, releaseFileName);
        }
    }
}