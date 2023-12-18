using NectarRCON.Updater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NectarRCON.Updater
{
    public class GithubUpdater : IUpdater
    {
        private static readonly HttpClient _client = new()
        {
            BaseAddress = new Uri("https://api.github.com/repos/zkhssb/NectarRcon/")
        };
        private bool _preEnable = false;
        private AppVersion? _version;

        /// <summary>
        /// 获取最新版本, null为没找到
        /// </summary>
        /// <param name="enablePre">是否允许pre版本</param>
        private AppVersion? GetLatestVersion(bool enablePre)
        {
            if (_version is null)
                return null;
            using(HttpRequestMessage request = new(HttpMethod.Get, "releases/latest"))
            {
                request.Headers.Add("User-Agent", $"{_version.AppName}-AppUpdater");
                using(HttpResponseMessage response = _client.Send(request))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException(response.StatusCode.ToString());
                    string resultString = string.Empty;
                    Task.Run(async () =>
                    {
                        resultString = await response.Content.ReadAsStringAsync();
                    }).Wait();
                    Release release = JsonSerializer.Deserialize<Release>(resultString) ?? throw new JsonException();
                    foreach(var asset in release.Assets)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(asset.Name);
                        try
                        {
                            fileName = "NectarRcon-x86-1.0.0-beta2";
                            AppVersion version = AppVersion.ParseVersion(fileName);
                            if(version.AppName.ToLower() == _version.AppName.ToLower() && version.Platform.ToLower() == _version.Platform.ToLower())
                            {
                                if (version.IsPreRelease && !enablePre)
                                    continue;
                                if (version > _version)
                                {
                                    return version;
                                }
                            }
                        }
                        catch (InvalidOperationException) { } // Invalid version format
                    }
                    return null;
                }
            }
        }

        public bool IsLatestVersion()
        {
            GetLatestVersion(_preEnable);
            return true;
        }

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void SetVersion(string version)
        {
            _version = AppVersion.ParseVersion(version);
        }

        public void SetPreEnable(bool value)
        {
            _preEnable = value;
        }

        public AppVersion GetLatestVersion()
        {
            throw new NotImplementedException();
        }
    }
}
