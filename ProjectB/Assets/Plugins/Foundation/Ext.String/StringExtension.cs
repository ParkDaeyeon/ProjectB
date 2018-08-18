using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
namespace Ext.String
{
    public static class StringExtension
    {
        public static string ToCommaSeparatedStringS(this short value, int digit = 3) { return StringExtension.ToSeparatedString(value, ',', digit); }
        public static string ToCommaSeparatedStringUS(this ushort value, int digit = 3) { return StringExtension.ToSeparatedString(value, ',', digit); }
        public static string ToCommaSeparatedStringI(this int value, int digit = 3) { return StringExtension.ToSeparatedString(value, ',', digit); }
        public static string ToCommaSeparatedStringUI(this uint value, int digit = 3) { return StringExtension.ToSeparatedString(value, ',', digit); }
        public static string ToCommaSeparatedStringL(this long value, int digit = 3) { return StringExtension.ToSeparatedString(value, ',', digit); }
        public static string ToCommaSeparatedStringUL(this ulong value, int digit = 3) { return StringExtension.ToSeparatedString(value, ',', digit); }

        public static string ToCommaSeparatedStringF(this float value, int digit = 3, int decimalDigit = 2, bool forceDigit = false) { return StringExtension.ToSeparatedString(value, ',', digit, decimalDigit, forceDigit); }
        public static string ToCommaSeparatedStringR(this double value, int digit = 3, int decimalDigit = 2, bool forceDigit = false) { return StringExtension.ToSeparatedString(value, ',', digit, decimalDigit, forceDigit); }
        public static string ToCommaSeparatedStringM(this decimal value, int digit = 3, int decimalDigit = 2, bool forceDigit = false) { return StringExtension.ToSeparatedString(value, ',', digit, decimalDigit, forceDigit); }

        static StringBuilder sbCommaSeparated = new StringBuilder();
        public static string ToSeparatedString(IFormattable value, char separate = ',', int digit = 3, int decimalDigit = -1, bool forceDigit = false)
        {
            var sb = StringExtension.sbCommaSeparated;
            sb.Length = 0;
            sb.Append('#').Append(separate);
            for (int n = 1; n < digit; ++n)
                sb.Append('#');
            sb.Append('0');

            if (-1 < decimalDigit)
            {
                sb.Append('.');
                if (forceDigit)
                {
                    for (int n = 0; n < decimalDigit; ++n)
                        sb.Append('0');
                }
                else
                {
                    for (int n = 0; n < decimalDigit; ++n)
                        sb.Append('#');
                }
            }

            return value.ToString(sb.ToString(), null);
        }



        public static bool IsValidEmail(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(value, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        }


        public static bool IsValidLength(this string value, int min, int max)
        {
            if (null == value)
                return false;

            int len = value.Length;
            return min <= len && len <= max;
        }



        public static byte[] Utf16ToUtf8(this string utf16String)
        {
            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Return UTF8 bytes as ANSI string
            return utf8Bytes;
        }

        public static string GetUnicodeString(this string s)
        {
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                sb.Append("\\u");
                sb.Append(string.Format("{0:x4}", (int)c));
            }
            return sb.ToString();
        }

        public static StringBuilder AppendIndent(this StringBuilder sb, int depth, int spacePerDepth = 4)
        {
            for (int n = 0; n < depth; ++n)
            {
                for (int n2 = 0; n2 < spacePerDepth; ++n2)
                    sb.Append(' ');
            }

            return sb;
        }

        public static StringBuilder AppendIndentText(this StringBuilder sb, int depth, string text, int spacePerDepth = 4)
        {
            return sb.AppendIndent(depth, spacePerDepth).Append(text);
        }

        public static StringBuilder AppendOpenBrace(this StringBuilder sb, ref int depth, int spacePerDepth = 4)
        {
            return sb.AppendIndent(depth++, spacePerDepth).Append('{');
        }

        public static StringBuilder AppendCloseBrace(this StringBuilder sb, ref int depth, int spacePerDepth = 4)
        {
            return sb.AppendIndent(--depth, spacePerDepth).Append('}');
        }

        public static StringBuilder AppendOpenArray(this StringBuilder sb, ref int depth, int spacePerDepth = 4)
        {
            return sb.AppendIndent(depth++, spacePerDepth).Append('[');
        }

        public static StringBuilder AppendCloseArray(this StringBuilder sb, ref int depth, int spacePerDepth = 4)
        {
            return sb.AppendIndent(--depth, spacePerDepth).Append(']');
        }



