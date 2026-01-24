using System;
using System.Windows.Forms;
using System.Drawing;
using AKEndfieldDmgCalc.Calculators;
using AKEndfieldDmgCalc.Data;
using AKEndfieldDmgCalc.Helpers;

namespace EndfieldCalculator
{

    public partial class DamageCalculatorForm
    {
        private CollapsibleSectionManager sectionManagerLeft;
        private CollapsibleSectionManager sectionManagerRight;


        private ComboBox cmbPrimaryStatType;
        private ComboBox cmbSecondaryStatType;

        // Staggered controls
        private CheckBox chkStaggered;
        private NumericUpDown nudStaggeredBonus;

        private void InitializeDamageTab(TabPage tab)
        {
            tab.AutoScroll = true;
            int leftCol = 20, rightCol = 470, yPos = 20;

            sectionManagerLeft = new CollapsibleSectionManager();
            sectionManagerRight = new CollapsibleSectionManager();

            // Quick Build Selector at top
            AddLabel(tab, "Quick Load Build:", leftCol, yPos);
            cmbBuildProfiles = new ComboBox
            {
                Location = new Point(leftCol + 120, yPos - 3),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbBuildProfiles.SelectedIndexChanged += (s, e) => {
                if (cmbBuildProfiles.SelectedIndex > 0)
                {
                    QuickLoadBuild();
                }
            };
            tab.Controls.Add(cmbBuildProfiles);

            // Calculate and Expand/Collapse buttons
            btnCalculate = new Button
            {
                Text = "Calculate",
                Location = new Point(rightCol, yPos - 3),
                Size = new Size(100, 30),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            btnCalculate.Click += BtnCalculate_Click;
            tab.Controls.Add(btnCalculate);

            var btnExpandAll = new Button
            {
                Text = "Expand All",
                Location = new Point(rightCol + 110, yPos - 3),
                Size = new Size(100, 30),
                Font = new Font("Arial", 8)
            };
            btnExpandAll.Click += (s, e) => {
                sectionManagerLeft.ExpandAll();
                sectionManagerRight.ExpandAll();
                RepositionButtonsAndResults(tab);
            };
            tab.Controls.Add(btnExpandAll);

            var btnCollapseAll = new Button
            {
                Text = "Collapse All",
                Location = new Point(rightCol + 220, yPos - 3),
                Size = new Size(100, 30),
                Font = new Font("Arial", 8)
            };
            btnCollapseAll.Click += (s, e) => {
                sectionManagerLeft.CollapseAll();
                sectionManagerRight.CollapseAll();
                RepositionButtonsAndResults(tab);
            };
            tab.Controls.Add(btnCollapseAll);

            yPos += 40;

            // === LEFT COLUMN ===

            // === ATTACKER STATS SECTION (Collapsible) ===
            var attackerSection = new CollapsibleSection(tab, "ATTACKER STATS", leftCol, yPos, 420, Color.FromArgb(230, 240, 255), () => RepositionButtonsAndResults(tab));
            int innerY = 10;

            attackerSection.AddControls(
                CreateNumericWithLabel("Operator Base Attack:", leftCol + 10, ref innerY, 0, 10000, 100, out nudBaseAttack),
                CreateNumericWithLabel("Weapon Base Attack:", leftCol + 10, ref innerY, 0, 10000, 50, out nudWeaponAttack),
                CreateNumericWithLabel("Attack % Bonus:", leftCol + 10, ref innerY, 0, 500, 0, out nudAttackPercent, 2),
                CreateNumericWithLabel("Attack Flat Bonus:", leftCol + 10, ref innerY, 0, 5000, 0, out nudAttackFlat, 2)
            );

            attackerSection.AddControl(CreateStatWithDropdown("Primary Stat:", leftCol + 10, ref innerY, 0, 2000, 100, out nudPrimaryStat, out cmbPrimaryStatType));

            attackerSection.AddControl(CreateStatWithDropdown("Secondary Stat:", leftCol + 10, ref innerY, 0, 2000, 50, out nudSecondaryStat, out cmbSecondaryStatType));

            attackerSection.AddControls(
                CreateNumericWithLabel("Damage Multiplier %:", leftCol + 10, ref innerY, 0, 10000, 100, out nudDamageMultiplier, 2),
                CreateNumericWithLabel("Critical Rate %:", leftCol + 10, ref innerY, 0, 100, 5, out nudCritRate, 2),
                CreateNumericWithLabel("Critical Damage %:", leftCol + 10, ref innerY, 0, 500, 50, out nudCritDamage, 2),
                CreateNumericWithLabel("Source Stone Artistry:", leftCol + 10, ref innerY, 0, 1000, 0, out nudSourceStoneArtistry, 2)
            );
            sectionManagerLeft.AddSection(attackerSection);

            // === DAMAGE BONUSES SECTION (Collapsible) ===
            var bonusesSection = new CollapsibleSection(tab, "DAMAGE BONUSES", leftCol, attackerSection.GetBottom() + 10, 420, Color.FromArgb(255, 240, 230), () => RepositionButtonsAndResults(tab));
            innerY = 10;

            bonusesSection.AddControls(
                CreateNumericWithLabel("Elemental Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudElementalBonus, 2),
                CreateNumericWithLabel("Skill Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudSkillBonus, 2),
                CreateNumericWithLabel("Staggered Damage Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudStaggeredBonus, 2),
                CreateNumericWithLabel("Other Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudOtherBonus, 2)
            );

            sectionManagerLeft.AddSection(bonusesSection);

            // === RIGHT COLUMN ===

            // === TARGET STATS SECTION (Collapsible) ===
            var targetSection = new CollapsibleSection(tab, "TARGET STATS", rightCol, 60, 420, Color.FromArgb(255, 230, 230), () => RepositionButtonsAndResults(tab));
            innerY = 10;

            targetSection.AddControl(CreateNumericWithLabel("Target Defense:", rightCol + 10, ref innerY, -1000, 5000, 100, out nudTargetDefense, 2));
            targetSection.AddControl(CreateNumericWithLabel("Target Resistance:", rightCol + 10, ref innerY, 0, 100, 20, out nudTargetResistance, 2));

            // Damage Type dropdown
            var lblDmgType = new Label { Text = "Damage Type:", Location = new Point(10, innerY), AutoSize = true, Font = new Font("Arial", 9) };
            cmbDamageType = new ComboBox
            {
                Location = new Point(180, innerY - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbDamageType.Items.AddRange(new object[] {
                "Physical", "Pyro", "Electromagnetic", "Cryo", "Geo", "Transcendent", "True Damage"
            });
            cmbDamageType.SelectedIndex = 0;
            targetSection.AddControl(lblDmgType);
            targetSection.AddControl(cmbDamageType);
            innerY += 35;

            chkIsCritical = new CheckBox
            {
                Text = "Force Critical Hit",
                Location = new Point(10, innerY),
                Width = 200,
                Font = new Font("Arial", 9)
            };
            targetSection.AddControl(chkIsCritical);
            innerY += 30;

            chkIsTrueDamage = new CheckBox
            {
                Text = "True Damage",
                Location = new Point(10, innerY),
                Width = 200,
                Font = new Font("Arial", 9)
            };
            targetSection.AddControl(chkIsTrueDamage);
            innerY += 35;

            sectionManagerRight.AddSection(targetSection);

            var anomalySection = new CollapsibleSection(tab, "ANOMALY", rightCol, targetSection.GetBottom() + 10, 420, Color.FromArgb(240, 255, 240), () => RepositionButtonsAndResults(tab));
            innerY = 10;

            var lblAnomType = new Label { Text = "Anomaly Type:", Location = new Point(10, innerY), AutoSize = true, Font = new Font("Arial", 9) };
            cmbAnomalyType = new ComboBox
            {
                Location = new Point(180, innerY - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmbAnomalyType.Items.AddRange(new object[] {
                "None", "Knockback", "Knockdown", "Armor Shatter", "Smash",
                "Electrification", "Corrosion", "Burning", "Freeze", "Shatter Ice", "Spell Burst"
            });
            cmbAnomalyType.SelectedIndex = 0;
            cmbAnomalyType.SelectedIndexChanged += (s, e) => {

                string anomType = cmbAnomalyType.SelectedItem?.ToString() ?? "None";
                if (anomType != "None" && !anomType.Contains("Knock") &&
                    anomType != "Armor Shatter" && anomType != "Smash")
                {
                    nudLevel.Value = 99;
                }
            };
            anomalySection.AddControl(lblAnomType);
            anomalySection.AddControl(cmbAnomalyType);
            innerY += 35;

            anomalySection.AddControl(CreateNumericWithLabel("Anomaly Level (1-4):", 10, ref innerY, 1, 4, 1, out nudAnomalyLevel));


            nudLevel = new NumericUpDown { Minimum = 1, Maximum = 99, Value = 99, Visible = false };
            anomalySection.AddControl(nudLevel);

            sectionManagerRight.AddSection(anomalySection);


            var multipliersSection = new CollapsibleSection(tab, "MULTIPLIER ZONES", rightCol, anomalySection.GetBottom() + 10, 420, Color.FromArgb(255, 250, 230), () => RepositionButtonsAndResults(tab));
            innerY = 10;

            multipliersSection.AddControls(
                CreateNumericWithLabel("Vulnerability %:", 10, ref innerY, 0, 200, 0, out nudVulnerability, 2),
                CreateNumericWithLabel("Amplification %:", 10, ref innerY, 0, 200, 0, out nudAmplification, 2),
                CreateNumericWithLabel("Sanctuary %:", 10, ref innerY, 0, 100, 0, out nudSanctuary, 2),
                CreateNumericWithLabel("Fragility %:", 10, ref innerY, 0, 200, 0, out nudFragility, 2),
                CreateNumericWithLabel("Damage Reduction %:", 10, ref innerY, 0, 100, 0, out nudDamageReduction, 2),
                CreateNumericWithLabel("Special Multiplier %:", 10, ref innerY, 0, 200, 0, out nudSpecialMultiplier, 2)
            );

            // Add Staggered checkbox to Multiplier Zones
            chkStaggered = new CheckBox
            {
                Text = "Target is Staggered (+30% DMG)",
                Location = new Point(10, innerY),
                Width = 300,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkOrange
            };
            multipliersSection.AddControl(chkStaggered);
            innerY += 30;

            sectionManagerRight.AddSection(multipliersSection);


            int resultsY = multipliersSection.GetBottom() + 20;
            var resultsSection = new CollapsibleSection(tab, "RESULTS", rightCol, resultsY, 420, Color.FromArgb(230, 255, 230));
            int resultInnerY = 10;

            var pnlResults = new Panel
            {
                Location = new Point(10, resultInnerY),
                Size = new Size(400, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            var lblFinalAtk = new Label { Text = "Final Attack:", Location = new Point(0, 5), AutoSize = true, Font = new Font("Arial", 9) };
            txtFinalAttack = new TextBox
            {
                Location = new Point(180, 0),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.LightYellow,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlResults.Controls.Add(lblFinalAtk);
            pnlResults.Controls.Add(txtFinalAttack);

            var lblBaseDmg = new Label { Text = "Base Damage:", Location = new Point(0, 40), AutoSize = true, Font = new Font("Arial", 9) };
            txtBaseDamage = new TextBox
            {
                Location = new Point(180, 35),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.LightYellow,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlResults.Controls.Add(lblBaseDmg);
            pnlResults.Controls.Add(txtBaseDamage);

            var lblFinalDmg = new Label { Text = "Final Damage:", Location = new Point(0, 75), AutoSize = true, Font = new Font("Arial", 9) };
            txtFinalDamage = new TextBox
            {
                Location = new Point(180, 70),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.LightYellow,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlResults.Controls.Add(lblFinalDmg);
            pnlResults.Controls.Add(txtFinalDamage);

            resultsSection.AddControl(pnlResults);

            var rangeSection = new CollapsibleSection(tab, "DAMAGE RANGE", leftCol, bonusesSection.GetBottom() + 20, 420, Color.FromArgb(255, 240, 255));
            int rangeInnerY = 10;

            var pnlRange = new Panel
            {
                Location = new Point(10, rangeInnerY),
                Size = new Size(400, 155),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            var lblMinDmg = new Label { Text = "Min (Non-Crit):", Location = new Point(0, 5), AutoSize = true, Font = new Font("Arial", 9) };
            txtMinDamage = new TextBox
            {
                Location = new Point(180, 0),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.FromArgb(255, 235, 235),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlRange.Controls.Add(lblMinDmg);
            pnlRange.Controls.Add(txtMinDamage);

            var lblMaxDmg = new Label { Text = "Max (Crit):", Location = new Point(0, 40), AutoSize = true, Font = new Font("Arial", 9) };
            txtMaxDamage = new TextBox
            {
                Location = new Point(180, 35),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.FromArgb(235, 255, 235),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlRange.Controls.Add(lblMaxDmg);
            pnlRange.Controls.Add(txtMaxDamage);

            var lblAvgDmg = new Label { Text = "Average:", Location = new Point(0, 75), AutoSize = true, Font = new Font("Arial", 9) };
            txtAvgDamage = new TextBox
            {
                Location = new Point(180, 70),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.FromArgb(235, 235, 255),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlRange.Controls.Add(lblAvgDmg);
            pnlRange.Controls.Add(txtAvgDamage);

            var lblCritChance = new Label { Text = "Crit Chance:", Location = new Point(0, 110), AutoSize = true, Font = new Font("Arial", 9) };
            txtCritChance = new TextBox
            {
                Location = new Point(180, 105),
                Width = 180,
                ReadOnly = true,
                BackColor = Color.FromArgb(255, 255, 235),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            pnlRange.Controls.Add(lblCritChance);
            pnlRange.Controls.Add(txtCritChance);

            rangeSection.AddControl(pnlRange);


            int initialBreakdownY = Math.Max(rangeSection.GetBottom(), resultsSection.GetBottom()) + 20;
            var lblBreakdown = new Label
            {
                Text = "Breakdown:",
                Location = new Point(leftCol, initialBreakdownY),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            lblBreakdown.Name = "lblBreakdown";
            tab.Controls.Add(lblBreakdown);

            txtBreakdown = new TextBox
            {
                Location = new Point(leftCol, initialBreakdownY + 25),
                Size = new Size(870, 150),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            txtBreakdown.Name = "txtBreakdown";
            tab.Controls.Add(txtBreakdown);

            tab.Tag = new
            {
                btnCalculate,
                btnExpandAll,
                btnCollapseAll,
                resultsSection,
                rangeSection,
                lblBreakdown,
                txtBreakdown,
                attackerSection,
                bonusesSection,
                targetSection,
                anomalySection,
                multipliersSection
            };

            // Do initial positioning
            RepositionButtonsAndResults(tab);
        }


        private void RepositionButtonsAndResults(TabPage tab)
        {
            var tag = tab.Tag as dynamic;
            if (tag == null) return;

            int leftColBottom = tag.bonusesSection.GetBottom();
            int rangeY = leftColBottom + 20;


            int rightColBottom = tag.multipliersSection.GetBottom();
            int resultsY = rightColBottom + 20;

            tag.rangeSection.SetY(rangeY);


            tag.resultsSection.SetY(resultsY);

            // Reposition breakdown
            int leftBottom = tag.rangeSection.GetBottom();
            int rightBottom = tag.resultsSection.GetBottom();
            int newBreakdownY = Math.Max(leftBottom, rightBottom) + 20;

            tag.lblBreakdown.Location = new Point(tag.lblBreakdown.Location.X, newBreakdownY);
            tag.txtBreakdown.Location = new Point(tag.txtBreakdown.Location.X, newBreakdownY + 25);
        }


        private Control CreateStatWithDropdown(string labelText, int x, ref int y, decimal min, decimal max, decimal val, out NumericUpDown nud, out ComboBox cmb)
        {
            var panel = new Panel { Location = new Point(0, y), Size = new Size(400, 35), BackColor = Color.Transparent };

            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(0, 5),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };

            nud = new NumericUpDown
            {
                Location = new Point(180, 0),
                Width = 90,
                Minimum = min,
                Maximum = max,
                Value = val,
                DecimalPlaces = 2,
                Font = new Font("Arial", 9)
            };

            cmb = new ComboBox
            {
                Location = new Point(280, 0),
                Width = 60,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            cmb.Items.AddRange(new object[] { "STR", "AGL", "INT", "WIL" });
            cmb.SelectedIndex = 0;

            panel.Controls.Add(lbl);
            panel.Controls.Add(nud);
            panel.Controls.Add(cmb);
            y += 35;

            return panel;
        }


        private Control CreateNumericWithLabel(string labelText, int x, ref int y, decimal min, decimal max, decimal val, out NumericUpDown nud, int decimalPlaces = 0)
        {
            var panel = new Panel { Location = new Point(0, y), Size = new Size(400, 35), BackColor = Color.Transparent };

            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(0, 5),
                AutoSize = true,
                Font = new Font("Arial", 9)
            };

            nud = new NumericUpDown
            {
                Location = new Point(180, 0),
                Width = 90,
                Minimum = min,
                Maximum = max,
                Value = val,
                DecimalPlaces = decimalPlaces,
                Font = new Font("Arial", 9)
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(nud);
            y += 35;

            return panel;
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                var gearBonuses = CalculateGearBonuses();

                // Staggered provides:
                // 1. Numeric bonus from nudStaggeredBonus
                // 2. Flat +30% from checkbox if checked
                double baseStaggeredBonus = chkStaggered.Checked ? 30.0 : 0.0;
                double totalStaggeredBonus = baseStaggeredBonus + (double)nudStaggeredBonus.Value;

                var result = DamageCalculator.Calculate(
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
                    (double)nudElementalBonus.Value + gearBonuses.ElementalDamageBonus,
                    (double)nudSkillBonus.Value + gearBonuses.SkillDamageBonus + gearBonuses.AllDamageBonus,
                    totalStaggeredBonus, // Total staggered damage bonus
                    (double)nudOtherBonus.Value,
                    (double)nudTargetDefense.Value,
                    (double)nudTargetResistance.Value,
                    false, // isUnbalanced removed - no longer used
                    chkIsTrueDamage.Checked,
                    cmbAnomalyType.SelectedItem?.ToString() ?? "None",
                    (int)nudAnomalyLevel.Value,
                    (double)nudVulnerability.Value,
                    (double)nudAmplification.Value,
                    (double)nudSanctuary.Value,
                    (double)nudFragility.Value,
                    (double)nudDamageReduction.Value + gearBonuses.DamageReduction,
                    (double)nudSpecialMultiplier.Value,
                    chkIsCritical.Checked
                );

                // Display results with 2 decimal places
                txtFinalAttack.Text = result.FinalAttack.ToString("F2");
                txtBaseDamage.Text = result.BaseDamage.ToString("F2");
                txtFinalDamage.Text = result.FinalDamage.ToString("F2") +
                                     (chkIsCritical.Checked ? " (CRIT!)" : "");

                if (txtMinDamage != null) txtMinDamage.Text = result.MinDamage.ToString("F2");
                if (txtMaxDamage != null) txtMaxDamage.Text = result.MaxDamage.ToString("F2");
                if (txtAvgDamage != null) txtAvgDamage.Text = result.AverageDamage.ToString("F2");
                if (txtCritChance != null) txtCritChance.Text = $"{nudCritRate.Value + (decimal)gearBonuses.CritRate:F2}%";

                // Breakdown with stat type info
                string gearInfo = "";
                if (gearBonuses.AttackPercent > 0 || gearBonuses.AttackFlat > 0 ||
                    gearBonuses.ElementalDamageBonus > 0 || gearBonuses.SkillDamageBonus > 0)
                {
                    gearInfo = "\nGear Bonuses Applied:\n";
                    if (gearBonuses.AttackFlat > 0) gearInfo += $"  +{gearBonuses.AttackFlat:F2} Flat ATK\n";
                    if (gearBonuses.AttackPercent > 0) gearInfo += $"  +{gearBonuses.AttackPercent:F2}% ATK\n";
                    if (gearBonuses.CritRate > 0) gearInfo += $"  +{gearBonuses.CritRate:F2}% Crit Rate\n";
                    if (gearBonuses.CritDamage > 0) gearInfo += $"  +{gearBonuses.CritDamage:F2}% Crit Damage\n";
                    if (gearBonuses.ElementalDamageBonus > 0) gearInfo += $"  +{gearBonuses.ElementalDamageBonus:F2}% Elemental DMG\n";
                    if (gearBonuses.SkillDamageBonus > 0) gearInfo += $"  +{gearBonuses.SkillDamageBonus:F2}% Skill DMG\n";
                    if (gearBonuses.AllDamageBonus > 0) gearInfo += $"  +{gearBonuses.AllDamageBonus:F2}% All DMG\n";
                    if (gearBonuses.DamageReduction > 0) gearInfo += $"  +{gearBonuses.DamageReduction:F2}% DMG Reduction\n";
                }

                // Add stat type info
                string statInfo = $"\nStat Configuration:\n";
                statInfo += $"  Primary: {nudPrimaryStat.Value:F2} ({cmbPrimaryStatType.SelectedItem})\n";
                statInfo += $"  Secondary: {nudSecondaryStat.Value:F2} ({cmbSecondaryStatType.SelectedItem})\n";

                // Add staggered info
                if (chkStaggered.Checked || nudStaggeredBonus.Value > 0)
                {
                    statInfo += $"\nStaggered Bonuses:\n";
                    if (chkStaggered.Checked)
                        statInfo += $"  Target is Staggered: +30% DMG\n";
                    if (nudStaggeredBonus.Value > 0)
                        statInfo += $"  Staggered Damage Bonus: +{nudStaggeredBonus.Value:F2}%\n";
                    if (chkStaggered.Checked || nudStaggeredBonus.Value > 0)
                        statInfo += $"  Total Staggered Bonus: +{totalStaggeredBonus:F2}%\n";
                }

                txtBreakdown.Text = statInfo + gearInfo + "\n" + result.Breakdown;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}