using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class ImageCache : GraphicCache
    {
        public Image Image { get { return this.Graphic as Image; } }
        public SpritesComponent SharedSprites
        {
            get
            {
                var pool = this.GPool as ImagePool;
                return pool ? pool.Sprites : null;
            }
        }


        int spriteIndex = -1;
        public int SpriteIndex
        {
            set
            {
                var sprites = this.SharedSprites;
                if (sprites)
                {
                    if (value != this.spriteIndex)
                    {
                        var sc = sprites[this.spriteIndex = value];
                        this.Image.sprite = sc;
                    }
                }
            }
            get
            {
                return this.spriteIndex;
            }
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }
#endif// UNITY_EDITOR

        public override string ToString()
        {
            return string.Format("{{{0}, SharedSprites:{1}, SpriteIndex:{2}}}", base.ToString(), this.SharedSprites, this.spriteIndex);
        }
    }
}
