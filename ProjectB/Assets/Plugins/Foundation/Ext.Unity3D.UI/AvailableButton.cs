using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext;
using Ext.Unity3D;
using Ext.String;

namespace Ext.Unity3D.UI
{
    public class AvailableButton : ManagedUIComponent
    {
        [SerializeField]
        Button button;
        public Button Button
        {
            get { return this.button; }
        }
        public bool Interactable
        {
            set { this.button.interactable = value; }
            get { return this.button.interactable; }
        }


        [SerializeField]
        bool ifOnThenLock;
        public bool IfOnThenLock
        {
            set { this.ifOnThenLock = value; }
            get { return this.ifOnThenLock; }
        }

        [SerializeField]
        IndexableComponent[] onOffComponents;
        [SerializeField]
        UICache[] onAnimCaches;
        [SerializeField]
        UICache[] offAnimCaches;
        [SerializeField]
        [HideInInspector]
        bool isOn;
        public bool On
        {
            set
            {
                this.isOn = value;
                var index = value ? 1 : 0;

                for (int n = 0, cnt = this.onOffComponents.Length; n < cnt; ++n)
                {
                    var onOff = this.onOffComponents[n];
                    if (onOff)
                        onOff.Index = index;
                }

                for (int n = 0, cnt = this.onAnimCaches.Length; n < cnt; ++n)
                {
                    var anim = this.onAnimCaches[n].Animation;
                    if (anim)
                    {
                        if (value)
                            anim.ForcePlay(true);
                        else
                            anim.ForceStop(false);
                    }
                }

                for (int n = 0, cnt = this.offAnimCaches.Length; n < cnt; ++n)
                {
                    var anim = this.offAnimCaches[n].Animation;
                    if (anim)
                    {
                        if (value)
                            anim.ForceStop(false);
                        else
                            anim.ForcePlay(true);
                    }
                }

                if (this.ifOnThenLock)
                    this.Interactable = !value;
            }
            get { return this.isOn; }
        }


        [SerializeField]
        Text text;
        public string Text
        {
            set { if (this.text) this.text.text = value; }
            get { return this.text ? this.text.text : ""; }
        }
        public bool TextVisible
        {
            set { if (this.text) this.text.gameObject.SetActive(value); }
            get { return this.text ? this.text.gameObject.activeSelf : false; }
        }



#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.button = this.FindComponent<Button>();

            this.onOffComponents = this.FindComponentsInChildrenWithOptional<IndexableComponent>("OnOff;");

            var onAnims = this.FindComponentsInChildrenWithOptional<Animation>("OnAnim;");
            this.onAnimCaches = new UICache[onAnims.Length];
            for (int n = 0, cnt = onAnims.Length; n < cnt; ++n)
                this.onAnimCaches[n] = new UICache(onAnims[n].gameObject);

            var offAnims = this.FindComponentsInChildrenWithOptional<Animation>("OffAnim;");
            this.offAnimCaches = new UICache[offAnims.Length];
            for (int n = 0, cnt = offAnims.Length; n < cnt; ++n)
                this.offAnimCaches[n] = new UICache(offAnims[n].gameObject);
            
            this.text = this.FindComponent<Text>("Text");
        }


        [SerializeField]
        bool editorTestOn;
        [SerializeField]
        string editorTestText;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();

            this.On = this.editorTestOn;
            this.Text = this.editorTestText;
        }
#endif// UNITY_EDITOR
    }
}
