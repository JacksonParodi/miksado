using Miksado.Plugin.Shuffler.Random;
using System.Collections.Generic;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal class Bag : IShuffleAlgorithm
    {
        // bag starts empty then fills up as games are shuffled into it
        // when full, empty the bag and start again
        private readonly List<string> _usedGameBag = [];

        public string NextGamePath(IRandomNumberGenerator rng, string[] allGamePaths, string? currentGamePath)
        {
            // as it is, bag algo can repeat a game if it is the only one left and then the first game of the new bag
            if (currentGamePath != null)
            {
                _usedGameBag.Add(currentGamePath);
            }

            // might be better to make sure each element of both arrays are the same
            if (_usedGameBag.Count >= allGamePaths.Length)
            {
                _usedGameBag.Clear();
            }

            List<string> nextGameCandidates = [];
            foreach (string game in allGamePaths)
            {
                if (!_usedGameBag.Contains(game))
                {
                    nextGameCandidates.Add(game);
                }
            }

            if (nextGameCandidates.Count == 0)
            {
                string e = "zero available games";
                e += " allGamePaths: " + string.Join(", ", allGamePaths);
                e += " _bag: " + string.Join(", ", _usedGameBag);
                e += " currentGamePath: " + currentGamePath;
                e += " rng: " + rng.ToString();
                throw new System.Exception(e);
            }

            return nextGameCandidates[rng.Next() % nextGameCandidates.Count];
        }
    }
}
