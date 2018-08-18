using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    // TODO: Refactoring
    [AddComponentMenu("UI/Ext/Rect Culling")]
    public class RectCulling : ManagedUIComponent
    {
        [Serializable]
        public class CullData
        {
            [SerializeField]
            protected RectTransform target;
            public RectTransform Target
            {
                get { return this.target; }
            }
            public bool IsValid
            {
                get { return this.target; }
            }

            //protected Matrix4x4 worldToLocalMat;
            protected float m00;
            protected float m01;
//            protected float m02;
//            protected float m03;
            protected float m10;
            protected float m11;
//            protected float m12;
//            protected float m13;
//            protected float m20;
//            protected float m21;
//            protected float m22;
//            protected float m23;
//            protected float m30;
//            protected float m31;
//            protected float m32;
//            protected float m33;
            protected Vector2 offset;

            protected Rect rect;
            public Rect Rect
            {
                get
                {
                    return this.rect;
                }
            }

            [SerializeField]
            protected bool isEnabled = true;
            public bool IsEnabled
            {
                set
                {
                    this.isEnabled = value;
                }
                get
                {
                    return this.isEnabled;
                }
            }


            public void SetActive(bool value)
            {
                if (this.target)
                    this.target.gameObject.SetActive(value);
            }
            public bool IsActivated
            {
                get { return this.target ? this.target.gameObject.activeSelf : false; }
            }


            public void SetEnableAndActivate(bool value)
            {
                this.isEnabled = value;
                if (this.target)
                    this.target.gameObject.SetActive(value);
            }



            public CullData(RectCulling rc, RectTransform target)
            {
                this.target = target;
                this.UpdateEnable(rc);
                this.UpdateTransform();
            }

            public void UpdateTransform()
            {
                if (!this.target)
                    return;
                
                Matrix4x4 mat = this.target.worldToLocalMatrix;
                this.m00 = mat.m00;
                this.m01 = mat.m01;
//                this.m02 = mat.m02;
//                this.m03 = mat.m03;
                this.m10 = mat.m10;
                this.m11 = mat.m11;
//                this.m12 = mat.m12;
//                this.m13 = mat.m13;
//                this.m20 = mat.m20;
//                this.m21 = mat.m21;
//                this.m22 = mat.m22;
//                this.m23 = mat.m23;
//                this.m30 = mat.m30;
//                this.m31 = mat.m31;
//                this.m32 = mat.m32;
//                this.m33 = mat.m33;
                this.rect = this.target.rect;

                Vector2 sizeHalf = this.rect.size * 0.5f;
                Vector2 pivot = this.target.pivot;
                this.offset.x = Mathf.Lerp(sizeHalf.x, -sizeHalf.x, pivot.x);
                this.offset.y = Mathf.Lerp(sizeHalf.y, -sizeHalf.y, pivot.y);
            }


            public void UpdateEnable(RectCulling rc)
            {
                if (!this.target)
                {
                    this.isEnabled = false;
                    return;
                }

                this.isEnabled = this.target.gameObject.activeSelf;
            }


            //Vector4 cachedPos4;
            Vector3 cachedPos3;
            Vector2 cachedPos2;
            public void Update()
            {
                this.cachedPos3 = this.target.position;
                this.cachedPos2.x = this.m00 * this.cachedPos3.x + this.m01 * this.cachedPos3.y + this.offset.x;
                this.cachedPos2.y = this.m10 * this.cachedPos3.x + this.m11 * this.cachedPos3.y + this.offset.y;
                this.rect.center = this.cachedPos2;
            }
        }

        public enum TYPE
        {
            Fixed,
            Expand,
        }
        [SerializeField]
        protected TYPE type;
        public TYPE Type
        {
            set { this.type = value; }
            get { return this.type; }
        }
        
        [SerializeField]
        protected RectTransform cullRectTrans;
        public RectTransform CullRectTrans { get { return this.cullRectTrans; } }


        [SerializeField]
        protected List<CullData> list = new List<CullData>();
        public List<CullData> List { get { return this.list; } }

        [SerializeField]
        protected bool isScriptable = false;
        public bool IsScriptable { get { return this.isScriptable; } }

        void OnEnable()
        {
            this.UpdateTransforms();
        }

        void LateUpdate()
        {
            if (this.isScriptable)
                this.UpdateRectangles();
            else
                this.UpdateVisibles();
        }

        public Rect UpdateCullRect()
        {
            if (!this.cullRectTrans)
                return default(Rect);

            Rect rect = this.cullRectTrans.rect;
            Vector2 size = rect.size;
            if (TYPE.Expand == this.type)
            {
                Vector2 originSize = size;
                Vector2    scale = size.CalcExpandAspectScale();
                size.x *= scale.x;
                size.y *= scale.y;
                rect.size = size;

                Vector2 center;
                center.x = (size.x - originSize.x) * 0.5f;
                center.y = size.y - originSize.y;
                rect.center -= center;
            }
            Vector2 sizeHalf = size * 0.5f;
            Vector2 pivot = this.cullRectTrans.pivot;
            rect.center -= new Vector2(Mathf.Lerp(sizeHalf.x, -sizeHalf.x, pivot.x), Mathf.Lerp(sizeHalf.y, -sizeHalf.y, pivot.y));
            return rect;
        }

        public void UpdateRectangles()
        {
            this.UpdateCullRect();
            for (int n = 0, count = this.list.Count; n < count; ++n)
            {
                CullData data = this.list[n];
                if (!data.IsEnabled || !data.Target)
                    continue;

                data.Update();
            }
        }

        // NOTE: for performance! (duplicated code)
        public void UpdateVisibles()
        {
            Rect area = this.UpdateCullRect();
            for (int n = 0, count = this.list.Count; n < count; ++n)
            {
                CullData data = this.list[n];
                if (!data.IsEnabled)
                    continue;
                
                if (!data.IsValid)
                    continue;

                data.Update();

                Rect rect = data.Rect;
                if (area.xMin < rect.xMax && area.xMax > rect.xMin &&
                    area.yMin < rect.yMax && area.yMax > rect.yMin)
                {
                    if (!data.IsActivated)
                        data.SetActive(true);
                }
                else
                {
                    if (data.IsActivated)
                        data.SetActive(false);
                }
            }
        }

        public void UpdateTransforms()
        {
            for (int n = 0, count = this.list.Count; n < count; ++n)
                this.list[n].UpdateTransform();
        }

        public void UpdateEnables()
        {
            for (int n = 0, count = this.list.Count; n < count; ++n)
                this.list[n].UpdateEnable(this);
        }

        public void SetEnableAll(bool value)
        {
            for (int n = 0, count = this.list.Count; n < count; ++n)
                this.list[n].IsEnabled = value;
        }

        public void SetEnableAndActivateAll(bool value)
        {
            for (int n = 0, count = this.list.Count; n < count; ++n)
                this.list[n].SetEnableAndActivate(value);
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public void Add(RectTransform target)
        {
            this.list.Add(new RectCulling.CullData(this, target));
        }

        public RectCulling.CullData Find(RectTransform target)
        {
            int idx = this.IndexOf(target);
            if (-1 != idx)
                return this.list[idx];

            return null;
        }

        public RectCulling.CullData GetAt(int idx)
        {
            if (0 <= idx && idx < this.list.Count)
                return this.list[idx];

            return null;
        }

        public int IndexOf(RectTransform target)
        {
            for (int n = 0, cnt = this.list.Count; n < cnt; ++n)
                if (this.list[n].Target == target)
                    return n;

            return -1;
        }

        public bool Contains(RectTransform target)
        {
            return -1 != this.IndexOf(target);
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorDebugGizmos = true;
        [SerializeField]
        bool editorSetupClear = false;
        [SerializeField]
        bool editorAutoAddChildrens = false;
        [SerializeField]
        bool editorAutoAddChildIncludeInactive = true;
        [SerializeField]
        bool editorAutoAddChildDeepHirarchy = false;
        [SerializeField]
        Transform editorAutoAddChildStart = null;

        protected override void OnEditorSetting()
        {
             base.OnEditorSetting();

            if (this.editorSetupClear)
            {
                this.editorSetupClear = false;
                this.Clear();
            }
        }

        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            if (this.editorAutoAddChildrens)
            {
                this.editorAutoAddChildrens = false;
                Transform start = this.editorAutoAddChildStart ? this.editorAutoAddChildStart : this.CachedTransform;
                Graphic[] targets = start.GetComponentsInChildren<Graphic>(this.editorAutoAddChildIncludeInactive);
                foreach (Graphic g in targets)
                {
                    if (!g)
                        continue;

                    var trans = g.rectTransform;
                    if (trans == start)
                        continue;

                    if (this.Contains(trans))
                        continue;

                    if (!this.editorAutoAddChildDeepHirarchy)
                    {
                        bool isContinue = false;
                        var parent = trans.parent;
                        while (parent)
                        {
                            if (parent is RectTransform)
                            {
                                if (this.Contains((RectTransform)parent))
                                {
                                    isContinue = true;
                                    break;
                                }
                            }

                            parent = parent.parent;
                        }

                        if (isContinue)
                            continue;
                    }

                    this.Add(trans);
                }
            }
        }

        [SerializeField]
        bool editorModeTest = false;
        protected override void OnEditorUpdateAndDrawGizmos()
        {
            base.OnEditorUpdateAndDrawGizmos();

            this.UpdateTransforms();
            if (this.editorDebugGizmos)
            {
                if (this.cullRectTrans)
                {
                    this.UpdateRectangles();

                    Gizmos.matrix = this.cullRectTrans.localToWorldMatrix;
                    Vector2 cenPos = this.cullRectTrans.localPosition;

                    for (int n = 0, count = this.list.Count; n < count; ++n)
                    {
                        var data = this.list[n];
                        if (!data.Target)
                            continue;

                        Gizmos.color = data.IsActivated ? Color.white : new Color(0.4f, 0.4f, 0.4f);
                        Gizmos.DrawWireCube(data.Rect.center - cenPos, data.Rect.size);
                    }

                    Gizmos.color = Color.red;
                    Rect rect = this.UpdateCullRect();
                    Gizmos.DrawWireCube(rect.center - cenPos, rect.size);
                }
            }

            if (this.editorModeTest)
                this.LateUpdate();
        }


        [UnityEditor.MenuItem("GameObject/UI/Ext/Rect Culling")]
        static void OnCreateSampleObject()
        {
            Transform parent = UnityEditor.Selection.activeTransform;
            RectCulling.CreateSampleObject(parent);
        }
        public static RectCulling CreateSampleObject(Transform parent)
        {
            RectCulling component = UIExtension.CreateUIObject<RectCulling>("RectCulling", parent, Vector3.zero, new Vector2(100, 100));
            component.cullRectTrans = component.GetComponent<RectTransform>();
            component.list = new List<CullData>();

            var imgVisibled = UIExtension.CreateUIObject<Image>("Visibled", component.CachedTransform, new Vector3(0, 0), new Vector2(30, 30));
            component.list.Add(new CullData(component, imgVisibled.rectTransform));
            var imgInvisibled = UIExtension.CreateUIObject<Image>("Invisibled", component.CachedTransform, new Vector3(-150, 0), new Vector2(30, 30));
            component.list.Add(new CullData(component, imgInvisibled.rectTransform));
            component.EditorSetting();
            component.LateUpdate();
            return component;
        }
#endif// UNITY_EDITOR
    }
}