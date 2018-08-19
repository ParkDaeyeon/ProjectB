using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using Ext;
using Ext.Unity3D;
using Ext.Async;
using Ext.IO;

using Program.View;

namespace Program.Core
{
    public abstract class Present : InternalSingleton<Present._Singleton>
    {
        public static void Back()
        {
            Present.Singleton.Back();
        }

        public enum BackResult
        {
            Ok,
            Blocked,
        }
        
        public static Func<BackResult> OnCheckBack
        {
            set { Present.Singleton.OnCheckBack = value; }
            get { return Present.Singleton.OnCheckBack; }
        }

        public static bool IsSuspended
        {
            get { return Present.Singleton.IsSuspended; }
        }

        public static bool IsEnableBackButton
        {
            set { Present.Singleton.IsEnableBackButton = value; }
            get { return Present.Singleton.IsEnableBackButton; }
        }


        float loadingProgress;
        public float LoadingProgress
        {
            get { return this.loadingProgress; }
        }

        bool isPresentOpened;
        public bool IsPresentOpened
        {
            get { return this.isPresentOpened; }
        }

        public virtual bool DoOpen
        {
            get { return true; }
        }

        protected virtual void OnPreloading()
        {
#if LOG_DEBUG
            Debug.Log(string.Format("PRESENT<{0}={1:X8}>:PRELOADING",
                                     this.GetType().Name,
                                     (uint)this.GetHashCode()));
#endif// LOG_DEBUG
        }

        protected virtual void OnOpen(object openArg)
        {
#if LOG_DEBUG
            Debug.Log(string.Format("PRESENT<{0}={1:X8}>:OPEN:{2}",
                                     this.GetType().Name,
                                     (uint)this.GetHashCode(),
                                     openArg));
#endif// LOG_DEBUG
        }

        protected virtual void OnClose(Type nextPresent)
        {
#if LOG_DEBUG
            Debug.Log(string.Format("PRESENT<{0}={1:X8}>:CLOSE:NEXT:<{2}={3:X8}>",
                                     this.GetType().Name,
                                     (uint)this.GetHashCode(),
                                     null != nextPresent ? nextPresent.Name : "null",
                                     null != nextPresent ? (uint)nextPresent.GetHashCode() : 0u));
#endif// LOG_DEBUG
        }

        protected virtual void OnUpdate() { }
        protected virtual void OnBack() { }

        bool isPresentSuspended;
        public bool IsPresentSuspended
        {
            get { return this.isPresentSuspended; }
        }
        protected virtual void OnSuspend() { }
        protected virtual void OnResume() { }



        public static Type PrevPresentType
        {
            get { return Present.Singleton.PrevPresentType; }
        }

        public static Type CurrentPresentType
        {
            get { return Present.Singleton.CurrentPresentType; }
        }
        public static Present CurrentPresent
        {
            get { return Present.Singleton.CurrentPresent; }
        }
        public static int CurrentPresentHashCode
        {
            get { return Present.Singleton.CurrentPresentHashCode; }
        }
        public static bool IsLoading
        {
            get { return Present.Singleton.IsLoading; }
        }
        public static bool IsChanging
        {
            get { return Present.Singleton.IsChanging; }
        }
        public static Action OpenListener
        {
            set { Present.Singleton.OpenListener = value; }
            get { return Present.Singleton.OpenListener; }
        }




        public enum LoadRule
        {
            FlushGarbage,
            Direct,
            PreloadedScene,
        }
        public class LoadRuleData
        {
            LoadRule rule;
            public LoadRule Rule
            {
                get { return this.rule; }
            }
            public LoadRuleData(bool isDirect = false)
            {
                this.rule = isDirect ? LoadRule.Direct : LoadRule.FlushGarbage;
            }

            AsyncOperation preloadedScene;
            public AsyncOperation Preloaded
            {
                get { return this.preloadedScene; }
            }
            public LoadRuleData(AsyncOperation preloaded)
            {
                this.rule = LoadRule.PreloadedScene;
                this.preloadedScene = preloaded;
            }

            public override string ToString()
            {
                return this.rule.ToString();
            }

            public readonly static LoadRuleData FlushGarbage = new LoadRuleData();
            public readonly static LoadRuleData Direct = new LoadRuleData(true);
        }

