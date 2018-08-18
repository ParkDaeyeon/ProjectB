using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Ext.IO.Internal
{
    internal class Storage_File : Storage.Backend
    {
        // ----------------------------------------
        // NOTE: Path
        // ----------------------------------------
        internal override bool Exists(string filename)
        {
            return File.Exists(filename);
        }
        internal override bool IsDirectory(string filename)
        {
            return Directory.Exists(filename);
        }


        // ----------------------------------------
        // NOTE: File
        // ----------------------------------------
        internal override int Write(string filename, byte[] content)
        {
            File.WriteAllBytes(filename, content);
            return Storage.OK;
        }
        internal override void WriteAsync(string filename, byte[] content, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    File.WriteAllBytes(filename, content);
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
            content = File.ReadAllBytes(filename);
            return Storage.OK;
        }
        internal override void ReadAsync(string filename, Action<int, byte[]> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    var content = File.ReadAllBytes(filename);
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
            File.Copy(filenameSource, filenameDest, overwrite);
            return Storage.OK;
        }
        internal override void CopyAsync(string filenameSource, string filenameDest, Action<int> callback, bool overwrite)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    File.Copy(filenameSource, filenameDest, overwrite);
                    if (null != callback)
                        callback(Storage.OK);
                }
                catch (Exception e)
                {
                    var ret = Storage.Log.OnError(Storage.ERROR_IO, string.Format("Copy", overwrite), true, e, filenameSource, filenameDest);
                    if (null != callback)
                        callback(ret);
                }
            });
        }
        internal override int Move(string filenameSource, string filenameDest)
        {
            File.Move(filenameSource, filenameDest);
            return Storage.OK;
        }
        internal override void MoveAsync(string filenameSource, string filenameDest, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    File.Move(filenameSource, filenameDest);
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
            File.Delete(filename);
            return Storage.OK;
        }
        internal override void DeleteAsync(string filename, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // NOTE: If not the MAIN THREAD, perform exception handling in the backend.
                try
                {
                    File.Delete(filename);
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
            Directory.CreateDirectory(foldername);
            return Storage.OK;
        }
        internal override int DeleteDirectory(string foldername)
        {
            Directory.Delete(foldername, true);
            return Storage.OK;
        }
    }
}
