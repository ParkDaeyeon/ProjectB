using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Ext;
using Ext.Unity3D;
using Ext.String;
using Ext.Event;
using Ext.Async;
using Ext.Unity3D.UI;
using Ext.IO;
using Ext.Algorithm;

using Newtonsoft.Json;

using Program.Core;
using Program.View;

namespace Program.Model.Service
{
    public sealed class NetStat : InternalSingleton<NetStat._Singleton>
    {
        NetStat() { }

        public static bool Locked
        {
            get { return NetStat.Singleton.Locked; }
        }

        public static void SetLock(uint flags, bool @lock)
        {
            NetStat.Singleton.SetLock(flags, @lock);
        }

        public static void Update()
        {
            NetStat.Singleton.Update();
        }

        public static void LateUpdate()
        {
            NetStat.Singleton.LateUpdate();
        }

        public static void Disconnect(Action onDone)
        {
            NetStat.Singleton.Disconnect(onDone);
        }

        public static void Suspend()
        {
            //NetStat.Singleton.Suspend();
        }

        public static void Resume()
        {
            //NetStat.Singleton.Resume();
        }
        
        public static void OpenNetwork(Action<bool> onDone)
        {
#if TEST_LATENCY
            Wait.Active(2f, false, (_) => {
#endif// TEST_LATENCY
            NetStat.Singleton.OpenNetwork(onDone);
#if TEST_LATENCY
            });
#endif// TEST_LATENCY
        }

        public static void CloseNetwork()
        {
            NetStat.Singleton.CloseNetwork();
        }

        public static bool IsNetworkConnected()
        {
            return NetStat.Singleton.IsNetworkConnected();
        }
#if UNITY_SWITCH && !UNITY_EDITOR
#else// UNITY_SWITCH && !UNITY_EDITOR
        public static bool FakeNetworkConn
        {
            set { NetStat.Singleton.FakeNetworkConn = value; }
            get { return NetStat.Singleton.FakeNetworkConn; }
        }
        public static bool FakeNetworkApi
        {
            set { NetStat.Singleton.FakeNetworkApi = value; }
            get { return NetStat.Singleton.FakeNetworkApi; }
        }
#endif// UNITY_SWITCH && UNITY_EDITOR

        public static bool IsNetworkConnecting()
        {
            return NetStat.Singleton.IsNetworkConnecting();
        }

#if UNITY_SWITCH
        public struct NexInfo
        {
            public NexInfo(uint gameServerId,
                           string accessKey)
            {
                this.gameServerId = gameServerId;
                this.accessKey = accessKey;
            }

            uint gameServerId;
            internal uint GameServerId
            {
                set { this.gameServerId = value; }
                get { return this.gameServerId; }
            }

            string accessKey;
            internal string AccessKey
            {
                set { this.accessKey = value; }
                get { return this.accessKey; }
            }
        }
        public static void SetNexInfo(NexInfo value)
        {
            NetStat.Singleton.SetNexInfo(value);
        }
        public static NexInfo GetNexInfo()
        {
            return NetStat.Singleton.GetNexInfo();
        }

        public static void OpenNex(Action<Exception> onDone)
        {
            NetStat.Singleton.OpenNex(onDone);
        }

        public static void CloseNex()
        {
            NetStat.Singleton.CloseNex();
        }

        public static bool AvailableNex
        {
            get { return NetStat.Singleton.AvailableNex; }
        }

        public static bool LoggedNex
        {
            get { return NetStat.Singleton.LoggedNex; }
        }
        
        public static event Action OnLostNex
        {
            add { NetStat.Singleton.OnLostNex += value; }
            remove { NetStat.Singleton.OnLostNex -= value; }
        }

        public static bool KeepAlive
        {
            set { NetStat.Singleton.KeepAlive = value; }
            get { return NetStat.Singleton.KeepAlive; }
        }

        public class NexHandle
        {
            public NexHandle(UserHandle userHandle,
                             NetworkServiceAccountId nsaId,
                             byte[] nsaIdTok,
                             ulong ppid,
                             IntPtr pngs)
            {
                this.userHandle = userHandle;
                this.nsaId = nsaId;
                this.nsaIdTok = nsaIdTok;
                this.ppid = ppid;
                this.pngs = pngs;
            }

            UserHandle userHandle;
            public UserHandle UserHandle
            {
                get { return this.userHandle; }
            }

