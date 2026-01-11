using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using AKEndfieldDmgCalc.Data;

namespace EndfieldCalculator
{
   
    public partial class DamageCalculatorForm
    {
        private void InitializeGearTab(TabPage tab)
        {
            tab.AutoScroll = true;
            int leftCol = 30, rightCol = 500, yPos = 30;

            // === GEAR SET SECTION ===
            AddLabel(tab, "=== GEAR SET (3-Piece Bonus) ===", leftCol, yPos, true);
            yPos += 35;

            AddLabel(tab, "Select Gear Set:", leftCol, yPos);
            cmbGearSet = new ComboBox
            {
                Location = new Point(leftCol + 150, yPos - 3),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            // Populate gear sets
            cmbGearSet.Items.Add(GearSetDatabase.GetNoneSet().Name);
            foreach (var set in GearSetDatabase.AllGearSets.OrderBy(s => s.Name))
            {
                cmbGearSet.Items.Add(set.Name);
            }
            cmbGearSet.SelectedIndex = 0;
            cmbGearSet.SelectedIndexChanged += CmbGearSet_SelectedIndexChanged;
            tab.Controls.Add(cmbGearSet);
            yPos += 40;

            // Gear Set Info Panel
            pnlGearInfo = new Panel
            {
                Location = new Point(leftCol, yPos),
                Size = new Size(430, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 248, 255),
                AutoScroll = true
            };

            lblGearSetInfo = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(405, 180),
                Font = new Font("Arial", 8.5f),
                Text = "Select a gear set to see bonuses.\n\nBonuses are applied when you click 'Calculate Damage'.",
                ForeColor = Color.DarkBlue
            };
            pnlGearInfo.Controls.Add(lblGearSetInfo);
            tab.Controls.Add(pnlGearInfo);
            yPos += 220;

            // === STANDALONE ITEM SECTION ===
            AddLabel(tab, "=== STANDALONE ITEM (4th Slot) ===", leftCol, yPos, true);
            yPos += 35;

            AddLabel(tab, "Select Item:", leftCol, yPos);
            cmbStandaloneItem = new ComboBox
            {
                Location = new Point(leftCol + 150, yPos - 3),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            // Populate standalone items
            cmbStandaloneItem.Items.Add(StandaloneItemDatabase.GetNoneItem().Name);
            foreach (var item in StandaloneItemDatabase.AllItems.OrderBy(i => i.Name))
            {
                cmbStandaloneItem.Items.Add(item.Name);
            }
            cmbStandaloneItem.SelectedIndex = 0;
            cmbStandaloneItem.SelectedIndexChanged += CmbStandaloneItem_SelectedIndexChanged;
            tab.Controls.Add(cmbStandaloneItem);
            yPos += 40;

            // Standalone Item Info Panel
            var pnlStandaloneInfo = new Panel
            {
                Location = new Point(leftCol, yPos),
                Size = new Size(430, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 250, 240),
                AutoScroll = true
            };

            lblStandaloneInfo = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(405, 130),
                Font = new Font("Arial", 8.5f),
                Text = "Select a standalone item for additional stats.\n\nUse this for your 4th equipment slot when not using a full set.",
                ForeColor = Color.DarkSlateGray
            };
            pnlStandaloneInfo.Controls.Add(lblStandaloneInfo);
            tab.Controls.Add(pnlStandaloneInfo);
            yPos += 170;

            // === WEAPON SECTION (PLACEHOLDER) ===
            yPos += 20;
            AddLabel(tab, "=== WEAPON (Coming Soon) ===", leftCol, yPos, true);
            yPos += 35;

            var lblWeaponPlaceholder = new Label
            {
                Location = new Point(leftCol, yPos),
                Size = new Size(430, 60),
                Font = new Font("Arial", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = "Weapon selection will be added here in a future update.\n\nFor now, enter weapon stats manually in the Damage Calculator tab."
            };
            tab.Controls.Add(lblWeaponPlaceholder);

            // === COMBINED BONUSES DISPLAY (RIGHT SIDE) ===
            yPos = 30;
            AddLabel(tab, "=== TOTAL GEAR BONUSES ===", rightCol, yPos, true);
            yPos += 35;

            var pnlTotalBonuses = new Panel
            {
                Location = new Point(rightCol, yPos),
                Size = new Size(380, 600),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 255, 250),
                AutoScroll = true
            };

            var lblTotalBonuses = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(355, 580),
                Font = new Font("Consolas", 9),
                ForeColor = Color.DarkGreen,
                Text = GenerateTotalBonusesDisplay()
            };
            lblTotalBonuses.Name = "lblTotalBonuses";
            pnlTotalBonuses.Controls.Add(lblTotalBonuses);
            tab.Controls.Add(pnlTotalBonuses);

