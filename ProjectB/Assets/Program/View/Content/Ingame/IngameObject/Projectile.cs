using UnityEngine;

using Ext.Unity3D;
using System;

namespace Program.View.Content.Ingame.IngameObject
{
    public class Projectile : ManagedComponent
    {
        Unit owner;
        public void Init(Unit unit)
        {
            this.owner = unit;
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }
#endif// UNITY_EDITOR
    }
}
