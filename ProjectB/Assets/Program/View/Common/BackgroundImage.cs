using UnityEngine;
using UnityEngine.UI;

using Ext.Unity3D;
using Ext.Unity3D.UI;

namespace Program.View.Common
{
    public class BackgroundImage : ManagedComponent
    {
        static BackgroundImage instance;
        public static BackgroundImage Instance
        {
            get { return BackgroundImage.instance; }
        }

        void Awake()
        {
            if (!BackgroundImage.instance)
                BackgroundImage.instance = this;
        }
        void OnDestroy()
        {
            Debug.Assert(BackgroundImage.instance == this);

            BackgroundImage.instance = null;
        }

        [SerializeField]
        Image image;
        public void SetBackground(Sprite sprite)
        {
            if (this.image.sprite != sprite)
                this.image.sprite = sprite;

            this.OnSetBackground(sprite);
        }
        protected virtual void OnSetBackground(Sprite sprite) { }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.image = this.FindComponent<Image>("Image");
        }
#endif// UNITY_EDITOR
    }
}
