using System;

namespace Ext.Algorithm
{
    public static class BitFlag
    {
        // -----------------------------------------------------------
        // NOTE: 1 BYTE
        // -----------------------------------------------------------
        public static sbyte Add(sbyte flags, sbyte value)
        {
            // NOTE: CS0675 Bug
#pragma warning disable 0675
            return (sbyte)(flags | value);
#pragma warning restore 0675
        }
        public static byte Add(byte flags, byte value)
        {
            return (byte)(flags | value);
        }

        public static sbyte Remove(sbyte flags, sbyte value)
        {
            return (sbyte)(flags & (~value));
        }
        public static byte Remove(byte flags, byte value)
        {
            return (byte)(flags & (~value));
        }

        public static sbyte Toggle(sbyte flags, sbyte value)
        {
            return (sbyte)(flags ^ value);
        }
        public static byte Toggle(byte flags, byte value)
        {
            return (byte)(flags ^ value);
        }

        public static bool Contains(sbyte flags, sbyte value)
        {
            return 0 != (flags & value);
        }
        public static bool Contains(byte flags, byte value)
        {
            return 0 != (flags & value);
        }

        public static sbyte SetField(sbyte flags, int index, bool value)
        {
            var field = BitFlag.GetField8(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }
        public static byte SetField(byte flags, int index, bool value)
        {
            var field = BitFlag.GetField8U(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }

        public static sbyte GetField8(int index)
        {
            if (index < 0 || 7 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_8:INDEX:{0}", index));

            return (sbyte)(1 << index);
        }
        public static byte GetField8U(int index)
        {
            if (index < 0 || 7 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_8U:INDEX:{0}", index));

            return (byte)(1 << index);
        }

        public static int Count(sbyte flags)
        {
            return BitFlag.Count((int)flags);
        }
        public static int Count(byte flags)
        {
            return BitFlag.Count((int)flags);
        }


        // -----------------------------------------------------------
        // NOTE: 2 BYTE
        // -----------------------------------------------------------
        public static short Add(short flags, short value)
        {
            return (short)(flags | value);
        }
        public static ushort Add(ushort flags, ushort value)
        {
            return (ushort)(flags | value);
        }

        public static short Remove(short flags, short value)
        {
            return (short)(flags & (~value));
        }
        public static ushort Remove(ushort flags, ushort value)
        {
            return (ushort)(flags & (~value));
        }

        public static short Toggle(short flags, short value)
        {
            return (short)(flags ^ value);
        }
        public static ushort Toggle(ushort flags, ushort value)
        {
            return (ushort)(flags ^ value);
        }

        public static bool Contains(short flags, short value)
        {
            return 0 != (flags & value);
        }
        public static bool Contains(ushort flags, ushort value)
        {
            return 0 != (flags & value);
        }

        public static short SetField(short flags, int index, bool value)
        {
            var field = BitFlag.GetField16(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }
        public static ushort SetField(ushort flags, int index, bool value)
        {
            var field = BitFlag.GetField16U(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }

        public static short GetField16(int index)
        {
            if (index < 0 || 15 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_16:INDEX:{0}", index));

            return (short)(1 << index);
        }
        public static ushort GetField16U(int index)
        {
            if (index < 0 || 15 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_16U:INDEX:{0}", index));

            return (ushort)(1 << index);
        }

        public static int Count(short flags)
        {
            return BitFlag.Count((int)flags);
        }
        public static int Count(ushort flags)
        {
            return BitFlag.Count((int)flags);
        }


        // -----------------------------------------------------------
        // NOTE: 4 BYTE
        // -----------------------------------------------------------
        public static int Add(int flags, int value)
        {
            return flags | value;
        }
        public static uint Add(uint flags, uint value)
        {
            return flags | value;
        }

        public static int Remove(int flags, int value)
        {
            return flags & (~value);
        }
        public static uint Remove(uint flags, uint value)
        {
            return flags & (~value);
        }

        public static int Toggle(int flags, int value)
        {
            return flags ^ value;
        }
        public static uint Toggle(uint flags, uint value)
        {
            return flags ^ value;
        }

        public static bool Contains(int flags, int value)
        {
            return 0 != (flags & value);
        }
        public static bool Contains(uint flags, uint value)
        {
            return 0 != (flags & value);
        }

        public static int SetField(int flags, int index, bool value)
        {
            var field = BitFlag.GetField32(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }
        public static uint SetField(uint flags, int index, bool value)
        {
            var field = BitFlag.GetField32U(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }

        public static int GetField32(int index)
        {
            if (index < 0 || 31 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_32:INDEX:{0}", index));

            return 1 << index;
        }
        public static uint GetField32U(int index)
        {
            if (index < 0 || 31 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_32U:INDEX:{0}", index));

            return 1U << index;
        }

        public static int Count(int flags)
        {
            unchecked
            {
                flags = flags - ((flags >> 1) & 0x55555555);
                flags = (flags & 0x33333333) + ((flags >> 2) & 0x33333333);
                return (((flags + (flags >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
            }
        }
        public static int Count(uint flags)
        {
            unchecked
            {
                flags = flags - ((flags >> 1) & 0x55555555U);
                flags = (flags & 0x33333333U) + ((flags >> 2) & 0x33333333U);
                return (int)((((flags + (flags >> 4)) & 0x0F0F0F0FU) * 0x01010101U) >> 24);
            }
        }


        // -----------------------------------------------------------
        // NOTE: 8 BYTE
        // -----------------------------------------------------------
        public static long Add(long flags, long value)
        {
            return flags | value;
        }
        public static ulong Add(ulong flags, ulong value)
        {
            return flags | value;
        }

        public static long Remove(long flags, long value)
        {
            return flags & (~value);
        }
        public static ulong Remove(ulong flags, ulong value)
        {
            return flags & (~value);
        }

        public static long Toggle(long flags, long value)
        {
            return flags ^ value;
        }
        public static ulong Toggle(ulong flags, ulong value)
        {
            return flags ^ value;
        }

        public static bool Contains(long flags, long value)
        {
            return 0 != (flags & value);
        }
        public static bool Contains(ulong flags, ulong value)
        {
            return 0 != (flags & value);
        }

        public static long SetField(long flags, int index, bool value)
        {
            var field = BitFlag.GetField64(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }
        public static ulong SetField(ulong flags, int index, bool value)
        {
            var field = BitFlag.GetField64U(index);
            return value ? BitFlag.Add(flags, field) : BitFlag.Remove(flags, field);
        }

        public static long GetField64(int index)
        {
            if (index < 0 || 63 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_64:INDEX:{0}", index));

            return 1L << index;
        }
        public static ulong GetField64U(int index)
        {
            if (index < 0 || 63 < index)
                throw new IndexOutOfRangeException(string.Format("BITFLAG:GET_FIELD_64U:INDEX:{0}", index));

            return 1UL << index;
        }
        
        public static int Count(long flags)
        {
            unchecked
            {
                flags = flags - ((flags >> 1) & 0x5555555555555555);
                flags = (flags & 0x3333333333333333) + ((flags >> 2) & 0x3333333333333333);
                return (int)((((flags + (flags >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
            }
        }
        public static int Count(ulong flags)
        {
            unchecked
            {
                flags = flags - ((flags >> 1) & 0x5555555555555555UL);
                flags = (flags & 0x3333333333333333UL) + ((flags >> 2) & 0x3333333333333333UL);
                return (int)((((flags + (flags >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
            }
        }
    }
}