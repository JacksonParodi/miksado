using Miksado.Plugin.Shuffler.Random;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal interface IShuffleAlgorithm
    {
        public string NextGamePath(IRandomNumberGenerator rng, List<string> allGamePaths, string? currentGamePath);
    }
}
