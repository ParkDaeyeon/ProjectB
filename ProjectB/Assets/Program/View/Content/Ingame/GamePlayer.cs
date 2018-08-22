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
        public Unit[] UnitsPool
        {
            get { return this.unitsPool; }
        }

        [SerializeField]
        Projectile[] projectilesPool;
        public Projectile[] ProjectilesPool
        {
            get { return this.projectilesPool; }
        }

        [SerializeField]
        RectTransform[] spawnPosition;

        [SerializeField]
        UnitFactory[] factories;

        public void StartFactory(int index, IEnumerable<UnitFactory.UnitSpawnData> spawnData)
        {
            if (-1 < index && index < this.GetFactoryCount())
                this.StartCoroutine(this.factories[index].StartSpawn(spawnData));
        }
        public UnitFactory GetFactoryAt(int index)
        {
            if (-1 < index && index < this.GetFactoryCount())
                return this.factories[index];
            else
                return null;
        }

        public int GetFactoryCount()
        {
            return null != this.factories ? this.factories.Length : 0;
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

            this.spawnPosition = this.FindComponentsInChildren<RectTransform>();

            this.factories = this.FindComponentsInChildren<UnitFactory>("Factories");
        }
#endif// UNITY_EDITOR
    }
}
