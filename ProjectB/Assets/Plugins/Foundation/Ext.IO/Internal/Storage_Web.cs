using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Ext.IO.Internal
{
    internal class Storage_Web : Storage.Backend
    {
        // ----------------------------------------
        // NOTE: File
        // ----------------------------------------
        internal override bool Exists(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            return false;
        }
        internal override bool IsDirectory(string foldername)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            return false;
        }


        // ----------------------------------------
        // NOTE: File
        // ----------------------------------------
        internal override int Write(string filename, byte[] content)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            return Storage.ERROR_NOT_SUPPORTED;
        }
        internal override void WriteAsync(string filename, byte[] content, Action<int> callback)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            if (null != callback)
                callback(Storage.ERROR_NOT_SUPPORTED);
        }
        internal override int Read(string filename, out byte[] content)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            content = null;
            return Storage.ERROR_NOT_SUPPORTED;
        }
        internal override void ReadAsync(string filename, Action<int, byte[]> callback)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            if (null != callback)
                callback(Storage.ERROR_NOT_SUPPORTED, null);
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
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB
                        
            return Storage.ERROR_NOT_SUPPORTED;
        }
        internal override void MoveAsync(string filenameSource, string filenameDest, Action<int> callback)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            if (null != callback)
                callback(Storage.ERROR_NOT_SUPPORTED);
        }
        internal override int Delete(string filename)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            return Storage.ERROR_NOT_SUPPORTED;
        }
        internal override void DeleteAsync(string filename, Action<int> callback)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            if (null != callback)
                callback(Storage.ERROR_NOT_SUPPORTED);
        }


        // ----------------------------------------
        // NOTE: Directory
        // ----------------------------------------
        internal override int CreateDirectory(string foldername)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            return Storage.ERROR_NOT_SUPPORTED;
        }
        internal override int DeleteDirectory(string foldername)
        {
#if STORAGE_WEB_INDEXEDDB
            // TODO: IndexedDB
#endif// STORAGE_WEB_INDEXEDDB

            return Storage.ERROR_NOT_SUPPORTED;
        }
    }
}
