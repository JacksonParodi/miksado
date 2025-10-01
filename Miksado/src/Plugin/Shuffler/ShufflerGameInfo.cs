using System;

namespace Miksado.Plugin.Shuffler
{
    internal class ShufflerGameInfo
    {
        public string GamePath { get; set; } = "_NO_GAME_PATH_";
        public string? StatePath { get; set; } = null;
        public DateTime LastPlayed { get; set; } = DateTime.MinValue;
        public bool IsCompleted { get; set; } = false;
    }
}
