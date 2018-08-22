using UnityEngine;
using UnityEngine.UI;

using System;

using Ext.Unity3D;
using Ext.Unity3D.UI;

using System.Collections;
using System.Collections.Generic;
using Program.View.Content.Ingame.Skill;
using Program.View.Content.Ingame.IngameObject.UI;

namespace Program.View.Content.Ingame.IngameObject
{
    public class Unit : ManagedUIComponent
    {
        public void Spawn(UnitData unitData, Vector2 spawnPosition, int direction)
        {
            this.SetActive(true);

            this.Init(unitData);

            this.CachedRectTransform.anchoredPosition = spawnPosition;
            this.Direction = direction;

            this.StartCoroutine(this.MainLoop());
            this.StartCoroutine(this.MovingLoop());
        }

        public void Init(UnitData unitData)
        {
            this.baseData = unitData;
            this.currentData = unitData;

            
            this.isDamaged = false;
            this.isMoving = false;
            this.isUsingSkill = false;

            this.hpBar.Init(this);

            if (null != this.enemiesInRange)
                this.enemiesInRange.Clear();
            else
                this.enemiesInRange = new List<Unit>(32);

            this.unitRange.OnTriEnter = this.OnTriEnter;
            this.unitRange.OnTriExit = this.OnTriExit;
        }

        IEnumerator MainLoop()
        {
            while (this.IsLive)
            {
                if (this.CanUseSkill)
                    yield return this.SkillLoop();

                yield return null;
            }
        }

#if UNITY_EDITOR
        [System.Serializable]
#endif// UNITY_EDITOR
        public struct UnitData
        {
            public long teamId;
            public long hp;
            public long mSpeed;
            public long damage;
            public long knockbackPower;
            public long range;
            public long aSpeed;

            public SkillKind[] skillKinds;
        }

#if UNITY_EDITOR
        [SerializeField]
#endif// UNITY_EDITOR
        UnitData baseData;
        public UnitData BaseData
        {
            get { return this.baseData; }
        }
#if UNITY_EDITOR
        [SerializeField]
#endif// UNITY_EDITOR
        UnitData currentData;
        public UnitData CurrentData
        {
            get { return this.currentData; }
        }
        public long TeamId
        {
            get { return this.currentData.teamId; }
        }
        public long Hp
        {
            set
            {
                this.currentData.hp = value;

                this.hpBar.UIUpdate();

                if (!this.IsLive)
                    this.Dead();
            }
            get { return this.currentData.hp; }
        }
        public long MoveSpeed
        {
            get { return this.currentData.mSpeed; }
        }
        public long Damage
        {
            get { return this.currentData.damage; }
        }
        public long KnockbackPower
        {
            get { return this.currentData.knockbackPower; }
        }
        public long Range
        {
            get { return this.currentData.range; }
        }
        public long AttackSpeed
        {
            get { return this.currentData.aSpeed; }
        }
        public SkillKind[] GetSkillKinds()
        {
            return this.currentData.skillKinds;
        }
        public SkillKind GetSkillKindAt(int index)
        {
            return this.currentData.skillKinds[index];
        }

        [SerializeField]
        RectTransform body;
#if UNITY_EDITOR
        [SerializeField]
#endif// UNITY_EDITOR
        int direction = 1; // -1 == left   1 == right
        public int Direction
        {
            set
            {
                if (0 == value)
                    return;

                this.direction = value;

                var tr = this.body;
                var scale = tr.localScale;
                scale.x = this.direction;

                tr.localScale = scale;
            }
            get { return this.direction; }
        }

        [SerializeField]
        HpBar hpBar;

        public bool IsLive
        {
            get { return 0 < this.Hp; }
        }
        bool isDamaged = false;
        public bool IsDamaged
        {
            get { return this.isDamaged; }
        }
        public void Damaged(Unit caster)
        {
            this.StartCoroutine(this.OnDamaged(caster));
        }
        IEnumerator OnDamaged(Unit caster)
        {
            if (!this.IsLive)
                yield break;

            this.isDamaged = true;

            this.Hp -= caster.Damage;

            yield return this.OnKnockback(caster);

            this.isDamaged = false;
        }
        float knockbackTime = 0.2f;
        IEnumerator OnKnockback(Unit caster)
        {
            var rt = this.CachedRectTransform;

            var velocity = -1 * this.Direction * caster.KnockbackPower;

            var t = 0f;
            while (knockbackTime > (t += Time.deltaTime))
            {
                if (!this.IsLive)
                    yield break;   

                var pos = rt.anchoredPosition;
                pos.x += velocity;
                rt.anchoredPosition = pos;
                yield return null;
            }
        }
        public void Dead()
        {
            this.SetActive(false);
        }

