using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
namespace Ext.String
{
    public static class NativeStringConverter
    {
        public static byte[] ToByte(this IntPtr p, int len)
        {
            if (IntPtr.Zero == p)
                return null;

            byte[] bytes = new byte[len];
            Marshal.Copy(p, bytes, 0, len);
            return bytes;
        }




        public static string ToManagedString(this IntPtr p)
        {
            if (IntPtr.Zero == p)
                return "";

            return Marshal.PtrToStringAnsi(p);
        }

        public static string ToManagedStringFromWide(this IntPtr p)
        {
            if (IntPtr.Zero == p)
                return "";

            return Marshal.PtrToStringUni(p);
        }

        public static string ToManagedStringFromEncoding(this IntPtr p, int len, Encoding encoding)
        {
            if (IntPtr.Zero == p || 0 == len)
                return "";

            return encoding.GetString(p.ToByte(len));
        }



        public static int strlen(this IntPtr p)
        {
            if (IntPtr.Zero == p)
                return 0;

            int len = 0;
            while (Marshal.ReadByte(p, len) != 0)
                ++len;
            return len;
        }

        public static string ToManagedStringFromUtf8(this IntPtr p)
        {
            return p.ToManagedStringFromEncoding(p.strlen(), Encoding.UTF8);
        }

        public static string ToManagedStringFromUtf8(this IntPtr p, int len)
        {
            return p.ToManagedStringFromEncoding(len, Encoding.UTF8);
        }




        public static string ToManagedStringFromMBCS(this IntPtr p)
        {
            return p.ToManagedStringFromEncoding(p.strlen(), Encoding.Default);
        }

        public static string ToManagedStringFromMBCS(this IntPtr p, int codePage)
        {
            return p.ToManagedStringFromEncoding(p.strlen(), Encoding.GetEncoding(codePage));
        }

        public static string ToManagedStringFromMBCS(this IntPtr p, int len, int codePage)
        {
            return p.ToManagedStringFromEncoding(len, Encoding.GetEncoding(codePage));
        }




        public static IntPtr ToNativeStr(this string managedString, out int len, Encoding encoding)
        {
            if (string.IsNullOrEmpty(managedString))
            {
                len = 0;
                return IntPtr.Zero;
            }

            len = encoding.GetByteCount(managedString) + 1;
            byte[] buffer = new byte[len];
            encoding.GetBytes(managedString, 0, managedString.Length, buffer, 0);
            buffer[len - 1] = 0;
            IntPtr p = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, p, buffer.Length);
            return p;
        }

        public static void FreeNativeStr(this IntPtr p)
        {
            if (IntPtr.Zero != p)
                Marshal.FreeHGlobal(p);
        }



        public static IntPtr ToNativeAscii(this string managedString, out int len)
        {
            return managedString.ToNativeStr(out len, Encoding.ASCII);
        }

        public static IntPtr ToNativeUtf8(this string managedString, out int len)
        {
            return managedString.ToNativeStr(out len, Encoding.UTF8);
        }




        public static IntPtr ToNativeMBCS(this string managedString, out int len)
        {
            return managedString.ToNativeStr(out len, Encoding.Default);
        }

        public static IntPtr ToNativeMBCS(this string managedString, out int len, int codePage)
        {
            return managedString.ToNativeStr(out len, Encoding.GetEncoding(codePage));
        }



        public static IntPtr AllocNativeStringArray(this string[] stringArrayManaged)
        {
            int count = stringArrayManaged.Length;
            int ptrSize = IntPtr.Size;
            IntPtr pptr = Marshal.AllocHGlobal(ptrSize * count);
            for (int n = 0; n < count; ++n)
            {
                int len;
                IntPtr p = stringArrayManaged[n].ToNativeAscii(out len);
                Marshal.WriteIntPtr(pptr, n * ptrSize, p);
            }

            return pptr;
        }

        public static void ReleaseNativeStringArray(this IntPtr pptr, int count)
        {
            int ptrSize = IntPtr.Size;
            for (int n = 0; n < count; ++n)
            {
                IntPtr p = Marshal.ReadIntPtr(pptr, n * ptrSize);
                p.FreeNativeStr();
            }
            Marshal.FreeHGlobal(pptr);
        }
    }
}