        public static LoadRuleData DefaultLoadRule = LoadRuleData.Direct;

        public static void OpenNextPresent(Type presentType, LoadRuleData rule = null, object openArg = null, Action callback = null)
        {
            Present.Singleton.OpenNextPresent(presentType, rule, openArg, callback);
        }

        public static void UpdatePresent()
        {
            Present.Singleton.UpdatePresent();
        }

        public static void Suspend()
        {
#if LOG_DEBUG && LOG_LIFECYCLE
            Debug.Log(LogHelper.CurrentMethod);
#endif// LOG_DEBUG && LOG_LIFECYCLE
            Present.Singleton.Suspend();
        }

        public static void Resume()
        {
#if LOG_DEBUG && LOG_LIFECYCLE
            Debug.Log(LogHelper.CurrentMethod);
#endif// LOG_DEBUG && LOG_LIFECYCLE
            Present.Singleton.Resume();
        }

        PresentView abstractView;
        public PresentView AbstractView
        {
            get { return this.abstractView; }
        }

        FiniteStateMachine fsm = new FiniteStateMachine();
        public void SetState<T>(T state)
        {
#if LOG_DEBUG
            Debug.Log(string.Format("PRESENT<{0}={1:X8}>:STATE:{2}",
                                     this.GetType().Name,
                                     (uint)this.GetHashCode(),
                                     state));
#endif// LOG_DEBUG
#if LOG_MEMORY
            var presentType = Present.IsOpened ? Present.CurrentPresentType : null;
            var stateOfPresent = state.GetHashCode();
            Debug.Log(string.Format("APP:{0}, {1}, {2}",
                                    DebugImpl.CreateUptimeChunk(),
                                    DebugImpl.CreatePresentChunk("AT", presentType, stateOfPresent),
                                    DebugImpl.CreatePresentChunk("EVENT:SET_STATE_PREV", presentType, this.GetState())));
#endif// LOG_MEMORY
            this.fsm.SetState(state);
        }
        //public void SetState(int state)
        //{
        //    this.fsm.SetState(state);
        //}
        public int GetState()
        {
            return this.fsm.GetState();
        }
        public void ClearState()
        {
            this.fsm.ClearState();
        }
        //public bool CompareState(int state)
        //{
        //    return this.fsm.CompareState(state);
        //}
        public bool CompareState<T>(T state)
        {
            return this.fsm.CompareState(state);
        }
        
        public void AddStateTask(Action task)
        {
            this.fsm.AddStateTask(task);
        }
        public void RemoveStateTask(Action task)
        {
            this.fsm.RemoveStateTask(task);
        }
        public void ResetStateTask(Action task)
        {
            this.fsm.ResetStateTask(task);
        }
        public void ClearStateTask()
        {
            this.fsm.ClearStateTask();
        }
        public bool HasStateTasks()
        {
            return this.fsm.HasStateTasks();
        }

        public void AddStateCloseEvent(Action @event)
        {
            this.fsm.AddStateCloseEvent(@event);
        }
        public void RemoveStateCloseEvent(Action @event)
        {
            this.fsm.RemoveStateCloseEvent(@event);
        }
        public void ResetStateCloseEvent(Action @event)
        {
            this.fsm.ResetStateCloseEvent(@event);
        }
        public void ClearStateCloseEvent()
        {
            this.fsm.ClearStateCloseEvent();
        }
        public bool HasStateCloseEvents()
        {
            return this.fsm.HasStateCloseEvents();
        }

        public void AddTask(Action task)
        {
            this.fsm.AddTask(task);
        }
        public void RemoveTask(Action task)
        {
            this.fsm.RemoveTask(task);
        }
        public void ResetTask(Action task)
        {
            this.fsm.ResetTask(task);
        }
        public void ClearTask()
        {
            this.fsm.ClearTask();
        }
        public bool HasTasks()
        {
            return this.fsm.HasTasks();
        }

        public void Discard()
        {
            this.abstractView.Discard();
        }




        public static void Hide()
        {
            Present.Singleton.Hide();
        }

        public static void Show()
        {
            Present.Singleton.Show();
        }

        public static bool IsShown
        {
            get { return Present.Singleton.IsShown; }
        }

        public CancellableSignal CreateGenericSignal()
        {
            return Present.Singleton.CreateGenericSignal(this);
        }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
            }

