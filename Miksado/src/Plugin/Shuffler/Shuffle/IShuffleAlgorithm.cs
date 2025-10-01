using Miksado.Plugin.Shuffler.Random;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal interface IShuffleAlgorithm
    {
        public string NextGamePath(IRandomNumberGenerator rng, string[] allGamePaths, string? currentGamePath);
    }
}
