using System;

namespace AKEndfieldDmgCalc.Calculators
{
    /// Contains DPS calculation formulas
   
    public static class DPSCalculator
    {
      
        public class DPSResult
        {
            // Base values
            public double MinBaseDPS { get; set; }
            public double MaxBaseDPS { get; set; }
            public double AvgBaseDPS { get; set; }

            // With skill
            public double MinDPSWithSkill { get; set; }
            public double MaxDPSWithSkill { get; set; }
            public double AvgDPSWithSkill { get; set; }

            // With ultimate
            public double MinDPSWithUlt { get; set; }
            public double MaxDPSWithUlt { get; set; }
            public double AvgDPSWithUlt { get; set; }

            // Combined
            public double MinCombinedDPS { get; set; }
            public double MaxCombinedDPS { get; set; }
            public double AvgCombinedDPS { get; set; }

            // Additional info
            public double SequenceTotal { get; set; }
            public double SequenceTime { get; set; }
            public string HitSequence { get; set; }
            public bool Is5HitCombo { get; set; }
            public string Breakdown { get; set; }
        }


        public static DPSResult Calculate(
            double minHitDamage,
            double maxHitDamage,
            double avgHitDamage,
            double seq1, double seq2, double seq3, double seq4, double seq5,
            bool use5Hits,
            double sequenceTime,
            double damageMultiplier,
            double skillDamageBonus,
            double skillUptime,
            double ultDamageBonus,
            double ultUptime)
        {
            var result = new DPSResult
            {
                SequenceTime = sequenceTime,
                Is5HitCombo = use5Hits
            };

            // Calculate sequence total
            result.SequenceTotal = seq1 + seq2 + seq3 + seq4;
            result.HitSequence = $"{seq1}% + {seq2}% + {seq3}% + {seq4}%";

            if (use5Hits)
            {
                result.SequenceTotal += seq5;
                result.HitSequence += $" + {seq5}%";
            }

            result.SequenceTotal /= 100.0;
            double dmgMultiplierDecimal = damageMultiplier / 100.0;

            // Calculate damage per sequence
            double minSeqDamage = minHitDamage * result.SequenceTotal / dmgMultiplierDecimal;
            double maxSeqDamage = maxHitDamage * result.SequenceTotal / dmgMultiplierDecimal;
            double avgSeqDamage = avgHitDamage * result.SequenceTotal / dmgMultiplierDecimal;

            // Base DPS
            result.MinBaseDPS = minSeqDamage / sequenceTime;
            result.MaxBaseDPS = maxSeqDamage / sequenceTime;
            result.AvgBaseDPS = avgSeqDamage / sequenceTime;

            // Calculate multipliers
            double avgSkillBonus = 1.0 + skillDamageBonus / 100.0 * skillUptime / 100.0;
            double avgUltBonus = 1.0 + ultDamageBonus / 100.0 * ultUptime / 100.0;

            // With Battle Skill
            result.MinDPSWithSkill = result.MinBaseDPS * avgSkillBonus;
            result.MaxDPSWithSkill = result.MaxBaseDPS * avgSkillBonus;
            result.AvgDPSWithSkill = result.AvgBaseDPS * avgSkillBonus;

            // With Ultimate
            result.MinDPSWithUlt = result.MinBaseDPS * avgUltBonus;
            result.MaxDPSWithUlt = result.MaxBaseDPS * avgUltBonus;
            result.AvgDPSWithUlt = result.AvgBaseDPS * avgUltBonus;

            // Combined
            result.MinCombinedDPS = result.MinBaseDPS * avgSkillBonus * avgUltBonus;
            result.MaxCombinedDPS = result.MaxBaseDPS * avgSkillBonus * avgUltBonus;
            result.AvgCombinedDPS = result.AvgBaseDPS * avgSkillBonus * avgUltBonus;

            // Generate breakdown
            result.Breakdown = GenerateBreakdown(result, minSeqDamage, maxSeqDamage, avgSeqDamage,
                avgSkillBonus, avgUltBonus, skillDamageBonus, skillUptime, ultDamageBonus, ultUptime);

            return result;
        }

        private static string GenerateBreakdown(
            DPSResult result,
            double minSeqDamage,
            double maxSeqDamage,
            double avgSeqDamage,
            double avgSkillBonus,
            double avgUltBonus,
            double skillDamageBonus,
            double skillUptime,
            double ultDamageBonus,
            double ultUptime)
        {
            return $@"=== DPS BREAKDOWN ===

            Basic Attack Sequence ({(result.Is5HitCombo ? "5-hit" : "4-hit")} combo):
              Hits: {result.HitSequence}
              Total: {result.SequenceTotal * 100:F1}% over {result.SequenceTime} seconds
              
            Damage per Sequence:
              Min: {minSeqDamage:F0} | Max: {maxSeqDamage:F0} | Avg: {avgSeqDamage:F0}
              
            Base DPS (no buffs):
              Min: {result.MinBaseDPS:F0} | Max: {result.MaxBaseDPS:F0} | Avg: {result.AvgBaseDPS:F0}
            
            Battle Skill:
              Damage Bonus: +{skillDamageBonus}%
              Active Time: {skillUptime}% uptime
              Avg Multiplier: ×{avgSkillBonus:F2}
              DPS: Min {result.MinDPSWithSkill:F0} | Max {result.MaxDPSWithSkill:F0} | Avg {result.AvgDPSWithSkill:F0}
            
            Ultimate:
              Damage Bonus: +{ultDamageBonus}%
              Active Time: {ultUptime}% uptime
              Avg Multiplier: ×{avgUltBonus:F2}
              DPS: Min {result.MinDPSWithUlt:F0} | Max {result.MaxDPSWithUlt:F0} | Avg {result.AvgDPSWithUlt:F0}
              
            Combined (Skill + Ult):
              Min DPS: {result.MinCombinedDPS:F0}
              Max DPS: {result.MaxCombinedDPS:F0}
              Avg DPS: {result.AvgCombinedDPS:F0}
              Variance: {result.MaxCombinedDPS - result.MinCombinedDPS:F0}
            
            30 Second Damage: {result.AvgCombinedDPS * 30:F0}
            60 Second Damage: {result.AvgCombinedDPS * 60:F0}";
        }
    }
}