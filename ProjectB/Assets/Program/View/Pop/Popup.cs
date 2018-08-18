using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.Event;

using Newtonsoft.Json;
using Program.Core;
using Ext.Async;

namespace Program.View.Pop
{
    public class Popup : BaseView
    {
        public class Params
        {
            public object[] btnOkLabelDatas;
            public object[] btnCancelLabelDatas;
            
            public bool blind;

            public bool useBtnX = true;
            
            public bool backBtnIsOk;
            public bool backBtnLock;

            public float contentsHeight = 0;

            public bool foremost;

            public bool hideScene;

            public bool solo;

            public bool managed = true;

            public object extraData = null;

            public bool recycle = true;

            public Func<Popup, GameObject, bool> onClickOverrideExternal;
            public Action<Popup> onBackOverrideExternal;

            public bool muteBtnOk;
            public bool muteBtnCancel;
            public bool muteBtnX;

            public bool directBuild;

            public bool keepDestroy;
        }
        protected Params keepParam = null;
        public Params GetParam()
        {
            return this.keepParam;
        }
        public T GetParam<T>() where T : Params
        {
            return this.keepParam as T;
        }


        public class Result
        {
            public Popup pop;
            public bool isOk;

            public override string ToString()
            {
                return ConvertString.Execute(this);
            }
        }


        public delegate void OnResultCallback(Result ret);
        OnResultCallback resultCallback;
        public OnResultCallback ResultCallback { get { return this.resultCallback; } }

        public delegate void OnLoadedCallback(Popup pop);
        OnLoadedCallback loadedCallback;
        public OnLoadedCallback LoadedCallback { get { return this.loadedCallback; } }

        static event Action<Popup> onOpenedListener;
        public static event Action<Popup> OnOpenedListener
        {
            add { Popup.onOpenedListener += value; }
            remove { Popup.onOpenedListener -= value; }
        }

        static event Action<Popup> onClosedListener;
        public static event Action<Popup> OnClosedListener
        {
            add { Popup.onClosedListener += value; }
            remove { Popup.onClosedListener -= value; }
        }




        [SerializeField]
        bool forceHideScene;
        public bool ForceHideScene { get { return this.forceHideScene; } }
        
        [SerializeField]
        bool forceBlind;
        public bool ForceBlind { get { return this.forceBlind; } }
        
        [SerializeField]
        bool forceSolo;
        public bool ForceSolo { get { return this.forceSolo; } }

        [SerializeField]
        bool forceManaged;
        public bool ForceManaged
        {
            get { return this.forceManaged; }
        }

        bool managedStatus;
        public bool Managed
        {
            set
            {
                if (this.managedStatus == value)
                    return;

                this.managedStatus = value;
                if (value)
                    Popup.Regist(this);
                else
                    Popup.Unregist(this);
            }
            get { return this.managedStatus; }
        }

        [SerializeField]
        protected UICache touchLocked;

        [SerializeField]
        protected ButtonCache btnOk;
        [SerializeField]
        protected ButtonCache btnCancel;
        [SerializeField]
        protected ButtonCache btnX;
        
        [SerializeField]
        protected UICache blindBase;
        
        int order = 0;
        static int lastOrder = 0;


        [SerializeField]
        UICache contentBase;
        public UICache ContentBase
        {
            get { return this.contentBase; }
        }
        public RectTransform ContentBaseTransform
        {
            get { return this.ContentBase.CachedRectTransform; }
        }

        
        float contentsDefaultHeight = 0;
        public float ContentsDefaultHeight
        {
            get
            {
                return this.contentsDefaultHeight;
            }
        }

        public float ContentsHeight
        {
            set
            {
                this.ContentBaseTransform.SetHeight(value);
            }
            get
            {
                return this.ContentBaseTransform.GetHeight();
            }
        }

