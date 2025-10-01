using Miksado.Plugin.Shuffler.Random;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal class TrueRandom : IShuffleAlgorithm
    {
        public string NextGamePath(IRandomNumberGenerator rng, string[] allGamePaths, string? _)
        {
            int random = rng.Next();
            return allGamePaths[random % allGamePaths.Length];
        }
    }
}
