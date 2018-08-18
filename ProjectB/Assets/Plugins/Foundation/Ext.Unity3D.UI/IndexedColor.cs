using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Color")]
    public class IndexedColor : IndexableUIComponent
    {
        [SerializeField]
        List<Graphic> targets = new List<Graphic>();
        public List<Graphic> Targets { get { return this.targets; } }

        [SerializeField]
        protected bool alphaOnly;
        public bool AlphaOnly
        {
            set { this.alphaOnly = value; }
            get { return this.alphaOnly; }
        }

        [SerializeField]
        List<Color> colors = new List<Color>();
        public List<Color> Colors { get { return this.colors; } }
        public Color GetColor(int index)
        {
            if (0 > index || index >= this.colors.Count)
                return Color.magenta;

            return this.colors[index];
        }
        
        public override int Count { get { return this.colors.Count; } }



        protected override void OnApply()
        {
            base.OnApply();

            var color = this.GetColor(this.index);
            for (int n = 0, cnt = this.targets.Count; n < cnt; ++n)
            {
                var target = this.targets[n];
                if (target)
                {
                    if (this.alphaOnly)
                        target.SetAlpha(color.a);
                    else
                        target.color = color;
                    target.SetVerticesDirty();
                }
            }
        }


#if UNITY_EDITOR
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            this.EditorSetIndexableComponent();
        }
        
        [SerializeField]
        Transform[] editorAutoResetGroups;
        public Transform[] EditorAutoResetGroups
        {
            set { this.editorAutoResetGroups = value; }
            get { return this.editorAutoResetGroups; }
        }

        [SerializeField]
        Transform[] editorAutoResetIgnoreGroups;
        public Transform[] EditorAutoResetIgnoreGroups
        {
            set { this.editorAutoResetIgnoreGroups = value; }
            get { return this.editorAutoResetIgnoreGroups; }
        }
        protected override void OnEditorRebuild()
        {
            base.OnEditorRebuild();
            this.targets.Clear();
            this.targets.AddRange(this.GetComponentsInChildren<Graphic>(true));
            foreach (var group in this.editorAutoResetGroups)
            {
                if (!group)
                    continue;

                this.targets.AddRange(group.GetComponentsInChildren<Graphic>(true));
            }

            var ignores = new List<Graphic>();
            foreach (var group in this.editorAutoResetIgnoreGroups)
            {
                if (!group)
                    continue;

                ignores.AddRange(group.GetComponentsInChildren<Graphic>(true));
            }

            foreach (var graphic in ignores)
                this.targets.Remove(graphic);
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();
            if (null == this.targets || 0 >= this.targets.Count)
                return;

            var target = this.targets[0];
            if (!target)
                return;

            this.colors.Add(target.color);
        }
#endif// UNITY_EDITOR
    }
}
