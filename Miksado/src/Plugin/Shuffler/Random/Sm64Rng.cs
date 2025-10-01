namespace Miksado.Plugin.Shuffler.Random
{
    internal class Sm64Rng : IRandomNumberGenerator
    {
        private readonly static int InitialSeed = 0;
        private int _seed = 0;

        public Sm64Rng()
        {
            SetSeed(InitialSeed);
        }

        public Sm64Rng(int seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(int newSeed)
        {
            _seed = newSeed;
        }

        public int Next()
        {
            ushort upperSeed = (ushort)((_seed & 0xFFFF0000) >> 16);
            ushort lowerSeed = (ushort)(_seed & 0x0000FFFF);

            ushort upperValue = NextSm64(upperSeed);
            ushort lowerValue = NextSm64(lowerSeed);

            _seed = upperValue << 16 | lowerValue;
            if (_seed < 0)
            {
                _seed = -1 * _seed;
            }
            return _seed;
        }

        private ushort NextSm64(ushort input)
        {
            ushort s0, s1;
            if (input == 22026)
            {
                input = 0;
            }

            s0 = (ushort)((input & 255) << 8);
            s0 = (ushort)(s0 ^ input);

            input = (ushort)(((s0 & 255) << 8) + ((s0 & 65280) >> 8));

            s0 = (ushort)((s0 & 255) << 1 ^ input);
            s1 = (ushort)(s0 >> 1 ^ 65408);

            if ((s0 & 1) == 0)
            {
                if (s1 == 43605)
                {
                    input = 0;
                }
                else
                {
                    input = (ushort)(s1 ^ 8180);
                }
            }
            else
            {
                input = (ushort)(s1 ^ 33152);
            }
            return input;
        }

        //// Generate a pseudorandom integer from 0 to 65535 from the random seed, and update the seed.
        //u16 random_u16(void)
        //{
        //    u16 temp1, temp2;

        //    if (gRandomSeed16 == 22026)
        //    {
        //        gRandomSeed16 = 0;
        //    }

        //    temp1 = (gRandomSeed16 & 0x00FF) << 8;
        //    temp1 = temp1 ^ gRandomSeed16;

        //    gRandomSeed16 = ((temp1 & 0x00FF) << 8) + ((temp1 & 0xFF00) >> 8);

        //    temp1 = ((temp1 & 0x00FF) << 1) ^ gRandomSeed16;
        //    temp2 = (temp1 >> 1) ^ 0xFF80;

        //    if ((temp1 & 1) == 0)
        //    {
        //        if (temp2 == 43605)
        //        {
        //            gRandomSeed16 = 0;
        //        }
        //        else
        //        {
        //            gRandomSeed16 = temp2 ^ 0x1FF4;
        //        }
        //    }
        //    else
        //    {
        //        gRandomSeed16 = temp2 ^ 0x8180;
        //    }

        //    return gRandomSeed16;
        //}
    }
}
