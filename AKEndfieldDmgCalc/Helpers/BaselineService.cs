using System;
using System.Collections.Generic;
using AKEndfieldDmgCalc.Models;
using EndfieldCalculator;

namespace AKEndfieldDmgCalc.Helpers
{
    public sealed class BaselineService
    {
        private readonly BaselineCache _cache = new();
        private readonly Func<double, double, double, DamageCalculator.DamageResult> _calcFromUiWithOverrides;

        public BaselineService(Func<double, double, double, DamageCalculator.DamageResult> calcFromUiWithOverrides)
        {
            _calcFromUiWithOverrides = calcFromUiWithOverrides
                ?? throw new ArgumentNullException(nameof(calcFromUiWithOverrides));
        }

        public void InvalidateAll() => _cache.Clear();

        public (double Raw100, double Final100) GetBaseline100(HitContext hit, IEnumerable<IEffect> effects)
        {
            if (_cache.TryGet(hit, out var raw, out var final))
                return (raw, final);

            var buckets = BuffAggregator.ComputeBuckets(hit, effects);

            double elementalBonus = buckets.ElementalBonusPercent;
            double skillBonus = buckets.SkillBonusPercent;
            double otherBonus = buckets.OtherBonusPercent;

          
            if (hit.Scope == DamageScope.BasicAttack)
                skillBonus += buckets.BasicAttackBonusPercent;

            // multiplier = 100% baseline
            var r100 = _calcFromUiWithOverrides(elementalBonus, skillBonus, otherBonus);

            raw = r100.BaseDamage;
            final = r100.FinalDamage;

            _cache.Set(hit, raw, final);
            return (raw, final);
        }
    }
}
