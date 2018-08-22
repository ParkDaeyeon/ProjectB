using UnityEngine;

using Ext.Unity3D;
using Program.View;
using Program.View.Content.Ingame;
using Program.View.Content.Ingame.UI;

namespace Program.View.Content
{
    public class IngameView : PresentView
    {
        [SerializeField]
        GamePlayer gamePlayer;
        public GamePlayer GamePlayer
        {
            get { return this.gamePlayer; }
        }

        [SerializeField]
        GameUI gameUi;
        public GameUI GameUI
        {
            get { return this.gameUi; }
        }
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            var canvas = this.FindTransform("Canvas");
            var content = canvas.FindTransform("Content");

            this.gamePlayer = content.FindComponent<GamePlayer>("GamePlayer");

            this.gameUi = content.FindComponent<GameUI>("GameUI");
        }
#endif// UNITY_EDITOR
    }
}
