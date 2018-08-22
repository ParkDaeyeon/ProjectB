using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Ext.Unity3D;
using Program.View.Content.Ingame.IngameObject;
using Program.View.Content.Ingame.IngameObject.UI;

namespace Program.View.Content.Ingame
{
    public class GamePlayer : ManagedComponent
    {
        [SerializeField]
        Unit[] unitsPool;
        [SerializeField]
        Projectile[] projectilesPool;

        bool isOpened;
        public bool IsOpened
        {
            get { return this.isOpened; }
        }
        public void Open()
        {
            this.isOpened = true;

            IngameObjectManager.Open(this.unitsPool.ToList(), this.projectilesPool.ToList());
            IngameObjectUIManager.Open();
        }

        public void Close()
        {
            IngameObjectUIManager.Close();
            IngameObjectManager.Close();

            this.isOpened = false;
        }
        
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.unitsPool = this.FindComponentsInChildren<Unit>("UnitPool");
            for (int n = 0, cnt = this.unitsPool.Length; n < cnt; ++n)
                this.unitsPool[n].name = n.ToString();

            this.projectilesPool = this.FindComponentsInChildren<Projectile>("ProjectilePool");
            for (int n = 0, cnt = this.projectilesPool.Length; n < cnt; ++n)
                this.projectilesPool[n].name = n.ToString();
        }
#endif// UNITY_EDITOR
    }
}
