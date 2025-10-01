using Miksado.Plugin.Shuffler.Random;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal class NoImmediateRepeat : IShuffleAlgorithm
    {
        public string NextGamePath(IRandomNumberGenerator rng, string[] allGamePaths, string? currentGamePath)
        {
            if (currentGamePath == null)
            {
                return allGamePaths[rng.Next() % allGamePaths.Length];
            }

            int tryCount = 0;

            string newGamePath = currentGamePath;

            while (newGamePath == currentGamePath && allGamePaths.Length > 1 && tryCount <= Misc.Constant.ShuffleRetryLimit)
            {
                tryCount++;
                int random = rng.Next();
                newGamePath = allGamePaths[random % allGamePaths.Length];
            }

            // if newGamePath is still the same as currentGamePath, it means we have exhausted our retry limit
            return newGamePath;
        }
    }
}