            public void CloseSingleton()
            {
                this.ClosePresent(null);
            }

            bool isSuspended;
            internal bool IsSuspended
            {
                get { return this.isSuspended; }
            }
            bool isResumeStart;
            bool isEnableBackButton;
            internal bool IsEnableBackButton
            {
                set { this.isEnableBackButton = value; }
                get { return this.isEnableBackButton; }
            }

            int hideCount;
            internal bool IsHide
            {
                get { return 0 < this.hideCount; }
            }

            Type presentTypePrev = null;
            internal Type PrevPresentType
            {
                get { return this.presentTypePrev; }
            }
            internal string PrevPresentName
            {
                get
                {
                    if (null != this.presentTypePrev)
                        return this.presentTypePrev.Name;

                    return "null";
                }
            }

            Type presentType = null;
            internal Type CurrentPresentType
            {
                get { return this.presentType; }
            }
            internal string CurrentPresentName
            {
                get
                {
                    if (null != this.presentType)
                        return this.presentType.Name;

                    return "null";
                }
            }


            Present preloadPresent;
            Present loadedPresent;
            internal Present CurrentPresent
            {
                get { return this.loadedPresent; }
            }
            internal int CurrentPresentHashCode
            {
                get { return null != this.loadedPresent ? this.loadedPresent.GetHashCode() : 0; }
            }
            

            internal bool IsLoading
            {
                get { return null != this.preloadPresent; }
            }


            object openArg;

            Action openCallback;
            Action openListener;
            internal Action OpenListener
            {
                set { this.openListener = value; }
                get { return this.openListener; }
            }

            AsyncOperation asyncOper;
            AsyncOperation asyncOperFakeOpenForGarbageCollect;
            bool fastOpenWait;
            bool isChanging;
            internal bool IsChanging
            {
                get { return this.isChanging; }
            }


            class WaitData
            {
                Type presentType;
                internal Type PresentType
                {
                    get { return this.presentType; }
                }

                LoadRuleData rule;
                internal LoadRuleData Rule
                {
                    get { return this.rule; }
                }

                object openArg;
                internal object OpenArg
                {
                    get { return this.openArg; }
                }

                Action callback;
                internal Action Callback
                {
                    get { return this.callback; }
                }

                internal WaitData(Type presentType, LoadRuleData rule, object openArg, Action callback)
                {
                    this.presentType = presentType;
                    this.rule = rule;
                    this.openArg = openArg;
                    this.callback = callback;
                }

                public override string ToString()
                {
                    return string.Format("{{\"type\": \"{0}\", \"rule\": \"{1}\", \"arg\": \"{2}\", \"cb\": \"{3}\"}}",
                                          null != this.presentType ? this.presentType.Name : "null",
                                          this.rule,
                                          this.openArg,
                                          this.callback);
                }
            }

