using Miksado.Plugin.Shuffler.Random;
using System.Collections.Generic;
using System.Linq;

namespace Miksado.Plugin.Shuffler.Shuffle
{
    internal class Bag : IShuffleAlgorithm
    {
        // bag starts empty then fills up as games are shuffled into it
        // when full, empty the bag and start again
        private readonly List<string> _bag = [];

        public string NextGamePath(IRandomNumberGenerator rng, List<string> allGamePaths, string? currentGamePath)
        {
            // as it is, bag algo can repeat a game if it is the only one left and then the first game of the new bag
            if (currentGamePath != null)
            {
                _bag.Add(currentGamePath);
            }

            // might be better to make sure each element of both arrays are the same
            if (_bag.Count == allGamePaths.Count && _bag.All(allGamePaths.Contains))
            {
                _bag.Clear();
            }

            List<string> availableGames = [];

            foreach (string game in allGamePaths)
            {
                if (!_bag.Contains(game))
                {
                    availableGames.Add(game);
                }
            }

            if (availableGames.Count == 0)
            {
                string e = "zero available games";
                e += " allGamePaths: " + string.Join(", ", allGamePaths);
                e += " _bag: " + string.Join(", ", _bag);
                e += " currentGamePath: " + currentGamePath;
                e += " rng: " + rng.ToString();
                throw new System.Exception(e);
            }

            return availableGames[rng.Next() % availableGames.Count];
        }
    }
}
