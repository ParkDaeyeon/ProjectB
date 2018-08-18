using System;
using System.Collections.Generic;
using System.Text;
namespace Ext.IO
{
    public static class Storage
    {
        internal abstract class Backend
        {
            // ----------------------------------------
            // NOTE: Path
            // ----------------------------------------
            internal virtual string Normalize(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return path;

                return path;
            }

            static void Collect(LinkedList<string> list, StringBuilder sb, StringBuilder sbSubstr, int start, int last)
            {
                var len = last - start;
                if (0 >= len)
                    return;

                for (int n = 0; n < len; ++n)
                    sbSubstr.Append(sb[start + n]);
                var name = sbSubstr.ToString();
                sbSubstr.Length = 0;
                if (".." == name)
                {
                    // NOTE: remove prev
                    if (null != list.Last)
                        list.RemoveLast();
                }
                else if ("." == name)
                {
                    // NOTE: continue;
                }
                else
                {
                    list.AddLast(name);
                }
            }
            protected static string AbsName(string filename)
            {
                if (string.IsNullOrEmpty(filename))
                    return filename;

                var sb = new StringBuilder(filename, filename.Length);
                var sbSubstr = new StringBuilder(sb.Length);

                // NOTE: Remove all '//', '../', './'
                var lastSlash = -1;
                var list = new LinkedList<string>();
                for (int n = 1, cnt = sb.Length; n < cnt; ++n)
                {
                    var c = sb[n];
                    if ('\\' == c)
                        c = '/';

                    if ('/' != c)
                        continue;

                    Backend.Collect(list, sb, sbSubstr, lastSlash + 1, n);
                    lastSlash = n;
                }
                if (lastSlash != (sb.Length - 1))
                    Backend.Collect(list, sb, sbSubstr, lastSlash + 1, sb.Length);
                else
                    list.AddLast("");

                // NOTE: Add All
                sb.Length = 0;
                var node = list.First;
                while (null != node)
                {
                    if (0 < sb.Length)
                        sb.Append('/');
                    sb.Append(node.Value);
                    node = node.Next;
                }
                return sb.ToString();
            }
            internal virtual string DirectoryName(string filename)
            {
                filename = Backend.AbsName(filename);
                if (string.IsNullOrEmpty(filename))
                    return null;

                var index = 0;
                if (-1 == (index = filename.LastIndexOf('/')))
                    return null;

                if (0 == index || -1 == filename.LastIndexOf('/', index - 1))
                    return null;

                return filename.Substring(0, index);
            }
            internal abstract bool Exists(string filename);
            internal abstract bool IsDirectory(string filename);

            // ----------------------------------------
            // NOTE: File
            // ----------------------------------------
            internal abstract int Write(string filename, byte[] content);
            internal abstract void WriteAsync(string filename, byte[] content, Action<int> callback);
            internal abstract int Read(string filename, out byte[] content);
            internal abstract void ReadAsync(string filename, Action<int, byte[]> callback);
            internal abstract int Copy(string filenameSource, string filenameDest, bool overwrite);
            internal abstract void CopyAsync(string filenameSource, string filenameDest, Action<int> callback, bool overwrite);
            internal abstract int Move(string filenameSource, string filenameDest);
            internal abstract void MoveAsync(string filenameSource, string filenameDest, Action<int> callback);
            internal abstract int Delete(string filename);
            internal abstract void DeleteAsync(string filename, Action<int> callback);

            // ----------------------------------------
            // NOTE: Directory
            // ----------------------------------------
            internal abstract int CreateDirectory(string foldername);
            internal abstract int DeleteDirectory(string foldername);
        }

        static Backend backend;
        public static void Open()
        {
            Storage.backend =
#if !UNITY_EDITOR && UNITY_WEBPLAYER
                new Internal.Storage_Web();
#elif !UNITY_EDITOR && UNITY_SWITCH
                new Internal.Storage_NS();
#else// UNITY_*
                new Internal.Storage_File();
#endif// UNITY_*
        }

