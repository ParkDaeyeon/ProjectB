namespace Ext.Algorithm
{
    public static class Oscillate
    {
        public static int CalcI(int input, int min, int max)
        {
            var range = max - min;
            var value = ((input + range) % (range * 2)) - range;

            // NOTE: Absolute
            if (0 > value)
                value = -value;

            return min + value;
        }
    }
}
