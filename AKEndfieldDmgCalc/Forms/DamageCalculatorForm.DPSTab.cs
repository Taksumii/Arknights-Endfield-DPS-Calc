using System;
using System.Windows.Forms;
using System.Drawing;
using AKEndfieldDmgCalc.Calculators;

namespace EndfieldCalculator
{

    /// DPS Calculator Tab initialization and logic

    public partial class DamageCalculatorForm
    {
        private void InitializeDPSTab(TabPage tab)
        {
            tab.AutoScroll = true;
            int leftCol = 30, yPos = 30;

            AddLabel(tab, "=== BASIC ATTACK SEQUENCE ===", leftCol, yPos, true);
            yPos += 35;

            // Toggle for 5-hit combo
            chkUse5Hits = new CheckBox
            {
                Text = "Enable 5-Hit Combo (some operators)",
                Location = new Point(leftCol, yPos),
                Width = 300,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Checked = false
            };
            chkUse5Hits.CheckedChanged += ChkUse5Hits_CheckedChanged;
            tab.Controls.Add(chkUse5Hits);
            yPos += 30;

            AddLabel(tab, "Enter damage multipliers for each hit in sequence:", leftCol, yPos);
            yPos += 30;

            int seqX = leftCol;
            AddLabel(tab, "Hit 1:", seqX, yPos);
            nudSeq1 = new NumericUpDown
            {
                Location = new Point(seqX + 50, yPos - 3),
                Width = 80,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 36,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(nudSeq1);
            AddLabel(tab, "%", seqX + 135, yPos);
            seqX += 160;

            AddLabel(tab, "Hit 2:", seqX, yPos);
            nudSeq2 = new NumericUpDown
            {
                Location = new Point(seqX + 50, yPos - 3),
                Width = 80,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 54,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(nudSeq2);
            AddLabel(tab, "%", seqX + 135, yPos);
            seqX += 160;

            AddLabel(tab, "Hit 3:", seqX, yPos);
            nudSeq3 = new NumericUpDown
            {
                Location = new Point(seqX + 50, yPos - 3),
                Width = 80,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 56,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(nudSeq3);
            AddLabel(tab, "%", seqX + 135, yPos);
            seqX += 160;

            AddLabel(tab, "Hit 4:", seqX, yPos);
            nudSeq4 = new NumericUpDown
            {
                Location = new Point(seqX + 50, yPos - 3),
                Width = 80,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 88,
                Font = new Font("Arial", 9)
            };
            tab.Controls.Add(nudSeq4);
            AddLabel(tab, "%", seqX + 135, yPos);
            seqX += 160;

            AddLabel(tab, "Hit 5:", seqX, yPos);
            nudSeq5 = new NumericUpDown
            {
                Location = new Point(seqX + 50, yPos - 3),
                Width = 80,
                Maximum = 10000,
                DecimalPlaces = 2,
                Value = 119,
                Font = new Font("Arial", 9),
                Enabled = false
            };
            tab.Controls.Add(nudSeq5);
            AddLabel(tab, "%", seqX + 135, yPos);

            yPos += 40;
            nudSeqTime = AddNumeric(tab, "Full Sequence Time (seconds):", leftCol, ref yPos, 0.1m, 30, 3, 2);

            yPos += 20;
            AddLabel(tab, "=== BATTLE SKILL ===", leftCol, yPos, true);
            yPos += 35;

            nudSkillDamageBonus = AddNumeric(tab, "Skill Damage Bonus % (avg):", leftCol, ref yPos, 0, 1000, 50, 2);
            nudSkillUptime = AddNumeric(tab, "Skill Uptime % (active time):", leftCol, ref yPos, 0, 100, 30, 2);

            yPos += 20;
            AddLabel(tab, "=== ULTIMATE ===", leftCol, yPos, true);
            yPos += 35;

            nudUltDamageBonus = AddNumeric(tab, "Ultimate Damage Bonus %:", leftCol, ref yPos, 0, 1000, 80, 2);
            nudUltUptime = AddNumeric(tab, "Ultimate Uptime % (active time):", leftCol, ref yPos, 0, 100, 50, 2);

            yPos += 30;
            btnCalculateDPS = new Button
            {
                Text = "Calculate DPS",
                Location = new Point(leftCol, yPos),
                Size = new Size(180, 40),
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.LightBlue
            };
            btnCalculateDPS.Click += BtnCalculateDPS_Click;
            tab.Controls.Add(btnCalculateDPS);

            yPos += 60;
            AddLabel(tab, "=== DPS RESULTS ===", leftCol, yPos, true);
            yPos += 35;

            txtBaseDPS = AddResult(tab, "Base DPS (no skills):", leftCol, ref yPos, 250);
            txtWithSkillDPS = AddResult(tab, "With Battle Skill:", leftCol, ref yPos, 250);
            txtWithUltDPS = AddResult(tab, "With Ultimate:", leftCol, ref yPos, 250);
            txtCombinedDPS = AddResult(tab, "Combined DPS:", leftCol, ref yPos, 250);

            yPos += 10;
            AddLabel(tab, "=== DPS RANGE ===", leftCol, yPos, true);
            yPos += 30;

            txtMinDPS = AddResult(tab, "Min DPS (No Crit):", leftCol, ref yPos, 250);
            txtMaxDPS = AddResult(tab, "Max DPS (All Crit):", leftCol, ref yPos, 250);
            txtAvgDPSRange = AddResult(tab, "Average DPS:", leftCol, ref yPos, 250);

            yPos += 20;
            AddLabel(tab, "DPS Breakdown:", leftCol, yPos);
            yPos += 25;
            txtDPSBreakdown = new TextBox
            {
                Location = new Point(leftCol, yPos),
                Size = new Size(820, 150),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9)
            };
            tab.Controls.Add(txtDPSBreakdown);
        }

        private void ChkUse5Hits_CheckedChanged(object sender, EventArgs e)
        {
            if (nudSeq5 != null)
            {
                nudSeq5.Enabled = chkUse5Hits.Checked;
                if (!chkUse5Hits.Checked)
                {
                    nudSeq5.Value = 0;
                }
                else
                {
                    nudSeq5.Value = 119;
                }
            }
        }

        private void BtnCalculateDPS_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if damage has been calculated first
                if (string.IsNullOrEmpty(txtFinalDamage.Text))
                {
                    MessageBox.Show("Please calculate damage in the Damage Calculator tab first!",
                        "Calculate Damage First", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get min, max, and average damage from the damage calculator
                string minDamageText = txtMinDamage?.Text ?? "";
                string maxDamageText = txtMaxDamage?.Text ?? "";
                string avgDamageText = txtAvgDamage?.Text ?? "";

                if (string.IsNullOrEmpty(minDamageText) || string.IsNullOrEmpty(maxDamageText))
                {
                    MessageBox.Show("Please calculate damage in the Damage Calculator tab first!",
                        "Calculate Damage First", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(minDamageText, out double minHitDamage) ||
                    !double.TryParse(maxDamageText, out double maxHitDamage) ||
                    !double.TryParse(avgDamageText, out double avgHitDamage))
                {
                    MessageBox.Show("Invalid damage values. Calculate damage first.", "Error");
                    return;
                }

                var result = DPSCalculator.Calculate(
                    minHitDamage, maxHitDamage, avgHitDamage,
                    (double)nudSeq1.Value, (double)nudSeq2.Value, (double)nudSeq3.Value,
                    (double)nudSeq4.Value, (double)nudSeq5.Value,
                    chkUse5Hits.Checked,
                    (double)nudSeqTime.Value,
                    (double)nudDamageMultiplier.Value,
                    (double)nudSkillDamageBonus.Value,
                    (double)nudSkillUptime.Value,
                    (double)nudUltDamageBonus.Value,
                    (double)nudUltUptime.Value
                );

                // Display results with 2 decimal places
                txtBaseDPS.Text = result.AvgBaseDPS.ToString("N2");
                txtWithSkillDPS.Text = result.AvgDPSWithSkill.ToString("N2");
                txtWithUltDPS.Text = result.AvgDPSWithUlt.ToString("N2");
                txtCombinedDPS.Text = result.AvgCombinedDPS.ToString("N2");

                if (txtMinDPS != null) txtMinDPS.Text = result.MinCombinedDPS.ToString("N2");
                if (txtMaxDPS != null) txtMaxDPS.Text = result.MaxCombinedDPS.ToString("N2");
                if (txtAvgDPSRange != null) txtAvgDPSRange.Text = result.AvgCombinedDPS.ToString("N2");

                txtDPSBreakdown.Text = result.Breakdown;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}