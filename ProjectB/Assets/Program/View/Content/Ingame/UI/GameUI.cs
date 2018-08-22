using Ext.Unity3D;
using UnityEngine;
using UnityEngine.UI;

namespace Program.View.Content.Ingame.UI
{
    public class GameUI : BaseView
    {
        public enum Event
        {
            BtnDown,
        }

        [SerializeField]
        Button[] leftLineButtons;

        protected override void Awake()
        {
            base.Awake();

            var btns = this.leftLineButtons;
            for (int n = 0, cnt = btns.Length; n < cnt; ++n)
            {
                var btn = btns[n];
                var index = n;
                btn.onClick.AddListener(() =>
                {
                    this.DoEvent(Event.BtnDown, index);
                });
            }
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.leftLineButtons = this.FindComponentsInChildren<Button>();
        }
#endif// UNITY_EDITOR
    }
}