        public object[] BtnOkLabelDatas
        {
            set
            {
                if (this.btnOk && this.btnOk.Label)
                    this.btnOk.Label.SetDatas(value);

                this.BtnOkVisible = this.btnOk.Label.IsValidAny();
            }
            get
            {
                if (this.btnOk && this.btnOk.Label)
                    return this.btnOk.Label.GetDatas();

                return null;
            }
        }
        public bool BtnOkVisible
        {
            set
            {
                if (this.btnOk)
                    this.btnOk.SetActive(value);
            }
            get
            {
                if (this.btnOk)
                    return this.btnOk.IsActivated;

                return false;
            }
        }

        public bool BtnOkEnable
        {
            set
            {
                if (this.btnOk && this.btnOk.Button)
                    this.btnOk.Button.interactable = value;
            }
            get
            {
                if (this.btnOk && this.btnOk.Button)
                    return this.btnOk.Button.interactable;

                return false;
            }
        }

        public virtual bool IsBtnOk(GameObject go)
        {
            return go && this.btnOk && this.btnOk.GameObject == go;
        }


        public object[] BtnCancelLabelDatas
        {
            set
            {
                if (this.btnCancel && this.btnCancel.Label)
                    this.btnCancel.Label.SetDatas(value);

                this.BtnCancelVisible = this.btnCancel.Label.IsValidAny();
            }
            get
            {
                if (this.btnCancel && this.btnCancel.Label)
                    return this.btnCancel.Label.GetDatas();

                return null;
            }
        }

        public bool BtnCancelVisible
        {
            set
            {
                if (this.btnCancel)
                    this.btnCancel.SetActive(value);
            }
            get
            {
                if (this.btnCancel)
                    return this.btnCancel.IsActivated;

                return false;
            }
        }

        public bool BtnCancelEnable
        {
            set
            {
                if (this.btnCancel && this.btnCancel.Button)
                    this.btnCancel.Button.interactable = value;
            }
            get
            {
                if (this.btnCancel && this.btnCancel.Button)
                    return this.btnCancel.Button.interactable;

                return false;
            }
        }


        public bool BtnXVisible
        {
            set
            {
                if (this.btnX)
                    this.btnX.SetActive(value);
            }
            get
            {
                if (this.btnX)
                    return this.btnX.IsActivated;

                return false;
            }
        }


        public bool Blind
        {
            set
            {
                if (this.blindBase)
                    this.blindBase.SetActive(value);
            }
            get
            {
                if (this.blindBase)
                    return this.blindBase.IsActivated;

                return false;
            }
        }


        static bool testMode;
        public static bool TestMode
        {
            set { Popup.testMode = value; }
            get { return Popup.testMode; }
        }

        public static Popup Open(string name,
                                 Params p,
                                 OnResultCallback resultCallback = null,
                                 OnLoadedCallback loadedCallback = null,
                                 Popup externalPrefab = null)
        {
            Popup pop = Popup.Load(name, p.foremost, externalPrefab);
            if (null != pop)
            {
                pop.Open(p, resultCallback, loadedCallback);
                return pop;
            }
            else
            {
                if (Popup.testMode)
                {
                    if (null != loadedCallback)
                        loadedCallback(null);
                }

                if (null != resultCallback)
                    resultCallback(new Popup.Result { pop = pop, isOk = Popup.testMode ? true : false, });
            }
            return null;
        }




