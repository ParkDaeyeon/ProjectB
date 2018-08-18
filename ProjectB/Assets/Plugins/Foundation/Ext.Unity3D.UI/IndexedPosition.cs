using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Position")]
    public class IndexedPosition : IndexableUIComponent
    {
        [SerializeField]
        List<RectTransform> targets = new List<RectTransform>();
        public List<RectTransform> Targets { get { return this.targets; } }
        
        [SerializeField]
        List<Vector2> datas = new List<Vector2>();
        public List<Vector2> Datas { get { return this.datas; } }
        public Vector2 GetData(int index)
        {
            if (0 > index || index >= this.datas.Count)
                return default(Vector2);

            return this.datas[index];
        }
        public override int Count { get { return this.datas.Count; } }



        protected override void OnApply()
        {
            base.OnApply();

            var data = this.GetData(this.index);
            for (int n = 0, cnt = this.targets.Count; n < cnt; ++n)
            {
                var target = this.targets[n];
                if (target)
                    target.anchoredPosition = data;
            }
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
            this.targets.Clear();
            this.targets.AddRange(this.GetComponentsInChildren<RectTransform>(true));
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();
            if (null == this.targets || 0 >= this.targets.Count)
                return;

            var target = this.targets[0];
            if (!target)
                return;

            this.datas.Add(target.anchoredPosition);
        }
#endif// UNITY_EDITOR
    }
}
