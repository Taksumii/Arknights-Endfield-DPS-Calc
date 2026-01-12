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
            yPos += 40;

            // === LEFT COLUMN ===

            // === ATTACKER STATS SECTION (Collapsible) ===
            var attackerSection = new CollapsibleSection(tab, "ATTACKER STATS", leftCol, yPos, 420, Color.FromArgb(230, 240, 255));
            int innerY = 10;

            attackerSection.AddControls(
                CreateNumericWithLabel("Operator Base Attack:", leftCol + 10, ref innerY, 0, 10000, 100, out nudBaseAttack),
                CreateNumericWithLabel("Weapon Base Attack:", leftCol + 10, ref innerY, 0, 10000, 50, out nudWeaponAttack),
                CreateNumericWithLabel("Attack % Bonus:", leftCol + 10, ref innerY, 0, 500, 0, out nudAttackPercent, true),
                CreateNumericWithLabel("Attack Flat Bonus:", leftCol + 10, ref innerY, 0, 5000, 0, out nudAttackFlat),
                CreateNumericWithDropdown("Primary Stat:", leftCol + 10, ref innerY, 0, 2000, 100, out nudPrimaryStat, out cmbPrimaryStat),
                CreateNumericWithDropdown("Secondary Stat:", leftCol + 10, ref innerY, 0, 2000, 50, out nudSecondaryStat, out cmbSecondaryStat),
                CreateNumericWithLabel("Damage Multiplier %:", leftCol + 10, ref innerY, 0, 10000, 100, out nudDamageMultiplier, true),
                CreateNumericWithLabel("Critical Rate %:", leftCol + 10, ref innerY, 0, 100, 5, out nudCritRate, true),
                CreateNumericWithLabel("Critical Damage %:", leftCol + 10, ref innerY, 0, 500, 50, out nudCritDamage, true),
                CreateNumericWithLabel("Source Stone Artistry:", leftCol + 10, ref innerY, 0, 1000, 0, out nudSourceStoneArtistry)
            );
            sectionManagerLeft.AddSection(attackerSection);

            // === DAMAGE BONUSES SECTION (Collapsible) ===
            var bonusesSection = new CollapsibleSection(tab, "DAMAGE BONUSES", leftCol, attackerSection.GetBottom() + 10, 420, Color.FromArgb(255, 240, 230));
            innerY = 10;

            bonusesSection.AddControls(
                CreateNumericWithLabel("Elemental Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudElementalBonus, true),
                CreateNumericWithLabel("Skill Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudSkillBonus, true),
                CreateNumericWithLabel("Unbalance Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudUnbalanceBonus, true),
                CreateNumericWithLabel("Other Bonus %:", leftCol + 10, ref innerY, 0, 500, 0, out nudOtherBonus, true)
            );
            sectionManagerLeft.AddSection(bonusesSection);

            // === RIGHT COLUMN ===

            // === TARGET STATS SECTION (Collapsible) ===
            var targetSection = new CollapsibleSection(tab, "TARGET STATS", rightCol, 60, 420, Color.FromArgb(255, 230, 230));
            innerY = 10;

            targetSection.AddControl(CreateNumericWithLabel("Target Defense:", rightCol + 10, ref innerY, -1000, 5000, 100, out nudTargetDefense));
            targetSection.AddControl(CreateNumericWithLabel("Target Resistance:", rightCol + 10, ref innerY, 0, 100, 20, out nudTargetResistance));

            // Damage Type dropdown
            var lblDmgType = new Label { Text = "Damage Type:", Location = new Point(rightCol + 10, innerY), AutoSize = true, Font = new Font("Arial", 9) };
            cmbDamageType = new ComboBox
            {
                Location = new Point(rightCol + 190, innerY - 3),
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

            chkIsUnbalanced = new CheckBox
            {
                Text = "Target is Unbalanced (1.3x)",
                Location = new Point(rightCol + 10, innerY),
                Width = 280,
                Font = new Font("Arial", 9)
            };
            targetSection.AddControl(chkIsUnbalanced);
            innerY += 30;

            chkIsCritical = new CheckBox
            {
                Text = "Force Critical Hit",
                Location = new Point(rightCol + 10, innerY),
                Width = 200,
                Font = new Font("Arial", 9)
            };
            targetSection.AddControl(chkIsCritical);
            innerY += 30;

            chkIsTrueDamage = new CheckBox
            {
                Text = "True Damage",
                Location = new Point(rightCol + 10, innerY),
                Width = 200,
                Font = new Font("Arial", 9)
            };
            targetSection.AddControl(chkIsTrueDamage);
            innerY += 35;

            sectionManagerRight.AddSection(targetSection);

            // === ANOMALY SECTION (Collapsible) ===
            var anomalySection = new CollapsibleSection(tab, "ANOMALY", rightCol, targetSection.GetBottom() + 10, 420, Color.FromArgb(240, 255, 240));
            innerY = 10;

            var lblAnomType = new Label { Text = "Anomaly Type:", Location = new Point(rightCol + 10, innerY), AutoSize = true, Font = new Font("Arial", 9) };
            cmbAnomalyType = new ComboBox
            {
                Location = new Point(rightCol + 190, innerY - 3),
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
                // Auto-set level to 99 if anomaly affects spell level zone
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

            anomalySection.AddControl(CreateNumericWithLabel("Anomaly Level (1-4):", rightCol + 10, ref innerY, 1, 4, 1, out nudAnomalyLevel));

          
            nudLevel = new NumericUpDown { Minimum = 1, Maximum = 99, Value = 99, Visible = false };
            anomalySection.AddControl(nudLevel);

            sectionManagerRight.AddSection(anomalySection);

            // === MULTIPLIER ZONES SECTION (Collapsible) ===
            var multipliersSection = new CollapsibleSection(tab, "MULTIPLIER ZONES", rightCol, anomalySection.GetBottom() + 10, 420, Color.FromArgb(255, 250, 230));
            innerY = 10;

            multipliersSection.AddControls(
                CreateNumericWithLabel("Vulnerability %:", rightCol + 10, ref innerY, 0, 200, 0, out nudVulnerability, true),
                CreateNumericWithLabel("Amplification %:", rightCol + 10, ref innerY, 0, 200, 0, out nudAmplification, true),
                CreateNumericWithLabel("Sanctuary %:", rightCol + 10, ref innerY, 0, 100, 0, out nudSanctuary, true),
                CreateNumericWithLabel("Fragility %:", rightCol + 10, ref innerY, 0, 200, 0, out nudFragility, true),
                CreateNumericWithLabel("Damage Reduction %:", rightCol + 10, ref innerY, 0, 100, 0, out nudDamageReduction, true),
                CreateNumericWithLabel("Special Multiplier %:", rightCol + 10, ref innerY, 0, 200, 0, out nudSpecialMultiplier, true)
            );
            sectionManagerRight.AddSection(multipliersSection);

            // === BUTTONS AND RESULTS (DYNAMIC POSITION) ===
            // Calculate position based on the bottom of the last section in each column
            int leftColBottom = bonusesSection.GetBottom();
            int rightColBottom = multipliersSection.GetBottom();
            int buttonY = Math.Max(leftColBottom, rightColBottom) + 20; 

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

            // Expand/Collapse All buttons
            var btnExpandAll = new Button
            {
                Text = "Expand All ▼",
                Location = new Point(leftCol + 210, buttonY),
                Size = new Size(100, 40),
                Font = new Font("Arial", 9)
            };
            btnExpandAll.Click += (s, e) => {
                sectionManagerLeft.ExpandAll();
                sectionManagerRight.ExpandAll();
                RepositionButtonsAndResults();
            };
            tab.Controls.Add(btnExpandAll);

            var btnCollapseAll = new Button
            {
                Text = "Collapse All ▶",
                Location = new Point(leftCol + 320, buttonY),
                Size = new Size(100, 40),
                Font = new Font("Arial", 9)
            };
            btnCollapseAll.Click += (s, e) => {
                sectionManagerLeft.CollapseAll();
                sectionManagerRight.CollapseAll();
                RepositionButtonsAndResults();
            };
            tab.Controls.Add(btnCollapseAll);

            // Store buttons for repositioning
            tab.Tag = new { btnCalculate, btnExpandAll, btnCollapseAll };

            // === RESULTS AS COLLAPSIBLE SECTION ===
            int resultsY = buttonY;
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

            // === DAMAGE RANGE AS COLLAPSIBLE SECTION ===
            int rangeY = buttonY + 50;
            var rangeSection = new CollapsibleSection(tab, "DAMAGE RANGE", leftCol, rangeY, 420, Color.FromArgb(255, 240, 255));
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

            // Store result sections for repositioning
            var resultSections = new { resultsSection, rangeSection };

            // Breakdown
            int breakdownY = rangeY + 250;
            var lblBreakdown = new Label
            {
                Text = "Breakdown:",
                Location = new Point(leftCol, breakdownY),
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            lblBreakdown.Name = "lblBreakdown";
            tab.Controls.Add(lblBreakdown);

            txtBreakdown = new TextBox
            {
                Location = new Point(leftCol, breakdownY + 25),
                Size = new Size(870, 150),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 8),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            txtBreakdown.Name = "txtBreakdown";
            tab.Controls.Add(txtBreakdown);

            // Update tag to include all repositionable elements
            tab.Tag = new
            {
                btnCalculate,
                btnExpandAll,
                btnCollapseAll,
                resultsSection,
                rangeSection,
                lblBreakdown,
                txtBreakdown,
                bonusesSection,
                multipliersSection
            };
        }

        
        private void RepositionButtonsAndResults()
        {
            var damageTab = tabControl.TabPages[0];
            var tag = damageTab.Tag as dynamic;
            if (tag == null) return;

            // Calculate new button position based on section bottoms
            int leftColBottom = tag.bonusesSection.GetBottom();
            int rightColBottom = tag.multipliersSection.GetBottom();
            int newButtonY = Math.Max(leftColBottom, rightColBottom) + 20;

            // Reposition buttons
            tag.btnCalculate.Location = new Point(tag.btnCalculate.Location.X, newButtonY);
            tag.btnExpandAll.Location = new Point(tag.btnExpandAll.Location.X, newButtonY);
            tag.btnCollapseAll.Location = new Point(tag.btnCollapseAll.Location.X, newButtonY);

            // Reposition result sections
            tag.resultsSection.SetY(newButtonY);
            tag.rangeSection.SetY(newButtonY + 50);

            // Reposition breakdown
            int newBreakdownY = tag.rangeSection.GetBottom() + 20;
            tag.lblBreakdown.Location = new Point(tag.lblBreakdown.Location.X, newBreakdownY);
            tag.txtBreakdown.Location = new Point(tag.txtBreakdown.Location.X, newBreakdownY + 25);
        }

       
        private Control CreateNumericWithLabel(string labelText, int x, ref int y, decimal min, decimal max, decimal val, out NumericUpDown nud, bool percent = false)
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
                DecimalPlaces = percent ? 1 : 0,
                Font = new Font("Arial", 9)
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(nud);
            y += 35;

            return panel;
        }

        
        private Control CreateNumericWithDropdown(string labelText, int x, ref int y, decimal min, decimal max, decimal val, out NumericUpDown nud, out ComboBox cmb)
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
                DecimalPlaces = 0,
                Font = new Font("Arial", 9)
            };

            cmb = new ComboBox
            {
                Location = new Point(275, 0),
                Width = 60,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 8)
            };
            cmb.Items.AddRange(new object[] { "STR", "AGL", "INT", "WLL" });
            cmb.SelectedIndex = 0; 

            panel.Controls.Add(lbl);
            panel.Controls.Add(nud);
            panel.Controls.Add(cmb);
            y += 35;

            return panel;
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                var gearBonuses = CalculateGearBonuses();

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
                    (double)nudUnbalanceBonus.Value,
                    (double)nudOtherBonus.Value,
                    (double)nudTargetDefense.Value,
                    (double)nudTargetResistance.Value,
                    chkIsUnbalanced.Checked,
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

                // Display results
                txtFinalAttack.Text = Math.Floor(result.FinalAttack).ToString("F0");
                txtBaseDamage.Text = Math.Floor(result.BaseDamage).ToString("F0");
                txtFinalDamage.Text = Math.Floor(result.FinalDamage).ToString("F0") +
                                     (chkIsCritical.Checked ? " (CRIT!)" : "");

                if (txtMinDamage != null) txtMinDamage.Text = Math.Floor(result.MinDamage).ToString("F0");
                if (txtMaxDamage != null) txtMaxDamage.Text = Math.Floor(result.MaxDamage).ToString("F0");
                if (txtAvgDamage != null) txtAvgDamage.Text = Math.Floor(result.AverageDamage).ToString("F0");
                if (txtCritChance != null) txtCritChance.Text = $"{nudCritRate.Value + (decimal)gearBonuses.CritRate}%";

                // Breakdown showing gear bonuses
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