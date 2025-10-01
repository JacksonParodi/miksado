using System.Collections.Generic;

namespace Miksado.Misc
{
    internal static class Helper
    {
        public static string VersionToString(int major, int minor, int patch)
        {
            return $"v{major}.{minor}.{patch}";
        }

        public static Dictionary<string, int> VersionFromString(string version)
        {
            Dictionary<string, int> versionDict = new()
            {
                { "major", 0 },
                { "minor", 0 },
                { "patch", 0 }
            };

            if (version.StartsWith("v"))
            {
                version = version[1..];
            }

            string[] parts = version.Split('.');

            if (parts.Length >= 1 && int.TryParse(parts[0], out int major))
            {
                versionDict["major"] = major;
            }
            if (parts.Length >= 2 && int.TryParse(parts[1], out int minor))
            {
                versionDict["minor"] = minor;
            }
            if (parts.Length >= 3 && int.TryParse(parts[2], out int patch))
            {
                versionDict["patch"] = patch;
            }

            return versionDict;
        }
    }
}
