namespace ApplicationUpdater
{
    internal interface IReleaseResponse : IResponse
    {
        Release LatestRelease { get; set; }
    }
}