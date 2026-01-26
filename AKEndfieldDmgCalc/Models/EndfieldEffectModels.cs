namespace AKEndfieldDmgCalc.Models
{
    public enum DamageScope { BasicAttack, BattleSkill, ComboSkill, Ultimate, AllDamage }

    public enum SkillParam
    {
        BaseMultiplier,
        DotPerSeqMultiplier,
        AdditionalMultiplier,
        FinisherMultiplier,
        CombustionDurationSeconds
    }

    public interface IEffect { }

    public sealed record StatFlatEffect(
        int HP = 0, int ATK = 0, int DEF = 0,
        int STR = 0, int AGL = 0, int INT = 0, int WIL = 0
    ) : IEffect;

  
    public sealed record DamageTypeBonusEffect(DamageType Type, double BonusPercent) : IEffect;

    public sealed record DamageTypeAndScopeBonusEffect(DamageType Type, DamageScope Scope, double BonusPercent) : IEffect;

    public sealed record DamageMultiplierEffect(DamageScope Scope, double Multiplier) : IEffect;

    public sealed record SkillParamMultiplierEffect(string SkillId, SkillParam Param, double Multiplier) : IEffect;

    public sealed record UltimateCostMultiplierEffect(double Multiplier) : IEffect;

    public sealed record EnhancedBasicMultiplierDuringUltimateEffect(double Multiplier) : IEffect;

    public sealed record UltimateDurationOnKillEffect(double SecondsPerKill, double MaxExtraSeconds) : IEffect;

    public sealed record TeamSpGainOnSkillHitEffect(string SkillId, double TeamSpGain, bool AppliesToAdditionalHitOnly) : IEffect;
}