            // Update total bonuses when gear changes
            cmbGearSet.SelectedIndexChanged += (s, e) => UpdateTotalBonusesDisplay(pnlTotalBonuses);
            cmbStandaloneItem.SelectedIndexChanged += (s, e) => UpdateTotalBonusesDisplay(pnlTotalBonuses);
        }

        private string GenerateTotalBonusesDisplay()
        {
            var bonuses = CalculateGearBonuses();

            if (selectedGearSet == null && selectedStandaloneItem == null)
            {
                return "No gear selected.\n\nSelect gear and standalone items to see total bonuses.";
            }

            var display = "These bonuses will be applied when\ncalculating damage:\n\n";
            display += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

            // Attack bonuses
            if (bonuses.AttackFlat > 0 || bonuses.AttackPercent > 0)
            {
                display += "【 ATTACK 】\n";
                if (bonuses.AttackFlat > 0)
                    display += $"  Flat ATK: +{bonuses.AttackFlat}\n";
                if (bonuses.AttackPercent > 0)
                    display += $"  ATK %: +{bonuses.AttackPercent}%\n";
                display += "\n";
            }

            // Crit bonuses
            if (bonuses.CritRate > 0 || bonuses.CritDamage > 0)
            {
                display += "【 CRITICAL 】\n";
                if (bonuses.CritRate > 0)
                    display += $"  Crit Rate: +{bonuses.CritRate}%\n";
                if (bonuses.CritDamage > 0)
                    display += $"  Crit Damage: +{bonuses.CritDamage}%\n";
                display += "\n";
            }

            // Damage bonuses
            if (bonuses.ElementalDamageBonus > 0 || bonuses.SkillDamageBonus > 0 ||
                bonuses.ComboSkillDamageBonus > 0 || bonuses.UltimateDamageBonus > 0 ||
                bonuses.NormalAttackDamageBonus > 0 || bonuses.AllDamageBonus > 0)
            {
                display += "【 DAMAGE BONUSES 】\n";
                if (bonuses.ElementalDamageBonus > 0)
                    display += $"  Elemental DMG: +{bonuses.ElementalDamageBonus}%\n";
                if (bonuses.SkillDamageBonus > 0)
                    display += $"  Skill DMG: +{bonuses.SkillDamageBonus}%\n";
                if (bonuses.ComboSkillDamageBonus > 0)
                    display += $"  Combo Skill DMG: +{bonuses.ComboSkillDamageBonus}%\n";
                if (bonuses.UltimateDamageBonus > 0)
                    display += $"  Ultimate DMG: +{bonuses.UltimateDamageBonus}%\n";
                if (bonuses.NormalAttackDamageBonus > 0)
                    display += $"  Normal Attack DMG: +{bonuses.NormalAttackDamageBonus}%\n";
                if (bonuses.AllDamageBonus > 0)
                    display += $"  All DMG: +{bonuses.AllDamageBonus}%\n";
                display += "\n";
            }

            // Defensive bonuses
            if (bonuses.DamageReduction > 0)
            {
                display += "【 DEFENSE 】\n";
                display += $"  DMG Reduction: +{bonuses.DamageReduction}%\n\n";
            }

            // Utility bonuses
            if (bonuses.ComboSkillEfficiency > 0 || bonuses.UltimateGain > 0)
            {
                display += "【 UTILITY 】\n";
                if (bonuses.ComboSkillEfficiency > 0)
                    display += $"  Combo Skill Efficiency: +{bonuses.ComboSkillEfficiency}%\n";
                if (bonuses.UltimateGain > 0)
                    display += $"  Ultimate Gain: +{bonuses.UltimateGain}%\n";
                display += "\n";
            }

            display += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

            // Show what's equipped
            display += "【 EQUIPPED 】\n";
            if (selectedGearSet != null)
                display += $"  Set: {selectedGearSet.Name}\n";
            if (selectedStandaloneItem != null)
                display += $"  Item: {selectedStandaloneItem.Name}\n";

            if (selectedGearSet == null && selectedStandaloneItem == null)
                display += "  Nothing equipped\n";

            return display;
        }

        private void UpdateTotalBonusesDisplay(Panel panel)
        {
            var label = panel.Controls.Find("lblTotalBonuses", false).FirstOrDefault() as Label;
            if (label != null)
            {
                label.Text = GenerateTotalBonusesDisplay();
            }
        }

