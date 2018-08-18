using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Indexed Image")]
    public class IndexedImage : IndexableUIComponent
    {
        [SerializeField]
        Image image;
        public Image Image
        {
            set { this.image = value; }
            get { return this.image; }
        }

        [SerializeField]
        SpritesComponent sprites;
        public SpritesComponent Sprites
        {
            set { this.sprites = value; }
            get { return this.sprites; }
        }
        public override int Count
        {
            get { return this.sprites ? this.sprites.Count : 0; }
        }

        
        protected override void OnApply()
        {
            base.OnApply();

            if (!this.image)
                return;

            if (!this.sprites)
                return;

            var sc = this.sprites[this.index];
            var sprite = default(Sprite);
            if (sc)
            {
                sprite = sc;
                this.image.sprite = sprite;
                if (this.autoNativeSize)
                    this.image.SetNativeSize();
            }
            else
            {
                this.image.sprite = null;
            }

            if (this.autoVisible)
            {
                this.SetActive(null != sprite);
            }
        }

        [SerializeField]
        bool autoNativeSize = false;
        public bool IsAutoNativeSize
        {
            set { this.autoNativeSize = value; }
            get { return this.autoNativeSize; }
        }


        [SerializeField]
        bool autoVisible = false;
        public bool IsAutoVisible
        {
            set { this.autoVisible = value; }
            get { return this.autoVisible; }
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.EditorSetIndexableComponent();
        }

        protected override void OnEditorRebuild()
        {
            base.OnEditorRebuild();
            
            if (!this.image)
                this.image = this.GetComponent<Image>();

            if (!this.sprites)
                this.sprites = this.GetComponent<SpritesComponent>();
        }

        protected override void OnEditorAddCurrent()
        {
            base.OnEditorAddCurrent();


        }
#endif// UNITY_EDITOR
    }
}
