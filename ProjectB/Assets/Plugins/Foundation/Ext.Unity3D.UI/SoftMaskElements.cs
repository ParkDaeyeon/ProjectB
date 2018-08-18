using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/SoftMaskElements")]
    public class SoftMaskElements : ManagedUIComponent
    {
        [Serializable]
        public class Element
        {
            [SerializeField]
            Graphic graphic;
            public Graphic Graphic
            {
                set { this.graphic = value; }
                get { return this.graphic; }
            }

            [SerializeField]
            Material originMaterial;
            public Material OriginMaterial
            {
                set { this.originMaterial = value; }
                get { return this.originMaterial; }
            }

            public Element(Graphic graphic, Material originMaterial)
            {
                this.graphic = graphic;
                this.originMaterial = originMaterial;
            }

            bool isApplied;
            public bool IsApplied
            {
                set { this.isApplied = value; }
                get { return this.isApplied; }
            }

            public void Rollback()
            {
                if (!this.isApplied)
                    return;

                this.graphic.material = this.originMaterial;
                this.isApplied = false;
            }

            public void ApplySoftMaterial(Material softMaterial)
            {
                this.graphic.material = softMaterial;
                this.isApplied = true;
            }
        }


        [SerializeField]
        List<Element> elements = new List<Element>();

        public int Count { get { return this.elements.Count; } }
        public Element this[int index]
        {
            get
            {
                if (0 > index || index >= this.Count)
                    return null;

                return this.elements[index];
            }
        }
        
        public int FindIndex(Element elem)
        {
            return null != elem ? this.FindIndex(elem.Graphic) : -1;
        }
        public int FindIndex(Graphic graphic)
        {
            if (graphic)
            {
                for (int n = 0, cnt = this.elements.Count; n < cnt; ++n)
                {
                    var elem = this.elements[n];
                    if (elem.Graphic == graphic)
                        return n;
                }
            }

            return -1;
        }

        public bool Contains(Element elem)
        {
            return -1 != this.FindIndex(elem);
        }
        
        public bool Contains(Graphic graphic)
        {
            return -1 != this.FindIndex(graphic);
        }



        public bool AddElement(Graphic graphic)
        {
            if (!graphic)
                return false;

            var originMaterial = graphic.material;
            if (!originMaterial)
                return false;

            if (this.Contains(graphic))
                return false;

            this.elements.Add(new Element(graphic, originMaterial));
            return true;
        }

        public void AddElements(IEnumerable<Graphic> graphics)
        {
            var enumerator = graphics.GetEnumerator();
            while (enumerator.MoveNext())
                this.AddElement(enumerator.Current);
        }
        
        public void AddElementsInHierarchy(Transform transform)
        {
            if (!transform)
                return;

            this.AddElement(transform.GetComponent<Graphic>());

            for (int n = 0, cnt = transform.childCount; n < cnt; ++n)
                this.AddElementsInHierarchy(transform.GetChild(n));
        }




        public bool RemoveAtElement(int index)
        {
            var elem = this[index];
            if (null == elem)
                return false;

            elem.Rollback();
            this.elements.RemoveAt(index);
            return true;
        }

        public bool RemoveElement(Element elem)
        {
            return this.RemoveAtElement(this.FindIndex(elem));
        }

        public bool RemoveElement(Graphic graphic)
        {
            return this.RemoveAtElement(this.FindIndex(graphic));
        }

        public void RemoveElementsInChildren(Transform transform)
        {
            if (!transform)
                return;

            this.RemoveElement(transform.GetComponent<Graphic>());

            for (int n = 0, cnt = transform.childCount; n < cnt; ++n)
                this.RemoveElementsInChildren(transform.GetChild(n));
        }





        SoftMask mask;
        public SoftMask Mask
        {
            set { this.mask = value; }
            get { return this.mask; }
        }

        public void Sync()
        {
            if (!this.mask)
                return;
            
            this.RemoveDestroyedGraphics();
            for (int n = 0, cnt = this.elements.Count; n < cnt; ++n)
            {
                var elem = this.elements[n];
                if (!elem.Graphic)
                    continue;

                var originMaterial = elem.OriginMaterial;
                if (!originMaterial)
                    continue;

                var softMaterial = this.mask.GetMaterialByOriginShader(originMaterial.shader);
                if (!softMaterial)
                    continue;

                if (originMaterial == softMaterial)
                    continue;
                
                elem.ApplySoftMaterial(softMaterial);
            }
        }
        
        void RemoveDestroyedGraphics()
        {
            for (int n = this.elements.Count - 1; n >= 0; --n)
            {
                var elem = this.elements[n];
                if (!elem.Graphic)
                    this.elements.RemoveAt(n);
            }
        }
        
        public void Rollback()
        {
            for (int n = 0, cnt = this.elements.Count; n < cnt; ++n)
                this.elements[n].Rollback();
        }


        void Awake()
        {
            this.Sync();
        }

        void OnDestroy()
        {
            this.Rollback();
        }


        

#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }

        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();
            
            if (this.editorRebuild)
            {
                this.Rollback();
                this.elements.Clear();

                var graphics = this.GetComponentsInChildren<Graphic>(true);
                this.AddElements(graphics);
            }

            this.Rollback();
        }
#endif// UNITY_EDITOR
    }
}
