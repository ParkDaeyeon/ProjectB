using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Transforms")]
    public class IndexedTransforms : IndexableComponent
    {
        [SerializeField]
        List<Transform> targets = new List<Transform>();
        public List<Transform> Targets { get { return this.targets; } }
        

        [Serializable]
        public class Data
        {
            [SerializeField]
            List<Transform> sources = new List<Transform>();
            public List<Transform> Sources { get { return this.sources; } }

            public int Count { get { return null != this.sources ? this.sources.Count : 0; } }
            public Transform this[int index] { get { return -1 < index && index < this.Count ? this.sources[index] : null; } }
        }

        [SerializeField]
        List<Data> datas = new List<Data>();
        public List<Data> Datas { get { return this.datas; } }
        public Data GetData(int index)
        {
            if (0 > index || index >= this.datas.Count)
                return default(Data);

            return this.datas[index];
        }
        public override int Count { get { return this.datas.Count; } }


        [SerializeField]
        bool useRectTransform;
        public bool UseRectTransform
        {
            set { this.useRectTransform = value; }
            get { return this.useRectTransform; }
        }


        protected override void OnApply()
        {
            base.OnApply();

            var data = this.GetData(this.index);
            if (null == data)
                return;

            for (int n = 0, cnt = this.targets.Count; n < cnt; ++n)
            {
                if (n >= data.Count)
                    break;

                var soruce = data[n];
                if (!soruce)
                    continue;

                var target = this.targets[n];
                if (!target)
                    continue;

                IndexedTransforms.ApplyTransform(target, soruce, this.useRectTransform);
            }
        }

        public static void ApplyTransform(Transform target, Transform source, bool useRectTrans = false)
        {
            var sourceScale = source.lossyScale;
            var targetScale = target.lossyScale;
            var scale = target.localScale;

            scale.x *= sourceScale.x / targetScale.x;
            scale.y *= sourceScale.y / targetScale.y;
            scale.z *= sourceScale.z / targetScale.z;

            target.localScale = scale;
            target.rotation = source.rotation;
            target.position = source.position;

            if (useRectTrans)
            {
                if (target is RectTransform && source is RectTransform)
                {
                    var targetR = (RectTransform)target;
                    var sourceR = (RectTransform)source;

                    targetR.pivot = sourceR.pivot;
                    targetR.offsetMin = sourceR.offsetMin;
                    targetR.offsetMax = sourceR.offsetMax;
                }
            }
        }


#if UNITY_EDITOR
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            this.EditorSetIndexableComponent();
        }
#endif// UNITY_EDITOR
    }
}