        public static void Close()
        {
            Storage.backend = null;
        }

        public const int OK = 0;
        public const int NONE = 1;

        public const int ERROR_INVALID_PATH = -1;
        public const int ERROR_IO = -2;
        public const int ERROR_NOT_SUPPORTED = -3;
        public const int ERROR_CANNOT_CREATE_DIRECTORY = -4;
        public const int ERROR_ALREADY_EXISTS = -5;
        public const int ERROR_NOT_FOUND = -6;
        public const int ERROR_INVALID_ENCODING = -7;

        public static string ToErrorString(int code)
        {
            switch (code)
            {
            case Storage.OK: return "OK";
            case Storage.NONE: return "NONE";

            case Storage.ERROR_INVALID_PATH: return "ERROR_INVALID_PATH";
            case Storage.ERROR_IO: return "ERROR_IO";
            case Storage.ERROR_NOT_SUPPORTED: return "ERROR_NOT_SUPPORTED";
            case Storage.ERROR_CANNOT_CREATE_DIRECTORY: return "ERROR_CANNOT_CREATE_DIRECTORY";
            case Storage.ERROR_ALREADY_EXISTS: return "ERROR_ALREADY_EXISTS";
            case Storage.ERROR_NOT_FOUND: return "ERROR_NOT_FOUND";
            case Storage.ERROR_INVALID_ENCODING: return "ERROR_INVALID_ENCODING";

            default: return code.ToString();
            }
        }


        // ----------------------------------------
        // NOTE: Path
        // ----------------------------------------
        public static string Normalize(string path)
        {
            return Storage.backend.Normalize(path);
        }
        public static string DirectoryName(string filename)
        {
            return Storage.backend.DirectoryName(filename);
        }
        public static bool Exists(string filename)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
                return false;

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:EXISTS:{0}", filename));
#endif// LOG_STORAGE
            return Storage._Exists(filename);
        }
        static bool _Exists(string filename)
        {
            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:EXISTS:{0}", filename));
#endif// LOG_STORAGE_DEEP
                return Storage.backend.Exists(filename);
            }
            catch (Exception e)
            {
                Storage._Break1(Storage.ERROR_IO, "Exists", filename, null, e);
                return false;
            }
        }
        public static bool IsDirectory(string filename)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
                return false;

            return Storage._IsDirectory(filename);
        }
        static bool _IsDirectory(string filename)
        {
            try
            {
                return Storage.backend.IsDirectory(filename);
            }
            catch (Exception e)
            {
                Storage._Break1(Storage.ERROR_IO, "IsDirectory", filename, null, e);
                return false;
            }
        }


        // ----------------------------------------
        // NOTE: File
        // ----------------------------------------
        public static int WriteAllBytes(string filename, byte[] content)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
                return Storage._Break1(Storage.ERROR_INVALID_PATH, "WriteAllBytes", filename, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:WRITE:{0}", filename));
#endif// LOG_STORAGE
            return Storage._Write(filename, content, null);
        }
        public static void WriteAllBytesAsync(string filename, byte[] content, Action<int> callback)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                Storage._Break1(Storage.ERROR_INVALID_PATH, "WriteAllBytes", filename, callback);
                return;
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:WRITE_A:{0}", filename));
#endif// LOG_STORAGE
            Storage._Write(filename, content, callback);
        }
        public static int WriteAllText(string filename, string content, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = Encoding.ASCII;

            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
                return Storage._Break1(Storage.ERROR_INVALID_PATH, string.Format("WriteAllText({0})", encoding), filename, null);

            var bytes = default(byte[]);
            try
            {
                bytes = encoding.GetBytes(content);
            }
            catch (Exception e)
            {
                return Storage._Break1(Storage.ERROR_INVALID_ENCODING, string.Format("WriteAllText({0})", encoding), filename, null, e);
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:WRITE_TXT:{0}, ENC:{1}", filename, encoding));
#endif// LOG_STORAGE
            return Storage._Write(filename, bytes, null);
        }
        public static void WriteAllTextAsync(string filename, string content, Action<int> callback, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = Encoding.ASCII;

            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                Storage._Break1(Storage.ERROR_INVALID_PATH, string.Format("WriteAllText({0})", encoding), filename, callback);
                return;
            }

            var bytes = default(byte[]);
            try
            {
                bytes = encoding.GetBytes(content);
            }
            catch (Exception e)
            {
                Storage._Break1(Storage.ERROR_INVALID_ENCODING, string.Format("WriteAllText({0})", encoding), filename, callback, e);
                return;
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:WRITE_TXT_A:{0}, ENC:{1}", filename, encoding));
#endif// LOG_STORAGE
            Storage._Write(filename, bytes, callback);
        }
        static int _Write(string filename, byte[] content, Action<int> callback)
        {
            var foldername = Storage.DirectoryName(filename);
            if (null != foldername)
            {
                var retDict = Storage._CreateDirectory(foldername);
                if (Storage.OK > retDict)
                    return Storage._Break1(Storage.ERROR_CANNOT_CREATE_DIRECTORY, "Write", filename, callback);
            }

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:WRITE:{0}, A:{1}", filename, null != callback));
#endif// LOG_STORAGE_DEEP
                if (null != callback)
                {
                    Storage.backend.WriteAsync(filename, content, callback);
                    return Storage.OK;
                }
                else
                    return Storage.backend.Write(filename, content);
            }
            catch (Exception e)
            {
                return Storage._Break1(Storage.ERROR_IO, "Write", filename, callback, e);
            }
        }
        public static int ReadAllBytes(string filename, out byte[] content)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                content = null;
                return Storage._Break1(Storage.ERROR_INVALID_PATH, "ReadAllBytes", filename, null);
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:READ:{0}", filename));
#endif// LOG_STORAGE
            return Storage._Read(filename, out content, null);
        }
        public static void ReadAllBytesAsync(string filename, Action<int, byte[]> callback)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                Storage._Break1(Storage.ERROR_INVALID_PATH, "ReadAllBytes", filename, callback);
                return;
            }

            byte[] _;
