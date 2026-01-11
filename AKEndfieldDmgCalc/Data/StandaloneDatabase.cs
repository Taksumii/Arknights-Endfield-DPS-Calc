using System.Collections.Generic;

namespace AKEndfieldDmgCalc.Data
{
   
    public class StandaloneItem
    {
        public string Name { get; set; }
        public string Slot { get; set; } 
        public int Level { get; set; }

      
        public decimal DefenseFlat { get; set; }

      
        public decimal AttackFlat { get; set; }
        public decimal AttackPercent { get; set; }
        public decimal CritRate { get; set; }
        public decimal CritDamage { get; set; }
        public decimal DamageReduction { get; set; }
        public decimal ComboSkillEfficiency { get; set; }
        public decimal UltimateGain { get; set; }

        
        public decimal StrengthBonus { get; set; }
        public decimal AgilityBonus { get; set; }
        public decimal IntellectBonus { get; set; }
        public decimal WillBonus { get; set; }

        
        public decimal HPPercent { get; set; }
        public decimal HealOutput { get; set; }
    }

    public static class StandaloneItemDatabase
    {
        public static List<StandaloneItem> AllItems = new List<StandaloneItem>
        {
           
            new StandaloneItem
            {
                Name = "Redeemer Tag (Lv.70)",
                Slot = "EDC",
                Level = 70,
                DefenseFlat = 21,
                StrengthBonus = 43,
                DamageReduction = 17.8m
            },
            new StandaloneItem
            {
                Name = "Redeemer Tag T1 (Lv.70)",
                Slot = "EDC",
                Level = 70,
                DefenseFlat = 21,
                AgilityBonus = 43,
                ComboSkillEfficiency = 43.2m
            },
            new StandaloneItem
            {
                Name = "Redeemer Seal (Lv.70)",
                Slot = "EDC",
                Level = 70,
                DefenseFlat = 21,
                IntellectBonus = 43,
                UltimateGain = 25.7m
            },
            new StandaloneItem
            {
                Name = "Redeemer Seal T1 (Lv.70)",
                Slot = "EDC",
                Level = 70,
                DefenseFlat = 21,
                WillBonus = 43,
                HealOutput = 21.6m
            },

            
            new StandaloneItem
            {
                Name = "Miner Armor T1 (Lv.50)",
                Slot = "Body",
                Level = 50,
                DefenseFlat = 40,
                StrengthBonus = 65,
                WillBonus = 43,
                CritRate = 3.9m
            },
            new StandaloneItem
            {
                Name = "Miner Overalls T1 (Lv.50)",
                Slot = "Body",
                Level = 50,
                DefenseFlat = 40,
                IntellectBonus = 65,
                AgilityBonus = 43,
                AttackPercent = 7.8m
            },

           
            new StandaloneItem
            {
                Name = "Miner Turbine T1 (Lv.50)",
                Slot = "EDC",
                Level = 50,
                DefenseFlat = 15,
                StrengthBonus = 31,
                DamageReduction = 13.5m
            },
            new StandaloneItem
            {
                Name = "Miner Compression Core T1 (Lv.50)",
                Slot = "EDC",
                Level = 50,
                DefenseFlat = 15,
                AgilityBonus = 31,
                ComboSkillEfficiency = 31.2m
            },
            new StandaloneItem
            {
                Name = "Miner Drive Wheel T1 (Lv.50)",
                Slot = "EDC",
                Level = 50,
                DefenseFlat = 15,
                IntellectBonus = 31,
                UltimateGain = 18.6m
            },
            new StandaloneItem
            {
                Name = "Miner Comm T1 (Lv.50)",
                Slot = "EDC",
                Level = 50,
                DefenseFlat = 15,
                WillBonus = 31,
                HealOutput = 15.6m
            },

            
            new StandaloneItem
            {
                Name = "Miner Gloves T2 (Lv.50)",
                Slot = "Hand",
                Level = 50,
                DefenseFlat = 30,
                IntellectBonus = 49,
                AgilityBonus = 32,
                AttackPercent = 13.0m
            },
            new StandaloneItem
            {
                Name = "Miner Fists T1 (Lv.50)",
                Slot = "Hand",
                Level = 50,
                DefenseFlat = 30,
                WillBonus = 49,
                IntellectBonus = 32,
                UltimateGain = 15.5m
            },

            
            new StandaloneItem
            {
                Name = "Miner Turbine (Lv.36)",
                Slot = "EDC",
                Level = 36,
                DefenseFlat = 11,
                StrengthBonus = 22,
                ComboSkillEfficiency = 22.8m
            },
            new StandaloneItem
            {
                Name = "Miner Drive Wheel (Lv.36)",
                Slot = "EDC",
                Level = 36,
                DefenseFlat = 11,
                AgilityBonus = 22,
                CritRate = 5.7m
            },
            new StandaloneItem
            {
                Name = "Miner Compression Core (Lv.36)",
                Slot = "EDC",
                Level = 36,
                DefenseFlat = 11,
                IntellectBonus = 22,
                CritRate = 5.7m
            },
            new StandaloneItem
            {
                Name = "Miner Comm (Lv.36)",
                Slot = "EDC",
                Level = 36,
                DefenseFlat = 11,
                WillBonus = 22,
                HPPercent = 22.8m
            },

            new StandaloneItem
            {
                Name = "Miner Gauntlets T1 (Lv.36)",
                Slot = "Hand",
                Level = 36,
                DefenseFlat = 22,
                StrengthBonus = 36,
                IntellectBonus = 24,
                AttackPercent = 9.5m
            },
            new StandaloneItem
            {
                Name = "Miner Gloves T1 (Lv.36)",
                Slot = "Hand",
                Level = 36,
                DefenseFlat = 22,
                IntellectBonus = 36,
                AgilityBonus = 24,
                HPPercent = 19.0m
            }
        };

        public static StandaloneItem GetNoneItem()
        {
            return new StandaloneItem
            {
                Name = "None (No Standalone Item)",
                Slot = "N/A",
                Level = 0
            };
        }
    }
}