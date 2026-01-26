using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKEndfieldDmgCalc.Models
{
    public readonly record struct HitContext(DamageType Type, DamageScope Scope);

    public sealed class BonusBuckets
    {
        public double ElementalBonusPercent { get; set; }
        public double SkillBonusPercent { get; set; }
        public double OtherBonusPercent { get; set; }
        public double BasicAttackBonusPercent { get; set; }

    }
}
