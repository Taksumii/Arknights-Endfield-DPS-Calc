using System.Collections.Generic;
using AKEndfieldDmgCalc.Models;

namespace AKEndfieldDmgCalc.Helpers
{
    public static class BuffAggregator
    {
        public static BonusBuckets ComputeBuckets(HitContext hit, IEnumerable<IEffect> effects)
        {
            var b = new BonusBuckets();

            foreach (var e in effects)
            {
                switch (e)
                {
                    case DamageMultiplierEffect d:
                        ApplyScopeMultiplierAsBonus(d, hit, b);
                        break;

                    case DamageTypeBonusEffect dt:
                        if (dt.Type == hit.Type)
                            b.ElementalBonusPercent += dt.BonusPercent;
                        break;

                    case DamageTypeAndScopeBonusEffect dts:
                        if (dts.Type == hit.Type && dts.Scope == hit.Scope)
                            b.ElementalBonusPercent += dts.BonusPercent;
                        break;
                }
            }

            return b;
        }

        private static void ApplyScopeMultiplierAsBonus(
            DamageMultiplierEffect d,
            HitContext hit,
            BonusBuckets b)
        {
            double bonus = (d.Multiplier - 1.0) * 100.0;
            if (bonus == 0) return;

            if (d.Scope == DamageScope.AllDamage)
            {
                b.OtherBonusPercent += bonus;
                return;
            }

            if (d.Scope == DamageScope.BasicAttack)
            {
                b.BasicAttackBonusPercent += bonus;
                return;
            }

            if (d.Scope == DamageScope.BattleSkill ||
                d.Scope == DamageScope.ComboSkill ||
                d.Scope == DamageScope.Ultimate)
            {
                b.SkillBonusPercent += bonus;
            }
        }
    }
}
