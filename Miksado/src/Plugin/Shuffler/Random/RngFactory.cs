using System;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler.Random
{
    static class RngFactory
    {
        private static readonly Dictionary<string, Func<IRandomNumberGenerator>> RngCreators = new()
    {
        { "System", () => new SystemRng() },
        { "Super Mario 64", () => new Sm64Rng() },
        { "Majora's Mask", () => new MajoraRng() },
        // Add more as you implement them
        // { "Doom", () => new DoomRng() },
        // { "Final Fantasy I", () => new FinalFantasy1Rng() },
    };

        public static string[] GetAvailableRngNames() => [.. RngCreators.Keys];

        public static IRandomNumberGenerator CreateRng(string name)
        {
            return RngCreators.TryGetValue(name, out var creator)
                ? creator()
                : new SystemRng(); // fallback
        }

        public static IRandomNumberGenerator CreateRng(string name, int seed)
        {
            var rng = CreateRng(name);
            rng.SetSeed(seed);
            return rng;
        }
    }
}
