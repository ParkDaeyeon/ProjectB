using UnityEngine;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    public class IndexedGroupCanvasAlpha : IndexableComponent
    {
        [SerializeField]
        protected CanvasGroup[] targets;
        public CanvasGroup[] Targets
        {
            set { this.targets = value; }
            get { return this.targets; }
        }


        [SerializeField, Range(0f, 1f)]
        List<float> alphas = new List<float>();
        public List<float> Alphas { get { return this.alphas; } }
        public float GetAlpha(int index)
        {
            if (0 > index || index >= this.alphas.Count)
                return 1f;

            return Mathf.Clamp01(this.alphas[index]);
        }

        public override int Count { get { return this.alphas.Count; } }

        protected override void OnApply()
        {
            base.OnApply();

            var alpha = this.GetAlpha(this.index);
            for (int n = 0, cnt = this.targets.Length; n < cnt; ++n)
            {
                var target = this.targets[n];
                if (target)
                    target.alpha = alpha;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuildElements;
        [SerializeField]
        Transform[] editorAutoResetGroups;
        [SerializeField]
        bool editorRemoveEmpties;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }

        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            if (this.editorRebuildElements)
            {
                this.editorRebuildElements = false;

                List<CanvasGroup> list = new List<CanvasGroup>();
                foreach (Transform t in this.editorAutoResetGroups)
                {
                    if (!t)
                        continue;

                    list.AddRange(t.GetComponentsInChildren<CanvasGroup>(true));
                }

                this.targets = list.ToArray();
            }

            if (this.editorRemoveEmpties)
            {
                this.editorRemoveEmpties = false;

                List<CanvasGroup> list = new List<CanvasGroup>();
                foreach (CanvasGroup g in this.targets)
                {
                    if (!g)
                        continue;

                    list.Add(g);
                }
                this.targets = list.ToArray();
            }

            this.EditorSetIndexableComponent();
        }
#endif// UNITY_EDITOR
    }
}
