using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    /// <summary>
    /// Button 의 정보를 추가로 더 담은 Cache
    /// </summary>
    [Serializable]
    public class ButtonCache : UICache
    {
        [SerializeField]
        Selectable button;
        public Selectable Button
        {
            get { return this.button; }
        }

        [SerializeField]
        Label label;
        public Label Label
        {
            get { return this.label; }
        }

        public ButtonCache(GameObject go)
            : base(go)
        {
        }
        
        public override void Assign(GameObject go)
        {
            base.Assign(go);
            if (go)
            {
                this.button = go.FindComponent<Selectable>();
                this.label = new Label(go.FindComponentsInChildren<Graphic>("Label"));
            }
            else
            {
                this.button = null;
                this.label = null;
            }
        }
        
        public static implicit operator bool(ButtonCache cache)
        {
            return null != cache && cache.GameObject;
        }


        public override string ToString()
        {
            return string.Format("{{BUTTON:{0}, LABEL:{1}, SFX:{2}, BASE:{3}}}",
                                 (!this.button ? "null" : this.button.ToString()),
                                 (!this.label ? "null" : this.label.ToString()),
                                 base.ToString());
        }
    }
}