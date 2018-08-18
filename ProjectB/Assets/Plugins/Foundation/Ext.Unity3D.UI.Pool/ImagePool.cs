using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class ImagePool : GraphicPool, IEnumerable<ImageCache>
    {
        [SerializeField]
        SpritesComponent sprites;
        public SpritesComponent Sprites { get { return this.sprites; } }

        public new ImageCache this[int index] { get { return base[index] as ImageCache; } }

        public new IEnumerator<ImageCache> GetEnumerator()
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
            if (!this.sprites)
                this.sprites = this.GetComponent<SpritesComponent>();

            base.OnEditorSetting();
        }

        protected override void OnEditorBuildCache(PoolCache c_, Transform ct)
        {
            base.OnEditorBuildCache(c_, ct);
        }
#endif// UNITY_EDITOR

        public override string ToString()
        {
            return string.Format("{{{0}, Sprites:{1}}}", base.ToString(), this.sprites);
        }
    }
}
