using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Text")]
    public class IndexedText : IndexableUIComponent
    {
        [SerializeField]
        List<Text> targets = new List<Text>();
        public List<Text> Targets { get { return this.targets; } }

        [SerializeField]
        List<string> strings = new List<string>();
        public List<string> Strings { get { return this.strings; } }
        public string GetString(int index)
        {
            if (0 > index || index >= this.strings.Count)
                return "";

            return this.strings[index];
        }

        public override int Count { get { return this.strings.Count; } }




        protected override void OnApply()
        {
            base.OnApply();

            var str = this.GetString(this.index);
            for (int n = 0, cnt = this.targets.Count; n < cnt; ++n)
            {
                var target = this.targets[n];
                if (target)
                    target.text = str;
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
            this.targets.AddRange(this.GetComponentsInChildren<Text>(true));
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();
            if (null == this.targets || 0 >= this.targets.Count)
                return;

            var target = this.targets[0];
            if (!target)
                return;

            this.strings.Add(target.text);
        }
#endif// UNITY_EDITOR
    }
}
