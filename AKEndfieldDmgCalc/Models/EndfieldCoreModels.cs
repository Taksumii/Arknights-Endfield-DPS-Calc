using EndfieldCalculator;
using System;
using System.Collections.Generic;

namespace AKEndfieldDmgCalc.Models
{
    public enum Rank { Lv9, M3 }
    public enum SkillKind { BattleSkill, ComboSkill }
    public enum DamageType { Physical, Heat, Electric, Cryo, Nature, Aether, True, LifeDrain }
    public enum UltEnergyGainTrigger { OnHit, AdditionalHitOnly }

    public sealed class OperatorData
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public int Rarity { get; init; }

        public OperatorStats Lv90 { get; init; } = new();

        public NormalAttackProfile Normal { get; init; } = new();

        public SkillProfile BattleSkill { get; init; } = new();
        public SkillProfile ComboSkill { get; init; } = new();

        public UltimateProfile Ultimate { get; init; } = new();

        public List<TalentProfile> Talents { get; init; } = new();
        public List<PotentialTier> Potentials { get; init; } = new();
    }

    public sealed class OperatorStats
    {
        public int HP { get; init; }
        public int ATK { get; init; }
        public int DEF { get; init; }

        public int STR { get; init; }
        public int AGL { get; init; }
        public int INT { get; init; }
        public int WIL { get; init; }

        public double BaseCritRate { get; init; } = 0.05;
        public double AttackSpeed { get; init; } = 1.0;
        public double AttackRangeMeters { get; init; } = 3.0;
    }

    public sealed class NormalAttackProfile
    {
        public DamageType DamageType { get; init; } = DamageType.Physical;

        // Decimal multipliers: 0.29 = 29%
        public Dictionary<Rank, double[]> BasicHitMultipliers { get; init; } = new();

        // TEAM SP is shared; heavy hit restores TEAM SP (not ultimate energy).
        public int HeavyHitIndex { get; init; } // 1-based
        public double HeavyHitTeamSpRestore { get; init; }

        public double? FinalStrikeStagger { get; init; }

        public Dictionary<Rank, double>? FinisherMultiplier { get; init; }
        public Dictionary<Rank, double>? DiveMultiplier { get; init; }
    }

    public sealed class SkillProfile
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public SkillKind Kind { get; init; }

        public DamageType DamageType { get; init; } = DamageType.Physical;

        public double SpCost { get; init; }
        public double CooldownSeconds { get; init; }
        public double DurationSeconds { get; init; }

        public string Trigger { get; init; } = "";

        public HashSet<string> AffectedByStates { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<Rank, SkillRankData> Ranks { get; init; } = new();
    }

    public sealed class UltimateProfile
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public DamageType DamageType { get; init; } = DamageType.Physical;

        // Ultimate Energy is per-operator and separate from TEAM SP.
        public double UltEnergyCost { get; init; }

        public double DurationSeconds { get; init; }

        // Decimal multipliers: 1.17 = 117%
        public Dictionary<Rank, double[]>? EnhancedBasicMultipliers { get; init; }

        public string? AppliesState { get; init; }
        public HashSet<string> AffectsSkills { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public sealed class SkillRankData
    {
        public double? BaseMultiplier { get; init; }
        public double? DotPerSeqMultiplier { get; init; }
        public double? AdditionalMultiplier { get; init; }
        public double? FinisherMultiplier { get; init; }

        public double? CombustionDurationSeconds { get; init; }
        public double? Stagger { get; init; }

        public double TeamSpGainFlat { get; init; }

        public double UltEnergyGainFlat { get; init; }
        public Dictionary<int, double>? UltEnergyGainByTargetsHit { get; init; }
        public UltEnergyGainTrigger UltEnergyGainAppliesTo { get; init; } = UltEnergyGainTrigger.OnHit;
    }

    public sealed class TalentProfile
    {
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
    }

    public sealed class PotentialTier
    {
        public int Tier { get; init; } // 0..5
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
        public List<IEffect> Effects { get; init; } = new();
    }
}
