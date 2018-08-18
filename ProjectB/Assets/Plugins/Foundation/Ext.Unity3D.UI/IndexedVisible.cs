using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Visible")]
    public class IndexedVisible : IndexableUIComponent
    {
        [Serializable]
        public class Group
        {
            [SerializeField]
            List<GameObject> list = new List<GameObject>();
            public List<GameObject> List { get { return this.list; } }

            public Group()
            {
            }
            public Group(IEnumerable<GameObject> values)
            {
                if (null != values)
                    this.list.AddRange(values);
            }

            public void SetActive(bool value)
            {
                for (int n = 0, cnt = this.list.Count; n < cnt; ++n)
                {
                    var go = this.list[n];
                    if (!go)
                        continue;

                    go.SetActive(value);
                }
            }
        }

        [SerializeField]
        List<Group> groups = new List<Group>();
        public List<Group> Groups { get { return this.groups; } }
        
        public Group GetGroup(int index)
        {
            if (0 > index || index >= this.groups.Count)
                return null;

            return this.groups[index];
        }
        public override int Count { get { return this.groups.Count; } }

        
        protected override void OnApply()
        {
            base.OnApply();

            var selected = this.GetGroup(this.index);
            for (int n = 0, cnt = this.groups.Count; n < cnt; ++n)
            {
                if (n == this.index)
                    continue;

                var group = this.groups[n];
                if (null != group)
                    group.SetActive(false);
            }

            if (null != selected)
                selected.SetActive(true);
        }
        

#if UNITY_EDITOR
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();
            this.EditorSetIndexableComponent();
        }

        protected override void OnEditorRebuild()
        {
            base.OnEditorRebuild();
            Debug.LogWarning("INDEXED_VISIBLE:NOT_SUPPORTED_EDITOR_REBUILD");
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();
            Debug.LogWarning("INDEXED_VISIBLE:NOT_SUPPORTED_EDITOR_ADD_CURRENT");
        }
#endif// UNITY_EDITOR
    }
}
