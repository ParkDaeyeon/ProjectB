using System;
using System.Collections.Generic;
using System.IO;
namespace Ext
{
    public static class SystemExtension
    {
        public static void ForEachAction<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        public static Stream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        static Random randomSafe = new Random();
        static object randomSafeLock = new object();
        public static void RandomSeedSafe(int seed)
        {
            lock (SystemExtension.randomSafeLock)
                SystemExtension.randomSafe = new Random(seed);
        }
        public static int RandomRangeSafe(int min, int max)
        {
            lock (SystemExtension.randomSafeLock)
                return SystemExtension.randomSafe.Next(min, max);
        }
        public static double RandomValueSafe
        {
            get
            {
                lock (SystemExtension.randomSafeLock)
                    return SystemExtension.randomSafe.NextDouble();
            }
        }


        static Random random = new Random();
        public static void RandomSeed(int seed)
        {
            SystemExtension.random = new Random(seed);
        }
        public static int RandomRange(int min, int max)
        {
            return SystemExtension.random.Next(min, max);
        }
        public static double RandomValue
        {
            get
            {
                return SystemExtension.random.NextDouble();
            }
        }

        public static readonly DateTime DateNone = default(DateTime);



        public static string ToStringSafe(this object obj)
        {
            return null != obj ? obj.ToString() : "";
        }
        public static int IndexOfSafe(this string data, string keyword, bool inv = false)
        {
            var dataSafe = SystemExtension.ToStringSafe(data);
            var keywordSafe = SystemExtension.ToStringSafe(keyword);

            return inv ? keywordSafe.IndexOf(dataSafe) : dataSafe.IndexOf(keywordSafe);
        }

        public enum Filter
        {
            Contains,
            NotContains,
            ContainsInv,
            NotContainsInv,
            Equals,
            NotEquals,
        }
        public static bool ToFilter(this string data, string keyword, Filter filter = Filter.Contains)
        {
            switch (filter)
            {
            case Filter.Contains:
            case Filter.ContainsInv:
            case Filter.NotContains:
            case Filter.NotContainsInv:
                {
                    var inv = Filter.ContainsInv == filter || Filter.NotContainsInv == filter;
                    var index = SystemExtension.IndexOfSafe(data, keyword, inv);

                    switch (filter)
                    {
                    case Filter.Contains:
                    case Filter.ContainsInv:
                        return -1 != index;
                    case Filter.NotContains:
                    case Filter.NotContainsInv:
                        return -1 == index;
                    }

                    return false;
                }

            case Filter.Equals:
                return SystemExtension.ToStringSafe(data) == SystemExtension.ToStringSafe(keyword);
            case Filter.NotEquals:
                return SystemExtension.ToStringSafe(data) != SystemExtension.ToStringSafe(keyword);
            }

            return false;
        }


        
        public static uint ToHexRGB(float r, float g, float b)
        {
            return SystemExtension.ToHexRGB((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
        public static uint ToHexRGB(byte r, byte g, byte b)
        {
            return (((uint)r) << 16) + (((uint)r) << 8) + b;
        }

        public static uint ToHexRGBA(float r, float g, float b, float a)
        {
            return SystemExtension.ToHexRGBA((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
        }
        public static uint ToHexRGBA(byte r, byte g, byte b, byte a)
        {
            return (((uint)r) << 24) + (((uint)r) << 16) + (((uint)b) << 8) + a;
        }

        public static bool SequenceValueEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
            where T : struct
        {
            if (null == a)
                return null == b;
            if (null == b)
                return null == a;

            var count1 = 0;
            var count2 = 0;

            var e1 = a.GetEnumerator();
            var e2 = b.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                var value1 = e1.Current;
                var value2 = e2.Current;

                if (!Object.Equals(value1, value2))
                    return false;
            }

            return count1 == count2;
        }

        public static bool SequenceObjectEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
            where T : class
        {
            if (null == a)
                return null == b;
            if (null == b)
                return null == a;

            var count1 = 0;
            var count2 = 0;

            var e1 = a.GetEnumerator();
            var e2 = b.GetEnumerator();

            while (e1.MoveNext() && e2.MoveNext())
            {
                var value1 = e1.Current;
                var value2 = e2.Current;

                var null1 = null == value1;
                var null2 = null == value2;
                if (null1 == null2)
                {
                    if (null1)
                        continue;

                    if (!Object.Equals(value1, value2))
                        return false;
                }
                else
                    return false;
            }

            return count1 == count2;
        }
    }
}