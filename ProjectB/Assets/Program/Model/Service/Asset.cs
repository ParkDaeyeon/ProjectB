using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Ext.Unity3D;
using Ext.IO;
using Ext.Async;
using Ext.String;
using Program.Core;
using Program.Model.Domain.Asset;
using Program.Model.Service.Implement;

namespace Program.Model.Service
{
    using System.Threading.Collections;

    public sealed class Asset : ManagedSingleton<Asset._Singleton>
    {
        Asset() { }

        public static void Start()
        {
            Asset.Singleton.Start();
        }

        public enum State
        {
            None,
            OpenSheets,
            OpenServiceModels,
            Done,
            Except,
        }

        public static State GetState()
        {
            return Asset.Singleton.State;
        }

        public static bool IsDone
        {
            get
            {
                var state = Asset.Singleton.State;
                return State.Done == state || State.Except == state;
            }
        }

        public static bool IsSucceed
        {
            get { return State.Done == Asset.Singleton.State; }
        }

        public static float Progress
        {
            get { return Asset.Singleton.GetProgress(); }
        }

        public static string GetErrorLog()
        {
            return Asset.Singleton.StateErrorLog;
        }

        public static string GetDetailErrorLog()
        {
            return Asset.Singleton.StateDetailErrorLog;
        }
        
        public struct Report<T>
        {
            public bool succeed;
            public Dictionary<string, T> results;
        }
        public sealed class SubTaskHandle
        {
            CancellableSignal signal;
            public CancellableSignal Signal
            {
                get { return this.signal; }
            }

            object inData;
            public object InData
            {
                get { return this.inData; }
            }

            object exData;
            public object ExData
            {
                set { this.exData = value; }
                get { return this.exData; }
            }

            bool isDone = false;
            public bool IsDone
            {
                get { return this.isDone; }
            }

            float progress = 0;
            public float Progress
            {
                get { return this.progress; }
            }

            Exception except = null;
            public Exception Except
            {
                get { return this.except; }
            }
            public bool IsError
            {
                get { return null != this.except; }
            }


            internal SubTaskHandle(CancellableSignal signal, object implData = null)
            {
                this.signal = signal;
                this.inData = implData;
                this.Clear();
            }

            public void Clear()
            {
                this.isDone = false;
                this.progress = 0;
                this.except = null;
            }

            public void Complete()
            {
                this.isDone = true;
                this.progress = 1;
            }

            public void SetProgress(float p)
            {
                if (this.progress < p)
                    this.progress = p;
            }

            public void SetFailed(Exception except)
            {
                this.except = except;
                this.isDone = true;
            }

            public void SkipError()
            {
                this.except = null;
                if (1 > this.progress)
                    this.isDone = false;
            }
        }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
                ServiceModelManager.Open();

                this.stateMgrs = new StateManager[]
                {
                    null,// None,		
			        new StateManager { func = this.OnOpenSheets, progress = 0.1f, },// OpenSheets,
			        new StateManager { func = this.OnOpenServiceModels, progress = 0.2f, },// OpenServiceModels,
			        null,// Done,
			        null,// Except,
                };
            }

            public void CloseSingleton()
            {
                this.Stop();
                
                MVC.ModelDisposer.CloseAll();

                Resources.UnloadUnusedAssets();

                this.progress = 0;
                this.progressBegin = 0;
                this.progressEnd = 0;

                this.state = State.None;
                this.stateErrLog = "";

                ServiceModelManager.Close();
            }

            CancellableSignal signal;
            Coroutine coroutine;
            internal bool IsStarted
            {
                get { return null != this.signal; }
            }

            internal void Start()
            {
                this.Stop();

                this.stateErrLog = "";
                this.signal = new CancellableSignal();
                this.coroutine = CoroutineTaskManager.AddTask(this.OnStart(this.signal));
            }

            internal void Stop()
            {
                CancellableSignal.Cancel(this.signal);
                this.signal = null;

                if (this.coroutine != null)
                {
                    CoroutineTaskManager.RemoveTask(this.coroutine);
                    this.coroutine = null;
                }
            }


            State state;
            internal State State
            {
                get { return this.state; }
            }
            internal void SetFailed(CancellableSignal signal, char stateInitial, string errLog, string errLogDetail)
            {
                this.state = State.Except;
                this.stateErrLog = string.Format("{0}:{1}", stateInitial, errLog);
                this.stateDetailErrLog = string.Format("{0}:{1}", stateInitial, errLogDetail);

                CancellableSignal.Cancel(signal);
            }

            string stateErrLog = "";
            internal string StateErrorLog
            {
                get { return this.stateErrLog; }
            }

