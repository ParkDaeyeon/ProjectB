using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Collection
{
    public static class ArrayExtension
    {
        // -------------------------
        // NOTE: sbyte
        // -------------------------
        public static int IndexOf(this sbyte[] array, int startIdx, int endIdx, sbyte value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this sbyte[] array, int startIdx, int endIdx, sbyte value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this sbyte[] array, int startIdx, int endIdx, sbyte value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: byte
        // -------------------------
        public static int IndexOf(this byte[] array, int startIdx, int endIdx, byte value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this byte[] array, int startIdx, int endIdx, byte value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this byte[] array, int startIdx, int endIdx, byte value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: short
        // -------------------------
        public static int IndexOf(this short[] array, int startIdx, int endIdx, short value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this short[] array, int startIdx, int endIdx, short value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this short[] array, int startIdx, int endIdx, short value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: ushort
        // -------------------------
        public static int IndexOf(this ushort[] array, int startIdx, int endIdx, ushort value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this ushort[] array, int startIdx, int endIdx, ushort value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this ushort[] array, int startIdx, int endIdx, ushort value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: int
        // -------------------------
        public static int IndexOf(this int[] array, int startIdx, int endIdx, int value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this int[] array, int startIdx, int endIdx, int value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            
            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this int[] array, int startIdx, int endIdx, int value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: uint
        // -------------------------
        public static int IndexOf(this uint[] array, int startIdx, int endIdx, uint value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this uint[] array, int startIdx, int endIdx, uint value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this uint[] array, int startIdx, int endIdx, uint value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: long
        // -------------------------
        public static int IndexOf(this long[] array, int startIdx, int endIdx, long value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this long[] array, int startIdx, int endIdx, long value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this long[] array, int startIdx, int endIdx, long value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: ulong
        // -------------------------
        public static int IndexOf(this ulong[] array, int startIdx, int endIdx, ulong value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this ulong[] array, int startIdx, int endIdx, ulong value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this ulong[] array, int startIdx, int endIdx, ulong value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: float
        // -------------------------
        public static int IndexOf(this float[] array, int startIdx, int endIdx, float value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this float[] array, int startIdx, int endIdx, float value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this float[] array, int startIdx, int endIdx, float value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: double
        // -------------------------
        public static int IndexOf(this double[] array, int startIdx, int endIdx, double value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf(this double[] array, int startIdx, int endIdx, double value)
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, value.GetType().Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains(this double[] array, int startIdx, int endIdx, double value)
        {
            return -1 != array.IndexOf(startIdx, endIdx, value);
        }


        // -------------------------
        // NOTE: class
        // -------------------------
        public static int IndexOf<T>(this T[] array, int startIdx, int endIdx, T value) where T : class
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF(CLASS)_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF(CLASS)_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));

            for (int n = startIdx; n < endIdx; ++n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static int LastIndexOf<T>(this T[] array, int startIdx, int endIdx, T value) where T : class
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF(CLASS)_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF(CLASS)_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (array[n] == value)
                    return n;

            return -1;
        }
        public static bool Contains<T>(this T[] array, int startIdx, int endIdx, T value) where T : class
        {
            return -1 != array.IndexOf<T>(startIdx, endIdx, value);
        }



        // -------------------------
        // NOTE: struct
        // -------------------------
        public static int IndexOf_Struct<T>(this T[] array, int startIdx, int endIdx, T value) where T : struct
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF(STRUCT)_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF(STRUCT)_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));

            EqualityComparer<T> compare = EqualityComparer<T>.Default;

            for (int n = startIdx; n < endIdx; ++n)
                if (compare.Equals(array[n], value))
                    return n;

            return -1;
        }
        public static int LastIndexOf_Struct<T>(this T[] array, int startIdx, int endIdx, T value) where T : struct
        {
            if (null == array)
                throw new NullReferenceException(string.Format("INDEXOF(STRUCT)_ARRAY_IS_NULL! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));
            if (0 > startIdx)
                throw new ArgumentException(string.Format("INDEXOF(STRUCT)_INVALID_ARGS! START:{0}, END:{1}, VALUE:{2}, TYPE:{3}", startIdx, endIdx, value, typeof(T).Name));

            EqualityComparer<T> compare = EqualityComparer<T>.Default;

            for (int n = endIdx - 1; n >= startIdx; --n)
                if (compare.Equals(array[n], value))
                    return n;

            return -1;
        }
        public static bool Contains_Struct<T>(this T[] array, int startIdx, int endIdx, T value) where T : struct
        {
            return -1 != array.IndexOf_Struct<T>(startIdx, endIdx, value);
        }




        // NOTE: Fisher–Yates Shuffle (algorithm)
        public static void Shuffle<T>(this T[] array)
        {
            if (null == array)
                return;

            int count = array.Length;
            for (int n = 0; n < count; ++n)
            {
                int indexToSwap = SystemExtension.RandomRange(n, count);
                T oldValue = array[n];
                array[n] = array[indexToSwap];
                array[indexToSwap] = oldValue;
            }
        }

        public static void ShuffleWithSeed<T>(this T[] array, int seed)
        {
            if (null == array)
                return;

            int count = array.Length;
            SystemExtension.RandomSeed(seed);
            for (int n = 0; n < count; ++n)
            {
                //int indexToSwap = SystemExtension.RandomRange(n, count);
                int indexToSwap = SystemExtension.RandomRange(n, count);
                T oldValue = array[n];
                array[n] = array[indexToSwap];
                array[indexToSwap] = oldValue;
            }
        }

        public static T[] Trim<T>(this T[] array, int start, int count)
        {
            if (null == array)
                return default(T[]);

            var newArray = new T[count];
            for (int n = 0; n < count; ++n)
                newArray[n] = array[start + n];

            return newArray;
        }


        interface INumberPolicy<T>
        {
            T ToValue(int number);
        }

        class NumberPolicy_sbyte    : INumberPolicy<    sbyte   >{ public   sbyte   ToValue(int number) { return    (sbyte  )number; } }
        class NumberPolicy_byte     : INumberPolicy<    byte    >{ public   byte    ToValue(int number) { return    (byte   )number; } }
        class NumberPolicy_short    : INumberPolicy<    short   >{ public   short   ToValue(int number) { return    (short  )number; } }
        class NumberPolicy_ushort   : INumberPolicy<    ushort  >{ public   ushort  ToValue(int number) { return    (ushort )number; } }
        class NumberPolicy_int      : INumberPolicy<    int     >{ public   int     ToValue(int number) { return    (int    )number; } }
        class NumberPolicy_uint     : INumberPolicy<    uint    >{ public   uint    ToValue(int number) { return    (uint   )number; } }
        class NumberPolicy_long     : INumberPolicy<    long    >{ public   long    ToValue(int number) { return    (long   )number; } }
        class NumberPolicy_ulong    : INumberPolicy<    ulong   >{ public   ulong   ToValue(int number) { return    (ulong  )number; } }
        class NumberPolicy_float    : INumberPolicy<    float   >{ public   float   ToValue(int number) { return    (float  )number; } }
        class NumberPolicy_double   : INumberPolicy<    double  >{ public   double  ToValue(int number) { return    (double )number; } }

        static readonly NumberPolicy_sbyte  numberPolicy_sbyte  = new   NumberPolicy_sbyte  ();
        static readonly NumberPolicy_byte   numberPolicy_byte   = new   NumberPolicy_byte   ();
        static readonly NumberPolicy_short  numberPolicy_short  = new   NumberPolicy_short  ();
        static readonly NumberPolicy_ushort numberPolicy_ushort = new   NumberPolicy_ushort ();
        static readonly NumberPolicy_int    numberPolicy_int    = new   NumberPolicy_int    ();
        static readonly NumberPolicy_uint   numberPolicy_uint   = new   NumberPolicy_uint   ();
        static readonly NumberPolicy_long   numberPolicy_long   = new   NumberPolicy_long   ();
        static readonly NumberPolicy_ulong  numberPolicy_ulong  = new   NumberPolicy_ulong  ();
        static readonly NumberPolicy_float  numberPolicy_float  = new   NumberPolicy_float  ();
        static readonly NumberPolicy_double numberPolicy_double = new   NumberPolicy_double ();

        static T[] ToIndexArray<T>(T count_, INumberPolicy<T> parser)
        {
            var count = count_.GetHashCode();
            if (0 >= count)
                return new T[0];

            var array = new T[count];
            for (int n = 0; n < count; ++n)
                array[n] = parser.ToValue(n);

            return array;
        }

        static T[] ToIndexArrayWithRandom<T>(T count_, INumberPolicy<T> parser)
        {
            var array = ArrayExtension.ToIndexArray<T>(count_, parser);
            array.Shuffle();
            return array;
        }

        public static   sbyte   [] ToIndexArray(this    sbyte   count) { return ArrayExtension.ToIndexArray<    sbyte   >(count, ArrayExtension.numberPolicy_sbyte  ); }
        public static   byte    [] ToIndexArray(this    byte    count) { return ArrayExtension.ToIndexArray<    byte    >(count, ArrayExtension.numberPolicy_byte   ); }
        public static   short   [] ToIndexArray(this    short   count) { return ArrayExtension.ToIndexArray<    short   >(count, ArrayExtension.numberPolicy_short  ); }
        public static   ushort  [] ToIndexArray(this    ushort  count) { return ArrayExtension.ToIndexArray<    ushort  >(count, ArrayExtension.numberPolicy_ushort ); }
        public static   int     [] ToIndexArray(this    int     count) { return ArrayExtension.ToIndexArray<    int     >(count, ArrayExtension.numberPolicy_int    ); }
        public static   uint    [] ToIndexArray(this    uint    count) { return ArrayExtension.ToIndexArray<    uint    >(count, ArrayExtension.numberPolicy_uint   ); }
        public static   long    [] ToIndexArray(this    long    count) { return ArrayExtension.ToIndexArray<    long    >(count, ArrayExtension.numberPolicy_long   ); }
        public static   ulong   [] ToIndexArray(this    ulong   count) { return ArrayExtension.ToIndexArray<    ulong   >(count, ArrayExtension.numberPolicy_ulong  ); }
        public static   float   [] ToIndexArray(this    float   count) { return ArrayExtension.ToIndexArray<    float   >(count, ArrayExtension.numberPolicy_float  ); }
        public static   double  [] ToIndexArray(this    double  count) { return ArrayExtension.ToIndexArray<    double  >(count, ArrayExtension.numberPolicy_double ); }

        

        public static   sbyte   [] ToIndexArrayWithRandom(this  sbyte   count) { return ArrayExtension.ToIndexArrayWithRandom<  sbyte   >(count, ArrayExtension.numberPolicy_sbyte  ); }
        public static   byte    [] ToIndexArrayWithRandom(this  byte    count) { return ArrayExtension.ToIndexArrayWithRandom<  byte    >(count, ArrayExtension.numberPolicy_byte   ); }
        public static   short   [] ToIndexArrayWithRandom(this  short   count) { return ArrayExtension.ToIndexArrayWithRandom<  short   >(count, ArrayExtension.numberPolicy_short  ); }
        public static   ushort  [] ToIndexArrayWithRandom(this  ushort  count) { return ArrayExtension.ToIndexArrayWithRandom<  ushort  >(count, ArrayExtension.numberPolicy_ushort ); }
        public static   int     [] ToIndexArrayWithRandom(this  int     count) { return ArrayExtension.ToIndexArrayWithRandom<  int     >(count, ArrayExtension.numberPolicy_int    ); }
        public static   uint    [] ToIndexArrayWithRandom(this  uint    count) { return ArrayExtension.ToIndexArrayWithRandom<  uint    >(count, ArrayExtension.numberPolicy_uint   ); }
        public static   long    [] ToIndexArrayWithRandom(this  long    count) { return ArrayExtension.ToIndexArrayWithRandom<  long    >(count, ArrayExtension.numberPolicy_long   ); }
        public static   ulong   [] ToIndexArrayWithRandom(this  ulong   count) { return ArrayExtension.ToIndexArrayWithRandom<  ulong   >(count, ArrayExtension.numberPolicy_ulong  ); }
        public static   float   [] ToIndexArrayWithRandom(this  float   count) { return ArrayExtension.ToIndexArrayWithRandom<  float   >(count, ArrayExtension.numberPolicy_float  ); }
        public static   double  [] ToIndexArrayWithRandom(this  double  count) { return ArrayExtension.ToIndexArrayWithRandom<  double  >(count, ArrayExtension.numberPolicy_double ); }



        public static int[] ToIndexArrayAdvanced(int countMin, int countMax, int startIndex, int endIndex)
        {
            if (0 > countMin || countMin > countMax || 0 > startIndex || startIndex > endIndex)
            {
                throw new ArgumentException(string.Format("GENERATE_UNIQUE_RANDOM_INDEXES_FAILED:INVALID_ARGS:C_MIN:{0}, C_MAX:{1}, S:{2}, E:{3}",
                                                            countMin, countMax, startIndex, endIndex));
            }

            int randomRange = endIndex - startIndex;

            if (countMax > randomRange)
            {
                throw new ArgumentException(string.Format("GENERATE_UNIQUE_RANDOM_INDEXES_FAILED:CANT_GENERATE:RANDOM_RANGE:{0}, C_MIN:{1}, C_MAX:{2}, S:{3}, E:{4}",
                                                           randomRange, countMin, countMax, startIndex, endIndex));
            }


            // NOTE: Linear Interpolation
            double lerp = 0;
            {
                var t = SystemExtension.RandomValue;
                lerp = countMin + (countMax - countMin) * t;
            }
            int count = (int)Math.Round(lerp);
            if (0 >= count)
                return new int[0];

            int[] randomArray = new int[randomRange];
            for (int n = 0; n < randomRange; ++n)
                randomArray[n] = n + startIndex;
            randomArray.Shuffle();

            int[] indexArray = new int[count];
            for (int n = 0; n < count; ++n)
                indexArray[n] = randomArray[n];

            return indexArray;
        }



        public static void InsertionSort<T>(IList<T> list, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException("ARRAY_EXTENSION:LIST_IS_NULL");
            if (comparison == null)
                throw new ArgumentNullException("ARRAY_EXTENSION:COMPARISON_IS_NULL");

            int count = list.Count;
            for (int n = 1; n < count; ++n)
            {
                T key = list[n];

                int n2 = n - 1;
                for (; n2 >= 0 && comparison(list[n2], key) > 0; --n2)
                    list[n2 + 1] = list[n2];

                list[n2 + 1] = key;
            }
        }


        public static T GetAtSafe<T>(this T[] array, int index)
        {
            if (null != array)
            {
                if (-1 < index && index < array.Length)
                    return array[index];
            }

            return default(T);
        }

        public static int GetLengthSafe<T>(this T[] array)
        {
            return null != array ? array.Length : 0;
        }
        

        public static T[] ToArray<T>(this Array sysArray)
        {
            var array = new T[sysArray.Length];
            for (int n = 0, cnt = array.Length; n < cnt; ++n)
                array[n] = (T)sysArray.GetValue(n);
            return array;
        }
    }
}