        static Popup OnLoaded(Popup prefab, string path, string popName, bool foremost, bool isExternalPrefab)
        {
            if (null == prefab)
            {
#if LOG_DEBUG
                Debug.Log("POPUP_LOAD_FAILED:" + popName + ", PATH:" + path + ", RESOURCES_LOAD_FAILED");
#endif// LOG_DEBUG
                return null;
            }

            GameObject instance = (GameObject)GameObject.Instantiate(prefab.gameObject);
            if (null == instance)
            {
#if LOG_DEBUG
                Debug.Log("POPUP_LOAD_FAILED:" + popName + ", PATH:" + path + ", GAMEOBJECT_INSTANTIATE_FAILED");
#endif// LOG_DEBUG
                if (!isExternalPrefab)
                    prefab.Destroy();
                return null;
            }

            var pop = instance.GetComponent<Popup>();
            if (null == pop)
            {
#if LOG_DEBUG
                Debug.Log("POPUP_LOAD_FAILED:" + popName + ", PATH:" + path + ", GETCOMPONENT_FAILED");
#endif// LOG_DEBUG
                instance.Destroy();
                if (!isExternalPrefab)
                    prefab.Destroy();
                return null;
            }
            
            pop.name = popName;

            var trans = pop.CachedRectTransform;
            var prefabTrans = prefab.CachedRectTransform;
            var parent = foremost ? PopupRoot.Instance.ForemostCanvasTrans : PopupRoot.Instance.CachedTransform;
            trans.SetParent(parent);
            trans.localPosition = Vector3.zero;
            trans.anchoredPosition = prefabTrans.anchoredPosition;
            trans.sizeDelta = prefabTrans.sizeDelta;
            trans.anchorMax = prefabTrans.anchorMax;
            trans.anchorMin = prefabTrans.anchorMin;
            trans.offsetMax = prefabTrans.offsetMax;
            trans.offsetMin = prefabTrans.offsetMin;
            trans.pivot = prefabTrans.pivot;
            trans.localScale = prefabTrans.localScale;

            pop.gameObject.SetActive(false);

            return pop;
        }


        
        public static Popup Load(string popName,
                                 bool foremost = false,
                                 Popup externalPrefab = null)
        {
            if (Popup.testMode)
                return null;

#if LOG_DEBUG
            Debug.Log("POPUP_LOAD:" + popName);
#endif// LOG_DEBUG

            if (!PopupRoot.Instance)
            {
#if LOG_DEBUG
                Debug.Log("POPUP_LOAD_FAILED:" + popName + ", ROOT_IS_NULL");
#endif// LOG_DEBUG
                return null;
            }
            
            var popRecycle = Popup.CancelDestroyWithRecycle(popName);
            if (popRecycle)
                return popRecycle;

            Popup.sbPath.Length = Popup.basePath.Length;
            Popup.sbPath.Append(popName);
            string path = Popup.sbPath.ToString();

            return Popup.OnLoaded(externalPrefab ? externalPrefab : Resources.Load<Popup>(path),
                                  path,
                                  popName,
                                  foremost,
                                  externalPrefab);
        }









        public static ResourceRequest LoadAsync(CancellableSignal signal,
                                                string popName,
                                                bool foremost = false,
                                                Action<Popup> loadDoneCallback = null)
        {
            if (Popup.testMode)
            {
                if (null != loadDoneCallback)
                    loadDoneCallback(null);

                return null;
            }

#if LOG_DEBUG
            Debug.Log("POPUP_LOAD_ASYNC:" + popName);
#endif// LOG_DEBUG

            if (!PopupRoot.Instance)
            {
#if LOG_DEBUG
                Debug.Log("POPUP_LOAD_ASYNC_FAILED:" + popName + ", ROOT_IS_NULL");
#endif// LOG_DEBUG
                return null;
            }

            var popRecycle = Popup.CancelDestroyWithRecycle(popName);
            if (popRecycle)
            {
                if (null != loadDoneCallback)
                    loadDoneCallback(popRecycle);

                return null;
            }

            Popup.sbPath.Length = Popup.basePath.Length;
            Popup.sbPath.Append(popName);
            string path = Popup.sbPath.ToString();

            var req = Resources.LoadAsync<Popup>(path);
            if (null == req)
            {
#if LOG_DEBUG
                Debug.Log("POPUP_LOAD_ASYNC_FAILED:" + popName + ", PATH:" + path + ", RESOURCES_LOAD_FAILED");
#endif// LOG_DEBUG
                return null;
            }

            CoroutineTaskManager.AddTask(Popup.OnLoadAsync(signal, req, path, popName, foremost, loadDoneCallback));
            return req;
        }

