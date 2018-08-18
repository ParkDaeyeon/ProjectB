using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public class IndexableComponent : ManagedComponent, IIndexable<int>
    {
        [SerializeField]
        protected int index;
        bool first = true;
        public int Index
        {
            set
            {
                if (value == this.index && !this.first)
                    return;

                this.SetIndex(value);
            }
            get { return this.index; }
        }

        public void SetIndex(int value)
        {
            this.index = value;
            this.Apply();
        }

        public virtual int Count { get { return 0; } }
        public int Last { get { return Mathf.Max(0, this.Count - 1); } }

        public void Apply()
        {
            this.first = false;
            this.OnApply();
        }

        protected virtual void OnApply() { }


        public static void SetIndexIterate<T>(IEnumerable<T> components, int value, bool force = false)
            where T : IndexableComponent
        {
            if (null == components)
                return;

            var enumerator = components.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var component = enumerator.Current;
                if (!component)
                    continue;

                if (force) component.SetIndex(value);
                else component.Index = value;
            }
        }

        public static int GetIndexIterate<T>(IEnumerable<T> components)
            where T : IndexableComponent
        {
            if (null == components)
                return -1;

            var enumerator = components.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var component = enumerator.Current;
                if (!component)
                    continue;

                return component.Index;
            }

            return -1;
        }

        public static int GetCountIterate<T>(IEnumerable<T> components)
            where T : IndexableComponent
        {
            if (null == components)
                return 0;

            var enumerator = components.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var component = enumerator.Current;
                if (!component)
                    continue;

                return component.Count;
            }

            return 0;
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }

        [SerializeField]
        bool editorRebuild = false;
        public bool EditorRebuild
        {
            set { this.editorRebuild = value; }
            get { return this.editorRebuild; }
        }
        [SerializeField]
        bool editorAddCurrent = false;

        protected void EditorSetIndexableComponent()
        {
            base.OnEditorSetting();

            if (this.editorRebuild)
            {
                this.editorRebuild = false;
                this.OnEditorRebuild();
            }

            if (this.editorAddCurrent)
            {
                this.editorAddCurrent = false;
                this.OnEditorAddCurrent();
            }

            this.Apply();
        }

        protected virtual void OnEditorRebuild() { }
        protected virtual void OnEditorAddCurrent() { }
#endif// UNITY_EDITOR
    }
}
