using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class UIAnimImagePool : ImagePool, IEnumerable<UIAnimImageCache>
    {
        public new UIAnimImageCache this[int index] { get { return base[index] as UIAnimImageCache; } }

        public new IEnumerator<UIAnimImageCache> GetEnumerator()
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

        protected override void OnEditorBuildCache(PoolCache c_, Transform ct)
        {
            base.OnEditorBuildCache(c_, ct);

            var c = c_ as UIAnimImageCache;
            var image = c.Graphic as OptimizedImage;
            image.Caches = this.Sprites;
        }
#endif// UNITY_EDITOR
    }
}
