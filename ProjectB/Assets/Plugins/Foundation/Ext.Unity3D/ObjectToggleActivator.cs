using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public class ObjectToggleActivator : IndexableComponent
    {
        [SerializeField]
        ObjectsComponent objects;
        public ObjectsComponent Objects { get { return this.objects; } }

        [SerializeField]
        bool invert;
        public bool Invert
        {
            set { this.invert = value; }
            get { return this.invert; }
        }
        public override int Count { get { return this.objects.Count; } }

        public GameObject SelectedObject { get { return this.objects[this.index]; } }

        public void Next()
        {
            var idx = this.index;
            ++idx;
            if (idx > this.objects.Last)
                idx = 0;

            this.Index = idx;
        }

        public void Prev()
        {
            var idx = this.index;
            --idx;
            if (0 > idx)
                idx = this.objects.Last;

            this.Index = idx;
        }


        protected override void OnApply()
        {
            base.OnApply();

            for (int n = 0, cnt = this.objects.Count; n < cnt; ++n)
            {
                var go = this.objects[n];
                if (!go)
                    continue;
                
                go.SetActive(this.invert ? n != this.index : n == this.index);
            }
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            base.EditorSetIndexableComponent();
        }

        protected override void OnEditorRebuild()
        {
            base.OnEditorRebuild();

            if (!this.objects)
                this.objects = this.FindComponent<ObjectsComponent>();
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();


        }
#endif// UNITY_EDITOR
    }
}