            NetworkServiceAccountId nsaId;
            public NetworkServiceAccountId NsaId
            {
                get { return this.nsaId; }
            }

            byte[] nsaIdTok;
            [JsonIgnore]
            public byte[] NsaIdTok
            {
                get { return this.nsaIdTok; }
            }

            ulong ppid;
            public ulong PrincipalId
            {
                get { return this.ppid; }
            }

            IntPtr pngs;
            public IntPtr NgsFacadePtr
            {
                get { return this.pngs; }
            }

            public override string ToString()
            {
                return ConvertString.Execute(this);
            }
        }

        public enum NexLoginResult
        {
            Ok,
            Expired,
            Cancelled,
            Unavailable,
            InvalidNsa,
            NotFoundNsa,
            EnsureIdToken,
            EnsureIdTokenPost,
            LoadIdToken,
            FailedLoginAsync,
            Failed,
            Except,
        }
        public static void LoginNex(UserHandle userHandle,
                                    Action<NexLoginResult, NexHandle> onDone)
        {
#if TEST_LATENCY
            Wait.Active(2f, false, (_) => {
#endif// TEST_LATENCY
            NetStat.Singleton.LoginNex(userHandle, onDone);
#if TEST_LATENCY
            });
#endif// TEST_LATENCY
        }
        
        public static void LogoutNex(Action onDone)
        {
#if TEST_LATENCY
            Wait.Active(2f, false, (_) => {
#endif// TEST_LATENCY
            NetStat.Singleton.LogoutNex(onDone);
#if TEST_LATENCY
            });
#endif// TEST_LATENCY
        }
        
        public static bool VerifyAsyncResult(NexPlugin.AsyncResult aret)
        {
            return NetStat.Singleton.VerifyAsyncResult(aret);
        }
        public static bool VerifyNnResult(nn.Result nnret)
        {
            return NetStat.Singleton.VerifyNnResult(nnret);
        }
#else// UNITY_SWITCH
        public class NexHandle
        {
        }
#endif// UNITY_SWITCH

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
            }

            public void CloseSingleton()
            {
#if UNITY_SWITCH
                this.cachedHandle = null;
                this.CloseNex();
                this.CloseNetwork();
#endif// UNITY_SWITCH
            }

            uint lockFlags = 0;
            internal bool Locked
            {
                get { return 0 != this.lockFlags; }
            }

            internal void SetLock(uint flags, bool @lock)
            {
                if (@lock)
                    this.lockFlags = BitFlag.Add(this.lockFlags, flags);
                else
                    this.lockFlags = BitFlag.Remove(this.lockFlags, flags);
            }

            internal void Disconnect(Action onDone)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:DISCONNECT:START"));
#endif// LOG_NET

#if UNITY_SWITCH
                this.LogoutNex(() =>
#endif// UNITY_SWITCH
                {
#if UNITY_SWITCH
                    this.CloseNex();
#endif// UNITY_SWITCH
                    this.CloseNetwork();
#if LOG_NET
                    Debug.Log(string.Format("NETSTAT:DISCONNECT:DONE"));
#endif// LOG_NET
                    if (null != onDone)
                        onDone();
                }
#if UNITY_SWITCH
                );
#endif// UNITY_SWITCH
            }

            internal void Update()
            {
#if UNITY_SWITCH
                this.UpdateNexLostConnection();
                this.UpdateNexAsync();
                this.UpdateNexDispatch();
#endif// UNITY_SWITCH
            }
            internal void LateUpdate()
            {
#if UNITY_SWITCH
                this.UpdateNexDispatch();
#endif// UNITY_SWITCH
            }

            internal void OpenNetwork(Action<bool> onDone)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NET:OPEN:START"));
