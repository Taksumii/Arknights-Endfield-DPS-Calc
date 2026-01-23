namespace AKEndfieldDmgCalc.Models
{
    /// Represents a saved build configuration for an operator 
    public class BuildProfile
    {
        public string Name { get; set; }
        public string Notes { get; set; }

        // Attacker Stats
        public decimal BaseAttack { get; set; }
        public decimal WeaponAttack { get; set; }
        public decimal AttackPercent { get; set; }
        public decimal AttackFlat { get; set; }
        public decimal PrimaryStat { get; set; }
        public decimal SecondaryStat { get; set; }

        // Stat Types
        public string PrimaryStatType { get; set; } = "STR";
        public string SecondaryStatType { get; set; } = "STR";

        public decimal DamageMultiplier { get; set; }
        public decimal CritRate { get; set; }
        public decimal CritDamage { get; set; }
        public decimal Level { get; set; }
        public decimal SourceStoneArtistry { get; set; }
        public string GearSetName { get; set; }

        // Damage Bonuses
        public decimal ElementalBonus { get; set; }
        public decimal SkillBonus { get; set; }

        
        public bool IsStaggered { get; set; } 

       
        public decimal UnbalanceBonus { get; set; }

        public decimal OtherBonus { get; set; }

        // Target Stats
        public decimal TargetDefense { get; set; }
        public decimal TargetResistance { get; set; }
        public string DamageType { get; set; }
        public bool IsUnbalanced { get; set; }
        public bool IsCritical { get; set; }
        public bool IsTrueDamage { get; set; }

        // Anomaly
        public string AnomalyType { get; set; }
        public decimal AnomalyLevel { get; set; }

        // Multiplier Zones
        public decimal Vulnerability { get; set; }
        public decimal Amplification { get; set; }
        public decimal Sanctuary { get; set; }
        public decimal Fragility { get; set; }
        public decimal DamageReduction { get; set; }
        public decimal SpecialMultiplier { get; set; }
    }
}