#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:READ_A:{0}", filename));
#endif// LOG_STORAGE
            Storage._Read(filename, out _, callback);
        }
        public static int ReadAllText(string filename, out string content, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = Encoding.ASCII;

            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                content = null;
                return Storage._Break1(Storage.ERROR_INVALID_PATH, string.Format("ReadAllText({0})", encoding), filename, null);
            }

            byte[] bytes;
#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:READ_TXT:{0}, ENC:{1}", filename, encoding));
#endif// LOG_STORAGE
            var ret = Storage._Read(filename, out bytes, null);
            if (Storage.OK != ret)
            {
                content = null;
                return ret;
            }

            try
            {
                content = encoding.GetString(bytes);
                return Storage.OK;
            }
            catch (Exception e)
            {
                content = null;
                return Storage._Break1(Storage.ERROR_INVALID_ENCODING, string.Format("ReadAllText({0})", encoding), filename, null, e);
            }
        }
        public static void ReadAllTextAsync(string filename, Action<int, string> callback, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = Encoding.ASCII;

            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                Storage._Break1(Storage.ERROR_INVALID_PATH, string.Format("ReadAllText({0})", encoding), filename, callback);
                return;
            }

            byte[] _;
#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:READ_TXT_A:{0}, ENC:{1}", filename, encoding));
#endif// LOG_STORAGE
            Storage._Read(filename, out _, (ret, bytes) =>
            {
                if (Storage.OK != ret)
                {
                    if (null != callback)
                        callback(ret, null);
                    return;
                }
                
                string content;
                try
                {
                    content = encoding.GetString(bytes);
                }
                catch (Exception e)
                {
                    Storage._Break1(Storage.ERROR_INVALID_ENCODING, string.Format("ReadAllText({0})", encoding), filename, callback, e);
                    return;
                }

                if (null != callback)
                    callback(Storage.OK, content);
            });
        }
        static int _Read(string filename, out byte[] content, Action<int, byte[]> callback)
        {
            if (!Storage._Exists(filename))
            {
                content = null;
                return Storage._Break1(Storage.ERROR_NOT_FOUND, "Read", filename, callback);
            }

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:READ:{0}, A:{1}", filename, null != callback));
#endif// LOG_STORAGE_DEEP
                if (null != callback)
                {
                    Storage.backend.ReadAsync(filename, callback);
                    content = null;
                    return Storage.OK;
                }
                else
                    return Storage.backend.Read(filename, out content);
            }
            catch (Exception e)
            {
                content = null;
                return Storage._Break1(Storage.ERROR_IO, "Read", filename, callback, e);
            }
        }
        public static int Copy(string filenameSource, string filenameDest, bool overwrite = false)
        {
            filenameSource = Storage.Normalize(filenameSource);
            filenameDest = Storage.Normalize(filenameDest);
            if (string.IsNullOrEmpty(filenameSource) || string.IsNullOrEmpty(filenameDest))
                return Storage._Break2(Storage.ERROR_INVALID_PATH, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:COPY:S:{0}, D:{1}", filenameSource, filenameDest));
#endif// LOG_STORAGE
            return Storage._Copy(filenameSource, filenameDest, null, overwrite);
        }
        public static void CopyAsync(string filenameSource, string filenameDest, Action<int> callback, bool overwrite = false)
        {
            filenameSource = Storage.Normalize(filenameSource);
            filenameDest = Storage.Normalize(filenameDest);
            if (string.IsNullOrEmpty(filenameSource) || string.IsNullOrEmpty(filenameDest))
            {
                Storage._Break2(Storage.ERROR_INVALID_PATH, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);
                return;
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:COPY_A:S:{0}, D:{1}", filenameSource, filenameDest));
#endif// LOG_STORAGE
            Storage._Copy(filenameSource, filenameDest, callback, overwrite);
        }
        static int _Copy(string filenameSource, string filenameDest, Action<int> callback, bool overwrite)
        {
            if (!Storage._Exists(filenameSource))
                return Storage._Break2(Storage.ERROR_NOT_FOUND, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);

            if (filenameSource == filenameDest)
                return Storage._Break2(Storage.NONE, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);

            if (!overwrite && Storage.Exists(filenameDest))
                return Storage._Break2(Storage.ERROR_ALREADY_EXISTS, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);
            
            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:COPY:S:{0}, D:{1}, A:{2}", filenameSource, filenameDest, null != callback));
#endif// LOG_STORAGE_DEEP
                if (null != callback)
                {
                    Storage.backend.CopyAsync(filenameSource, filenameDest, callback, overwrite);
                    return Storage.OK;
                }
                else
                    return Storage.backend.Copy(filenameSource, filenameDest, overwrite);
            }
            catch (Exception e)
            {
                return Storage._Break2(Storage.ERROR_IO, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback, e);
            }
        }
        public static int CopyByWrite(string filenameSource, string filenameDest, bool overwrite = false)
        {
            filenameSource = Storage.Normalize(filenameSource);
            filenameDest = Storage.Normalize(filenameDest);
            if (string.IsNullOrEmpty(filenameSource) || string.IsNullOrEmpty(filenameDest))
                return Storage._Break2(Storage.ERROR_INVALID_PATH, string.Format("CopyByWrite({0})", overwrite), filenameSource, filenameDest, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:COPY_W:S:{0}, D:{1}", filenameSource, filenameDest));
#endif// LOG_STORAGE
            return Storage._CopyByWrite(filenameSource, filenameDest, null, overwrite);
        }
        public static void CopyByWriteAsync(string filenameSource, string filenameDest, Action<int> callback, bool overwrite = false)
        {
            filenameSource = Storage.Normalize(filenameSource);
            filenameDest = Storage.Normalize(filenameDest);
            if (string.IsNullOrEmpty(filenameSource) || string.IsNullOrEmpty(filenameDest))
            {
                Storage._Break2(Storage.ERROR_INVALID_PATH, string.Format("CopyByWrite({0})", overwrite), filenameSource, filenameDest, callback);
                return;
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:COPY_W_A:S:{0}, D:{1}", filenameSource, filenameDest));
#endif// LOG_STORAGE
            Storage._CopyByWrite(filenameSource, filenameDest, callback, overwrite);
        }
        static int _CopyByWrite(string filenameSource, string filenameDest, Action<int> callback, bool overwrite)
        {
            if (!Storage._Exists(filenameSource))
                return Storage._Break2(Storage.ERROR_NOT_FOUND, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);

            if (filenameSource == filenameDest)
                return Storage._Break2(Storage.NONE, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);

            if (!overwrite && Storage.Exists(filenameDest))
                return Storage._Break2(Storage.ERROR_ALREADY_EXISTS, string.Format("Copy({0})", overwrite), filenameSource, filenameDest, callback);

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:COPY_W:S:{0}, D:{1}, A:{2}", filenameSource, filenameDest, null != callback));
#endif// LOG_STORAGE_DEEP
                if (null != callback)
                {
                    Storage.ReadAllBytesAsync(filenameSource, (retRead, content) =>
                    {
                        if (Storage.OK > retRead)
                        {
                            if (null != callback)
                                callback(retRead);
                            return;
                        }

                        try
                        {
                            Storage.WriteAllBytesAsync(filenameDest, content, callback);
                        }
                        catch (Exception e)
                        {
                            Storage._Break2(Storage.ERROR_IO, string.Format("CopyByWrite({0})", overwrite), filenameSource, filenameDest, callback, e);
                        }
                    });
                    return Storage.OK;
                }
                else
                {
                    byte[] content;
                    var ret = Storage.ReadAllBytes(filenameSource, out content);
                    if (Storage.OK > ret)
                        return ret;

                    return Storage.WriteAllBytes(filenameDest, content);
                }
            }
            catch (Exception e)
            {
                return Storage._Break2(Storage.ERROR_IO, string.Format("CopyByWrite({0})", overwrite), filenameSource, filenameDest, callback, e);
            }
        }
        public static int Move(string filenameSource, string filenameDest)
        {
            filenameSource = Storage.Normalize(filenameSource);
            filenameDest = Storage.Normalize(filenameDest);
            if (string.IsNullOrEmpty(filenameSource) || string.IsNullOrEmpty(filenameDest))
                return Storage._Break2(Storage.ERROR_INVALID_PATH, "Move", filenameSource, filenameDest, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:MOVE:S:{0}, D:{1}", filenameSource, filenameDest));
#endif// LOG_STORAGE
            return Storage._Move(filenameSource, filenameDest, null);
        }
        public static void MoveAsync(string filenameSource, string filenameDest, Action<int> callback)
        {
            filenameSource = Storage.Normalize(filenameSource);
            filenameDest = Storage.Normalize(filenameDest);
            if (string.IsNullOrEmpty(filenameSource) || string.IsNullOrEmpty(filenameDest))
            {
                Storage._Break2(Storage.ERROR_INVALID_PATH, "Move", filenameSource, filenameDest, callback);
                return;
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:MOVE_A:S:{0}, D:{1}", filenameSource, filenameDest));
#endif// LOG_STORAGE
            Storage._Move(filenameSource, filenameDest, callback);
        }
        static int _Move(string filenameSource, string filenameDest, Action<int> callback)
        {
            if (!Storage._Exists(filenameSource))
                return Storage._Break2(Storage.ERROR_NOT_FOUND, "Move", filenameSource, filenameDest, callback);

            if (filenameSource == filenameDest)
                return Storage._Break2(Storage.NONE, "Move", filenameSource, filenameDest, callback);

            if (Storage._Exists(filenameDest))
                return Storage._Break2(Storage.ERROR_ALREADY_EXISTS, "Move", filenameSource, filenameDest, callback);

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:MOVE:S:{0}, D:{1}, A:{2}", filenameSource, filenameDest, null != callback));
#endif// LOG_STORAGE_DEEP
                if (null != callback)
                {
                    Storage.backend.MoveAsync(filenameSource, filenameDest, callback);
                    return Storage.OK;
                }
                else
                    return Storage.backend.Move(filenameSource, filenameDest);
            }
            catch (Exception e)
            {
                return Storage._Break2(Storage.ERROR_IO, "Move", filenameSource, filenameDest, callback, e);
            }
        }
        public static int Delete(string filename)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
                return Storage._Break1(Storage.ERROR_INVALID_PATH, "Delete", filename, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:DELETE:{0}", filename));
#endif// LOG_STORAGE
            return Storage._Delete(filename, null);
        }
        public static void DeleteAsync(string filename, Action<int> callback)
        {
            filename = Storage.Normalize(filename);
            if (string.IsNullOrEmpty(filename))
            {
                Storage._Break1(Storage.ERROR_INVALID_PATH, "Delete", filename, callback);
                return;
            }

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:DELETE_A:{0}", filename));
#endif// LOG_STORAGE
            Storage._Delete(filename, callback);
        }
        static int _Delete(string filename, Action<int> callback)
        {
            if (!Storage._Exists(filename))
                return Storage._Break1(Storage.ERROR_NOT_FOUND, "Delete", filename, callback);

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:DELETE:{0}, A:{1}", filename, null != callback));
#endif// LOG_STORAGE_DEEP
                if (null != callback)
                {
                    Storage.backend.DeleteAsync(filename, callback);
                    return Storage.OK;
                }
                return Storage.backend.Delete(filename);
            }
            catch (Exception e)
            {
                return Storage._Break1(Storage.ERROR_IO, "Delete", filename, callback, e);
            }
        }


        // ----------------------------------------
        // NOTE: Directory
        // ----------------------------------------
        public static int CreateDirectory(string foldername)
        {
            foldername = Storage.Normalize(foldername);
            if (string.IsNullOrEmpty(foldername))
                return Storage._Break1(Storage.ERROR_INVALID_PATH, "CreateDirectory", foldername, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:MKDIR:{0}", foldername));
#endif// LOG_STORAGE
            return Storage._CreateDirectory(foldername);
        }
        static int _CreateDirectory(string foldername)
        {
            if (Storage._Exists(foldername))
                return Storage._Break1(Storage.ERROR_ALREADY_EXISTS, "CreateDirectory", foldername, null);

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:MKDIR:{0}", foldername));
#endif// LOG_STORAGE_DEEP
                return Storage.backend.CreateDirectory(foldername);
            }
            catch (Exception e)
            {
                return Storage._Break1(Storage.ERROR_IO, "CreateDirectory", foldername, null, e);
            }
        }
        public static int DeleteDirectory(string foldername)
        {
            foldername = Storage.Normalize(foldername);
            if (string.IsNullOrEmpty(foldername))
                return Storage._Break1(Storage.ERROR_INVALID_PATH, "DeleteDirectory", foldername, null);

#if LOG_STORAGE
            Log.OnLog(string.Format("STORAGE:RMDIR:{0}", foldername));
#endif// LOG_STORAGE
            return Storage._DeleteDirectory(foldername);
        }
        static int _DeleteDirectory(string foldername)
        {
            if (!Storage._IsDirectory(foldername))
                return Storage._Break1(Storage.ERROR_NOT_FOUND, "DeleteDirectory", foldername, null);

            try
            {
#if LOG_STORAGE_DEEP
                Log.OnLog(string.Format("STORAGE_DEEP:RMDIR:{0}", foldername));
#endif// LOG_STORAGE_DEEP
                return Storage.backend.DeleteDirectory(foldername);
            }
            catch (Exception e)
            {
                return Storage._Break1(Storage.ERROR_IO, "DeleteDirectory", foldername, null, e);
            }
        }


        public static class Auto
        {
            public static void WriteAllBytes(string filename, byte[] content, Action<int> callback, bool sync = true)
            {
                if (sync)
                {
                    var ret = Storage.WriteAllBytes(filename, content);
                    if (null != callback)
                        callback(ret);
                }
                else
                    Storage.WriteAllBytesAsync(filename, content, callback);
            }
            public static void WriteAllText(string filename, string content, Action<int> callback, Encoding encoding = null, bool sync = true)
            {
                if (sync)
                {
                    var ret = Storage.WriteAllText(filename, content, encoding);
                    if (null != callback)
                        callback(ret);
                }
                else
                    Storage.WriteAllTextAsync(filename, content, callback, encoding);
            }
            public static void ReadAllBytes(string filename, Action<int, byte[]> callback, bool sync = true)
            {
                if (sync)
                {
                    byte[] content;
                    var ret = Storage.ReadAllBytes(filename, out content);
                    if (null != callback)
                        callback(ret, content);
                }
                else
                    Storage.ReadAllBytesAsync(filename, callback);
            }
            public static void ReadAllText(string filename, Action<int, string> callback, Encoding encoding = null, bool sync = true)
            {
                if (sync)
                {
                    string content;
                    var ret = Storage.ReadAllText(filename, out content, encoding);
                    if (null != callback)
                        callback(ret, content);
                }
                else
                    Storage.ReadAllTextAsync(filename, callback, encoding);
            }
            public static void Copy(string filenameSource, string filenameDest, Action<int> callback, bool overwrite = false, bool sync = true)
            {
                if (sync)
                {
                    var ret = Storage.Copy(filenameSource, filenameDest, overwrite);
                    if (null != callback)
                        callback(ret);
                }
                else
                    Storage.CopyAsync(filenameSource, filenameDest, callback, overwrite);
            }
            public static void CopyByWrite(string filenameSource, string filenameDest, Action<int> callback, bool overwrite = false, bool sync = true)
            {
                if (sync)
                {
                    var ret = Storage.CopyByWrite(filenameSource, filenameDest, overwrite);
                    if (null != callback)
                        callback(ret);
                }
                else
                    Storage.CopyByWriteAsync(filenameSource, filenameDest, callback, overwrite);
            }
            public static void Move(string filenameSource, string filenameDest, Action<int> callback, bool sync = true)
            {
                if (sync)
                {
                    var ret = Storage.Move(filenameSource, filenameDest);
                    if (null != callback)
                        callback(ret);
                }
                else
                    Storage.MoveAsync(filenameSource, filenameDest, callback);
            }
            public static void Delete(string filename, Action<int> callback, bool sync = true)
            {
                if (sync)
                {
                    var ret = Storage.Delete(filename);
                    if (null != callback)
                        callback(ret);
                }
                else
                    Storage.DeleteAsync(filename, callback);
            }
        }


        static event Action<string> logger;
        public static event Action<string> Logger
        {
            add { Storage.logger += value; }
            remove { Storage.logger -= value; }
        }
        
        internal static class Log
        {
            internal static bool Available
            {
                get { return null != Storage.logger; }
            }

            static object lockObject = new object();

            internal static void OnLog(string content)
            {
                lock (Log.lockObject)
                {
                    var logger = Storage.logger;
                    if (null != logger)
                        logger(content);
                }
            }

            internal static int OnError(int code, string funcname, bool isAsync, Exception e, params string[] filenames)
            {
                if (!Log.Available)
                    return code;

                lock (Log.lockObject)
                {
                    Log.sb.Length = "STORAGE:ERROR:".Length;
                    Log.sb.Append(code).Append('(').Append(Storage.ToErrorString(code)).Append(')');
                    Log.WriteLine("FUNC", funcname);
                    if (isAsync)
                        Log.sb.Append("_async");
                    if (null != e)
                        Log.WriteLine("EXCEPT", e.ToString());
                    Log.WriteFiles(filenames);
                    Log.OnLog(Log.sb.ToString());
                }
                return code;
            }

            static StringBuilder sb = new StringBuilder("STORAGE:ERROR:");
            static void WriteLine(string key, string value)
            {
                Log.sb.Append(",\n").Append(key).Append(':').Append(value);
            }
            static void WriteFiles(string[] filenames)
            {
                if (null == filenames)
                    return;

                for (int n = 0, cnt = filenames.Length; n < cnt; ++n)
                {
                    var filename = filenames[n];
                    Log.WriteLine(string.Format("FNAME_{0}", n + 1), filename);
                }
            }
        }
        
        static int _Break1(int code, string funcname, string filename, Action<int> callback, Exception e = null)
        {
#if LOG_STORAGE_DEEP
            UnityEngine.Debug.Log(string.Format("STORAGE_DEEP:B1:{0}, FUNC:{1}, FILE_1:{2}, A:{3}",
                Storage.ToErrorString(code),
                funcname,
                filename,
                null != callback));
#endif// LOG_STORAGE_DEEP
            if (Storage.OK > code)
                Log.OnError(code, funcname, null != callback, e, filename);
            if (null != callback)
                callback(code);
            return code;
        }
        static int _Break2(int code, string funcname, string filename1, string filename2, Action<int> callback, Exception e = null)
        {
#if LOG_STORAGE_DEEP
            UnityEngine.Debug.Log(string.Format("STORAGE_DEEP:B2:{0}, FUNC:{1}, FILE_1:{2}, FILE_2:{3}, A:{4}",
                Storage.ToErrorString(code),
                funcname,
                filename1,
                filename2,
                null != callback));
#endif// LOG_STORAGE_DEEP
            if (Storage.OK > code)
                Log.OnError(code, funcname, null != callback, e, filename1, filename2);
            if (null != callback)
                callback(code);
            return code;
        }
        static int _Break1<T>(int code, string funcname, string filename, Action<int, T> callback, Exception e = null)
        {
#if LOG_STORAGE_DEEP
            UnityEngine.Debug.Log(string.Format("STORAGE_DEEP:B1_G<{0}>:{1}, FUNC:{2}, FILE:{3}, A:{4}",
                typeof(T).Name,
                Storage.ToErrorString(code),
                funcname,
                filename,
                null != callback));
#endif// LOG_STORAGE_DEEP
            if (Storage.OK > code)
                Log.OnError(code, funcname, null != callback, e, filename);
            if (null != callback)
                callback(code, default(T));
            return code;
        }
        static int _Break2<T>(int code, string funcname, string filename1, string filename2, Action<int, T> callback, Exception e = null)
        {
#if LOG_STORAGE_DEEP
            UnityEngine.Debug.Log(string.Format("STORAGE_DEEP:B2_G<{0}>:{1}, FUNC:{2}, FILE_1:{3}, FILE_2:{4}, A:{5}",
                typeof(T).Name,
                Storage.ToErrorString(code),
                funcname,
                filename1,
                filename2,
                null != callback));
#endif// LOG_STORAGE_DEEP
            if (Storage.OK > code)
                Log.OnError(code, funcname, null != callback, e, filename1, filename2);
            if (null != callback)
                callback(code, default(T));
            return code;
        }
    }
}