        static IEnumerator OnLoadAsync(CancellableSignal signal,
                                       ResourceRequest req,
                                       string path,
                                       string popName,
                                       bool foremost,
                                       Action<Popup> loadDoneCallback)
        {
            yield return req;

            if (CancellableSignal.IsCancelled(signal))
                yield break;

            var prefab = req.asset as Popup;
            
            var pop = Popup.OnLoaded(prefab,
                                     path,
                                     popName,
                                     foremost,
                                     false);

            if (null != loadDoneCallback)
                loadDoneCallback(pop);
        }




        class RecycleInstance
        {
            public string name;
            public Popup pop;
            public Wait.Handle waitHandle;
        }
        static LinkedList<RecycleInstance> recycles = new LinkedList<RecycleInstance>();

        const string basePath = "Prefab/Popup/";
        static StringBuilder sbPath = new StringBuilder(Popup.basePath, 1024);
        

        
        bool isOpened = false;
        public bool IsOpened { get { return this.isOpened; } }

        public void Open(Params p,
                         OnResultCallback resultCallback = null,
                         OnLoadedCallback loadedCallback = null)
        {
#if LOG_DEBUG
            Debug.Log("POPUP_OPEN:" + this.name + ", IS OPENED:" + this.IsOpened + ", POPUP_OPEN_COUNT:" + Popup.OpenCount + ", FOCUS:" + Popup.Focused + ", LAST_ORDER:" + Popup.lastOrder);
#endif// LOG_DEBUG

            if (this.IsOpened)
                this.Close();

            this.isOpened = true;
            this.resultCallback = resultCallback;
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
            this.SetShow(true);
            this.keepParam = p;
            this.order = Popup.lastOrder++;

            this.Managed = p.managed || this.forceManaged;

            if (this.isStarted)
            {
                if (p.directBuild || this.isRecycled)
                {
                    if (!this.Build())
                    {
                        Debug.LogError(string.Format("POPUP:OPEN_FAILED:{0}", this.name));
                        return;
                    }
                }

                this.loadedCallback = null;
                if (null != loadedCallback)
                    loadedCallback(this);
            }
            else
                this.loadedCallback = loadedCallback;


            if (null != Popup.onOpenedListener)
                Popup.onOpenedListener(this);
        }


        public void Close()
        {
            if (!this.IsOpened)
                return;

            --Popup.lastOrder;

#if LOG_DEBUG
            Debug.Log("POPUP_CLOSE:" + this.name + ", OPEN_COUNT:" + Popup.OpenCount + ", FOCUS:" + Popup.Focused + ", LAST_ORDER:" + Popup.lastOrder);
#endif// LOG_DEBUG

            if (0 > Popup.lastOrder)
            {
#if LOG_DEBUG
                Debug.LogWarning("POPUP_INVALID_LAST_ORDER:" + Popup.lastOrder);
#endif// LOG_DEBUG
                Popup.lastOrder = 0;
            }

            this.isOpened = false;
            this.isBuildDone = false;

            var p = this.keepParam;
            if (null == p || !p.keepDestroy)
                this.gameObject.SetActive(false);

            this.loadedCallback = null;

            // NOTE: memory managing
            var resultCallback = this.resultCallback;
            this.resultCallback = null;

            var ret = this.CreateResult();
            this.SetupResult(ret);

            var hideScene = this.keepParam.hideScene || this.forceHideScene;
            var solo = this.keepParam.solo || this.forceSolo;

            this.OnClose();
            Popup.Unregist(this);
            if (null != Popup.onClosedListener)
                Popup.onClosedListener(this);

            var node = Popup.opened.First;
            while (null != node)
            {
                var pop = node.Value;
                node = node.Next;
                
                if (solo)
                {
                    if (pop.order < this.order)
                        pop.ShowWithCount(true);
                }

                if (pop.order >= this.order)
                    --pop.order;
            }

            if (hideScene)
                Present.Show();

            if (null != resultCallback)
                resultCallback(ret);
        }


