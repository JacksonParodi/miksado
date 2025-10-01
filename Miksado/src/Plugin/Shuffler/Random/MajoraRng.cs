namespace Miksado.Plugin.Shuffler.Random
{
    internal class MajoraRng : IRandomNumberGenerator
    {

        //#define RAND_MULTIPLIER 1664525
        //#define RAND_INCREMENT 1013904223
        //f32 Rand_ZeroOne(void)
        //{
        //    sRandInt = (sRandInt * RAND_MULTIPLIER) + RAND_INCREMENT;
        //    gRandFloat.i = ((sRandInt >> 9) | 0x3F800000);
        //    return gRandFloat.f - 1.0f;
        //}

        private readonly static int Mult = 1664525;
        private readonly static int Inc = 1013904223;
        private readonly static int InitialSeed = 0;
        private int _seed = 0;

        public MajoraRng()
        {
            SetSeed(InitialSeed);
        }

        public MajoraRng(int seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(int newSeed)
        {
            _seed = newSeed;
        }

        public int Next()
        {
            long temp = (long)(_seed * Mult) + Inc;
            temp = temp % int.MaxValue;
            _seed = (int)temp;
            if (_seed < 0)
            {
                _seed = -1 * _seed;
            }
            return _seed;
        }
    }
}
