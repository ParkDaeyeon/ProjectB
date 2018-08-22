using UnityEngine;
using UnityEngine.UI;
using Ext.Unity3D.UI;

namespace Program.View.Content.Ingame.IngameObject.UI
{
    public class HpBar : ManagedUIComponent
    {
         Unit unit;

        [SerializeField]
        Image image;
        
        public void Init(Unit unit)
        {
            this.unit = unit;
            IngameObjectUIManager.Regist(this);
        }

        public void Release()
        {
            this.unit = null;
            IngameObjectUIManager.Unregist(this);
        }

        public void UIUpdate()
        {
            var baseHp = this.unit.BaseData.hp;
            var curHp = this.unit.CurrentData.hp;

            this.image.fillAmount = curHp / (float)baseHp;
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            this.image = this.GetComponent<Image>();
            this.image.type = Image.Type.Filled;
            this.image.fillMethod = Image.FillMethod.Horizontal;
        }
#endif// UNITY_EDITOR
    }
}