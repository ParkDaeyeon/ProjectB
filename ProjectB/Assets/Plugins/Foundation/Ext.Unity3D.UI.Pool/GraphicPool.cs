using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
namespace Ext.Unity3D.UI.Pool
{
    public class GraphicPool : _2DPool, IEnumerable<GraphicCache>
    {
        public new RectTransform CacheBase { get { return base.CacheBase as RectTransform; } }

        public new GraphicCache this[int index] { get { return base[index] as GraphicCache; } }

        public new IEnumerator<GraphicCache> GetEnumerator()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                yield return this[n];
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }
#endif// UNITY_EDITOR
    }
}