#endif// LOG_NET
                if (this.Locked)
                {
                    Debug.LogError(string.Format("NETSTAT:NET:OPEN:LOCKED"));
                    if (null != onDone)
                        onDone(false);
                    return;
                }

                var connected = this.IsNetworkConnected();
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NET:OPEN:CONNTED:{0}", connected));
#endif// LOG_NET
                if (connected)
                {
#if LOG_NET
                    Debug.Log(string.Format("NETSTAT:NET:OPEN:ALREADY_CONNTED"));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(true);
                }
                else
                {
                    this.CloseNetwork();
#if UNITY_SWITCH && !UNITY_EDITOR
                    var gen = NetStat.Generation;
                    var signal = new CancellableSignal(() => gen != NetStat.Generation);
                    this.connecting = true;
                    this.SetLock(0x80000000, true);
                    CoroutineTaskManager.AddTask(this.OnConnectNetworkTask(signal, (succeed) =>
                    {
                        this.SetLock(0x80000000, false);
                        this.connecting = false;
                        if (null != onDone)
                            onDone(succeed);
                    }));
#else// UNITY_SWITCH && !UNITY_EDITOR
                    this.testNetwork = this.fakeNetworkConn;
#if LOG_NET
                    Debug.Log(string.Format("NETSTAT:NET:OPEN:FAKE_TEST:{0}", this.testNetwork));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(this.testNetwork);
#endif// UNITY_SWITCH && !UNITY_EDITOR
                }
            }
#if UNITY_SWITCH && !UNITY_EDITOR
            bool connecting = false;
            IEnumerator OnConnectNetworkTask(CancellableSignal signal, Action<bool> onDone)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NET:OPEN:TASK:IN"));
#endif// LOG_NET
                NetworkInterfaceWrapper.EnterNetworkConnecting();
                while (NetworkInterfaceWrapper.IsNetworkConnecting())
                {
                    yield return null;
                    if (CancellableSignal.IsCancelled(signal))
                        yield break;
                }

                var succeed = this.IsNetworkConnected();
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NET:OPEN:TASK:SUCCEED:{0}", succeed));
#endif// LOG_NET
                if (null != onDone)
                    onDone(succeed);
            }
#endif// UNITY_SWITCH && !UNITY_EDITOR

            internal void CloseNetwork()
            {
                var count = 0;
#if UNITY_SWITCH && !UNITY_EDITOR
                count = NetworkInterfaceWrapper.GetNetworkReferenceCount();
#endif// UNITY_SWITCH && !UNITY_EDITOR
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NET:CLOSE:{0}", count));
#endif// LOG_NET
#if UNITY_SWITCH && !UNITY_EDITOR
                for (int n = count - 1; n >= 0; --n)
                    NetworkInterfaceWrapper.LeaveNetworkConnecting();
#else// UNITY_SWITCH && !UNITY_EDITOR
                this.testNetwork = false;
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            internal bool IsNetworkConnected()
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                return NetworkInterfaceWrapper.IsNetworkAccepted();
#else// UNITY_SWITCH && !UNITY_EDITOR
                return this.testNetwork;
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }
#if UNITY_SWITCH && !UNITY_EDITOR
#else// UNITY_SWITCH && !UNITY_EDITOR
            bool testNetwork = false;
            bool fakeNetworkConn = false;
            internal bool FakeNetworkConn
            {
                set
                {
                    this.fakeNetworkConn = value;
                    Debug.Log(string.Format("NETSTAT:FAKE_NET_CONN:{0}", value));
                }
                get { return this.fakeNetworkConn; }
            }
            bool fakeNetworkApi = false;
            internal bool FakeNetworkApi
            {
                set
                {
                    this.fakeNetworkApi = value;
                    Debug.Log(string.Format("NETSTAT:FAKE_NET_API:{0}", value));
                }
                get { return this.fakeNetworkApi; }
            }
#endif// UNITY_SWITCH && UNITY_EDITOR

            internal bool IsNetworkConnecting()
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                return this.connecting;
#else// UNITY_SWITCH && !UNITY_EDITOR
                return false;
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }


#if UNITY_SWITCH
            NexInfo nexInfo = new NexInfo();
            internal void SetNexInfo(NexInfo value)
            {
                this.nexInfo = value;
            }
            internal NexInfo GetNexInfo()
            {
                return this.nexInfo;
            }

            internal void OpenNex(Action<Exception> onDone)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:OPEN:START:PREV:{0}", this.availableNex));
