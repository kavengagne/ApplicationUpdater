using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ApplicationUpdater
{
    internal class ReleaseResponse : Response, IReleaseResponse
    {
        public Release LatestRelease { get; set; }

        public ReleaseResponse(bool isSuccess, HttpStatusCode statusCode, string releases, string errorMessage = "")
            : base(statusCode, errorMessage, isSuccess)
        {
            LatestRelease = GetLatestRelease(releases);
        }

        private List<Release> GetReleasesEntries(string releases)
        {
            if (string.IsNullOrWhiteSpace(releases))
            {
                return new List<Release>();
            }

            return Regex.Split(releases, "(\r\n|\n)")
                        .Where(rel => !string.IsNullOrWhiteSpace(rel))
                        .Select(ReleaseFactory.CreateReleaseEntry)
                        .Where(rel => rel.IsValid)
                        .ToList();
        }

        private Release GetLatestRelease(string releases)
        {
            List<Release> releaseEntries = GetReleasesEntries(releases);
            if (!releaseEntries.Any())
            {
                return new Release("", Assembly.GetEntryAssembly().GetName().Version.ToString());
            }

            Release latestRelease = releaseEntries.First();
            foreach (var release in releaseEntries)
            {
                if (release.Version > latestRelease.Version)
                {
                    latestRelease = release;
                }
            }

            return latestRelease;
        }
    }
}