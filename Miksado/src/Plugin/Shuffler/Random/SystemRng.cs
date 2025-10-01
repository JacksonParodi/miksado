namespace Miksado.Plugin.Shuffler.Random
{
    internal class SystemRng : IRandomNumberGenerator
    {
        private int _seed;
        private readonly System.Random _rng;

        public SystemRng()
        {
            _rng = new System.Random();
        }
        public SystemRng(int seed)
        {
            SetSeed(seed);
            _rng = new System.Random(_seed);
        }

        public int Next()
        {
            return _rng.Next();
        }

        public void SetSeed(int seed)
        {
            _seed = seed;
        }
    }
}
