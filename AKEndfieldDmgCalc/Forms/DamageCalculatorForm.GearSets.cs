using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using AKEndfieldDmgCalc.Data;

namespace EndfieldCalculator
{
 
    public partial class DamageCalculatorForm
    {
       
        public GearSet GetSelectedGearSet()
        {
            return selectedGearSet;
        }

        public StandaloneItem GetSelectedStandaloneItem()
        {
            return selectedStandaloneItem;
        }

        
        public GearBonuses CalculateGearBonuses()
        {
            var bonuses = new GearBonuses();
            string currentDamageType = cmbDamageType.SelectedItem?.ToString() ?? "Physical";

            // Apply gear set bonuses
            if (selectedGearSet != null)
            {
                ApplyGearSetBonuses(selectedGearSet, currentDamageType, ref bonuses);
            }

            // Apply standalone item bonuses
            if (selectedStandaloneItem != null)
            {
                ApplyStandaloneItemBonuses(selectedStandaloneItem, ref bonuses);
            }

            return bonuses;
        }

        private void ApplyGearSetBonuses(GearSet set, string damageType, ref GearBonuses bonuses)
        {
            bonuses.AttackFlat += (double)set.AttackFlat;
            bonuses.AttackPercent += (double)set.AttackPercent;
            bonuses.CritRate += (double)set.CritRate;
            bonuses.CritDamage += (double)set.CritDamage;
            bonuses.DamageReduction += (double)set.DamageReduction;

            // Apply skill damage bonus
            bonuses.SkillDamageBonus += (double)set.SkillDamageBonus;
            bonuses.ComboSkillDamageBonus += (double)set.ComboSkillDamageBonus;
            bonuses.UltimateDamageBonus += (double)set.UltimateDamageBonus;
            bonuses.NormalAttackDamageBonus += (double)set.NormalAttackDamageBonus;

            // Apply physical damage if using physical
            if (damageType == "Physical")
            {
                bonuses.ElementalDamageBonus += (double)set.PhysicalDamageBonus;
            }

            // Apply element-specific bonuses if they match
            switch (damageType)
            {
                case "Pyro":
                    bonuses.ElementalDamageBonus += (double)set.PyroDamageBonus;
                    break;
                case "Cryo":
                    bonuses.ElementalDamageBonus += (double)set.CryoDamageBonus;
                    break;
                case "Electromagnetic":
                    bonuses.ElementalDamageBonus += (double)set.ElectromagneticDamageBonus;
                    break;
                case "Geo":
                    bonuses.ElementalDamageBonus += (double)set.GeoDamageBonus;
                    break;
            }

            // General elemental bonus applies to all elemental damage
            if (damageType != "Physical" && damageType != "True Damage")
            {
                bonuses.ElementalDamageBonus += (double)set.ElementalDamageBonus;
            }

            // All damage bonus always applies
            bonuses.AllDamageBonus += (double)set.AllDamageBonus;
        }

        private void ApplyStandaloneItemBonuses(StandaloneItem item, ref GearBonuses bonuses)
        {
            bonuses.AttackFlat += (double)item.AttackFlat;
            bonuses.AttackPercent += (double)item.AttackPercent;
            bonuses.CritRate += (double)item.CritRate;
            bonuses.CritDamage += (double)item.CritDamage;
            bonuses.DamageReduction += (double)item.DamageReduction;
            bonuses.ComboSkillEfficiency += (double)item.ComboSkillEfficiency;
            bonuses.UltimateGain += (double)item.UltimateGain;
        }

        private void CmbGearSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGearSet.SelectedIndex <= 0)
            {
                selectedGearSet = null;
                lblGearSetInfo.Text = "No gear set selected.\n\nBonuses will not be applied.";
                return;
            }

            string selectedName = cmbGearSet.SelectedItem.ToString();
            selectedGearSet = GearSetDatabase.AllGearSets.FirstOrDefault(s => s.Name == selectedName);

            if (selectedGearSet != null)
            {
                DisplayGearSetInfo(selectedGearSet);
            }
        }

        private void CmbStandaloneItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStandaloneItem.SelectedIndex <= 0)
            {
                selectedStandaloneItem = null;
                if (lblStandaloneInfo != null)
                    lblStandaloneInfo.Text = "No standalone item selected.";
                return;
            }

            string selectedName = cmbStandaloneItem.SelectedItem.ToString();
            selectedStandaloneItem = StandaloneItemDatabase.AllItems.FirstOrDefault(i => i.Name == selectedName);

            if (selectedStandaloneItem != null)
            {
                DisplayStandaloneItemInfo(selectedStandaloneItem);
            }
        }
    }

   
    public class GearBonuses
    {
        public double AttackFlat { get; set; }
        public double AttackPercent { get; set; }
        public double CritRate { get; set; }
        public double CritDamage { get; set; }
        public double ElementalDamageBonus { get; set; }
        public double SkillDamageBonus { get; set; }
        public double ComboSkillDamageBonus { get; set; }
        public double UltimateDamageBonus { get; set; }
        public double NormalAttackDamageBonus { get; set; }
        public double AllDamageBonus { get; set; }
        public double DamageReduction { get; set; }
        public double ComboSkillEfficiency { get; set; }
        public double UltimateGain { get; set; }
    }
}