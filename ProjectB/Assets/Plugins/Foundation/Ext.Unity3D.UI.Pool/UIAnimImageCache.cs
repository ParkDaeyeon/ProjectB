using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class UIAnimImageCache : ImageCache
    {
        [SerializeField]
        UIAnim anim;
        public UIAnim Anim { get { return this.anim; } }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.anim)
                this.anim = this.FindComponent<UIAnim>();
            
            var sprAnim = this.FindComponent<SpriteAnimation>();
            if (sprAnim && this.Pool)
                sprAnim.SetSprites(((UIAnimImagePool)this.Pool).Sprites);
        }
#endif// UNITY_EDITOR

        public override string ToString()
        {
            return string.Format("{{{0}, anim:{1}}}", base.ToString(), this.anim);
        }
    }
}
