using System;
using System.Collections.Generic;
using System.Text;

namespace Ext
{
    public static class RomanNumerals
    {
        public static readonly string[] ThouLetters = { "", "M", "MM", "MMM" };
        public static readonly string[] HundLetters = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        public static readonly string[] TensLetters = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        public static readonly string[] OnesLetters = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

        static StringBuilder sb = new StringBuilder(128);
        public static string ToString(int value)
        {
            var sb = RomanNumerals.sb;
            sb.Length = 0;
            
            // See if it's >= 4000.
            if (value >= 4000)
            {
                // Use parentheses.
                sb.Append('(');
                RomanNumerals.AppendRoman(value / 1000);
                sb.Append(')');

                value %= 1000;
            }

            RomanNumerals.AppendRoman(value);
            return sb.ToString();
        }

        static void AppendRoman(int value)
        {
            var sb = RomanNumerals.sb;

            // Pull out thousands.
            sb.Append(RomanNumerals.ThouLetters[value / 1000]);
            value %= 1000;

            // Handle hundreds.
            sb.Append(RomanNumerals.HundLetters[value / 100]);
            value %= 100;

            // Handle tens.
            sb.Append(RomanNumerals.TensLetters[value / 10]);
            value %= 10;

            // Handle ones.
            sb.Append(RomanNumerals.OnesLetters[value]);
        }

        
        public static int ToInt(char value)
        {
            switch (value)
            {
            case 'i':
            case 'I': return 1;

            case 'v':
            case 'V': return 5;

            case 'x':
            case 'X': return 10;

            case 'l':
            case 'L': return 50;

            case 'c':
            case 'C': return 100;

            case 'd':
            case 'D': return 500;

            case 'm':
            case 'M': return 1000;
            }

            return 0;
        }
        public static int ToInt(string value)
        {
            // See if the number begins with (.
            if (value[0] == '(')
            {
                // Find the closing parenthesis.
                int pos = value.LastIndexOf(')');

                // Get the value inside the parentheses.
                string part1 = value.Substring(1, pos - 1);
                string part2 = value.Substring(pos + 1);
                return 1000 * RomanNumerals.ToInt(part1) + RomanNumerals.ToInt(part2);
            }


            var number = 0;
            var prev = 0;

            for (int n = value.Length - 1; n >= 0; --n)
            {
                var current = RomanNumerals.ToInt(value[n]);
                if (0 == current)
                    continue;

                if (current < prev)
                    number -= current;
                else
                {
                    number += current;
                    prev = current;
                }
            }
            return number;
        }
    }
}
