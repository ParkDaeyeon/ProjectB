using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Ext.Unity3D;

using Program.View.Content.Ingame.IngameObject;

namespace Program.View.Content.Ingame
{
    public class UnitFactory : ManagedComponent
    {
#if UNITY_EDITOR
        [Serializable]
#endif //UNITY_EDITOR
        public class UnitSpawnData
        {
            public double time;
            public Vector2 position;
            public int direction;
            public Unit.UnitData[] unitDatas;
        }

        public IEnumerator StartSpawn(IEnumerable<UnitSpawnData> spawnDatas)
        {
            var t = 0f;
            var e = spawnDatas.GetEnumerator();
            while (e.MoveNext())
            {
                var spawnData = e.Current;
                while (t < spawnData.time)
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                var unitDatas = spawnData.unitDatas;
                for (int n = 0, cnt = unitDatas.Length; n < cnt; ++n)
                {
                    var unit = IngameObjectManager.GetNextUnit();
                    var unitData = unitDatas[n];
                    unit.Spawn(unitData, UnitSpritesManager.GetBodySpritesByCode(unitData.code), spawnData.position, spawnData.direction);
                }
            }
        }
    }
}
