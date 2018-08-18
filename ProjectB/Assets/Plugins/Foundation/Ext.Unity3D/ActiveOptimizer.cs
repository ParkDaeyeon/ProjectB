using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
namespace Ext.Unity3D
{
    public class ActiveOptimizer : ManagedComponent
    {
        [SerializeField]
        bool automatic = false;
        public bool Automatic { get { return this.automatic; } }
        void Awake()
        {
            if (!this.automatic)
                return;

            this.Optimize(true);
            this.Optimize(false);
        }

        [Serializable]
        public struct ActiveInfo
        {
            public ActiveInfo(GameObject go)
            {
                this.go = go;
                this.defaultActiveState = go.activeSelf;
            }

            [SerializeField]
            GameObject go;
            public GameObject GameObject { set { this.go = value; } get { return this.go; } }

            [SerializeField]
            bool defaultActiveState;
            public bool DefaultActiveState { set { this.defaultActiveState = value; } get { return this.defaultActiveState; } }

            public void Update()
            {
                if (!this.go)
                    return;

                this.defaultActiveState = this.go.activeSelf;
            }
            
            public void Optimize(bool active)
            {
                if (!this.go)
                    return;

                if (active)
                {
                    if (!this.go.activeSelf)
                        this.go.SetActive(true);
                }
                else if (!this.defaultActiveState)
                {
                    if (this.go.activeSelf)
                        this.go.SetActive(false);
                }
            }
        }

        [SerializeField]
        List<ActiveInfo> list;
        public List<ActiveInfo> List { set { this.list = value; } get { return this.list; } }

        public void Rebuild()
        {
            if (null == this.list)
                this.list = new List<ActiveInfo>();

            this.list.Clear();
            this.Collect(this.gameObject);
        }

        public void UpdateAll()
        {
            if (null == this.list)
                return;

            for (int n = 0, cnt = this.list.Count; n < cnt; ++n)
                this.list[n].Update();
        }

        void Collect(GameObject go)
        {
            if (null == this.list)
                return;

            this.list.Add(new ActiveInfo(go));

            var trans = go.transform;
            for (int n = 0, cnt = trans.childCount; n < cnt; ++n)
            {
                var child = trans.GetChild(n);
                if (!child)
                    continue;

                this.Collect(child.gameObject);
            }
        }

        bool activated = false;
        public void Optimize(bool active)
        {
            if (null == this.list)
                return;

            this.activated = active;
            for (int n = 0, cnt = this.list.Count; n < cnt; ++n)
                this.list[n].Optimize(active);
        }

        public new bool IsActivated { get { return this.activated; } }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.Rebuild();
        }
#endif// UNITY_EDITOR
    }
}
