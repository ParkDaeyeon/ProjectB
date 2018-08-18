using UnityEngine;

using System;
using System.Collections.Generic;

using Ext.IO;

namespace Ext.Unity3D
{
    public static class AssetBundleHolder
    {
        static Archive<AssetBundle> achive = new Archive<AssetBundle>("ab");

        public static bool CheckScheme(string uri, bool schemeNameOnly = false)
        {
            return AssetBundleHolder.achive.CheckScheme(uri, schemeNameOnly);
        }

        public static int FindSeparateIndex(string uri, bool validScheme = false)
        {
            return AssetBundleHolder.achive.FindSeparateIndex(uri, validScheme);
        }

        public static string ExtractHostname(string uri, int separateIndex = -1)
        {
            return AssetBundleHolder.achive.ExtractHostname(uri, separateIndex);
        }

        public static string ExtractFilename(string uri, int separateIndex = -1)
        {
            return AssetBundleHolder.achive.ExtractFilename(uri, separateIndex);
        }

        public static Tuple<string, string> Extract(string uri, int separateIndex = -1)
        {
            return AssetBundleHolder.achive.Extract(uri, separateIndex);
        }
        
        public static string AddScheme(string content)
        {
            return AssetBundleHolder.achive.AddScheme(content);
        }

        public static string RemoveScheme(string content)
        {
            return AssetBundleHolder.achive.RemoveScheme(content);
        }
        
        public static void Mount(string hostname, AssetBundle bundle)
        {
            AssetBundleHolder.achive.Mount(hostname, bundle);
        }

        public static bool Unmount(string hostname)
        {
            return AssetBundleHolder.achive.Unmount(hostname);
        }

        public static void UnmountAll()
        {
            AssetBundleHolder.achive.UnmountAll();
        }

        public static AssetBundle GetBundle(string hostname)
        {
            return AssetBundleHolder.achive.Get(hostname);
        }

        public static IEnumerable<KeyValuePair<string, AssetBundle>> GetBundleAll()
        {
            return AssetBundleHolder.achive.GetAll();
        }

