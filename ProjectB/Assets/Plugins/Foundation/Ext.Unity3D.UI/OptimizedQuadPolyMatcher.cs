using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D;

namespace Ext.Unity3D.UI
{
    public class OptimizedQuadPolyMatcher : ManagedUIComponent
    {
        [SerializeField]
        OptimizedQuadPolyImage from;
        public OptimizedQuadPolyImage From
        {
            set { this.from = value; }
            get { return this.from; }
        }

        [SerializeField]
        List<OptimizedQuadPolyImage> to;
        public List<OptimizedQuadPolyImage> To
        {
            get { return this.to; }
        }

        public void Apply()
        {
            if (!this.from)
                return;

            for (int n = 0, cnt = this.to.Count; n < cnt; ++n)
            {
                var to = this.to[n];
                if (!to)
                    continue;

                to.AssignQuad(this.from);
            }
        }

        [SerializeField]
        bool applyOnEnable;
        public bool ApplyOnEnable
        {
            set { this.applyOnEnable = value; }
            get { return this.applyOnEnable; }
        }
        void OnEnable()
        {
            if (this.applyOnEnable)
                this.Apply();
        }


        [SerializeField]
        bool applyOnUpdate;
        public bool ApplyOnUpdate
        {
            set { this.applyOnUpdate = value; }
            get { return this.applyOnUpdate; }
        }

        void LateUpdate()
        {
            if (this.applyOnUpdate)
                this.Apply();
        }

        
        public bool Contains(OptimizedQuadPolyImage img)
        {
            return this.ContainsFrom(img) ? true : this.ContainsTo(img);
        }
        public bool ContainsFrom(OptimizedQuadPolyImage img)
        {
            if (!img)
                return false;

            return img == this.from;
        }
        public bool ContainsTo(OptimizedQuadPolyImage img)
        {
            if (!img)
                return false;

            for (int n = 0, cnt = this.to.Count; n < cnt; ++n)
            {
                var to = this.to[n];
                if (!to)
                    continue;

                if (img == to)
                    return true;
            }
            return false;
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild;
        [SerializeField]
        GameObject[] editorRebuildToGroups;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorRebuild)
            {
                this.editorRebuild = false;
                this.to.Clear();
                for (int n = 0, cnt = this.editorRebuildToGroups.Length; n < cnt; ++n)
                {
                    var group = this.editorRebuildToGroups[n];
                    if (!group)
                        continue;

                    var imgs = group.FindComponentsInChildren<OptimizedQuadPolyImage>();
                    foreach (var img in imgs)
                    {
                        if (img && !this.Contains(img))
                            this.to.Add(img);
                    }
                }

                if (!this.from)
                {
                    var img = this.FindComponent<OptimizedQuadPolyImage>();
                    if (img && !this.Contains(img))
                        this.from = img;
                }
            }

            this.Apply();
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.Apply();
        }
#endif// UNITY_EDITOR
    }
}