            string stateDetailErrLog = "";
            internal string StateDetailErrorLog
            {
                get { return this.stateDetailErrLog; }
            }


            delegate IEnumerator StateFunc(CancellableSignal signal);
            class StateManager
            {
                internal StateFunc func;
                internal float progress;
            }
            StateManager[] stateMgrs;

            IEnumerator SetState(State state, CancellableSignal signal)
            {
                this.state = state;

                int index = (int)state;
                if (-1 < index && index < this.stateMgrs.Length)
                {
                    var sd = this.stateMgrs[index];
                    if (null != sd)
                    {
                        this.SetProgressRange(sd.progress);
                        if (null != sd.func)
                            return sd.func(signal);
                    }
                }

                return null;
            }


            IEnumerator OnStart(CancellableSignal signal)
            {
                for (State state = State.None, cnt = State.Done; state < cnt; ++state)
                {
                    var enumerator = this.SetState(state, signal);
                    if (null == enumerator)
                        continue;

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                        if (CancellableSignal.IsCancelled(signal)) { yield break; }
                    }
                    if (CancellableSignal.IsCancelled(signal)) { yield break; }
                }

                // NOTE: last action
                if (this.progressEnd > this.progress)
                {
                    int lastFrameCount = (int)Mathf.Lerp(30, 15, this.progress);  // (0.5 ~ 0.25) + @ sec
                    float dt = 1 / (float)lastFrameCount;

                    for (int n = 0; n < lastFrameCount; ++n)
                    {
                        this.SetSubProgress(dt * n);
                        yield return null;
                        if (CancellableSignal.IsCancelled(signal)) { yield break; }
                    }
                }

                this.SetSubProgress(1);
                yield return null;
                if (CancellableSignal.IsCancelled(signal)) { yield break; }

                if (State.Except != this.state)
                {
                    this.state = State.Done;
                }

                this.coroutine = null;
            }


            IEnumerator OnOpenSheets(CancellableSignal signal)
            {
                var subTaskHandle = new SubTaskHandle(signal);
                var enumerator = ServiceModelManager.OpenSheetTask(subTaskHandle);
                while (enumerator.MoveNext())
                {
                    this.SetSubProgress(subTaskHandle.Progress);
                    yield return enumerator.Current;
                    if (CancellableSignal.IsCancelled(signal)) { yield break; }
                }

                if (subTaskHandle.IsError)
                {
#if LOG_DEBUG
                    Debug.LogWarning(string.Format("ASSET:EXCEPT:SHEETS:EXCEPT:{0}", subTaskHandle.Except));
#endif// LOG_DEBUG
                    var e = subTaskHandle.Except;
                    this.SetFailed(signal, 'S', e.GetType().Name, e.Message);
                    yield break;
                }

                this.SetSubProgress(1);
                yield return null;
                if (CancellableSignal.IsCancelled(signal)) { yield break; }
            }



            IEnumerator OnOpenServiceModels(CancellableSignal signal)
            {
                var subTaskHandle = new SubTaskHandle(signal);
                var tasks = ServiceModelManager.OpenServiceTask(subTaskHandle);
                int count = tasks.Length;
                float progress = 0f;
                float progressDelta = 1 / (float)count;

                foreach (var task in tasks)
                {
                    subTaskHandle.Clear();

                    while (task.MoveNext())
                    {
                        this.SetSubProgress(progress + (progressDelta * subTaskHandle.Progress));
                        yield return task.Current;
                        if (CancellableSignal.IsCancelled(signal)) { yield break; }
                    }

                    progress += progressDelta;
                    this.SetSubProgress(progress);

                    if (subTaskHandle.IsError)
                    {
#if LOG_DEBUG
                        Debug.LogWarning("ASSET:EXCEPT:SERVICE_MODELS");
#endif// LOG_DEBUG
                        var e = subTaskHandle.Except;
                        this.SetFailed(signal, 'C', e.GetType().Name, e.Message);
                        yield break;
                    }
                }

                this.SetSubProgress(1);
                yield return null;
                if (CancellableSignal.IsCancelled(signal)) { yield break; }
            }
            

            internal float GetProgress()
            {
                return this.progress;
            }
            float progress = 0;
            float progressBegin = 0;
            float progressEnd = 0;

            void SetProgressRange(float end)
            {
                this.progressBegin = this.progress = this.progressEnd;
                this.progressEnd = end;
            }

            void SetSubProgress(float t)
            {
                float value = this.progressBegin + (this.progressEnd - this.progressBegin) * Mathf.Clamp01(t);
                if (this.progress < value)
                    this.progress = value;
            }
        }
    }
}
