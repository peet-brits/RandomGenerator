using System;
using System.Text;

namespace RandomGenerator
{
    class Program
    {
        private static readonly Random _random = new();

        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Length == 1 && char.IsLetter(args[0][0]))
            {
                AlphabetUsage(args);
            }
            else if (args.Length > 0 && args[0].ToLower().Contains("d"))
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
                if (!int.TryParse(args[0], out min) ||
                    !int.TryParse(args[1], out max))
                {
                    Console.WriteLine("Unexpected minimum & maximum values: {0} {1}", args[0], args[1]);
                    return;
                }

                if (args.Length >= 3)
                {
                    if (!int.TryParse(args[2], out repeat))
                    {
                        Console.WriteLine("Unexpected repeat value: {0}", args[2]);
                        return;
                    }
                }
            }
            else if (args.Length >= 1)
            {
                if (!int.TryParse(args[0], out max))
                {
                    Console.WriteLine("Unexpected maximum value: {0}", args[0]);
                    return;
                }
            }

            if (min > max)
            {
                Console.WriteLine("The minimum value cannot be greater than the maximum value: {0} > {1}", min, max);
                return;
            }

            for (var i = 0; i < repeat; i++)
            {
                var next = _random.Next(min, max + 1);
                Console.WriteLine("Random number between {0} and {1}: {2}", min, max, next);
            }
        }

        private static void AlphabetUsage(params string[] args)
        {
            var min = 'A';
            var max = 'Z';
            var repeat = 1;

            if (args.Length >= 2)
            {
                if (args[0].Length != 1 || !char.IsLetter(args[0][0]))
                {
                    Console.WriteLine("Unexpected letter: {0}", args[0].ToUpper());
                    return;
                }

                if (args[1].Length != 1 || !char.IsLetter(args[1][0]))
                {
                    Console.WriteLine("Unexpected letter: {0}", args[1].ToUpper());
                    return;
                }

                min = args[0].ToUpper()[0];
                max = args[1].ToUpper()[0];

                if (args.Length >= 3)
                {
                    if (!int.TryParse(args[2], out repeat))
                    {
                        Console.WriteLine("Unexpected repeat value: {0}", args[2]);
                        return;
                    }
                }
            }
            else if (args.Length >= 1)
            {
                if (args[0].Length != 1 || !char.IsLetter(args[0][0]))
                {
                    Console.WriteLine("Unexpected letter: {0}", args[0].ToUpper());
                    return;
                }

                max = args[0].ToUpper()[0];
            }

            if (min > max)
            {
                Console.WriteLine("The first letter cannot be after the second letter: {0} > {1}", min, max);
                return;
            }

            for (var i = 0; i < repeat; i++)
            {
                var next = (char)(_random.Next(min, max + 1));
                Console.WriteLine("Random letter between {0} and {1}: {2}", min, max, next);
            }
        }

        private static void DieUsage(params string[] args)
        {
            var sb = new StringBuilder();
            var separator = string.Empty;
            var total = 0;
            var counter = 0;
            foreach (var argRaw in args)
            {
                // preparation

                var sign = '+';
                var argValue = argRaw;
                if (argValue.StartsWith("+") ||
                    argValue.StartsWith("-"))
                {
                    sign = argValue[0];
                    argValue = argValue.Substring(1);
                }

                var values = argValue.ToLower().Split("d");
                if (values.Length == 1 && int.TryParse(values[0], out var rawNumber))
                {
                    CalculateNextValue(rawNumber, Die.GetSign(sign), sb, ref separator, ref total, ref counter);
                    continue;
                }
                else if (values.Length != 2)
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

                // calculation

                for (int i = 0; i < die.Loop; i++)
                {
                    var next =
                        die.Fudge ? _random.Next(-1, 2) :
                        _random.Next(1, die.Amount + 1);
                    CalculateNextValue(next, die.Sign, sb, ref separator, ref total, ref counter);
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

        private static void CalculateNextValue(int next, Sign sign, StringBuilder sb, ref string separator, ref int total, ref int counter)
        {
            if (next < 0)
            {
                next *= -1;
                sign = sign switch
                {
                    Sign.Positive => sign = Sign.Negative,
                    Sign.Negative => sign = Sign.Positive,
                    _ => throw new Exception(string.Format("INTERNAL ERROR: Unknown sign: {0}", sign)),
                };
            }

            if (sign == Sign.Negative)
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

        public class Die
        {
            private Die(int amount, int loop, Sign sign, bool fudge)
            {
                Amount = amount;
                Loop = loop;
                Sign = sign;
                Fudge = fudge;
            }

            public int Amount { get; }
            public int Loop { get; }
            public Sign Sign { get; }
            public bool Fudge { get; }

            public static Die Create(int amount)
            {
                return new Die(amount, 1, Sign.Positive, false);
            }

            public static Die Create(int amount, int loop, Sign sign, bool fudge)
            {
                return new Die(amount, loop, sign, fudge);
            }

            public static Die Create(string amount, string loop, char sign)
            {
                if (loop == string.Empty)
                {
                    loop = "1";
                }

                var parsedSign = GetSign(sign);

                var parsedAmount = 1;
                var fudge = false;
                if (amount.ToUpper() == "F")
                {
                    fudge = true;
                }
                else if (!int.TryParse(amount, out parsedAmount) || parsedAmount < 1)
                {
                    return null;
                }

                if (int.TryParse(loop, out var parsedLoop))
                {
                    return Die.Create(
                        parsedAmount,
                        parsedLoop,
                        parsedSign,
                        fudge);
                }
                else
                {
                    return null;
                }
            }

            public static Sign GetSign(char sign)
            {
                return sign == '-' ? Sign.Negative : Sign.Positive;
            }
        }

        public enum Sign
        {
            Positive,
            Negative,
        }
    }
}
