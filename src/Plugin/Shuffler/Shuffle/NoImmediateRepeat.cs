using Miksado.Plugin.Shuffler.Random;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal class NoImmediateRepeat : IShuffleAlgorithm
    {
        public string NextGamePath(IRandomNumberGenerator rng, List<string> allGamePaths, string? currentGamePath)
        {
            if (currentGamePath == null)
            {
                return allGamePaths[rng.Next() % allGamePaths.Count];
            }

            int tryCount = 0;

            string newGamePath = currentGamePath;

            while (newGamePath == currentGamePath && allGamePaths.Count > 1 && tryCount <= Constant.Constant.ShuffleRetryLimit)
            {
                tryCount++;
                int random = rng.Next();
                newGamePath = allGamePaths[random % allGamePaths.Count];
            }

            // if newGamePath is still the same as currentGamePath, it means we have exhausted our retry limit
            return newGamePath;
        }
    }
}
