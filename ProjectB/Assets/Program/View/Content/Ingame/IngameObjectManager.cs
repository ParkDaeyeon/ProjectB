using UnityEngine;

using System;
using System.Collections.Generic;

using Program.View.Content.Ingame.IngameObject;

namespace Program.View.Content.Ingame
{
    public static class IngameObjectManager
    {
        static bool isOpened = false;
        public static bool IsOpened
        {
            get { return IngameObjectManager.isOpened; }
        }

        public static void Open(Unit[] units, Projectile[] projectiles)
        {
            IngameObjectManager.isOpened = true;

            IngameObjectManager.units = units;
            IngameObjectManager.projectiles = projectiles;

            Debug.Assert(null != IngameObjectManager.units, "units can't be null");
            Debug.Assert(null != IngameObjectManager.projectiles, "projectiles can't be null");
        }

        public static void Close()
        {
            IngameObjectManager.isOpened = false;

            Debug.Assert(null != IngameObjectManager.units, "units can't be null");
            Debug.Assert(null != IngameObjectManager.projectiles, "projectiles can't be null");

            IngameObjectManager.units = null;
            IngameObjectManager.projectiles = null;
        }

        static Unit[] units;

        public static Unit GetNextUnit()
        {
            if (!IngameObjectManager.IsOpened)
                return null;

            var list = IngameObjectManager.units;

            for (int n = 0, cnt = list.Length; n < cnt; ++n)
            {
                var unit = list[n];
                if (!unit.gameObject.activeInHierarchy)
                    return unit;
            }

            return null;
        }

        public static Unit GetUnitByGameObject(GameObject gameObject, bool isLive)
        {
            if (!IngameObjectManager.IsOpened)
                return null;

            var list = IngameObjectManager.units;

            for (int n = 0, cnt = list.Length; n < cnt; ++n)
            {
                var unit = list[n];
                if (unit.IsLive != isLive)
                    continue;

                if (unit.gameObject == gameObject)
                    return unit;
            }

            return null;
        }

        static Projectile[] projectiles;

        public static Projectile GetNextProjectile()
        {
            if (!IngameObjectManager.IsOpened)
                return null;

            var list = IngameObjectManager.projectiles;

            for (int n = 0, cnt = list.Length; n < cnt; ++n)
            {
                var projectile = list[n];
                if (!projectile.gameObject.activeInHierarchy)
                    return projectile;
            }

            return null;
        }
    }
}
