using UnityEngine;
using UnityEngine.UI;
using Ext.Unity3D;
using System.Collections.Generic;

namespace Program.View.Content.Ingame.IngameObject
{
    public class UnitBody : ManagedComponent
    {
        public class UnitBodySprites
        {
            public Sprite head;
            public Sprite body;
            public Sprite leftArm;
            public Sprite rightArm;
        }

        public void Init(UnitBodySprites sprites)
        {
            if (null == sprites)
                return;

            this.head.sprite = sprites.head;
            this.body.sprite = sprites.body;
            this.leftArm.sprite = sprites.leftArm;
            this.rightArm.sprite = sprites.rightArm;
        }

        [SerializeField]
        Image head;

        [SerializeField]
        Image body;

        [SerializeField]
        Image leftArm;

        [SerializeField]
        Image rightArm;
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.head = this.FindComponent<Image>("Head");
            this.body = this.FindComponent<Image>("Body");
            this.leftArm = this.FindComponent<Image>("LeftArm");
            this.rightArm = this.FindComponent<Image>("RightArm");
        }
#endif// UNITY_EDITOR
    }
}
