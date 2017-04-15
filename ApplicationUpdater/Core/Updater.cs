﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;


namespace ApplicationUpdater
{
    internal class Updater : IUpdater
    {
        #region Private Fields
        private readonly string _httpServerUrl;
        private readonly string _releasesFileName;
        private Release _latestRelease;
        #endregion Private Fields


        #region Public Properties
        public Version CurrentRelease { get; }
        public Action BeforeUpdate { get; set; }
        #endregion Public Properties


        #region Constructors
        public Updater(string httpServerUrl, string releasesFileName)
        {
            if (string.IsNullOrWhiteSpace(httpServerUrl))
            {
                throw new ArgumentNullException(nameof(httpServerUrl));
            }
            httpServerUrl = NormalizeHttpUrl(httpServerUrl);
            if (!IsWebServerAddress(httpServerUrl))
            {
                throw new ArgumentException(@"Must be a valid HTTP or HTTPS address.", nameof(httpServerUrl));
            }
            _httpServerUrl = httpServerUrl;

            if (string.IsNullOrWhiteSpace(releasesFileName))
            {
                throw new ArgumentNullException(nameof(releasesFileName));
            }
            _releasesFileName = releasesFileName;
            CurrentRelease = GetCurrentVersion();
        }
        #endregion Constructors


        #region Public Methods
        public async Task<ICheckForUpdateResult> CheckForUpdateAsync()
        {
            IReleaseResponse response = await GetLatestRelease();
            _latestRelease = response.LatestRelease;
            return new CheckForUpdateResult(CurrentRelease, _latestRelease.Version, response);
        }

        public ICheckForUpdateResult CheckForUpdate()
        {
            return CheckForUpdateAsync().Result;
        }

        public void Update()
        {
            if (_latestRelease == null || _latestRelease.Version == CurrentRelease)
            {
                return;
            }

            BeforeUpdate?.Invoke();

            var applicationPath = GetApplicationPath();
            var releasePath = GetReleasePath(applicationPath);

            RenewDirectory(releasePath);
            ExtractRemoteReleaseArchive(releasePath);

            LaunchUpdater(releasePath, applicationPath);

            Environment.Exit(0);
        }
        #endregion Public Methods


        #region Private Methods
        private async Task<IReleaseResponse> GetLatestRelease()
        {
            using (var client = new HttpClient { BaseAddress = new Uri(_httpServerUrl) })
            {
                string releases = string.Empty;
                string errorMessage = string.Empty;

                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(_releasesFileName);
                }
                catch (Exception ex)
                {
                    // TODO KG - Write error to EventLog
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                if (response.IsSuccessStatusCode)
                {
                    releases = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    errorMessage = GetErrorMessageFromHttpResponse(response);
                }
                return new ReleaseResponse(response.IsSuccessStatusCode, response.StatusCode, releases, errorMessage);
            }
        }

        // TODO: KG - Localize this
        private static string GetErrorMessageFromHttpResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return "Unable to connect to server";
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return "Unable to fetch releases";
            }
            return "Check for updates failed";
        }

        private static string GetApplicationPath()
        {
            var rootPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            rootPath = Directory.GetParent(rootPath).FullName;
            return rootPath;
        }

        private string GetReleasePath(string rootPath)
        {
            var releasePath = Path.Combine(rootPath, _latestRelease.FileName);
            return releasePath;
        }

        private static void RenewDirectory(string releasePath)
        {
            if (Directory.Exists(releasePath))
            {
                Directory.Delete(releasePath, true);
            }
            Directory.CreateDirectory(releasePath);
        }

        private void ExtractRemoteReleaseArchive(string releasePath)
        {
            var zipFileName = GetReleaseFileZip(_latestRelease.FileName);
            ZipFile.ExtractToDirectory(zipFileName, releasePath);
        }

        private static void LaunchUpdater(string releasePath, string rootPath)
        {
            var updaterPath = Path.Combine(rootPath, "Updater.exe");
            WriteUpdaterExecutable(updaterPath);

            var arguments = GetUpdaterArguments(releasePath);
            Process.Start(new ProcessStartInfo(updaterPath, arguments));
        }

        private static string GetUpdaterArguments(string releasePath)
        {
            var processId = Process.GetCurrentProcess().Id;
            var sourcePath = releasePath.TrimEnd('\\');
            var destinationPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            var executableFile = Assembly.GetEntryAssembly().Location?.TrimEnd('\\');

            var arguments = $"{processId} \"{sourcePath}\" \"{destinationPath}\" \"{executableFile}\"";
            return arguments;
        }

        private static void WriteUpdaterExecutable(string updaterPath)
        {
            var updaterBytes = Resources.Updater;
            File.WriteAllBytes(updaterPath, updaterBytes);
        }

        private static string NormalizeHttpUrl(string url)
        {
            url = url.ToLowerInvariant();
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }
            if (url.Contains("://"))
            {
                return url;
            }
            return "http://" + url;
        }

        private static bool IsWebServerAddress(string httpServerUrl)
        {
            Uri uriResult;
            return Uri.TryCreate(httpServerUrl, UriKind.Absolute, out uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private string GetReleaseFileZip(string releaseFileName)
        {
            var tempFileName = Path.GetTempFileName();
            // TODO: KG - Use HttpClient instead - IMPORTANT
            var client = new WebClient { BaseAddress = _httpServerUrl };
            client.DownloadFile(releaseFileName + ".zip", tempFileName);
            return tempFileName;
        }

        private static Version GetCurrentVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }
        #endregion Private Methods
    }
}