        bool isReleased;
        public void Release()
        {
            if (this.isReleased)
                return;

            this.Close();

            this.OnRelease();
            this.isReleased = true;

            if (null != this.keepParam && this.keepParam.recycle)
                this.DoDestroyWithRecycle();
            else
                this.DoDestroy();
        }

        bool isDestroy = false;
        void DoDestroy()
        {
            if (this.isDestroy)
                return;

            var p = this.keepParam;
            this.keepParam = null;

            this.isDestroy = true;
            if (null == p || !p.keepDestroy)
                this.FinalDestroy();
        }
        public void FinalDestroy()
        {
            this.gameObject.Destroy();
        }

        static string GetRecycleName(string name)
        {
            return name;
        }


        bool isRecycled;
        void DoDestroyWithRecycle()
        {
            var recycleName = Popup.GetRecycleName(this.name);
            var recycleInstOrigin = new RecycleInstance { name = recycleName, pop = this, };
            this.isRecycled = true;
            Popup.recycles.AddLast(recycleInstOrigin);
            recycleInstOrigin.waitHandle = Wait.Active(0.1f, false, (p) =>
            {
                var recycleNode = Popup.FindRecycleNode(recycleName);
                if (null != recycleNode)
                {
                    var recycleInst = recycleNode.Value;
                    Popup.RemoveRecycleNode(recycleNode);

                    var pop = recycleInst.pop;
                    if (pop)
                        pop.DoDestroy();
                }
            });
        }

        static void RemoveRecycleNode(LinkedListNode<RecycleInstance> recycleNode)
        {
            var recycleInst = recycleNode.Value;
            if (null != recycleInst.waitHandle)
            {
                Wait.Stop(recycleInst.waitHandle, false);
                recycleInst.waitHandle = null;
            }

            Popup.recycles.Remove(recycleNode);
        }

        static LinkedListNode<RecycleInstance> FindRecycleNode(string recycleName)
        {
            var recycleNode = Popup.recycles.First;
            while (null != recycleNode)
            {
                var recycleInst = recycleNode.Value;
                if (recycleInst.name == recycleName)
                    return recycleNode;

                recycleNode = recycleNode.Next;
            }

            return null;
        }

        static Popup CancelDestroyWithRecycle(string popName)
        {
            if (0 < Popup.recycles.Count)
            {
                var recycleName = Popup.GetRecycleName(popName);

                LinkedListNode<RecycleInstance> recycleNode = null;
                do
                {
                    recycleNode = Popup.FindRecycleNode(recycleName);
                    if (null != recycleNode)
                    {
                        var recycleInst = recycleNode.Value;
                        Popup.RemoveRecycleNode(recycleNode);

                        if (recycleInst.pop)
                        {
                            var pop = recycleInst.pop;
                            pop.isReleased = false;
                            pop.isBuildDone = false;
                            pop.isOk = false;
                            return pop;
                        }
                    }
                }
                while (null != recycleNode);
            }

            return null;
        }


        static void DoDestroyAllRecycles()
        {
            LinkedListNode<RecycleInstance> recycleNode = null;
            while (null != (recycleNode = Popup.recycles.First))
            {
                var recycleInst = recycleNode.Value;
                Popup.RemoveRecycleNode(recycleNode);

                var pop = recycleInst.pop;
                if (pop)
                    pop.DoDestroy();
            }
        }


        public void Ok()
        {
            if (!this.IsOpened)
                return;

            this.isOk = true;

            this.Release();
        }

        public void Cancel()
        {
            if (!this.IsOpened)
                return;

            this.isOk = false;

            this.Release();
        }

        public void Abort()
        {
            if (!this.IsOpened)
                return;

            this.resultCallback = null;
            this.Release();
        }

        public void OnClick(GameObject go)
        {
            if (!this.IsOpened)
                return;

            this.isOk = this.IsBtnOk(go);

            if (this.OnClickOverride(go))
            {
                var onClickOverrideExternal = this.keepParam.onClickOverrideExternal;
                if (null == onClickOverrideExternal || onClickOverrideExternal(this, go))
                    this.Release();
            }
        }

