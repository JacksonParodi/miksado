using System.IO;

namespace Miksado.Constant
{
    public static class Constant
    {
        //public static readonly IRandomNumberGenerator= RandomNumberGeneratorModel.System;
        //public static readonly IRandomNumberGenerator DefaultRandomNumberGenerator = new MajoraRng();
        //public static readonly IShuffleAlgorithm DefaultShuffleAlgorithm = new NoImmediateRepeat();
        //public static readonly ShuffleTrigger[] DefaultShuffleTriggers = [ShuffleTrigger.Timer];

        public static readonly string MiksadoPath = Path.Combine("miksado");
        public static readonly string MiksadoGamePath = Path.Combine(MiksadoPath, "game");
        public static readonly string MiksadoStatePath = Path.Combine(MiksadoPath, "state");
        public static readonly string MiksadoDataPath = Path.Combine(MiksadoPath, "data");

        public static readonly string[] InvalidGameExtensions = [".bin"];
        public static readonly string SaveStatePrefix = "MSAV_";
        public static readonly int ShuffleRetryLimit = 10;
        public static readonly int DefaultCooldown = 0;

        //private static readonly string DummyToken = "5vx8b5r4uru2s3qijmujdkvs48nmip";
        //private static readonly string JacksonTwitchId = "43920828";

        public static readonly int MajorVersion = 0;
        public static readonly int MinorVersion = 1;
        public static readonly int PatchVersion = 0;
    }
}
