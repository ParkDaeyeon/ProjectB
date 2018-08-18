#if UNITY_SWITCH
using System;
using System.Text;
using System.Threading;

using nn.fs;

using Ext.NS;

namespace Ext.IO.Internal
{
    using NSFile = nn.fs.File;
    using NSDirectory = nn.fs.Directory;
    internal class Storage_NS : Storage.Backend
    {
        // ----------------------------------------
        // NOTE: Path
        // ----------------------------------------
        internal override string Normalize(string path)
        {
            return NSPath.EraseRootSlashes(path);
        }
        internal override bool Exists(string filename)
        {
            return NSPath.Exists(filename);
        }
        internal override bool IsDirectory(string filename)
        {
            return NSPath.ExistsDirectory(filename);
        }

        
        // ----------------------------------------
        // NOTE: File
        // ----------------------------------------
        internal override int Write(string filename, byte[] content)
        {
            Internal.Write(filename, content);
            return Storage.OK;
        }
        internal override void WriteAsync(string filename, byte[] content, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    Internal.Write(filename, content);
                    if (null != callback)
                        callback(Storage.OK);
                }
                catch (Exception e)
                {
                    var ret = Storage.Log.OnError(Storage.ERROR_IO, "Write", true, e, filename);
                    if (null != callback)
                        callback(ret);
                }
            });
        }
        internal override int Read(string filename, out byte[] content)
        {
            content = Internal.Read(filename);
            return Storage.OK;
        }
        internal override void ReadAsync(string filename, Action<int, byte[]> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    var content = Internal.Read(filename);
                    if (null != callback)
                        callback(Storage.OK, content);
                }
                catch (Exception e)
                {
                    var ret = Storage.Log.OnError(Storage.ERROR_IO, "Read", true, e, filename);
                    if (null != callback)
                        callback(ret, null);
                }
            });
        }
        internal override int Copy(string filenameSource, string filenameDest, bool overwrite)
        {
            return Storage.CopyByWrite(filenameSource, filenameDest, overwrite);
        }
        internal override void CopyAsync(string filenameSource, string filenameDest, Action<int> callback, bool overwrite)
        {
            Storage.CopyByWriteAsync(filenameSource, filenameDest, callback, overwrite);
        }
        internal override int Move(string filenameSource, string filenameDest)
        {
            var ret = NSFile.Rename(filenameSource, filenameDest);
            if (!ret.IsSuccess())
                throw new System.IO.IOException(string.Format("NS.MOVE_A:{0}, SRC:{1}, DEST:{2}", ret.innerValue, filenameSource, filenameDest));

            return Storage.OK;
        }
        internal override void MoveAsync(string filenameSource, string filenameDest, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    var ret = NSFile.Rename(filenameSource, filenameDest);
                    if (!ret.IsSuccess())
                        throw new System.IO.IOException(string.Format("NS.MOVE_A:{0}, SRC:{1}, DEST:{2}", ret.innerValue, filenameSource, filenameDest));

                    if (null != callback)
                        callback(Storage.OK);
                }
                catch (Exception e)
                {
                    var ret = Storage.Log.OnError(Storage.ERROR_IO, "Move", true, e, filenameSource, filenameDest);
                    if (null != callback)
                        callback(ret);
                }
            });
        }
        internal override int Delete(string filename)
        {
            var ret = NSFile.Delete(filename);
            if (!ret.IsSuccess())
                throw new System.IO.IOException(string.Format("NS.DELETE:{0}, FNAME:{1}", ret.innerValue, filename));

            return Storage.OK;
        }
        internal override void DeleteAsync(string filename, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    var ret = NSFile.Delete(filename);
                    if (!ret.IsSuccess())
                        throw new System.IO.IOException(string.Format("NS.DELETE_A:{0}, FNAME:{1}", ret.innerValue, filename));

                    if (null != callback)
                        callback(Storage.OK);
                }
                catch (Exception e)
                {
                    var ret = Storage.Log.OnError(Storage.ERROR_IO, "Delete", true, e, filename);
                    if (null != callback)
                        callback(ret);
                }
            });
        }


        // ----------------------------------------
        // NOTE: Directory
        // ----------------------------------------
        internal override int CreateDirectory(string foldername)
        {
            var ret = NSDirectory.Create(foldername);
            if (!ret.IsSuccess())
                throw new System.IO.IOException(string.Format("NS.MKDIR:{0}, FNAME:{1}", ret.innerValue, foldername));

            return Storage.OK;
        }
        internal override int DeleteDirectory(string foldername)
        {
            var ret = NSDirectory.DeleteRecursively(foldername);
            if (!ret.IsSuccess())
                throw new System.IO.IOException(string.Format("NS.RMDIR:{0}, FNAME:{1}", ret.innerValue, foldername));

            return Storage.OK;
        }


        // ----------------------------------------
        // NOTE: Internal
        // ----------------------------------------
        static class Internal
        {
            internal static void Write(string filename, byte[] content)
            {
                using (var stream = new NSStream(filename, System.IO.FileMode.Create, content.Length))
                    stream.Write(content, 0, content.Length);
            }

            internal static byte[] Read(string filename)
            {
                var bytes = default(byte[]);
                using (var stream = new NSStream(filename, System.IO.FileMode.Open))
                {
                    bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                }
                return bytes;
            }
        }
    }
}
#endif// UNITY_SWITCH