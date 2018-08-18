using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Ext;
using Ext.Unity3D;
using Ext.String;
using Ext.Event;
using Ext.Collection.AntiGC;

using Program.Core;
using Program.Model.Domain.Code;
using Program.View.Pop;
using Program.View.Common;

namespace Program.Model.Service.Implement
{
    public sealed class UriHandler : InternalSingleton<UriHandler._Singleton>
    {
        UriHandler() { }
        
        public struct Result
        {
            public Result(Uri uri, bool succeed, string status = null)
            {
                this.uri = uri;
                this.succeed = succeed;
                this.status = status;
            }
            public Uri uri;
            public bool succeed;
            public string status;
        }

        public delegate void Function(Uri uri, Action<Result> onResult);
        public static void Regist(string scheme, Function function)
        {
            UriHandler.Singleton.Regist(scheme, function);
        }

        public static bool Unregist(string scheme)
        {
            return UriHandler.Singleton.Unregist(scheme);
        }

        public static void UnregistAll()
        {
            UriHandler.Singleton.UnregistAll();
        }

        public static void Execute(string uriRaw, Action<Result> onResult)
        {
            UriHandler.Singleton.Execute(uriRaw, onResult);
        }

        public static int Count
        {
            get { return UriHandler.Singleton.Count; }
        }
        public static IEnumerable<string> Schemes
        {
            get { return UriHandler.Singleton.Schemes; }
        }
        public static IEnumerable<Function> Functions
        {
            get { return UriHandler.Singleton.Functions; }
        }
        public static Function GetFunction(string scheme)
        {
            return UriHandler.Singleton.GetFunction(scheme);
        }


        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
                this.map = new Dictionary<string, Function>();
            }

            public void CloseSingleton()
            {
                this.map.Clear();
                this.map = null;
            }
            

            Dictionary<string, Function> map;
            internal void Regist(string scheme, Function function)
            {
                this.map[scheme] = function;
            }

            internal bool Unregist(string scheme)
            {
                return this.map.Remove(scheme);
            }

            internal void UnregistAll()
            {
                this.map.Clear();
            }

            internal void Execute(string uriRaw, Action<Result> onResult)
            {
                if (string.IsNullOrEmpty(uriRaw))
                {
                    if (null != onResult)
                        onResult(new Result(new Uri(""), false, "EMPTY_URI"));
                    return;
                }
                
                var uri = new Uri(uriRaw);
                var scheme = uri.Scheme;
                if (string.IsNullOrEmpty(scheme))
                {
                    if (null != onResult)
                        onResult(new Result(uri, false, "EMPTY_SCHEME"));
                    return;
                }
                
                scheme = scheme.ToLower();
                Function function;
                if (!this.map.TryGetValue(scheme, out function))
                {
                    if (null != onResult)
                        onResult(new Result(uri, false, "UNKNOWN_SCHEME"));
                    return;
                }
                
                if (null == function)
                {
                    if (null != onResult)
                        onResult(new Result(uri, false, "INVALID_FUNCTION"));
                    return;
                }

                function(uri, onResult);
            }

            internal int Count
            {
                get { return this.map.Count; }
            }
            internal IEnumerable<string> Schemes
            {
                get { return this.map.Keys; }
            }
            internal IEnumerable<Function> Functions
            {
                get { return this.map.Values; }
            }
            internal Function GetFunction(string scheme)
            {
                Function data;
                if (!this.map.TryGetValue(scheme, out data))
                    return null;

                return data;
            }
        }
    }
}
