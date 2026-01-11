using System;
using System.Windows.Forms;
using System.Drawing;
using AKEndfieldDmgCalc.Calculators;
using AKEndfieldDmgCalc.Data;
using AKEndfieldDmgCalc.Helpers;


namespace EndfieldCalculator
{

    /// Damage Calculator Tab initialization and logic

    public partial class DamageCalculatorForm
    {
        private void InitializeDamageTab(TabPage tab)
        {
            tab.AutoScroll = true;
            int leftCol = 20, rightCol = 450, yPos = 20;

            // Quick Build Selector
            AddLabel(tab, "Quick Load Build:", leftCol, yPos);
            cmbBuildProfiles = new ComboBox
            {
                Location = new Point(leftCol + 120, yPos - 3),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbBuildProfiles.SelectedIndexChanged += (s, e) =>
            {
                if (cmbBuildProfiles.SelectedIndex > 0)
                {
                    QuickLoadBuild();
                }
            };
            tab.Controls.Add(cmbBuildProfiles);
            yPos += 40;

            // Attacker Stats
            AddLabel(tab, "=== ATTACKER STATS ===", leftCol, yPos, true);
            yPos += 30;

            nudBaseAttack = AddNumeric(tab, "Operator Base Attack:", leftCol, ref yPos, 0, 10000, 100);
            nudWeaponAttack = AddNumeric(tab, "Weapon Base Attack:", leftCol, ref yPos, 0, 10000, 50);
            nudAttackPercent = AddNumeric(tab, "Attack % Bonus:", leftCol, ref yPos, 0, 500, 0, true);
            nudAttackFlat = AddNumeric(tab, "Attack Flat Bonus:", leftCol, ref yPos, 0, 5000, 0);
            nudPrimaryStat = AddNumeric(tab, "Primary Stat:", leftCol, ref yPos, 0, 2000, 100);
            nudSecondaryStat = AddNumeric(tab, "Secondary Stat:", leftCol, ref yPos, 0, 2000, 50);
            nudDamageMultiplier = AddNumeric(tab, "Damage Multiplier %:", leftCol, ref yPos, 0, 10000, 100, true);
            nudCritRate = AddNumeric(tab, "Critical Rate %:", leftCol, ref yPos, 0, 100, 5, true);
            nudCritDamage = AddNumeric(tab, "Critical Damage %:", leftCol, ref yPos, 0, 500, 50, true);
            nudLevel = AddNumeric(tab, "Operator Level:", leftCol, ref yPos, 1, 99, 99);
            nudSourceStoneArtistry = AddNumeric(tab, "Source Stone Artistry:", leftCol, ref yPos, 0, 1000, 0);

            yPos += 10;
            AddLabel(tab, "=== DAMAGE BONUSES ===", leftCol, yPos, true);
            yPos += 30;

            nudElementalBonus = AddNumeric(tab, "Elemental Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
            nudSkillBonus = AddNumeric(tab, "Skill Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
            nudUnbalanceBonus = AddNumeric(tab, "Unbalance Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
            nudOtherBonus = AddNumeric(tab, "Other Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
            


            // Target Stats
            yPos = 60;
            AddLabel(tab, "=== TARGET STATS ===", rightCol, yPos, true);
            yPos += 30;

            nudTargetDefense = AddNumeric(tab, "Target Defense:", rightCol, ref yPos, -1000, 5000, 100);
            nudTargetResistance = AddNumeric(tab, "Target Resistance:", rightCol, ref yPos, 0, 100, 20);

            AddLabel(tab, "Damage Type:", rightCol, yPos);
            cmbDamageType = new ComboBox
            {
                Location = new Point(rightCol + 180, yPos - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbDamageType.Items.AddRange(new object[] {
                "Physical", "Pyro", "Electromagnetic", "Cryo", "Geo", "Transcendent", "True Damage"
            });
            cmbDamageType.SelectedIndex = 0;
            tab.Controls.Add(cmbDamageType);
            yPos += 35;

            chkIsUnbalanced = new CheckBox
            {
                Text = "Target is Unbalanced (1.3x)",
                Location = new Point(rightCol, yPos),
                Width = 280,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(chkIsUnbalanced);
            yPos += 30;

            chkIsCritical = new CheckBox
            {
                Text = "Force Critical Hit",
                Location = new Point(rightCol, yPos),
                Width = 200,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(chkIsCritical);
            yPos += 30;

            chkIsTrueDamage = new CheckBox
            {
                Text = "True Damage",
                Location = new Point(rightCol, yPos),
                Width = 200,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(chkIsTrueDamage);
            yPos += 35;

            AddLabel(tab, "Anomaly Type:", rightCol, yPos);
            cmbAnomalyType = new ComboBox
            {
                Location = new Point(rightCol + 180, yPos - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbAnomalyType.Items.AddRange(new object[] {
                "None", "Knockback", "Knockdown", "Armor Shatter", "Smash",
                "Electrification", "Corrosion", "Burning", "Freeze", "Shatter Ice", "Spell Burst"
            });
            cmbAnomalyType.SelectedIndex = 0;
            tab.Controls.Add(cmbAnomalyType);
            yPos += 35;

            nudAnomalyLevel = AddNumeric(tab, "Anomaly Level (1-4):", rightCol, ref yPos, 1, 4, 1);

            yPos += 10;
            AddLabel(tab, "=== MULTIPLIER ZONES ===", rightCol, yPos, true);
            yPos += 30;

            nudVulnerability = AddNumeric(tab, "Vulnerability %:", rightCol, ref yPos, 0, 200, 0, true);
            nudAmplification = AddNumeric(tab, "Amplification %:", rightCol, ref yPos, 0, 200, 0, true);
            nudSanctuary = AddNumeric(tab, "Sanctuary %:", rightCol, ref yPos, 0, 100, 0, true);
            nudFragility = AddNumeric(tab, "Fragility %:", rightCol, ref yPos, 0, 200, 0, true);
            nudDamageReduction = AddNumeric(tab, "Damage Reduction %:", rightCol, ref yPos, 0, 100, 0, true);
            nudSpecialMultiplier = AddNumeric(tab, "Special Multiplier %:", rightCol, ref yPos, 0, 200, 0, true);

            // Calculate Button 
            int buttonY = 650;
            btnCalculate = new Button
            {
                Text = "Calculate Damage",
                Location = new Point(leftCol, buttonY),
                Size = new Size(200, 40),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            btnCalculate.Click += BtnCalculate_Click;
            tab.Controls.Add(btnCalculate);

            // Results - aligned next to button
            int resultsY = buttonY;
            AddLabel(tab, "=== RESULTS ===", rightCol, resultsY, true);
            resultsY += 35;

            txtFinalAttack = AddResult(tab, "Final Attack:", rightCol, ref resultsY, 180);
            txtBaseDamage = AddResult(tab, "Base Damage:", rightCol, ref resultsY, 180);
            txtFinalDamage = AddResult(tab, "Final Damage:", rightCol, ref resultsY, 180);

            // Damage Range section
            int rangeY = buttonY + 50;
            AddLabel(tab, "=== DAMAGE RANGE ===", leftCol, rangeY, true);
            rangeY += 30;

            txtMinDamage = AddResult(tab, "Min (Non-Crit):", leftCol, ref rangeY, 180);
            txtMaxDamage = AddResult(tab, "Max (Crit):", leftCol, ref rangeY, 180);
            txtAvgDamage = AddResult(tab, "Average:", leftCol, ref rangeY, 180);
            txtCritChance = AddResult(tab, "Crit Chance:", leftCol, ref rangeY, 180);

            // Breakdown
            int breakdownY = Math.Max(rangeY + 20, resultsY + 20);
            AddLabel(tab, "Breakdown:", leftCol, breakdownY);
            txtBreakdown = new TextBox
            {
                Location = new Point(leftCol, breakdownY + 25),
                Size = new Size(840, 150),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8)
            };
            tab.Controls.Add(txtBreakdown);
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                
                var gearBonuses = CalculateGearBonuses();

                var result = DamageCalculator.Calculate(
                    // Attacker Stats (with gear bonuses applied)
                    (double)nudBaseAttack.Value,
                    (double)nudWeaponAttack.Value,
                    (double)nudAttackPercent.Value + gearBonuses.AttackPercent,  
                    (double)nudAttackFlat.Value + gearBonuses.AttackFlat,      
                    (double)nudPrimaryStat.Value,
                    (double)nudSecondaryStat.Value,
                    (double)nudDamageMultiplier.Value,
                    (double)nudCritRate.Value + gearBonuses.CritRate,            
                    (double)nudCritDamage.Value + gearBonuses.CritDamage,        
                    (double)nudLevel.Value,
                    (double)nudSourceStoneArtistry.Value,
                    // Bonuses (with gear bonuses applied)
                    (double)nudElementalBonus.Value + gearBonuses.ElementalDamageBonus, 
                    (double)nudSkillBonus.Value + gearBonuses.SkillDamageBonus + gearBonuses.AllDamageBonus,  
                    (double)nudUnbalanceBonus.Value,
                    (double)nudOtherBonus.Value,
                    // Target
                    (double)nudTargetDefense.Value,
                    (double)nudTargetResistance.Value,
                    chkIsUnbalanced.Checked,
                    chkIsTrueDamage.Checked,
                    // Anomaly
                    cmbAnomalyType.SelectedItem?.ToString() ?? "None",
                    (int)nudAnomalyLevel.Value,
                    // Multipliers
                    (double)nudVulnerability.Value,
                    (double)nudAmplification.Value,
                    (double)nudSanctuary.Value,
                    (double)nudFragility.Value,
                    (double)nudDamageReduction.Value + gearBonuses.DamageReduction,  
                    (double)nudSpecialMultiplier.Value,
                    
                    chkIsCritical.Checked
                );

                // Display results
                txtFinalAttack.Text = Math.Floor(result.FinalAttack).ToString("F0");
                txtBaseDamage.Text = Math.Floor(result.BaseDamage).ToString("F0");
                txtFinalDamage.Text = Math.Floor(result.FinalDamage).ToString("F0") +
                                     (chkIsCritical.Checked ? " (CRIT!)" : "");

                if (txtMinDamage != null) txtMinDamage.Text = Math.Floor(result.MinDamage).ToString("F0");
                if (txtMaxDamage != null) txtMaxDamage.Text = Math.Floor(result.MaxDamage).ToString("F0");
                if (txtAvgDamage != null) txtAvgDamage.Text = Math.Floor(result.AverageDamage).ToString("F0");
                if (txtCritChance != null) txtCritChance.Text = $"{nudCritRate.Value + (decimal)gearBonuses.CritRate}%";

                // Enhanced breakdown showing gear bonuses
                string gearInfo = "";
                if (gearBonuses.AttackPercent > 0 || gearBonuses.AttackFlat > 0 ||
                    gearBonuses.ElementalDamageBonus > 0 || gearBonuses.SkillDamageBonus > 0)
                {
                    gearInfo = "\nGear Bonuses Applied:\n";
                    if (gearBonuses.AttackFlat > 0) gearInfo += $"  +{gearBonuses.AttackFlat} Flat ATK\n";
                    if (gearBonuses.AttackPercent > 0) gearInfo += $"  +{gearBonuses.AttackPercent}% ATK\n";
                    if (gearBonuses.CritRate > 0) gearInfo += $"  +{gearBonuses.CritRate}% Crit Rate\n";
                    if (gearBonuses.CritDamage > 0) gearInfo += $"  +{gearBonuses.CritDamage}% Crit Damage\n";
                    if (gearBonuses.ElementalDamageBonus > 0) gearInfo += $"  +{gearBonuses.ElementalDamageBonus}% Elemental DMG\n";
                    if (gearBonuses.SkillDamageBonus > 0) gearInfo += $"  +{gearBonuses.SkillDamageBonus}% Skill DMG\n";
                    if (gearBonuses.AllDamageBonus > 0) gearInfo += $"  +{gearBonuses.AllDamageBonus}% All DMG\n";
                    if (gearBonuses.DamageReduction > 0) gearInfo += $"  +{gearBonuses.DamageReduction}% DMG Reduction\n";
                }

                txtBreakdown.Text = gearInfo + "\n" + result.Breakdown;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}