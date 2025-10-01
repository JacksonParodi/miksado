using System;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    static class ShuffleAlgorithmFactory
    {
        private static readonly Dictionary<string, Func<IShuffleAlgorithm>> ShuffleAlgorithmCreators = new()
        {
            { "Bag", () => new Bag() },
            { "No Immediate Repeat", () => new NoImmediateRepeat() },
            { "True Random", () => new TrueRandom() },
        };

        public static string[] GetAvailableShuffleAlgorithmNames() => [.. ShuffleAlgorithmCreators.Keys];

        public static IShuffleAlgorithm CreateShuffleAlgorithm(string name)
        {
            return ShuffleAlgorithmCreators.TryGetValue(name, out var creator)
                ? creator()
                : new TrueRandom();
        }
    }
}