#endif// LOG_NET
                if (this.Locked)
                {
                    Debug.LogError(string.Format("NETSTAT:NEX:OPEN:LOCKED"));
                    if (null != onDone)
                        onDone(new Exception("Locked"));
                    return;
                }

                if (this.availableNex)
                {
                    if (null != onDone)
                        onDone(null);
                    return;
                }

                var gen = NetStat.Generation;
                var signal = new CancellableSignal(() => gen != NetStat.Generation);
                this.SetLock(0x80000000, true);
                CoroutineTaskManager.AddTask(this.OnOpenNexTask(signal, (e) =>
                {
                    this.SetLock(0x80000000, false);
                    if (null != onDone)
                        onDone(e);
                }));
            }

            internal void CloseNex()
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:CLOSE:PREV:{0}", this.availableNex));
#endif// LOG_NET
                this.availableNex = false;
#if UNITY_SWITCH && !UNITY_EDITOR
                NexPlugin.Common.FinalizeNex();
                NexPlugin.Common.FinalizeNexPlugin();
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            IEnumerator OnOpenNexTask(CancellableSignal signal, Action<Exception> onDone)
            {
                this.CloseNex();
#if UNITY_SWITCH && !UNITY_EDITOR
                uint KB = 1024;
                uint MB = 1024 * KB;
                uint PLUGIN_MEMSIZE = 2 * MB;
                uint NEX_MEMSIZE = 4 * MB;
                uint RESERVE_MEMSIZE = 1 * MB;
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:OPEN_TASK:INIT_PLUG:PLUGIN_MEMSIZE:{0}, NEX_MEMSIZE:{1}, RESERVE_MEMSIZE:{2}",
                                         PLUGIN_MEMSIZE, NEX_MEMSIZE, RESERVE_MEMSIZE));
#endif// LOG_NET
                if (!NexPlugin.Common.InitializeNexPlugin(PLUGIN_MEMSIZE, NEX_MEMSIZE, RESERVE_MEMSIZE))
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:OPEN_TASK:INIT_PLUG:FAILED"));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(new Exception("InitializeNexPlugin"));
                    yield break;
                }
                
                uint asyncId = 0;
                var aret = default(NexPlugin.AsyncResult);
                var wait = true;
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:OPEN_TASK:INIT_ASYNC"));
#endif// LOG_NET
                var initNexAsync = NexPlugin.Common.InitializeNexAsync(out asyncId, NexPlugin.Common.ThreadMode.ThreadModeSafeTransportBuffer, (aret_) =>
                {
                    aret = aret_;
                    wait = false;
                });
                if (!initNexAsync)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:OPEN_TASK:INIT_ASYNC:FAILED"));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(new Exception("InitializeNexAsync"));
                    yield break;
                }
                while (wait)
                {
                    yield return null;
                    if (CancellableSignal.IsCancelled(signal))
                        yield break;
                }
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:OPEN_TASK:INIT_ASYNC:ARET:{0}", aret));
#endif// LOG_NET
                if (aret.IsSuccess())
                {
                    this.availableNex = true;
                    if (null != onDone)
                        onDone(null);
                }
                else
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:OPEN_TASK:INIT_ASYNC:FAILED_2"));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(new Exception("InitializeNexAsyncError:" + aret.ToString()));
                }
