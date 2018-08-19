using Ext.Unity3D;
using UnityEngine;
using UnityEngine.UI;

namespace Program.View
{
    public class EntryView : PresentView
    {
        [SerializeField]
        Image logo;

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            var canvas = this.FindTransform("Canvas");
            var content = canvas.FindTransform("Content");

            this.logo = content.FindComponent<Image>("Logo");
        }
#endif// UNITY_EDITOR
    }
}
