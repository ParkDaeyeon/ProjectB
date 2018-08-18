using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ext.Unity3D.UI
{
    public abstract class Responsive : ManagedUIComponent
    {
        public void UpdateResize()
        {
            // TODO: Pre-processing

            this.OnResize();

            // TODO: Post-processing
        }

        protected abstract void OnResize();

        public virtual int Order
        {
            get { return 0; }
        }

        public enum AreaMode
        {
            Screen,
            Safe,
            Viewport,
        }
        [SerializeField]
        AreaMode areaMode = AreaMode.Screen;

        static Vector2 viewportAreaSize;
        public static Vector2 ViewportAreaSize
        {
            set { Responsive.viewportAreaSize = value; }
            get
            {
                if (0 == Responsive.viewportAreaSize.x ||
                    0 == Responsive.viewportAreaSize.y)
                    Responsive.viewportAreaSize = UnityExtension.ScreenSize;

                return Responsive.viewportAreaSize;
            }
        }

        public static Rect GetAreaRectByMode(AreaMode mode)
        {
            var viewRect = Rect.zero;
            switch (mode)
            {
            case AreaMode.Screen:
                viewRect = new Rect(Vector2.zero, UnityExtension.GetScaledScreenAreaSize(Responsive.ViewportAreaSize));
                break;

            case AreaMode.Safe:
                viewRect = UnityExtension.GetCalculatedSafeAreaRect(Responsive.ViewportAreaSize);
                break;

            case AreaMode.Viewport:
                viewRect = new Rect(Vector2.zero, Responsive.ViewportAreaSize);
                break;
            }

            return viewRect;
        }

        public static Vector2 GetAreaSizeByMode(AreaMode mode)
        {
            var areaRect = Responsive.GetAreaRectByMode(mode);

            return areaRect.size;
        }
        public Vector2 GetAreaSizeByCurrent()
        {
            return Responsive.GetAreaSizeByMode(this.areaMode);
        }
        

        static SortedDictionary<int, HashSet<Responsive>> registry = new SortedDictionary<int, HashSet<Responsive>>(new RegistryComparer());
        //static SortedDictionary<int, HashSet<Responsive>> newObjects = new SortedDictionary<int, HashSet<Responsive>>(new RegistryComparer());
        public class RegistryComparer : IComparer<int>
        {
            int IComparer<int>.Compare(int x, int y)
            {
                return x - y;
            }
        }

        //public static IEnumerable<Responsive> AllViews
        //{
        //    get { return Responsive.registry; }
        //}
        protected virtual void OnAwake()
        {
            if (!this.isRegisted)
            {
                this.Add(Responsive.registry, this);

                this.UpdateResize();

                this.isRegisted = true;
            }
        }

        void Awake()
        {
            this.OnAwake();
        }

        bool isRegisted = false;
        public void Add(SortedDictionary<int, HashSet<Responsive>> dict, Responsive responsive)
        {
            if (null == dict)
                return;

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

        //public void AddNewObject(Responsive responsive)
        //{
        //    this.Add(Responsive.newObjects, responsive);
        //}

//        public static void UpdateNewObjects()
//        {
//            var reg = Responsive.newObjects;
//            if (null == reg)
//                return;

//            if (reg.Count <= 0)
//                return;

//            foreach (var pair in reg)
//            {
//                var obj = pair.Value;
//                if (null == obj)
//                    continue;

//                foreach (var responsive in obj)
//                {
//                    if (null != responsive)
//                        responsive.UpdateResize();
//                    else
//                    {
//#if LOG_DEBUG
//                        Debug.LogWarning(string.Format("RESPONSIVE_NEW_OBJECT_IS_NULL:{0}", obj.ToString()));
//#endif// LOG_DEBUG
//                    }
//                }

//                obj.Clear();
//            }
//        }

        protected virtual void OnDestroyed()
        {
            Responsive.Remove(this);
        }

        public static void Remove(Responsive responsive)
        {
            var reg = Responsive.registry;
            if (null == reg)
                return;

            HashSet<Responsive> set = null;
            var order = responsive.Order;
            if (reg.ContainsKey(order))
            {
                set = reg[order];
                set.Remove(responsive);
            }
        }

        void OnDestroy()
        {
            this.OnDestroyed();
        }

        // ------------------------------
        // NOTE: RESPONSIVE
        // ------------------------------
        static Point2 keepScreenSize = new Point2(0, 0);
        public static Point2 GetScreenSize()
        {
            return Responsive.keepScreenSize;
        }

        
        public static void UpdateViews()
        {
            var screenSize = UnityExtension.ScreenSizeI;
            if (screenSize == Responsive.keepScreenSize)
                return;

            Responsive.keepScreenSize = screenSize;
            Responsive.ResetLayoutAll(true);            
        }

        public static void ResetLayoutAll(bool resize)
        {
            if (!resize)
                return;

            Canvas.ForceUpdateCanvases();

            var reg = Responsive.registry;

            foreach (var view in reg)
            {
                foreach (var res in view.Value)
                {
                    res.UpdateResize();
                }
            }
        }



        public enum LayoutMode
        {
            None,
            Portrait,
            Landscape,
        }

        static LayoutMode keepLayoutMode = LayoutMode.None;
        public static LayoutMode GetLayoutMode()
        {
            return Responsive.keepLayoutMode;
        }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorTestViewportUseAreaSize = false;
        [SerializeField]
        protected Vector2 editorTestViewportAreaSize;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();

            var temp = Vector2.zero;
            if (this.editorTestViewportUseAreaSize)
            {
                temp = Responsive.ViewportAreaSize;
                Responsive.ViewportAreaSize = this.editorTestViewportAreaSize;
            }

            this.UpdateResize();

            if (this.editorTestViewportUseAreaSize)
                Responsive.ViewportAreaSize = temp;
        }
#endif// UNITY_EDITOR
    }
}
