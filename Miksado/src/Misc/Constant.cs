using System.IO;

namespace Miksado.Misc
{
    public static class Constant
    {
        public static readonly string MiksadoDirPath = Path.GetFullPath("miksado");
        public static readonly string MGameDirPath = Path.Combine(MiksadoDirPath, "game");
        public static readonly string MStateDirPath = Path.Combine(MiksadoDirPath, "state");
        public static readonly string MDataDirPath = Path.Combine(MiksadoDirPath, "data");
        public static readonly string MConfigFilePath = Path.Combine(MDataDirPath, "config.json");

        public static readonly string[] InvalidGameExtensions = [".bin", ".zip"];
        public static readonly string SaveStatePrefix = "MSAV_";
        public static readonly int ShuffleRetryLimit = 10;
        public static readonly int DefaultCooldown = 0;

        public static readonly int MajorVersion = 0;
        public static readonly int MinorVersion = 1;
        public static readonly int PatchVersion = 2;
    }
}