        public virtual void OnBack()
        {
            if (this.keepParam.backBtnLock)
                return;
            
            if (null != this.keepParam.onBackOverrideExternal)
            {
                this.keepParam.onBackOverrideExternal(this);
                return;
            }

             if (this.keepParam.backBtnIsOk)
                this.Ok();
            else
                this.Release();
        }




        protected override void Awake()
        {
            base.Awake();

            this.contentsDefaultHeight = this.ContentsHeight;
        }

        bool isStarted = false;
        bool isBuildDone = false;
        public bool IsBuildDone { get { return this.isBuildDone; } }

        void Start()
        {
            this.isStarted = true;
            this.OnStart();

            if (!this.isBuildDone)
                if (!this.Build())
                    return;
            
            var loadedCallback = this.loadedCallback;
            this.loadedCallback = null;

            if (null != loadedCallback)
                loadedCallback(this);
        }

        
        bool Build()
        {
            var p = this.keepParam;
            if (null == p)
                return false;

            this.isReleased = false;
            
            if (p.hideScene || this.forceHideScene)
                Present.Hide();

            if (p.solo || this.forceSolo)
            {
                var node = Popup.opened.First;
                while (null != node)
                {
                    var pop = node.Value;
                    node = node.Next;

                    if (pop == this)
                        continue;

                    if (pop.order < this.order)
                        pop.ShowWithCount(false);
                }
            }

            this.OnBuild();

            if (0 < p.contentsHeight)
                this.ContentsHeight = p.contentsHeight;

            this.Refresh();

            var root = PopupRoot.Instance;
            var canvas = p.foremost ? root.Canvas : root.ForemostCanvas;
            canvas.enabled = false;
            canvas.enabled = true;


            this.ResetLayout();

            this.isBuildDone = true;
            return true;
        }


        public void Refresh()
        {
            if (!this.isStarted)
                return;

            this.OnRefresh();

            this.ResetLayout();
        }


        void ShowWithCount(bool value)
        {
            if (value)
            {
                --this.hideCount;
                if (0 > this.hideCount)
                {
#if LOG_DEBUG || UNITY_EDITOR
                    Debug.LogWarning("POP:BAD_HIDE_COUNT:" + this.hideCount);
#endif// LOG_DEBUG || UNITY_EDITOR
                    this.hideCount = 0;
                }

                if (0 == this.hideCount)
                    this.SetShow(true);
            }
            else
            {
                ++this.hideCount;

                if (1 == this.hideCount)
                    this.SetShow(false);
            }
        }
        int hideCount;



        protected bool isOk = false;

        #region overrides
        protected virtual Result CreateResult()
        {
            return new Result();
        }
        protected virtual void SetupResult(Result ret)
        {
            ret.pop = this;
            ret.isOk = this.isOk;
        }

        protected virtual void OnStart() { }
        protected virtual void OnBuild()
        {
            var p = this.keepParam;
            
            this.BtnOkLabelDatas = p.btnOkLabelDatas;
            this.BtnCancelLabelDatas = p.btnCancelLabelDatas;
            this.BtnXVisible = p.useBtnX;

            if (this.blindBase)
                this.blindBase.SetActive(p.blind || this.forceBlind);
        }
        protected virtual void OnRefresh() { }
        protected virtual void OnClose() { }
        protected virtual void OnRelease() { }
        protected virtual bool OnClickOverride(GameObject go) { return true; }

        #endregion overrides

        #region editor
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            this.blindBase = new UICache(this.FindObject("Blind"));
            this.touchLocked = new UICache(this.FindObject("TouchLock"));
            
            this.contentBase = new UICache(this.FindObjectInChildrenWithOptional("ContentBase;"));
            var contentObj = this.ContentBase.GameObject;
            
