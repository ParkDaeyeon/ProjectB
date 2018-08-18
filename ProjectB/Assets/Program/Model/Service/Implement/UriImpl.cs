using UnityEngine;
using System;
using System.Collections.Generic;
using Program.Core;
using Program.Model.Domain.Type;
using Program.Model.Service.Content;
using Program.Presents;
using Program.Presents.Helper;
using Program.View;
using Ext.String;

namespace Program.Model.Service.Implement
{
    public sealed class UriImpl : InternalSingleton<UriImpl._Singleton>
    {
        UriImpl() { }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
                UriHandler.Regist("http", this.ExecuteWeb);
                UriHandler.Regist("https", this.ExecuteWeb);
                UriHandler.Regist("pianista", this.ExecutePianista);
            }

            public void CloseSingleton()
            {
                UriHandler.Unregist("http");
                UriHandler.Unregist("https");
                UriHandler.Unregist("pianista");
            }

            void ExecuteWeb(Uri uri, Action<UriHandler.Result> onResult)
            {
                Application.OpenURL(uri.ToString());

                if (null != onResult)
                    onResult(new UriHandler.Result(uri, true, "WEB"));
            }

            void ExecutePianista(Uri uri, Action<UriHandler.Result> onResult)
            {
                var host = uri.Host;
                if (string.IsNullOrEmpty(host))
                {
                    if (null != onResult)
                        onResult(new UriHandler.Result(uri, false, "EMPTY_HOST"));
                    return;
                }

                host = host.ToLower();
                switch (host)
                {
                case "inapp":
                    this.ExecuteInApp(uri, onResult);
                    break;
                default:
                    if (null != onResult)
                        onResult(new UriHandler.Result(uri, false, "UNKNOWN_HOST"));
                    break;
                }
            }

            void ExecuteInApp(Uri uri, Action<UriHandler.Result> onResult)
            {
                var pathAndQuery = uri.PathAndQuery;
                if (string.IsNullOrEmpty(pathAndQuery))
                {
                    if (null != onResult)
                        onResult(new UriHandler.Result(uri, false, "EMPTY_PATH"));
                    return;
                }

                var parsed = UriQuery.Parse(pathAndQuery);
                var path = parsed.headText;
                var keyValues = parsed.keyValues;
                if (string.IsNullOrEmpty(path))
                {
                    if (null != onResult)
                        onResult(new UriHandler.Result(uri, false, "INVALID_PATH"));
                    return;
                }

                path = path.ToLower();
                switch (path)
                {
                default:
                    if (null != onResult)
                        onResult(new UriHandler.Result(uri, false, "UNKNOWN_PATH"));
                    break;
                }
            }
        }
    }
}
