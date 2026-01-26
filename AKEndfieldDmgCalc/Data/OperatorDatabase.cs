using System;
using System.Collections.Generic;
using AKEndfieldDmgCalc.Models;

namespace AKEndfieldDmgCalc
{
    public static class OperatorDatabase
    {
        public static readonly OperatorData Laevatain = BuildLaevatain();

        public static readonly IReadOnlyDictionary<string, OperatorData> ById;
        public static readonly IReadOnlyDictionary<string, OperatorData> ByName;

        static OperatorDatabase()
        {
            var byId = new Dictionary<string, OperatorData>(StringComparer.OrdinalIgnoreCase)
            {
                { Laevatain.Id, Laevatain }
            };

            var byName = new Dictionary<string, OperatorData>(StringComparer.OrdinalIgnoreCase)
            {
                { Laevatain.Name, Laevatain }
            };

            ById = byId;
            ByName = byName;
        }

        public static OperatorData GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return ById.TryGetValue(id.Trim(), out var op) ? op : null;
        }

        public static OperatorData GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return ByName.TryGetValue(name.Trim(), out var op) ? op : null;
        }

        private static OperatorData BuildLaevatain()
        {
            return new OperatorData
            {
                Id = "chr_0016_laevat",
                Name = "Laevatain",
                Rarity = 6,

                Lv90 = new OperatorStats
                {
                    HP = 5495,
                    ATK = 318,
                    DEF = 0,
                    STR = 121,
                    AGL = 99,
                    INT = 177,
                    WIL = 89,
                    BaseCritRate = 0.05,
                    AttackSpeed = 1.0,
                    AttackRangeMeters = 5.0
                },

                Normal = new NormalAttackProfile
                {
                    DamageType = DamageType.Heat,
                    BasicHitMultipliers = new Dictionary<Rank, double[]>
                    {
                        { Rank.Lv9, new[] { 0.29, 0.43, 0.45, 0.70, 0.95 } },
                        { Rank.M3,  new[] { 0.29, 0.43, 0.45, 0.70, 0.95 } }
                    },
                    HeavyHitIndex = 5,
                    HeavyHitTeamSpRestore = 20,
                    FinalStrikeStagger = 18,
                    FinisherMultiplier = new Dictionary<Rank, double>
                    {
                        { Rank.Lv9, 7.20 },
                        { Rank.M3,  9.00 }
                    },
                    DiveMultiplier = new Dictionary<Rank, double>
                    {
                        { Rank.Lv9, 1.44 },
                        { Rank.M3,  1.80 }
                    }
                },

                BattleSkill = new SkillProfile
                {
                    Id = "chr_0016_laevat_normal_skill",
                    Name = "Smouldering Fire",
                    Kind = SkillKind.BattleSkill,
                    DamageType = DamageType.Heat,
                    SpCost = 100,
                    CooldownSeconds = 0,
                    DurationSeconds = 0,
                    AffectedByStates = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "TwilightActive" },
                    Ranks = new Dictionary<Rank, SkillRankData>
                    {
                        {
                            Rank.Lv9,
                            new SkillRankData
                            {
                                BaseMultiplier = 1.12,
                                DotPerSeqMultiplier = 0.11,
                                AdditionalMultiplier = 6.16,
                                CombustionDurationSeconds = 5,
                                Stagger = 10,
                                TeamSpGainFlat = 0,
                                UltEnergyGainFlat = 100,
                                UltEnergyGainByTargetsHit = null,
                                UltEnergyGainAppliesTo = UltEnergyGainTrigger.AdditionalHitOnly
                            }
                        },
                        {
                            Rank.M3,
                            new SkillRankData
                            {
                                BaseMultiplier = 1.40,
                                DotPerSeqMultiplier = 0.14,
                                AdditionalMultiplier = 7.70,
                                CombustionDurationSeconds = 5,
                                Stagger = 10,
                                TeamSpGainFlat = 0,
                                UltEnergyGainFlat = 100,
                                UltEnergyGainByTargetsHit = null,
                                UltEnergyGainAppliesTo = UltEnergyGainTrigger.AdditionalHitOnly
                            }
                        }
                    }
                },

                ComboSkill = new SkillProfile
                {
                    Id = "chr_0016_laevat_combo_skill",
                    Name = "Seethe",
                    Kind = SkillKind.ComboSkill,
                    DamageType = DamageType.Heat,
                    SpCost = 0,
                    CooldownSeconds = 10,
                    DurationSeconds = 0,
                    Trigger = "Triggered by Combustion or Corrosion events.",
                    AffectedByStates = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "TwilightActive" },
                    Ranks = new Dictionary<Rank, SkillRankData>
                    {
                        {
                            Rank.Lv9,
                            new SkillRankData
                            {
                                BaseMultiplier = 4.32,
                                Stagger = 10,
                                TeamSpGainFlat = 0,
                                UltEnergyGainFlat = 0,
                                UltEnergyGainByTargetsHit = new Dictionary<int, double> { { 1, 25 }, { 2, 30 }, { 3, 35 } },
                                UltEnergyGainAppliesTo = UltEnergyGainTrigger.OnHit
                            }
                        },
                        {
                            Rank.M3,
                            new SkillRankData
                            {
                                BaseMultiplier = 5.40,
                                Stagger = 10,
                                TeamSpGainFlat = 0,
                                UltEnergyGainFlat = 0,
                                UltEnergyGainByTargetsHit = new Dictionary<int, double> { { 1, 25 }, { 2, 30 }, { 3, 35 } },
                                UltEnergyGainAppliesTo = UltEnergyGainTrigger.OnHit
                            }
                        }
                    }
                },

                Ultimate = new UltimateProfile
                {
                    Id = "chr_0016_laevat_ultimate_skill",
                    Name = "Twilight",
                    DamageType = DamageType.Heat,
                    UltEnergyCost = 300,
                    DurationSeconds = 15,
                    EnhancedBasicMultipliers = new Dictionary<Rank, double[]>
                    {
                        { Rank.Lv9, new[] { 1.17, 1.46, 2.08, 3.65 } },
                        { Rank.M3,  new[] { 1.46, 1.82, 2.60, 4.56 } }
                    },
                    AppliesState = "TwilightActive",
                    AffectsSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "chr_0016_laevat_normal_skill",
                        "chr_0016_laevat_combo_skill"
                    }
                },

                Talents = new List<TalentProfile>
                {
                    new TalentProfile
                    {
                        Name = "Scorching Heart",
                        Description = "Absorbs Heat Infliction on Final Strike/Finisher; grants Melting Flame stacks (max 4). At max stacks, ignores Heat resistance for a duration."
                    },
                    new TalentProfile
                    {
                        Name = "Re-Ignition",
                        Description = "When HP drops below 40%: gain Protection and restore a % of Max HP per second for a duration; triggers once per cooldown."
                    }
                },

                Potentials = new List<PotentialTier>
                {
                    new PotentialTier { Tier = 0, Name = "Base", Description = "No bonus." },

                    new PotentialTier
                    {
                        Tier = 1,
                        Name = "Heart of Melting Flame",
                        Description = "Smouldering Fire additional attack multiplier ×1.2; additional hit restores 20 SP.",
                        Effects = new List<IEffect>
                        {
                            new SkillParamMultiplierEffect("chr_0016_laevat_normal_skill", SkillParam.AdditionalMultiplier, 1.20),
                            new TeamSpGainOnSkillHitEffect("chr_0016_laevat_normal_skill", 20, true)
                        }
                    },

                    new PotentialTier
                    {
                        Tier = 2,
                        Name = "Pursuit of Memories",
                        Description = "INT +20; Basic Attack damage ×1.15.",
                        Effects = new List<IEffect>
                        {
                            new StatFlatEffect(INT: 20),
                            new DamageMultiplierEffect(DamageScope.BasicAttack, 1.15)
                        }
                    },

                    new PotentialTier
                    {
                        Tier = 3,
                        Name = "Fragments from the Past",
                        Description = "Smouldering Fire: Combustion duration ×1.5; Combustion damage ×1.5.",
                        Effects = new List<IEffect>
                        {
                            new SkillParamMultiplierEffect("chr_0016_laevat_normal_skill", SkillParam.CombustionDurationSeconds, 1.50),
                            new SkillParamMultiplierEffect("chr_0016_laevat_normal_skill", SkillParam.DotPerSeqMultiplier, 1.50)
                        }
                    },

                    new PotentialTier
                    {
                        Tier = 4,
                        Name = "Ice Cream Furnace",
                        Description = "Twilight Ultimate Energy cost ×0.85.",
                        Effects = new List<IEffect>
                        {
                            new UltimateCostMultiplierEffect(0.85)
                        }
                    },

                    new PotentialTier
                    {
                        Tier = 5,
                        Name = "Proof of Existence",
                        Description = "During Twilight: enhanced basic multipliers ×1.2; each kill extends duration +1s (max +7s).",
                        Effects = new List<IEffect>
                        {
                            new EnhancedBasicMultiplierDuringUltimateEffect(1.20),
                            new UltimateDurationOnKillEffect(1, 7)
                        }
                    }
                }
            };
        }
    }
}
