using Miksado.Misc;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler
{
    internal class ShufflerConfig : PluginConfig
    {
        private Dictionary<string, ShufflerGameInfo> _gameInfoMap = [];
        public Dictionary<string, ShufflerGameInfo> GameInfoMap
        {
            get => _gameInfoMap;
            set => _gameInfoMap = value;
        }
    };
}