#else// UNITY_SWITCH && !UNITY_EDITOR
                {
                    this.availableNex = true;
                    if (null != onDone)
                        onDone(null);
                    yield break;
                }
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            bool availableNex;
            internal bool AvailableNex
            {
                get { return this.availableNex; }
            }
            
            NexHandle cachedHandle = null;
            internal bool LoggedNex
            {
                get { return null != this.cachedHandle; }
            }

            event Action onLostNex;
            internal event Action OnLostNex
            {
                add { this.onLostNex += value; }
                remove { this.onLostNex -= value; }
            }

            bool keepAlive;
            internal bool KeepAlive
            {
                set { this.keepAlive = value; }
                get { return this.keepAlive; }
            }

            void UpdateNexLostConnection()
            {
                if (this.Locked)
                    return;

#if UNITY_SWITCH && !UNITY_EDITOR
                if (!NexPlugin.Common.IsInitializedNex())
                    return;

                if (!this.LoggedNex)
                    return;

                if (NexPlugin.Common.IsNetworkConnected())
                {
                    if (NexPlugin.NgsFacade.IsConnected(this.cachedHandle.NgsFacadePtr).IsSuccess())
                        return;
#if LOG_NET
                    else
                        Debug.Log(string.Format("NETSTAT:NEX:LOST:NGS_ISSUE"));
#endif// LOG_NET
                }
#if LOG_NET
                else
                    Debug.Log(string.Format("NETSTAT:NEX:LOST:NETWORK_ISSUE"));
#endif// LOG_NET

                this.LogoutNex(() =>
                {
                    this.CloseNex();
                    if (null != this.onLostNex)
                        this.onLostNex();
                });
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            void UpdateNexAsync()
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                NexPlugin.Common.UpdateAsyncResult();
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            void UpdateNexDispatch()
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                if (this.keepAlive)
                {
                    var flags = NexPlugin.Common.DispachFlag.DispatchKeepAliveOnly;
                    NexPlugin.Common.Dispatch(30000, flags);
                }
                else
                    NexPlugin.Common.Dispatch(30000);
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            internal void LoginNex(UserHandle userHandle,
                                   Action<NexLoginResult, NexHandle> onDone)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:LOGIN:START:UHAND:{0}", userHandle));
#endif// LOG_NET
                if (this.Locked)
                {
                    Debug.LogError(string.Format("NETSTAT:NEX:LOGIN:LOCKED"));
                    if (null != onDone)
                        onDone(NexLoginResult.Except, null);
                    return;
                }

                if (this.LoggedNex)
                {
                    var userHandleOld = this.cachedHandle.UserHandle;
                    if (userHandleOld._data0 == userHandle._data0 &&
                        userHandleOld._data1 == userHandle._data1 &&
                        userHandleOld._context == userHandle._context)
                    {
                        var nsaId = default(NetworkServiceAccountId);
                        var ret_ = this.GetNsaId(userHandle, out nsaId);
                        if (NsaIdResult.Ok == ret_)
                        {
                            if (null != onDone)
                                onDone(NexLoginResult.Ok, this.cachedHandle);
                            return;
                        }
                    }

                    this.LogoutNex(() =>
                    {
                        if (null != onDone)
                            onDone(NexLoginResult.Expired, null);
                    });
                    return;
                }

                if (!this.availableNex)
                {
                    if (null != onDone)
                        onDone(NexLoginResult.Unavailable, null);
                    return;
                }

                var gen = NetStat.Generation;
                var signal = new CancellableSignal(() => gen != NetStat.Generation);
                this.SetLock(0x80000000, true);
                CoroutineTaskManager.AddTask(this.OnLoginNex(signal, userHandle, (ret, handle) =>
                {
                    this.SetLock(0x80000000, false);
                    if (null != onDone)
                        onDone(ret, handle);
                }));
            }

            enum NsaIdResult
            {
                Ok,
                Cancelled,
                Invalid,
                NotFound,
                Except,
            }
            NsaIdResult GetNsaId(UserHandle userHandle,
                                 out NetworkServiceAccountId nsaId)
            {
                nsaId = default(NetworkServiceAccountId);
#if UNITY_SWITCH && !UNITY_EDITOR
                try
                {
#if LOG_NET
                    Debug.Log(string.Format("NETSTAT:NEX:GET_NAS_ID:ENSURE_AVAILABLE"));
#endif// LOG_NET
                    {
                        var nnret = NetworkServiceAccount.EnsureAvailable(userHandle);
                        if (Account.ResultCancelledByUser.Includes(nnret))
                            return NsaIdResult.Cancelled;
                        if (!this.VerifyNnResult(nnret))
                            return NsaIdResult.Invalid;
                    }
                    
#if LOG_NET
                    Debug.Log(string.Format("NETSTAT:NEX:GET_NAS_ID:GETTING"));
#endif// LOG_NET
                    {
                        var nnret = NetworkServiceAccount.GetId(ref nsaId, userHandle);
                        if (!this.VerifyNnResult(nnret))
                            return NsaIdResult.NotFound;
                    }
                }
                catch (Exception e)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:GET_NAS_ID:EXCEPT:{0}", e));
#endif// LOG_NET
                    return NsaIdResult.Except;
                }
