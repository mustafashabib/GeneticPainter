using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticPainterLib
{
    internal static class RandomNumber
    {
        private static Random random = new Random();

        internal static int GetRandom(int min, int max)
        {
            lock (random)
            {
                return random.Next(min,max);
            }

        }
    }
}
