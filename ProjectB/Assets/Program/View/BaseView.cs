using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.Algorithm;

namespace Program.View
{
    public partial class BaseView : ManagedUIComponent
    {
        // ------------------------------
        // NOTE: REGISTRY
        // ------------------------------
        static HashSet<BaseView> registry = new HashSet<BaseView>();
        public static IEnumerable<BaseView> AllViews
        {
            get { return BaseView.registry; }
        }
        
        protected virtual void Awake()
        {
            BaseView.registry.Add(this);
        }

        protected virtual void OnDestroy()
        {
            BaseView.registry.Remove(this);
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }


        // ------------------------------
        // NOTE: RESPONSIVE
        // ------------------------------
        static Point2 keepScreenSize = new Point2(0, 0);
        public static Point2 GetScreenSize()
        {
            return BaseView.keepScreenSize;
        }

        static bool layoutRemind;
        public static void UpdateViews() { }

        public enum LayoutMode
        {
            None,
            Portrait,
            Landscape,
        }

        static LayoutMode keepLayoutMode = LayoutMode.None;
        public static LayoutMode GetLayoutMode()
        {
            return BaseView.keepLayoutMode;
        }

        public void ResetLayout()
        {
            this.OnUpdateResize();
        }

        protected virtual void OnUpdateResize()
        {
            Responsive.ResetLayoutAll(true);
        }


        // ------------------------------
        // NOTE: EVENT
        // ------------------------------
        Dictionary<int, Action<int, object>> events = new Dictionary<int, Action<int, object>>();
        public Dictionary<int, Action<int, object>> Events
        {
            get { return this.events; }
        }

        public void DoEvent<T>(T key, object arg = null)
        {
#if LOG_DEBUG
            Debug.Log(string.Format("VIEW_EVENT:{0}, TYPE:{1}, KEY:{2}, ARG:{3}",
                                     this.name,
                                     this.GetType().Name,
                                     key,
                                     arg));
#endif// LOG_DEBUG
            Action<int, object> action;
            int keyi = key.GetHashCode();
            if (this.events.TryGetValue(keyi, out action))
            {
                if (null != action)
                    action(keyi, arg);
            }
        }

        public void BindEvent<T>(T key, Action<int, object> action)
        {
#if LOG_DEBUG
            Debug.Log(string.Format("VIEW_EVENT_BIND:{0}, TYPE:{1}, KEY:{2}, ACTION:{3}",
                                     this.name,
                                     this.GetType().Name,
                                     key,
                                     action));
#endif// LOG_DEBUG
            var keyHash = key.GetHashCode();
            if (this.events.ContainsKey(keyHash))
                this.events[keyHash] = action;
            else
                this.events.Add(keyHash, action);
        }

        public bool RemoveEvent<T>(T key)
        {
#if LOG_DEBUG
            Debug.Log(string.Format("VIEW_EVENT_UNBIND:{0}, TYPE:{1}, KEY:{2}", 
                                     this.name,
                                     this.GetType().Name,
                                     key));
#endif// LOG_DEBUG
            return this.events.Remove(key.GetHashCode());
        }

        public Action<int, object> FindEvent<T>(T key)
        {
            Action<int, object> action;
            if (this.events.TryGetValue(key.GetHashCode(), out action))
                return action;

            return null;
        }

        public static void AppSuspend()
        {
            foreach (var view in BaseView.registry)
                view.SetPause(true, 0x10000000);
        }

        public static void AppResume()
        {
            foreach (var view in BaseView.registry)
                view.SetPause(false, 0x10000000);
        }

        uint pausedStates = 0;
        public void SetPause(bool paused, uint flags = 0x00000001)
        {
            var prev = this.pausedStates;
            if (paused)
            {
                this.pausedStates = BitFlag.Add(this.pausedStates, flags);
                if (0 == prev && 0 != this.pausedStates)
                    this.OnPause(true);
            }
            else
            {
                this.pausedStates = BitFlag.Remove(this.pausedStates, flags);
                if (0 != prev && 0 == this.pausedStates)
                    this.OnPause(false);
            }
        }