#endif// UNITY_SWITCH && !UNITY_EDITOR
                return NsaIdResult.Ok;
            }

            IEnumerator OnLoginNex(CancellableSignal signal,
                                   UserHandle userHandle,
                                   Action<NexLoginResult, NexHandle> onDone)
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                var nsaId = default(NetworkServiceAccountId);
                {
                    var nsaret = this.GetNsaId(userHandle, out nsaId);
                    if (NsaIdResult.Ok != nsaret)
                    {
                        var ret_ = NexLoginResult.Except;
                        switch (nsaret)
                        {
                        case NsaIdResult.Cancelled:
                            ret_ = NexLoginResult.Cancelled; break;
                        case NsaIdResult.Invalid:
                            ret_ = NexLoginResult.InvalidNsa; break;
                        case NsaIdResult.NotFound:
                            ret_ = NexLoginResult.NotFoundNsa; break;
                        }
                        if (null != onDone)
                            onDone(ret_, null);
                        yield break;
                    }
                }

                var pOutContext = new AsyncContext();
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:ONLOGIN:ENSURE_ID_TOKEN_CACHE"));
#endif// LOG_NET
                try
                {
                    var nnret = NetworkServiceAccount.EnsurIdTokenCacheAsync(pOutContext, userHandle);
                    if (!this.VerifyNnResult(nnret))
                    {
                        if (null != onDone)
                            onDone(NexLoginResult.EnsureIdToken, null);
                        yield break;
                    }
                }
                catch (Exception e)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:ONLOGIN:ENSURE_ID_TOKEN_CACHE:EXCEPT:{0}", e));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(NexLoginResult.Except, null);
                    yield break;
                }

                var done = false;
                do
                {
                    yield return null;
                    if (CancellableSignal.IsCancelled(signal))
                        yield break;

                    try
                    {
                        pOutContext.HasDone(ref done);
                    }
                    catch (Exception e)
                    {
#if LOG_NET
                        Debug.LogWarning(string.Format("NETSTAT:NEX:ONLOGIN:ENSURE_ID_TOKEN_CACHE_WAIT:EXCEPT:{0}", e));
#endif// LOG_NET
                        if (null != onDone)
                            onDone(NexLoginResult.Except, null);
                        yield break;
                    }
                }
                while (!done);

                try
                {
                    var nnret = pOutContext.GetResult();
                    if (!this.VerifyNnResult(nnret))
                    {
                        if (null != onDone)
                            onDone(NexLoginResult.EnsureIdTokenPost, null);
                        yield break;
                    }
                }
                catch (Exception e)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:ONLOGIN:ENSURE_ID_TOKEN_CACHE_OUTPUT:EXCEPT:{0}", e));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(NexLoginResult.Except, null);
                    yield break;
                }

                var size = 0ul;
                var nsaIdTok = new byte[NetworkServiceAccount.IdTokenLengthMax];
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:ONLOGIN:LOAD_ID_TOKEN_CACHE"));
#endif// LOG_NET
                try
                {
                    var nnret = NetworkServiceAccount.LoadIdTokenCache(ref size, nsaIdTok, userHandle);
                    if (!this.VerifyNnResult(nnret))
                    {
                        if (null != onDone)
                            onDone(NexLoginResult.LoadIdToken, null);
                        yield break;
                    }
                }
                catch (Exception e)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:ONLOGIN:LOAD_ID_TOKEN_CACHE:EXCEPT:{0}", e));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(NexLoginResult.Except, null);
                    yield break;
                }

                uint asyncId;
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:ONLOGIN:ASYNC"));
#endif// LOG_NET
                try
                {
                    if (!NexPlugin.NgsFacade.LoginAsync(out asyncId,
                                                        this.nexInfo.GameServerId,
                                                        this.nexInfo.AccessKey,
                                                        nsaId,
                                                        nsaIdTok,
                                                        30000,
                    (aret, ppid, pngs) =>
                    {
                        if (CancellableSignal.IsCancelled(signal))
                            return;
#if LOG_NET
                        Debug.Log(string.Format("NETSTAT:NEX:ONLOGIN:ASYNC:RET:{0}", aret.ToString()));
#endif// LOG_NET
                        var ret = this.VerifyAsyncResult(aret) ? NexLoginResult.Ok : NexLoginResult.Failed;
                        var handle = default(NexHandle);
                        if (NexLoginResult.Ok == ret)
                        {
                            handle = this.cachedHandle = new NexHandle(userHandle,
                                                                       nsaId,
                                                                       nsaIdTok,
                                                                       ppid,
                                                                       pngs);
                        }
                        if (null != onDone)
                            onDone(ret, handle);
                    }))
                    {
                        if (null != onDone)
                            onDone(NexLoginResult.FailedLoginAsync, null);
                        yield break;
                    }
                }
                catch (Exception e)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:ONLOGIN:ASYNC:EXCEPT:{0}", e));
