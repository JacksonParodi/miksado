namespace Miksado.Random
{
    internal interface IRandomNumberGenerator
    {
        public abstract void SetSeed(int seed);
        public abstract int Next();
    }
}