        protected virtual void OnPause(bool paused)
        {
#if LOG_DEBUG
            if (paused)
                Debug.Log(string.Format("VIEW_PAUSED:{0}, TYPE:{1}, PAUSED_STATES:{2}",
                                         this.name,
                                         this.GetType().Name,
                                         this.pausedStates));
            else
                Debug.Log(string.Format("VIEW_RESUMED:{0}, TYPE:{1}",
                                         this.name,
                                         this.GetType().Name));
#endif// LOG_DEBUG
        }

#if UNITY_EDITOR

        // < H O W  T O > 
        // 에디터 시점에서 Responsive 테스트
        // 1. editorTest_AreaInfo 값 설정
        // 2. editorTesting 체크
        // 3. editor Auto Setup 체크

        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();

            this.EditorTest_Initialize();

            this.EditorTest_UpdateResponsive();

            this.EditorTest_Release();
        }
        
        void EditorTest_Initialize()
        {
            this.editorTest_OriginInfo = new EditorTest_AreaInfo
            {
                onDummyArea      = UnityExtension.EditorTestOnDummyArea,
                areaPivot        = UnityExtension.EditorTestDummyAreaPivot,
                areaRate         = UnityExtension.EditorTestDummyAreaRate,
                viewportAreaSize = Responsive.ViewportAreaSize,
            };


            UnityExtension.EditorTestOnDummyArea    = this.editorTest_AreaInfo.onDummyArea;
            UnityExtension.EditorTestDummyAreaPivot = this.editorTest_AreaInfo.areaPivot;
            UnityExtension.EditorTestDummyAreaRate  = this.editorTest_AreaInfo.areaRate;
            Responsive.ViewportAreaSize             = this.editorTest_AreaInfo.viewportAreaSize;
        }


        //TODO : Responsive의 로직과 중복됨, 개선해야함
        void EditorTest_UpdateResponsive()
        {
            SortedDictionary<int, HashSet<Responsive>> dict = new SortedDictionary<int, HashSet<Responsive>>(new Responsive.RegistryComparer());

            var responsives = this.GetComponentsInChildren<Responsive>(true);
            foreach (var responsive in responsives)
            {
                //SoftMask는 카메라를 사용하기 때문에 에디터 시점에서 반응형 업데이트를 하기가 힘듬
                if (responsive is SoftMask)
                    continue;

                HashSet<Responsive> set = null;
                var order = responsive.Order;
                if (dict.ContainsKey(order))
                {
                    set = dict[order];
                }
                else
                {
                    set = new HashSet<Responsive>();
                    dict.Add(order, set);
                }

                set.Add(responsive);
            }

            foreach (var view in dict)
            {
                foreach (var res in view.Value)
                    res.UpdateResize();
            }
        }

        void EditorTest_Release()
        {
            UnityExtension.EditorTestOnDummyArea    = this.editorTest_OriginInfo.onDummyArea;
            UnityExtension.EditorTestDummyAreaPivot = this.editorTest_OriginInfo.areaPivot;
            UnityExtension.EditorTestDummyAreaRate  = this.editorTest_OriginInfo.areaRate;
            Responsive.ViewportAreaSize             = this.editorTest_OriginInfo.viewportAreaSize;
        }

        [Serializable]
        class EditorTest_AreaInfo
        {
            public EditorTest_AreaInfo()
            {
                this.areaPivot        = new Vector2(0, 0);
                this.areaRate         = new Vector2(0.5f, 0.5f);
                this.viewportAreaSize = new Vector2(1280, 720);
                this.onDummyArea      = true;
            }

            public EditorTest_AreaInfo(Vector2 areaPivot, Vector2 areaRate, Vector2 viewportAreaSize, bool onDummyArea)
            {
                this.areaPivot        = areaPivot;
                this.areaRate         = areaRate;
                this.viewportAreaSize = viewportAreaSize;
                this.onDummyArea      = onDummyArea;
            }

            public Vector2 areaPivot;
            public Vector2 areaRate;
            [HideInInspector]
            public Vector2 viewportAreaSize;

            [HideInInspector]
            public bool onDummyArea;
        }

        [SerializeField]
        EditorTest_AreaInfo editorTest_AreaInfo;

        EditorTest_AreaInfo editorTest_OriginInfo;

#endif // UNITY_EDITOR
    }
}
