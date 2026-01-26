using AKEndfieldDmgCalc.Models;
using EndfieldCalculator;

namespace AKEndfieldDmgCalc.Helpers
{
    public sealed class RotationDamageBuilder
    {
        private readonly DamageCalculatorForm _form;

        public RotationDamageBuilder(DamageCalculatorForm form)
        {
            _form = form;
        }

        public DamageCalculator.DamageResult ComputeBaseline100(
            HitContext hit,
            IEnumerable<IEffect> effects)
        {
            var buckets = BuffAggregator.ComputeBuckets(hit, effects);

            double elementalBonus = buckets.ElementalBonusPercent;
            double skillBonus = buckets.SkillBonusPercent;
            double otherBonus = buckets.OtherBonusPercent;

           
            if (hit.Scope == DamageScope.BasicAttack)
                skillBonus += buckets.BasicAttackBonusPercent;

            return _form.CalculateFromUiForRotation(
                damageMultiplier: 100,
                elementalBonusOverride: elementalBonus,
                skillBonusOverride: skillBonus,
                otherBonusOverride: otherBonus
            );
        }
    }
}