#endif// LOG_NET
                    if (null != onDone)
                        onDone(NexLoginResult.Except, null);
                }
#else// UNITY_SWITCH && !UNITY_EDITOR
                {
                    this.cachedHandle = new NexHandle(userHandle, new NetworkServiceAccountId(), new byte[] { 0, 1, 2, 3, }, 1, (IntPtr)1);
                    if (null != onDone)
                        onDone(NexLoginResult.Ok, cachedHandle);
                    yield break;
                }
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            internal void LogoutNex(Action onDone)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:LOGOUT:START"));
#endif// LOG_NET
                if (this.Locked)
                {
                    Debug.LogError(string.Format("NETSTAT:NEX:LOGOUT:LOCKED"));
                    if (null != onDone)
                        onDone();
                    return;
                }

                var handle = this.cachedHandle;
                this.cachedHandle = null;
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:LOGOUT:HAS_HANDLE:{0}", handle));
#endif// LOG_NET
                if (null == handle)
                {
                    if (null != onDone)
                        onDone();
                    return;
                }

                this.SetLock(0x80000000, true);
                this.OnLogoutNex(handle, () =>
                {
                    this.SetLock(0x80000000, false);
                    if (null != onDone)
                        onDone();
                });
            }

            void OnLogoutNex(NexHandle handle, Action onDone)
            {
#if UNITY_SWITCH && !UNITY_EDITOR
                var gen = NetStat.Generation;
                uint asyncId;
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:NEX:LOGOUT:ASYNC"));
#endif// LOG_NET
                try
                {
                    if (!NexPlugin.NgsFacade.LogoutAsync(out asyncId,
                                                         handle.NgsFacadePtr,
                    (aret) =>
                    {
                        if (gen != NetStat.Generation)
                            return;
#if LOG_NET
                        Debug.Log(string.Format("NETSTAT:NEX:LOGOUT:ASYNC:RET:{0}", aret.ToString()));
#endif// LOG_NET
                        this.VerifyAsyncResult(aret);
                        if (null != onDone)
                            onDone();
                    }))
                    {
                        if (null != onDone)
                            onDone();
                        return;
                    }
                }
                catch (Exception e)
                {
#if LOG_NET
                    Debug.LogWarning(string.Format("NETSTAT:NEX:LOGOUT:ASYNC:EXCEPT:{0}", e));
#endif// LOG_NET
                    if (null != onDone)
                        onDone();
                }
#else// UNITY_SWITCH && !UNITY_EDITOR
                {
                    this.cachedHandle = null;
                    if (null != onDone)
                        onDone();
                }
#endif// UNITY_SWITCH && !UNITY_EDITOR
            }

            internal bool VerifyAsyncResult(NexPlugin.AsyncResult aret)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:VERIFY_ARET:{0}", aret));
#endif// LOG_NET
                if (null != aret)
                {
#if LOG_NET
                    Debug.Log(string.Format("NETSTAT:VERIFY_ARET:NNRET:{0}", aret.nnResult));
                    Debug.Log(string.Format("NETSTAT:VERIFY_ARET:RET_CODE:{0}, ERR_CODE:{1}, NETERR_CODE:{2}",
                                             aret.returnCode, aret.errorCode, aret.netErrCode));
#endif// LOG_NET
                    if (aret.IsFailure())
                    {
                        if (this.VerifyNnResult(aret.nnResult))
#if LOG_NET
                            Debug.LogWarning(string.Format("NETSTAT:VERIFY_ARET:FAILED_BUT_NNSUCCEED"));
#endif// LOG_NET
                        return false;
                    }
                }

                return true;
            }

            internal bool VerifyNnResult(nn.Result nnret)
            {
#if LOG_NET
                Debug.Log(string.Format("NETSTAT:VERIFY_NNRET:{0}", nnret));
#endif// LOG_NET
                if (null != nnret)
                {
                    if (!nnret.IsSuccess())
                    {
#if LOG_NET
                        Debug.LogWarning(string.Format("NETSTAT:VERIFY_NNRET:FAILED"));
#endif// LOG_NET
//#if DEBUG
                        nn.err.Error.Show(nnret, true);
//#endif// DEBUG
                        return false;
                    }
                }

                return true;
            }
#endif// UNITY_SWITCH
            }
        }
}
