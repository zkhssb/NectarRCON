using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NectarRCON.Updater
{
    public class AppVersion
    {
        public string AppName { get; set; } = string.Empty;
        public int Version { get; set; }
        public int Major { get;set; }
        public int Minor { get;set; }
        public int Patch { get;set; }
        public int? Build { get; set; }
        public string PreReleaseType { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public bool IsPreRelease
            => !string.IsNullOrEmpty(PreReleaseType);

        public override string ToString()
        {
            return $"{AppName}-{Platform}-{Major}.{Minor}.{Patch}" + (IsPreRelease ? $"-{PreReleaseType}{Build}" : string.Empty);
        }

        public override bool Equals(object? obj)
        {
            return obj?.ToString() == ToString();
        }

        public static bool operator <(AppVersion a, AppVersion b)
        {
            return a.Version < b.Version || (a.Build ?? 0) < (b.Build ?? 0);
        }

        public static bool operator >(AppVersion a, AppVersion b)
        {
            return a.Version > b.Version || (a.Build ?? 0) > (b.Build ?? 0);
        }

        public static bool operator ==(AppVersion a, AppVersion b)
        {
            return a.Version == b.Version && (a.Build ?? 0) == (b.Build ?? 0);
        }

        public static bool operator !=(AppVersion a, AppVersion b)
        {
            return a.Version != b.Version || (a.Build ?? 0) != (b.Build ?? 0);
        }

        private AppVersion() { }

        public static AppVersion ParseVersion(string version)
        {
            string[] versionParts = version.Split("-");
            if (versionParts.Length > 2)
            {
                AppVersion result = new();
                string name = versionParts[0];
                string platform = versionParts[1];
                string ver = versionParts[2];
                string preRelease = string.Empty;

                if (versionParts.Length > 3)
                {
                    preRelease = versionParts[3];
                }

                Regex versionRegex = new(@"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)");
                Match versionMatch = versionRegex.Match(ver);

                if (versionMatch.Success)
                {
                    result.Version = int.Parse(versionMatch.Groups["major"].Value + versionMatch.Groups["minor"].Value + versionMatch.Groups["patch"].Value);
                    result.Major = int.Parse(versionMatch.Groups["major"].Value);
                    result.Minor = int.Parse(versionMatch.Groups["minor"].Value);
                    result.Patch = int.Parse(versionMatch.Groups["patch"].Value);
                }

                Regex preReleaseRegex = new(@"(?<preRelease>[a-zA-Z]+)(?<build>\d+)");
                Match preReleaseMatch = preReleaseRegex.Match(preRelease);

                if (preReleaseMatch.Success)
                {
                    if (preReleaseMatch.Groups["build"].Success)
                    {
                        result.Build = int.Parse(preReleaseMatch.Groups["build"].Value);
                    }
                    if (preReleaseMatch.Groups["preRelease"].Success)
                    {
                        result.PreReleaseType = preReleaseMatch.Groups["preRelease"].Value;
                    }
                }

                result.Platform = platform;
                result.AppName = name;
                return result;
            }
            throw new InvalidOperationException("Invalid version format");
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(ToString());
        }
    }
}