            WaitData wait = null;
            void OpenPresentWait(Type presentType, LoadRuleData rule, object openArg, Action callback)
            {
                if (null != this.wait)
                {
                    Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:NEXT_DUPLICATED_EXCEPTION#1:{2}, RULE:{3}, ARG:{4}, CB{5}, WAIT:{6}, CURRENT:{7}",
                                                  this.CurrentPresentName,
                                                  (uint)this.CurrentPresentHashCode,
                                                  null != presentType ? presentType.Name : "null",
                                                  rule,
                                                  openArg,
                                                  callback,
                                                  this.wait,
                                                  this.presentType));
                    return;
                }

                this.wait = new WaitData(presentType, rule, openArg, callback);
            }
            

            Point2 screenSize;
            internal Point2 GetScreenSize()
            {
                return this.screenSize;
            }

            internal void Back()
            {
                if (!this.isEnableBackButton)
                    return;

                if (null != this.onCheckBack)
                {
                    var ret = BackResult.Ok;
                    try
                    {
                        ret = this.onCheckBack();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:CHECK_BACK_EXCEPT:{2}",
                                                      this.CurrentPresentName,
                                                      (uint)this.CurrentPresentHashCode,
                                                      e));
                    }
                    
                    switch (ret)
                    {
                    case BackResult.Blocked:
                        return;
                    }
                }

                if (null != this.loadedPresent)
                {
                    try
                    {
                        this.loadedPresent.OnBack();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:BACK_EXCEPT:{2}",
                                                      this.CurrentPresentName,
                                                      (uint)this.CurrentPresentHashCode,
                                                      e));
                    }
                }
            }

            Func<BackResult> onCheckBack;
            internal Func<BackResult> OnCheckBack
            {
                set { this.onCheckBack = value; }
                get { return this.onCheckBack; }
            }

            internal void Suspend()
            {
                if (this.isSuspended)
                    return;

                this.isSuspended = true;

                if (null != this.loadedPresent)
                {
                    try
                    {
                        this.loadedPresent.isPresentSuspended = true;
                        this.loadedPresent.OnSuspend();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("PRESENT <{0}={1:X8}>:SUSPEND_EXCEPT:{2}",
                                                      this.CurrentPresentName,
                                                      (uint)this.CurrentPresentHashCode,
                                                      e));
                    }
                }
            }

            internal void Resume()
            {
                if (!this.isSuspended)
                    return;

                this.isResumeStart = true;
            }





            internal void OpenNextPresent(Type presentType, LoadRuleData rule, object openArg, Action callback)
            {
                if (null == rule)
                    rule = Present.DefaultLoadRule;

#if LOG_DEBUG
                Debug.Log(string.Format("PRESENT<{0}={1:X8}>:NEXT:{2}, RULE:{3}, ARG:{4}, CB:{5}, STACK_TRACE:\n{6}",
                                         this.CurrentPresentName,
                                         (uint)this.CurrentPresentHashCode,
                                         null != presentType ? presentType.Name : "null",
                                         rule,
                                         openArg,
                                         callback,
                                         new System.Diagnostics.StackTrace(true).ToString()));
#endif// LOG_DEBUG
#if LOG_MEMORY
                var stateOfPresent = null != this.loadedPresent ? this.loadedPresent.GetState() : -1;
                Debug.Log(string.Format("APP:{0}, {1}, {2}",
                                        DebugImpl.CreateUptimeChunk(),
                                        DebugImpl.CreatePresentChunk("AT", presentType, -1),
                                        DebugImpl.CreatePresentChunk("EVENT:SET_PRESENT_PREV", this.presentType, stateOfPresent)));
#endif// LOG_MEMORY

                this.ClosePresent(presentType);

                if (null != this.asyncOperFakeOpenForGarbageCollect || null != this.asyncOper || this.fastOpenWait)
                {
#if LOG_DEBUG
                    Debug.Log(string.Format("PRESENT<{0}={1:X8}>:NEXT_DUPLICATED:{2}",
                                             this.CurrentPresentName,
                                             (uint)this.CurrentPresentHashCode,
                                             null != presentType ? presentType.Name : "null"));
#endif// LOG_DEBUG
                    this.OpenPresentWait(presentType, rule, openArg, callback);
                    return;
                }
                else if (null != this.wait)
                {
                    Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:NEXT_DUPLICATED_EXCEPTION#2:{2}, RULE:{3}, ARG:{4}, CB{5}, WAIT:{6}, CURRENT:{7}",
                                                  this.CurrentPresentName,
                                                  (uint)this.CurrentPresentHashCode,
                                                  null != presentType ? presentType.Name : "null",
                                                  rule,
                                                  openArg,
                                                  callback,
                                                  this.wait,
                                                  this.presentType));
                    return;
                }

                this.presentTypePrev = this.presentType;
                this.presentType = presentType;

                this.openArg = openArg;

                this.openCallback = callback;
                this.isChanging = true;
                
                Preference.Save();
                this.preloadPresent = (Present)Activator.CreateInstance(this.presentType);
#if LOG_DEBUG
                Debug.Log(string.Format("PRESENT<{0}={1:X8}>:CREATE_INSTANCE:<{2}={3:X8}>",
                                         this.CurrentPresentName,
                                         (uint)this.CurrentPresentHashCode,
                                         null != this.preloadPresent ? this.preloadPresent.GetType().Name : "null",
                                         null != this.preloadPresent ? (uint)this.preloadPresent.GetHashCode() : 0u));
