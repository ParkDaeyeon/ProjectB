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

        public static void Open(List<Unit> units, List<Projectile> projectiles)
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

            IngameObjectManager.units.Clear();
            IngameObjectManager.projectiles.Clear();

            IngameObjectManager.units = null;
            IngameObjectManager.projectiles = null;
        }

        static List<Unit> units;
        
        public static void AddUnit(Unit unit)
        {
            if (!IngameObjectManager.IsOpened)
                return;

            var list = IngameObjectManager.units;
            if (!list.Contains(unit))
                list.Add(unit);
        }
        public static void RemoveUnit(Unit unit)
        {
            if (!IngameObjectManager.IsOpened)
                return;

            var list = IngameObjectManager.units;
            if (list.Contains(unit))
                list.Remove(unit);
        }
        public static void ClearUnits()
        {
            if (!IngameObjectManager.IsOpened)
                return;

            var list = IngameObjectManager.units;
            list.Clear();
        }

        public static Unit GetNextUnit()
        {
            if (!IngameObjectManager.IsOpened)
                return null;

            var list = IngameObjectManager.units;

            for (int n = 0, cnt = list.Count; n < cnt; ++n)
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

            for (int n = 0, cnt = list.Count; n < cnt; ++n)
            {
                var unit = list[n];
                if (unit.IsLive != isLive)
                    continue;

                if (unit.gameObject == gameObject)
                    return unit;
            }

            return null;
        }

        static List<Projectile> projectiles;

        public static void AddProjectile(Projectile projectile)
        {
            if (!IngameObjectManager.IsOpened)
                return;

            var list = IngameObjectManager.projectiles;
            if (!list.Contains(projectile))
                list.Add(projectile);
        }
        public static void RemoveProjectile(Projectile projectile)
        {
            if (!IngameObjectManager.IsOpened)
                return;

            var list = IngameObjectManager.projectiles;
            if (list.Contains(projectile))
                list.Remove(projectile);
        }
        public static void ClearProjectiles()
        {
            if (!IngameObjectManager.IsOpened)
                return;

            var list = IngameObjectManager.projectiles;
            list.Clear();
        }

        public static Projectile GetNextProjectile()
        {
            if (!IngameObjectManager.IsOpened)
                return null;

            var list = IngameObjectManager.projectiles;

            for (int n = 0, cnt = list.Count; n < cnt; ++n)
            {
                var projectile = list[n];
                if (!projectile.gameObject.activeInHierarchy)
                    return projectile;
            }

            return null;
        }
    }
}
