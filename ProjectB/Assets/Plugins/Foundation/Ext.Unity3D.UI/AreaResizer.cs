using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ext.Unity3D.UI
{
    public class AreaResizer : Responsive
    {
        [SerializeField]
        bool automatic = false;
        public bool Automatic { get { return this.automatic; } }
        void Update()
        {
#if !TEST_AUTO_LAYOUT
            if (!this.automatic)
                return;
#endif// !TEST_AUTO_LAYOUT

            this.UpdateElements();
        }

        [SerializeField]
        RectTransform content;

        [SerializeField]
        ResponsiveAreaResizer[] elems;

        void OnRebuildElements(ResponsiveAreaResizer[] elems, RectTransform area, RectTransform content)
        {
            if (null == elems)
                return;

            for (int n = 0, cnt = elems.Length; n < cnt; ++n)
            {
                var elem = elems[n];
                if (elem)
                {
                    elem.ResetOriginValues();
                    //elem.SetBaseArea(area);
                    //elem.SetBaseContent(content);
                }
            }
        }

        public void UpdateElements()
        {
            //if (null == this.elems)
            //    return;

            //for (int n = 0, cnt = this.elems.Length; n < cnt; ++n)
            //    this.elems[n].UpdateSize();
        }

        protected override void OnResize()
        {
            this.UpdateElements();
        }

        public override int Order
        {
            get { return 7; }
        }


#if UNITY_EDITOR
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();

            this.UpdateElements();
        }
        [SerializeField]
        bool editorRebuild = false;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            this.content = this.FindComponent<RectTransform>("Content");
        }
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            this.elems = this.FindComponentsInChildren<ResponsiveAreaResizer>("Content");

            if (this.editorRebuild)
            {
                this.editorRebuild = false;
                this.OnRebuildElements(this.elems, this.CachedRectTransform, this.content);
            }
        }
#endif// UNITY_EDITOR
    }
}