        bool isMoving;
        public bool IsMoving
        {
            get { return this.isMoving; }
        }
        public bool IsMovable
        {
            get { return this.IsLive && !this.IsUsingSkill && !this.IsDamaged && !this.HasEnemyInRange; }
        }

        float moveResumeDelay = 1.5f;
        IEnumerator MovingLoop()
        {
            while (this.IsLive)
            {
                this.isMoving = true;

                while (this.IsMovable)
                {
                    this.OnMove();
                    yield return null;
                }

                this.isMoving = false;

                var t = 0f;
                while (this.moveResumeDelay > (t += Time.deltaTime))
                    yield return null;
            }
        }
        protected virtual void OnMove()
        {
            var tr = this.CachedRectTransform;
            var pos = tr.anchoredPosition;

            pos.x += this.Direction * this.MoveSpeed * Time.deltaTime;

            tr.anchoredPosition = pos;
        }

        bool isUsingSkill;
        public bool IsUsingSkill
        {
            get { return this.isUsingSkill; }
        }

        // NOTE: 힐러, 히어로 등은 새로 정의할 수 있게 나중에 virtual로 
        public bool CanUseSkill
        {
            get { return this.IsLive && this.HasEnemyInRange; }
        }
        protected virtual IEnumerator SkillLoop()
        {
            if (!this.IsLive)
                yield break;

            this.isUsingSkill = true;

            var skillKind = this.GetSkillKindAt(0);

            var usingSkill = true;
            while (this.CanUseSkill && usingSkill)
            {
                if (!this.IsLive)
                    yield break;

                yield return SkillManager.OnUseSkill(skillKind, this, () =>
                {
                    usingSkill = false;
                });
            }

            this.isUsingSkill = false;

            var isCoolTime = true;
            while(isCoolTime)
                yield return SkillManager.OnSkillCoolTime(skillKind, this, () => isCoolTime = false);
        }

        [SerializeField]
        UnitRange unitRange;

        List<Unit> enemiesInRange = new List<Unit>(32);
        public List<Unit> EnemiesInRange
        {
            get { return this.enemiesInRange; }
        }
        public Unit FirstEnemyInRange
        {
            get { return null != this.enemiesInRange && 0 < this.enemiesInRange.Count ? this.enemiesInRange[0] : null; }
        }
        public bool HasEnemyInRange
        {
            get { return null != this.enemiesInRange && 0 < this.enemiesInRange.Count; }
        }

        void OnTriEnter(Collider2D collision)
        {
            var colUnit = IngameObjectManager.GetUnitByGameObject(collision.transform.parent.gameObject, true);
            if (null != colUnit)
            {
                var isMe = this == colUnit;
                var isEnemy = this.TeamId != colUnit.TeamId;
                if (!isMe && isEnemy)
                {
                    if (!this.enemiesInRange.Contains(colUnit))
                        this.enemiesInRange.Add(colUnit);
                }
            }
        }
        void OnTriExit(Collider2D collision)
        {
            var colUnit = IngameObjectManager.GetUnitByGameObject(collision.transform.parent.gameObject, true);
            if (null != colUnit)
            {
                var isEnemy = this.TeamId != colUnit.TeamId;
                if (isEnemy)
                {
                    if (this.enemiesInRange.Contains(colUnit))
                        this.enemiesInRange.Remove(colUnit);
                }
            }
        }
#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.unitRange = this.FindComponent<UnitRange>("Range");
            this.body = this.FindComponent<RectTransform>("Body");
            this.hpBar = this.FindComponent<HpBar>("UI/HpBar");
        }
#endif// UNITY_EDITOR
    }
}