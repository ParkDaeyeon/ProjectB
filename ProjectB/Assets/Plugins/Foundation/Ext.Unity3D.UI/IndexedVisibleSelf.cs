using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Visible Self")]
    public class IndexedVisibleSelf : IndexableUIComponent
    {
        [SerializeField]
        List<bool> values;
        public List<bool> Values { get { return this.values; } }

        public bool GetValue(int index)
        {
            if (0 > index || index >= this.values.Count)
                return false;

            return this.values[index];
        }
        public override int Count { get { return this.values.Count; } }



        [SerializeField]
        IndexedVisible.Group group;
        public IndexedVisible.Group Group { get { return this.group; } }


        protected override void OnApply()
        {
            base.OnApply();

            var visibled = this.GetValue(this.index);
            if (null != this.group)
                this.group.SetActive(visibled);
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