#endif// LOG_DEBUG
                try
                {
                    this.preloadPresent.OnPreloading();
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:LOAD_EXCEPT:{2}",
                                                  this.CurrentPresentName,
                                                  (uint)this.CurrentPresentHashCode,
                                                  e));
                }
                
                switch (rule.Rule)
                {
                case LoadRule.FlushGarbage:
                    this.asyncOperFakeOpenForGarbageCollect = SceneManager.LoadSceneAsync("Next");
                    break;

                case LoadRule.Direct:
                    if (this.preloadPresent.DoOpen)
                        this.asyncOper = SceneManager.LoadSceneAsync(this.presentType.Name);
                    else
                        this.fastOpenWait = true;
                    break;

                case LoadRule.PreloadedScene:
                    this.asyncOper = rule.Preloaded;
                    break;
                }
            }
            

            internal void UpdatePresent()
            {
                if (this.isResumeStart)
                {
                    this.isSuspended = false;
                    this.isResumeStart = false;

                    if (null != this.loadedPresent)
                    {
                        try
                        {
                            if (this.loadedPresent.isPresentSuspended)
                            {
                                this.loadedPresent.isPresentSuspended = false;
                                this.loadedPresent.OnResume();
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:ONRESUME_EXCEPT:{2}",
                                                          this.CurrentPresentName,
                                                          (uint)this.CurrentPresentHashCode,
                                                          e));
                        }
                    }
                }
                else if (this.isSuspended)
                    return;



                var loaded = false;
                if (null != this.preloadPresent)
                {
                    if (this.fastOpenWait)
                    {
                        if (this.preloadPresent.DoOpen)
                        {
                            this.fastOpenWait = false;
                            this.asyncOper = SceneManager.LoadSceneAsync(this.presentType.Name);
                        }
                        else
                            return;
                    }
                    else if (null != this.asyncOperFakeOpenForGarbageCollect)
                    {
                        if (this.asyncOperFakeOpenForGarbageCollect.isDone && this.preloadPresent.DoOpen)
                        {
                            this.asyncOperFakeOpenForGarbageCollect = null;
                            this.asyncOper = SceneManager.LoadSceneAsync(this.presentType.Name);
                        }
                        else
                            return;
                    }

                    if (null != this.asyncOper)
                    {
                        if (!this.asyncOper.allowSceneActivation)
                            this.asyncOper.allowSceneActivation = true;

                        if (!this.asyncOper.isDone)
                        {
                            this.preloadPresent.loadingProgress = this.asyncOper.progress;
                            return;
                        }

                        this.asyncOper = null;
                        this.OpenPresent();

                        loaded = true;
                    }
                }

                if (loaded)
                {
                    var openCallback = this.openCallback;
                    this.openCallback = null;
                    if (null != openCallback)
                        openCallback();

                    if (null != this.openListener)
                        this.openListener();

                    this.isChanging = false;
                }

                if (null != this.loadedPresent)
                {
                    try
                    {
                        this.loadedPresent.OnUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:ONUPDATE_EXCEPT:{2}",
                                                      this.CurrentPresentName,
                                                      (uint)this.CurrentPresentHashCode,
                                                      e));
                    }

                    var fsm = this.loadedPresent.fsm;
                    if (fsm.HasStateTasks())
                    {
                        try
                        {
                            fsm.RunStateTasks();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:STATETASK_EXCEPT:{2}",
                                                          this.CurrentPresentName,
                                                          (uint)this.CurrentPresentHashCode,
                                                          e));
                        }
                    }

                    if (fsm.HasTasks())
                    {
                        try
                        {
                            fsm.RunTasks();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:TASK_EXCEPT:{2}",
                                                          this.CurrentPresentName,
                                                          (uint)this.CurrentPresentHashCode,
                                                          e));
                        }
                    }
                }
                
                if (loaded)
                {
                    if (null != this.wait)
                    {
                        var w = this.wait;
                        this.wait = null;
                        this.OpenNextPresent(w.PresentType, w.Rule, w.OpenArg, w.Callback);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    this.Back();
                }
            }

            

            internal void OpenPresent()
            {
                if (this.unloadWaitObj && null != this.unloadWait)
                {
                    //this.unloadAsync = SceneManager.UnloadSceneAsync(this.unloadWait.Name);
                    SceneManager.UnloadSceneAsync(this.unloadWait.Name);
                    this.unloadWait = null;
                    this.unloadWaitObj = null;
                }

                var presentName = this.presentType.Name;
                var scene = SceneManager.GetSceneByName(presentName);

                GameObject root = null;
                var roots = scene.GetRootGameObjects();
                for (int n = 0, cnt = roots.Length; n < cnt; ++n)
                {
                    var go = roots[n];
                    if (go && presentName == go.name)
                    {
                        root = go;
                        break;
                    }
                }

                if (root)
                {
                    var absView = root.GetComponent<PresentView>();
                    if (absView)
                    {
                        this.loadedPresent = this.preloadPresent;
                        this.preloadPresent = null;

                        this.loadedPresent.abstractView = absView;
                        //this.loadedPresent.screenInfo = new ResolutionChecker.ScreenInfo(ResolutionChecker.Current);

                        if (0 < this.hideCount)
                            absView.SetShow(false);

                        this.loadedPresent.loadingProgress = 1;
                        try
                        {
                            var openArg = this.openArg;
                            this.openArg = null;

                            this.loadedPresent.isPresentOpened = true;
                            this.loadedPresent.OnOpen(openArg);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:OPEN_EXCEPT:{2}",
                                                          this.CurrentPresentName,
                                                          (uint)this.CurrentPresentHashCode,
                                                          e));
                        }
                    }
                    else
                    {
                        // NOTE: BUG #2
                        Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:ERROR:VIEW_COMPONENT_NOT_FOUND",
                                                      this.CurrentPresentName,
                                                      (uint)this.CurrentPresentHashCode));
                    }
                }
                else
                {
                    // NOTE: BUG
                    Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:ERROR:VIEW_NOT_FOUND",
                                                  this.CurrentPresentName,
                                                  (uint)this.CurrentPresentHashCode));
                }
            }

            Type unloadWait;
            GameObject unloadWaitObj;
            //AsyncOperation unloadAsync;

            int presentGeneration = 0;
            internal void ClosePresent(Type nextPresent)
            {
                ++this.presentGeneration;

                if (null == this.loadedPresent && null == this.preloadPresent)
                    return;

                try
                {
                    var present = this.loadedPresent;
                    if (null == present)
                        present = this.preloadPresent;

                    present.fsm.Dispose();

                    present.OnClose(nextPresent);

                    //if (present.abstractView && present.abstractView.gameObject)
                    //    UnityEngine.Object.Destroy(present.abstractView.gameObject);
                    if (present.abstractView && present.abstractView.gameObject)
                    {
                        this.unloadWaitObj = present.abstractView.gameObject;
                        this.unloadWaitObj.SetActive(false);
                    }

                    present.abstractView = null;

                    this.unloadWait = present.GetType();
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("PRESENT<{0}={1:X8}>:CLOSE_EXCEPT:NEXT:{2}, EXCEPT:{3}",
                                                  this.CurrentPresentName,
                                                  (uint)this.CurrentPresentHashCode,
                                                  null != presentType ? presentType.Name : "null",
                                                  e));
                }
                finally
                {
                    this.loadedPresent = null;
                    this.preloadPresent = null;
                }
            }



            internal void Hide()
            {
                ++this.hideCount;

                if (1 == this.hideCount)
                {
                    if (null != this.loadedPresent && this.loadedPresent.abstractView)
                        this.loadedPresent.abstractView.SetShow(false);
                }
            }

            internal void Show()
            {
                --this.hideCount;
                if (0 > this.hideCount)
                {
#if LOG_DEBUG || UNITY_EDITOR
                    Debug.LogWarning(string.Format("PRESENT<{0}={1:X8}>:BAD_HIDE_COUNT:{2}",
                                                    this.CurrentPresentName,
                                                    (uint)this.CurrentPresentHashCode,
                                                    this.hideCount));
#endif// LOG_DEBUG || UNITY_EDITOR
                    this.hideCount = 0;
                }

                if (0 == this.hideCount)
                {
                    if (null != this.loadedPresent && this.loadedPresent.abstractView)
                        this.loadedPresent.abstractView.SetShow(true);
                }
            }

            internal bool IsShown
            {
                get { return 0 == this.hideCount; }
            }

            internal CancellableSignal CreateGenericSignal(Present current)
            {
                var presentGen = this.presentGeneration;
                var gen = Present.Generation;

                return new CancellableSignal(() =>
                {
                    return presentGen != this.presentGeneration || gen != Present.Generation;
                });
            }
        }
    }
}