        public static string FormatJson(this string str, int spacePerDepth = 4)
        {
            var depth = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                case '{':
                case '[':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, ++depth).ForEachAction(item => { for (int n2 = 0; n2 < spacePerDepth; ++n2) sb.Append(' '); });
                    }
                    break;
                case '}':
                case ']':
                    if (!quoted)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, --depth).ForEachAction(item => { for (int n2 = 0; n2 < spacePerDepth; ++n2) sb.Append(' '); });
                    }
                    sb.Append(ch);
                    break;
                case '"':
                    sb.Append(ch);
                    bool escaped = false;
                    var index = i;
                    while (index > 0 && str[--index] == '\\')
                        escaped = !escaped;
                    if (!escaped)
                        quoted = !quoted;
                    break;
                case ',':
                    sb.Append(ch);
                    if (!quoted)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, depth).ForEachAction(item => { for (int n2 = 0; n2 < spacePerDepth; ++n2) sb.Append(' '); });
                    }
                    break;
                case ':':
                    sb.Append(ch);
                    if (!quoted)
                        sb.Append(" ");
                    break;
                default:
                    sb.Append(ch);
                    break;
                }
            }
            return sb.ToString();
        }

        public static string ToZeroFilledString(this long value, int digitCount, long? prefix = null, bool isStrict = false)
        {
            StringBuilder sb = new StringBuilder(digitCount);

            string prefixAbsStr = null;
            int prefixDigitLength = 0;
            if (prefix.HasValue)
            {
                prefixAbsStr = Math.Abs(prefix.Value).ToString();
                prefixDigitLength = prefixAbsStr.Length;
            }

            string absValueStr = Math.Abs(value).ToString();
            int zeroCount = digitCount - prefixDigitLength - absValueStr.Length;

            if (isStrict)
            {
                if (0 > zeroCount)
                {
                    throw new ArgumentOutOfRangeException("VALUE:" + value + ", DIGIT_COUNT:" + digitCount);
                }
            }

            if (0 > value || (prefix.HasValue && 0 > prefix.Value))
                sb.Append('-');

            if (null != prefixAbsStr)
                sb.Append(prefixAbsStr);

            for (int n = 0; n < zeroCount; ++n)
                sb.Append(0);

            sb.Append(absValueStr);

            return sb.ToString();
        }

        public static long ToLong(this string zeroFilledString, int trimCount = 0)
        {
            if (string.IsNullOrEmpty(zeroFilledString))
                return 0;

            bool isNegative = false;
            if (0 < trimCount)
            {
                if ('-' == zeroFilledString[0])
                {
                    ++trimCount;
                    isNegative = true;
                }

                if (zeroFilledString.Length <= trimCount)
                    return 0;

                zeroFilledString = zeroFilledString.Substring(trimCount);
            }

            long number;
            if (long.TryParse(zeroFilledString, out number))
            {
                if (isNegative && 0 <= number)
                    number = -number;

                return number;
            }

            return 0;
        }

        public static string TrimChar(this string str, char token)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (1 < str.Length && token == str[0] && token == str[str.Length - 1])
                    str = str.Substring(1, str.Length - 2);
            }

            return str;
        }


        public delegate T ToValue<T>(string str);
        public static T[] ToArray<T>(this string str, ToValue<T> parser, params char[] separator)
        {
            if (null == str)
                return new T[0];

            var split = str.Split(separator);
            var values = new T[split.Length];
            for (int n = 0, cnt = split.Length; n < cnt; ++n)
            {
                try
                {
                    values[n] = parser(split[n]);
                }
                catch (Exception e)
                {
                    throw new ToArrayFailedException
                    {
                        Index = n,
                        Split = split[n],
                        Str = str,
                        Except = e,
                    };
                }
            }

            return values;
        }

        public class ToArrayFailedException : Exception
        {
            public int Index;
            public string Split;
            public string Str;
            public Exception Except;

            public override string ToString()
            {
                return string.Format("INDEX:{0}, SPLIT:{1}, STR:{2},\nEXCEPT:{3}", this.Index, this.Split, this.Str, this.Except.ToString());
            }
        }



        //public static bool IsNumber(char ch)
        //{
        //    return '0' <= ch && ch <= '9';
        //}
        static int IntPow(int x, uint pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        public static int ExtractInt(this string value, int start, int count)
        {
            var intValue = 0;
            for (int n = 0, digit = (count-1); n < count; ++n, --digit)
            {
                var ch = value[start + n];
                if (!char.IsNumber(ch))
                    continue;

                var number = ch - '0';
                if (0 == number)
                    continue;

                var scale = StringExtension.IntPow(10, (uint)digit);
                intValue += number * scale;
            }

            return intValue;
        }

        public static bool IsYYYY_MM_DD(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (value.Length != "YYYY.MM.DD".Length)
                return false;

            int idx = 0;
            if (!char.IsNumber(value[idx++]) || // NOTE: [0]: Y
                !char.IsNumber(value[idx++]) || // NOTE: [1]: Y
                !char.IsNumber(value[idx++]) || // NOTE: [2]: Y
                !char.IsNumber(value[idx++]) || // NOTE: [3]: Y
                !('.' == value[idx++])       || // NOTE: [4]: .
                !char.IsNumber(value[idx++]) || // NOTE: [5]: M
                !char.IsNumber(value[idx++]) || // NOTE: [6]: M
                !('.' == value[idx++])       || // NOTE: [7]: .
                !char.IsNumber(value[idx++]) || // NOTE: [8]: D
                !char.IsNumber(value[idx++]))   // NOTE: [9]: D
                return false;

            int month = value.ExtractInt(5, 2);
            if (1 > month || month > 12)
                return false;

            int day = value.ExtractInt(8, 2);
            if (1 > day || day > 31)
                return false;

            return true;
        }

        public struct YYYY_MM_DD
        {
            int year;
            public int Year { get { return this.year; } }
            
            int month;
            public int Month { get { return this.month; } }

            int day;
            public int Day { get { return this.day; } }

            public YYYY_MM_DD(int year, int month, int day)
            {
                this.year = year;
                this.month = month;
                this.day = day;
            }

            public override string ToString()
            {
                return string.Format("{0:D4}.{1:D2}.{2:D2}", this.year, this.month, this.day);
            }

            public const string DefaultText = "0000.01.01";
            public static readonly YYYY_MM_DD Empty = new YYYY_MM_DD(0, 1, 1);
        }
        public static YYYY_MM_DD ToYYYY_MM_DD(this string value)
        {
            if (!value.IsYYYY_MM_DD())
                value = YYYY_MM_DD.DefaultText;

            return new YYYY_MM_DD(value.ExtractInt(0, 4),
                                  value.ExtractInt(5, 2),
                                  value.ExtractInt(8, 2));
        }
    }
}