        private void DisplayGearSetInfo(GearSet set)
        {
            var info = $"【 {set.Name} 】\n";
            info += $"━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
            info += $"Pieces: {set.PieceCount}\n\n";
            info += $"{set.Description}\n\n";

            // Show what bonuses will be applied
            info += "Set Bonuses:\n";
            var hasBonuses = false;

            if (set.AttackFlat > 0) { info += $"  • ATK +{set.AttackFlat}\n"; hasBonuses = true; }
            if (set.AttackPercent > 0) { info += $"  • ATK +{set.AttackPercent}%\n"; hasBonuses = true; }
            if (set.CritRate > 0) { info += $"  • Crit Rate +{set.CritRate}%\n"; hasBonuses = true; }
            if (set.CritDamage > 0) { info += $"  • Crit DMG +{set.CritDamage}%\n"; hasBonuses = true; }
            if (set.SkillDamageBonus > 0) { info += $"  • Skill DMG +{set.SkillDamageBonus}%\n"; hasBonuses = true; }
            if (set.PhysicalDamageBonus > 0) { info += $"  • Physical DMG +{set.PhysicalDamageBonus}%\n"; hasBonuses = true; }
            if (set.PyroDamageBonus > 0) { info += $"  • Pyro DMG +{set.PyroDamageBonus}%\n"; hasBonuses = true; }
            if (set.CryoDamageBonus > 0) { info += $"  • Cryo DMG +{set.CryoDamageBonus}%\n"; hasBonuses = true; }
            if (set.ElectromagneticDamageBonus > 0) { info += $"  • EM DMG +{set.ElectromagneticDamageBonus}%\n"; hasBonuses = true; }
            if (set.GeoDamageBonus > 0) { info += $"  • Geo DMG +{set.GeoDamageBonus}%\n"; hasBonuses = true; }
            if (set.ElementalDamageBonus > 0) { info += $"  • Elemental DMG +{set.ElementalDamageBonus}%\n"; hasBonuses = true; }
            if (set.AllDamageBonus > 0) { info += $"  • All DMG +{set.AllDamageBonus}%\n"; hasBonuses = true; }
            if (set.DamageReduction > 0) { info += $"  • DMG Reduction +{set.DamageReduction}%\n"; hasBonuses = true; }

            if (!hasBonuses)
            {
                info += "  (Conditional bonuses only)\n";
            }

            if (!string.IsNullOrEmpty(set.ConditionalEffect))
            {
                info += $"\n⚠️ Conditional Effect:\n{set.ConditionalEffect}";
            }

            lblGearSetInfo.Text = info;
        }

        private void DisplayStandaloneItemInfo(StandaloneItem item)
        {
            var info = $"【 {item.Name} 】\n";
            info += $"━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";
            info += $"Slot: {item.Slot} | Level: {item.Level}\n\n";

            info += "Stats:\n";
            var hasStats = false;

            if (item.DefenseFlat > 0) { info += $"  • DEF +{item.DefenseFlat}\n"; hasStats = true; }
            if (item.AttackFlat > 0) { info += $"  • ATK +{item.AttackFlat}\n"; hasStats = true; }
            if (item.AttackPercent > 0) { info += $"  • ATK +{item.AttackPercent}%\n"; hasStats = true; }
            if (item.CritRate > 0) { info += $"  • Crit Rate +{item.CritRate}%\n"; hasStats = true; }
            if (item.CritDamage > 0) { info += $"  • Crit DMG +{item.CritDamage}%\n"; hasStats = true; }
            if (item.DamageReduction > 0) { info += $"  • DMG Reduction +{item.DamageReduction}%\n"; hasStats = true; }
            if (item.ComboSkillEfficiency > 0) { info += $"  • Combo Efficiency +{item.ComboSkillEfficiency}%\n"; hasStats = true; }
            if (item.UltimateGain > 0) { info += $"  • Ultimate Gain +{item.UltimateGain}%\n"; hasStats = true; }

            // Display sub-stats
            if (item.StrengthBonus > 0) { info += $"  • Strength +{item.StrengthBonus}\n"; hasStats = true; }
            if (item.AgilityBonus > 0) { info += $"  • Agility +{item.AgilityBonus}\n"; hasStats = true; }
            if (item.IntellectBonus > 0) { info += $"  • Intellect +{item.IntellectBonus}\n"; hasStats = true; }
            if (item.WillBonus > 0) { info += $"  • Will +{item.WillBonus}\n"; hasStats = true; }
            if (item.HPPercent > 0) { info += $"  • HP +{item.HPPercent}%\n"; hasStats = true; }
            if (item.HealOutput > 0) { info += $"  • Heal Output +{item.HealOutput}%\n"; hasStats = true; }

            if (!hasStats)
            {
                info += "  No stats\n";
            }

            lblStandaloneInfo.Text = info;
        }
    }
}