            this.EditorSetOnClickListener(this.btnOk = new ButtonCache(contentObj.FindObject("BtnOk")));
            this.EditorSetOnClickListener(this.btnCancel = new ButtonCache(contentObj.FindObject("BtnCancel")));
            this.EditorSetOnClickListener(this.btnX = new ButtonCache(contentObj.FindObject("BtnX")));
        }
        
        protected void EditorSetOnClickListener(ButtonCache cache)
        {
            if (null == cache || !cache.Button)
                return;

            var button = cache.Button;
            if (button is Button)
                UnityExtension.SetOnClickObjectPersistantListener((Button)button, this, "OnClick", this.OnClick, cache.GameObject);
        }

#endif // UNITY_EDITOR
        #endregion editor



        #region popup system

        public static void Setup()
        {
            Popup.Purge();
        }

        static LinkedList<Popup> opened = new LinkedList<Popup>();
        public static LinkedList<Popup> Opened { get { return Popup.opened; } }
        static void Regist(Popup pop)
        {
            if (!Popup.opened.Contains(pop))
            {
                Popup.opened.AddLast(pop);
            }
        }

        static void Unregist(Popup pop)
        {
            Popup.opened.Remove(pop);
        }
        
        public static int OpenCount
        {
            get
            {
                return Popup.opened.Count;
            }
        }

#if LOG_SUSPEND
        public static bool Active_ = true;
#else// LOG_SUSPEND
        static bool Active_ = true;
#endif// LOG_SUSPEND
        public static bool Activate
        {
            set
            {
                if (Popup.Active_ != value)
                {
                    Popup.Active_ = value;

                    var root = PopupRoot.Instance;

                    if (value)
                        root.SetShow(true);
                    else
                        root.SetShow(false);

                    Popup.UpdateScreenLock(value);


                    LinkedListNode<Popup> node = Popup.opened.First;
                    LinkedListNode<Popup> nodeTemp;
                    while (null != node)
                    {
                        if (node.Value)
                            node = node.Next;
                        else
                        {
                            nodeTemp = node;
                            node = node.Next;
                            Popup.opened.Remove(nodeTemp);
                        }
                    }
                }
            }
            get
            {
                return Popup.Active_;
            }
        }

        public static Popup Focused
        {
            get
            {
                LinkedListNode<Popup> node = Popup.opened.Last;
                if (null == node)
                    return null;

                return node.Value;
            }
        }

        public static bool IsShownPop
        {
            get
            {
                return Popup.Activate && 0 < Popup.OpenCount;
            }
        }


        public static void ReleaseAll(bool isAbort)
        {
            LinkedListNode<Popup> node = Popup.opened.First;
            while (null != node)
            {
                Popup pop = node.Value;

                node = node.Next;

                if (pop)
                {
                    if (isAbort)
                        pop.Abort();
                    else
                        pop.Release();
                }
            }

            Popup.opened.Clear();
        }

        public static void Purge()
        {
            Popup.ReleaseAll(true);
            Popup.DoDestroyAllRecycles();

            Popup.Active_ = true;
            PopupRoot.Instance.SetShow(true);
            Popup.UpdateScreenLock(false);

            Popup.lastOrder = 0;

            Popup.onOpenedListener = null;
            Popup.onClosedListener = null;
        }



        public const int SCREEN_LOCK = 7771;
        static bool ScreenLocked = false;
        static void UpdateScreenLock(bool value)
        {
            if (value == Popup.ScreenLocked)
                return;

            if (value)
            {
                if (Popup.IsShownPop)
                {
                    Popup.ScreenLocked = true;
                    BroadcastTunnel<int, bool>.Notify(Popup.SCREEN_LOCK, true);
                }
            }
            else
            {
                if (!Popup.IsShownPop)
                {
                    Popup.ScreenLocked = false;
                    BroadcastTunnel<int, bool>.Notify(Popup.SCREEN_LOCK, false);
                }
            }
        }
        #endregion popup system
    }
}