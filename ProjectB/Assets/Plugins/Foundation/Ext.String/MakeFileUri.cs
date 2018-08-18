using System;
using System.Text;
namespace Ext.String
{
    public static class MakeFileUri
    {
        public const int FILE_URI_START = 7;
        public static bool IsFileUri(string text)
        {
            if (text.Length >= FILE_URI_START &&
                ('f' == text[0] || 'F' == text[0]) &&
                ('i' == text[1] || 'I' == text[1]) &&
                ('l' == text[2] || 'L' == text[2]) &&
                ('e' == text[3] || 'E' == text[3]) &&
                ':' == text[4] &&
                '/' == text[5] &&
                '/' == text[6])
                return true;

            return false;
        }

        public const int APK_URI_START = 4;
        public static bool IsApkUri(string text)
        {
            if (text.Length >= APK_URI_START &&
                ('j' == text[0] || 'J' == text[0]) &&
                ('a' == text[1] || 'A' == text[1]) &&
                ('r' == text[2] || 'R' == text[2]) &&
                ':' == text[3])
                return true;

            return false;
        }

        public static bool IsUrl(string text)
        {
            int _;
            return MakeFileUri.IsExistScheme(text, out _);
        }

        const string Scheme = "://";
        public static bool IsExistScheme(string text, out int next)
        {
            if (MakeFileUri.IsFileUri(text))
            {
                next = MakeFileUri.FILE_URI_START;
                return true;
            }
            else if (MakeFileUri.IsApkUri(text))
            {
                next = MakeFileUri.APK_URI_START;
                return true;
            }

            int idx = text.IndexOf(MakeFileUri.Scheme);
            if (-1 != idx && idx < 10)
            {
                next = idx + MakeFileUri.Scheme.Length;
                return true;
            }
            else
            {
                next = 0;
                return false;
            }
        }

        const string WindowsScheme = ":/";
        public static bool IsExistWindowsDriver(string text, out int driverNameIdx, out int next)
        {
            int idx = text.IndexOf(MakeFileUri.WindowsScheme);
            if (-1 != idx && 1 <= idx && idx < 10 && ((idx + 2) < text.Length) && text[idx + 2] != '/')
            {
                next = idx + MakeFileUri.WindowsScheme.Length;
                driverNameIdx = text.LastIndexOf('/', idx);
                if (-1 != driverNameIdx)
                    ++driverNameIdx;
                else
                    driverNameIdx = 0;

                return true;
            }
            else
            {
                driverNameIdx = 0;
                next = 0;
                return false;
            }
        }



        static StringBuilder sbToUri = new StringBuilder("file://", 512);
        public static string Rebuild(string text)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return text;
#else// !UNITY_EDITOR && UNITY_WEBGL
            return MakeFileUri.OnRebuild(text, true);
#endif// !UNITY_EDITOR && UNITY_WEBGL
        }

        public static string RebuildWithoutRootSlash(string text)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return text;
#else// !UNITY_EDITOR && UNITY_WEBGL
            return MakeFileUri.OnRebuild(text, false);
#endif// !UNITY_EDITOR && UNITY_WEBGL
        }

        private static string OnRebuild(string text, bool isRootSlash)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return text;
#else// !UNITY_EDITOR && UNITY_WEBGL
            if (string.IsNullOrEmpty(text))
                return isRootSlash ? "file:///" : "file://";

#if LOG_FILE_URI
            UnityEngine.Debug.Log("MAKE_FILE_URI_FROM:" + text + ", ROOTSLASH:" + isRootSlash);
#endif// LOG_FILE_URI
            if (MakeFileUri.IsUrl(text))
            {
#if LOG_IO
                UnityEngine.Debug.Log("MakeFileUri (isUri) : text = " + text + ", url = " + text);
#endif// LOG_IO
#if LOG_FILE_URI
                UnityEngine.Debug.Log("MAKE_FILE_URI_TO#1:" + text);
#endif// LOG_FILE_URI
                return text;
            }

            if (!MakeFileUri.IsFileUri(text))
            {
                MakeFileUri.sbToUri.Length = FILE_URI_START;

                int start;
                int next;
                if (isRootSlash && MakeFileUri.IsExistWindowsDriver(text, out start, out next))
                {
                    MakeFileUri.sbToUri.Append('/').Append(text.Substring(start));
#if LOG_FILE_URI
                    UnityEngine.Debug.Log("MAKE_FILE_URI_WINDOWS_DRIVER");
#endif// LOG_FILE_URI
                }
                else
                {
                    MakeFileUri.sbToUri.Append(text);
#if LOG_FILE_URI
                    UnityEngine.Debug.Log("MAKE_FILE_URI_UNIX");
#endif// LOG_FILE_URI
                }
            }

            if (isRootSlash)
            {
                if ('/' != MakeFileUri.sbToUri[FILE_URI_START])
                    MakeFileUri.sbToUri.Insert(FILE_URI_START, '/');
            }

            string url = MakeFileUri.sbToUri.ToString();
#if LOG_FILE_URI
            UnityEngine.Debug.Log("MAKE_FILE_URI_TO#2:" + url);
#endif// LOG_FILE_URI
#if LOG_IO
            UnityEngine.Debug.Log("MakeFileUri : text = " + text + ", url = " + url);
#endif// LOG_IO
            return url;
#endif// !UNITY_EDITOR && UNITY_WEBGL
        }

        public static string ExtractPath(string uri, params string[] additional)
        {
            var index = uri.IndexOf(MakeFileUri.Scheme);
            if (-1 == index)
                return MakeFileUri.Combine(uri, additional);

            int skipLength = MakeFileUri.Scheme.Length;
            int addedLength = MakeFileUri.GetLength(additional);
            var sb = new StringBuilder(uri.Length + addedLength - skipLength);
            if (skipLength < uri.Length && '/' == uri[skipLength])
            {
                int winIndex = uri.IndexOf(MakeFileUri.WindowsScheme, skipLength + 1);
                if (-1 != winIndex && winIndex < 10)
                    skipLength += 1;
            }

            sb.Append(uri, skipLength, uri.Length - skipLength);
            return MakeFileUri.Combine(sb, additional);
        }

        static int GetLength(string[] additional)
        {
            int length = 0;
            if (null != additional)
            {
                for (int n = 0, cnt = additional.Length; n < cnt; ++n)
                {
                    var add = additional[n];
                    if (null == add)
                        continue;

                    length += add.Length;
                }
            }
            return length;
        }

        static string Combine(string origin, string[] additional)
        {
            if (string.IsNullOrEmpty(origin))
                return origin;

            if (null == additional)
                return origin;

            return MakeFileUri.Combine(new StringBuilder(origin, origin.Length + MakeFileUri.GetLength(additional)), additional);
        }

        static string Combine(StringBuilder sb, string[] additional)
        {
            if (null != additional)
            {
                for (int n = 0, cnt = additional.Length; n < cnt; ++n)
                {
                    var add = additional[n];
                    if (null == add)
                        continue;

                    sb.Append(add);
                }
            }

            return sb.ToString();
        }
    }
}