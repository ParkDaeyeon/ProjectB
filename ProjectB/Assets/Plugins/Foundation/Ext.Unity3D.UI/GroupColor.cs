    using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Group Color")]
    public class GroupColor : ManagedUIComponent
    {
        [SerializeField]
        protected Graphic[] targets;
        public Graphic[] Targets
        {
            set { this.targets = value; }
            get { return this.targets; }
        }

        [SerializeField]
        protected bool alphaOnly;
        public bool AlphaOnly
        {
            set { this.alphaOnly = value; }
            get { return this.alphaOnly; }
        }

        [SerializeField]
        protected Color color;
        public Color Color
        {
            set
            {
                if (this.color != value)
                    this.SetColor(value);
            }
            get { return this.color; }
        }

        public void SetColor(Color color)
        {
            this.color = color;
            this.UpdateColor();
        }
    
        bool isDirty = true;
        Color prevColor;
        public void UpdateColor()
        {
            this.isDirty = false;
            this.prevColor = this.color;

            for (int n = 0; n < this.targets.Length; ++n)
            {
                var target = this.targets[n];
                if (!target)
                    continue;

                if (this.alphaOnly)
                    target.SetAlpha(this.color.a);
                else
                    target.color = this.color;

                target.SetVerticesDirty();
            }
        }

        void LateUpdate()
        {
            if (this.isDirty || this.prevColor != this.color)
                this.UpdateColor();
        }

#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild;
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

            if (this.editorRebuild)
            {
                this.editorRebuild = false;

                List<Graphic> list = new List<Graphic>();
                foreach (Transform t in this.editorAutoResetGroups)
                {
                    if (!t)
                        continue;

                    list.AddRange(t.GetComponentsInChildren<Graphic>(true));
                }

                this.targets = list.ToArray();
            }

            if (this.editorRemoveEmpties)
            {
                this.editorRemoveEmpties = false;

                List<Graphic> list = new List<Graphic>();
                foreach (Graphic g in this.targets)
                {
                    if (!g)
                        continue;

                    list.Add(g);
                }
                this.targets = list.ToArray();
            }
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.UpdateColor();
        }
#endif// UNITY_EDITOR
    }
}
