using System;
using System.Collections;
using Program.View.Content.Ingame.IngameObject;

namespace Program.View.Content.Ingame.Skill
{
    public static class SkillManager
    {
        public static IEnumerator OnUseSkill(SkillKind skillKind, Unit caster, Action done)
        {
            switch (skillKind)
            {
                case SkillKind.Hit:
                    yield return SkillManager.Skill_Hit(caster, done);
                    break;
            }
        }

        const float Hit_Delay = 1f;
        static IEnumerator Skill_Hit(Unit caster, Action done)
        {
            if (caster.IsLive)
            {
                var enemy = caster.FirstEnemyInRange;
                enemy.Damaged(caster);
                var t = 0f;
                while (SkillManager.Hit_Delay > (t += UnityEngine.Time.deltaTime))
                {
                    if (!caster.IsLive)
                    {
                        if (null != done)
                            done();

                        yield break;
                    }

                    yield return null;
                }

                if (!enemy.IsLive)
                    caster.EnemiesInRange.Remove(enemy);
            }

            if (null != done)
                done();
        }


        public static IEnumerator OnSkillCoolTime(SkillKind skillKind, Unit caster, Action done)
        {
            switch (skillKind)
            {
                case SkillKind.Hit:
                    yield return SkillManager.CoolTime_Hit(caster, done);
                    break;
            }
        }

        const float Hit_CoolTime = 3f;
        static IEnumerator CoolTime_Hit(Unit caster, Action done)
        {
            if (caster.IsLive)
            {
                var t = 0f;
                while (SkillManager.Hit_CoolTime > (t += UnityEngine.Time.deltaTime))
                {
                    if (!caster.IsLive)
                    {
                        if (null != done)
                            done();
                        yield break;
                    }

                    yield return null;
                }
            }
            if (null != done)
                done();
        }
    }
}
