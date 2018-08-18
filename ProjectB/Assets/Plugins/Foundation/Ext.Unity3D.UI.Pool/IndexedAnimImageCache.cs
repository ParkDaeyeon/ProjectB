using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class IndexedAnimImageCache : ImageCache
    {
        [SerializeField]
        IndexedAnimation indexedAnim;
        public IndexedAnimation IndexedAnim { get { return this.indexedAnim; } }
        public Animation Anim { get { return this.indexedAnim.Anim; } }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.indexedAnim)
                this.indexedAnim = this.FindComponent<IndexedAnimation>();

            if (this.indexedAnim)
                this.indexedAnim.EditorSetting();

            var sprAnim = this.FindComponent<SpriteAnimation>();
            if (sprAnim)
                sprAnim.SetSprites(((IndexedAnimImagePool)this.Pool).Sprites);
        }
#endif// UNITY_EDITOR

        public override string ToString()
        {
            return string.Format("{{{0}, indexedAnim:{1}}}", base.ToString(), this.indexedAnim);
        }
    }
}
