using System;

namespace RandomGenerator
{
    class Program
    {
        private static Random _random = new Random();

        static void Main(string[] args)
        {
            var min = 1;
            var max = 10;

            if (args.Length >= 2)
            {
                min = Convert.ToInt32(args[0]);
                max = Convert.ToInt32(args[1]);
            }
            else if (args.Length >= 1)
            {
                max = Convert.ToInt32(args[0]);
            }

            var next = _random.Next(min, max + 1);
            Console.WriteLine("Random number between {0} and {1}: {2}", min, max, next);
        }
    }
}
