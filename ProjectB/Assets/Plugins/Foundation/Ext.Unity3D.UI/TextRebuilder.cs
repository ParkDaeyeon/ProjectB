using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class TextRebuilder : ManagedUIComponent
    {
        [SerializeField]
        List<Text> texts = new List<Text>();
        public List<Text> Texts
        {
            get { return this.texts; }
        }

        public void DoRebuild()
        {
            for (int n = 0, cnt = this.texts.Count; n < cnt; ++n)
            {
                var text = this.texts[n];
                if (!text)
                    continue;

                if (!text.font)
                    continue;

                text.font.characterInfo = null;
                text.font.RequestCharactersInTexture(text.text, text.fontSize, text.fontStyle);
                text.FontTextureChanged();
            }
        }

        void OnEnable()
        {
            this.DoRebuild();
        }


#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild;
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();
            
            if (this.editorRebuild)
            {
                this.editorRebuild = false;
                this.texts = new List<Text>(this.FindComponentsInChildren<Text>());

                this.DoRebuild();
            }
        }
#endif// UNITY_EDITOR
    }
}