        // ----------------------------------------------------------------------
        // NOTE: Unity3d interfaces
        // ----------------------------------------------------------------------
        public static UnityEngine.Object[] LoadAllAssets(string hostname)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAllAssets();
        }
        public static UnityEngine.Object[] LoadAllAssets(string hostname, Type type)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAllAssets(type);
        }
        public static T[] LoadAllAssets<T>(string hostname) where T : UnityEngine.Object
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAllAssets<T>();
        }
        public static AssetBundleRequest LoadAllAssetsAsync(string hostname)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAllAssetsAsync();
        }
        public static AssetBundleRequest LoadAllAssetsAsync(string hostname, Type type)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAllAssetsAsync(type);
        }
        public static AssetBundleRequest LoadAllAssetsAsync<T>(string hostname)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAllAssetsAsync<T>();
        }
        public static UnityEngine.Object LoadAsset(string uri, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAsset(tuple._1, tuple._2);
        }
        public static UnityEngine.Object LoadAsset(string hostname, string filename)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAsset(filename);
        }
        public static UnityEngine.Object LoadAsset(string uri, Type type, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAsset(tuple._1, tuple._2, type);
        }
        public static UnityEngine.Object LoadAsset(string hostname, string filename, Type type)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAsset(filename, type);
        }
        public static T LoadAsset<T>(string uri, int separateIndex = -1) where T : UnityEngine.Object
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAsset<T>(tuple._1, tuple._2);
        }
        public static T LoadAsset<T>(string hostname, string filename) where T : UnityEngine.Object
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAsset<T>(filename);
        }
        public static AssetBundleRequest LoadAssetAsync(string uri, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetAsync(tuple._1, tuple._2);
        }
        public static AssetBundleRequest LoadAssetAsync(string hostname, string filename)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetAsync(filename);
        }
        public static AssetBundleRequest LoadAssetAsync(string uri, Type type, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetAsync(tuple._1, tuple._2, type);
        }
        public static AssetBundleRequest LoadAssetAsync(string hostname, string filename, Type type)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetAsync(filename, type);
        }
        public static AssetBundleRequest LoadAssetAsync<T>(string uri, int separateIndex = -1) where T : UnityEngine.Object
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetAsync<T>(tuple._1, tuple._2);
        }
        public static AssetBundleRequest LoadAssetAsync<T>(string hostname, string filename) where T : UnityEngine.Object
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetAsync<T>(filename);
        }
        public static UnityEngine.Object[] LoadAssetWithSubAssets(string uri, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetWithSubAssets(tuple._1, tuple._2);
        }
        public static UnityEngine.Object[] LoadAssetWithSubAssets(string hostname, string filename)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetWithSubAssets(filename);
        }
        public static UnityEngine.Object[] LoadAssetWithSubAssets(string uri, Type type, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetWithSubAssets(tuple._1, tuple._2, type);
        }
        public static UnityEngine.Object[] LoadAssetWithSubAssets(string hostname, string filename, Type type)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetWithSubAssets(filename, type);
        }
        public static T[] LoadAssetWithSubAssets<T>(string uri, int separateIndex = -1) where T : UnityEngine.Object
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetWithSubAssets<T>(tuple._1, tuple._2);
        }
        public static T[] LoadAssetWithSubAssets<T>(string hostname, string filename) where T : UnityEngine.Object
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetWithSubAssets<T>(filename);
        }
        public static AssetBundleRequest LoadAssetWithSubAssetsAsync(string uri, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetWithSubAssetsAsync(tuple._1, tuple._2);
        }
        public static AssetBundleRequest LoadAssetWithSubAssetsAsync(string hostname, string filename)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetWithSubAssetsAsync(filename);
        }
        public static AssetBundleRequest LoadAssetWithSubAssetsAsync(string uri, Type type, int separateIndex = -1)
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetWithSubAssetsAsync(tuple._1, tuple._2, type);
        }
        public static AssetBundleRequest LoadAssetWithSubAssetsAsync(string hostname, string filename, Type type)
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetWithSubAssetsAsync(filename, type);
        }
        public static AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string uri, int separateIndex = -1) where T : UnityEngine.Object
        {
            var tuple = AssetBundleHolder.Extract(uri, separateIndex);
            return AssetBundleHolder.LoadAssetWithSubAssetsAsync<T>(tuple._1, tuple._2);
        }
        public static AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string hostname, string filename) where T : UnityEngine.Object
        {
            var bundle = AssetBundleHolder.GetBundle(hostname);
            if (!bundle)
                return null;

            return bundle.LoadAssetWithSubAssetsAsync<T>(filename);
        }
        public static void Unload(string hostname, bool unloadAllLoadedObjects)
        {
            var bundle = AssetBundleHolder.GetBundle(AssetBundleHolder.RemoveScheme(hostname));
            if (!bundle)
                return;

            bundle.Unload(unloadAllLoadedObjects);
        }
        public static void UnloadAllAssetBundles(bool unloadAllObjects)
        {
            foreach (var pair in AssetBundleHolder.GetBundleAll())
                AssetBundleHolder.Unload(pair.Key, unloadAllObjects);
        }

        //public static bool CloseAssetBundles(AssetBundle[] bundles, Action<bool> doneCallback)
        //{
        //}

        //static IEnumerator CloseAssetBundlesTask(CancellableSignal signal, string[] hostnames, Action<bool> doneCallback)
        //{
        //    var done = false;
        //    while (!done)
        //    {
        //        if (CancellableSignal.IsCancelled(signal))
        //        {
        //            if (null != doneCallback)
        //                doneCallback(false);
        //        }

        //        foreach (var hostname in hostnames)
        //        {
        //            if (null == hostname)
        //            {
        //                Debug.LogWarning(string.Format("ASSET_BUNDLE_HOLDER:CLOSE_ASSET_BUNDLES_TASK:EXCEPT:INVALID_TARGET:{0}", hostname));
        //                continue;
        //            }

        //            AssetBundleHolder.Unmount(hostname);
        //            AssetBundleHolder.Unload(hostname, true);

        //            yield return null;
        //        }

        //        done = true;
        //        hostnames = null;
                
        //        yield return null;
        //    }

        //    if (null != doneCallback)
        //        doneCallback(true);
        //}
    }
}
