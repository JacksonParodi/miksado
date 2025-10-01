using Miksado.Plugin.Shuffler.Random;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal class TrueRandom : IShuffleAlgorithm
    {
        public string NextGamePath(IRandomNumberGenerator rng, List<string> allGamePaths, string? _)
        {
            int random = rng.Next();
            return allGamePaths[random % allGamePaths.Count];
        }
    }
}
