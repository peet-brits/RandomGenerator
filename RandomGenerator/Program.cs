using System;
using System.Collections.Generic;
using System.Text;

namespace RandomGenerator
{
    class Program
    {
        private static readonly Random _random = new();

        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower().Contains("d"))
            {
                DieUsage(args);
            }
            else
            {
                RegularUsage(args);
            }
        }

        private static void RegularUsage(params string[] args)
        {
            var min = 1;
            var max = 10;
            var repeat = 1;

            if (args.Length >= 2)
            {
                min = Convert.ToInt32(args[0]);
                max = Convert.ToInt32(args[1]);

                if (args.Length >= 3)
                {
                    repeat = Convert.ToInt32(args[2]);
                }
            }
            else if (args.Length >= 1)
            {
                max = Convert.ToInt32(args[0]);
            }

            for (var i = 0; i < repeat; i++)
            {
                var next = _random.Next(min, max + 1);
                Console.WriteLine("Random number between {0} and {1}: {2}", min, max, next);
            }
        }

        private static void DieUsage(params string[] args)
        {
            var dieList = new List<Die>();
            foreach (var argRaw in args)
            {
                var sign = '+';
                var argValue = argRaw;
                if (argValue.StartsWith("+") ||
                    argValue.StartsWith("-"))
                {
                    sign = argValue[0];
                    argValue = argValue.Substring(1);
                }

                var values = argValue.ToLower().Split("d");
                if (values.Length != 2)
                {
                    Console.WriteLine("Invalid format, expected e.g. 3d6 or d12: {0}", argRaw);
                    return;
                }

                var die = Die.Create(values[1], values[0], sign);
                if (die == null)
                {
                    Console.WriteLine("Invalid format, parse failed: {0}", argRaw);
                    return;
                }

                dieList.Add(die);
            }

            var sb = new StringBuilder();
            var separator = string.Empty;
            var total = 0;
            var counter = 0;
            foreach (var die in dieList)
            {
                for (int i = 0; i < die.Loop; i++)
                {
                    var next = _random.Next(1, die.Amount + 1);

                    if (die.Sign == Sign.Negative)
                    {
                        total -= next;

                        if (separator != string.Empty)
                        {
                            separator = " - ";
                        }
                        else
                        {
                            separator = "-";
                        }
                    }
                    else
                    {
                        total += next;
                    }

                    sb.AppendFormat("{0}{1}", separator, next);
                    separator = " + ";
                    counter++;
                }
            }

            sb.AppendFormat(" = {0}", total);

            if (counter <= 1)
            {
                Console.WriteLine("Result: {0}", total);
            }
            else
            {
                Console.WriteLine("Result: {0}", sb.ToString());
            }
        }

        public class Die
        {
            private Die(int amount, int loop, Sign sign)
            {
                Amount = amount;
                Loop = loop;
                Sign = sign;
            }

            public int Amount { get; }
            public int Loop { get; }
            public Sign Sign { get; set; }

            public static Die Create(int amount, int loop = 1, Sign sign = Sign.Positive)
            {
                return new Die(amount, loop, sign);
            }

            public static Die Create(string amount, string loop, char sign)
            {
                if (loop == string.Empty)
                {
                    loop = "1";
                }

                var parsedSign = Sign.Positive;
                if (sign == '-')
                {
                    parsedSign = Sign.Negative;
                }

                if (int.TryParse(amount, out var parsedAmount) &&
                    int.TryParse(loop, out var parsedLoop))
                {
                    return Die.Create(
                        parsedAmount,
                        parsedLoop,
                        parsedSign);
                }
                else
                {
                    return null;
                }
            }
        }

        public enum Sign
        {
            Positive,
            Negative,
        }
    }
}
