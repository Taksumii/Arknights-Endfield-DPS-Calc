using AKEndfieldDmgCalc.Calculators;
using AKEndfieldDmgCalc.Models;
using EndfieldCalculator;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace EndfieldCalculator
{
    
    /// Comparison Mode - Compare up to 3 builds side-by-side
   
    public partial class DamageCalculatorForm
    {
        // Comparison slot data
        private BuildProfile buildA, buildB, buildC;
        private DamageCalculator.DamageResult resultA, resultB, resultC;
        private DPSCalculator.DPSResult dpsResultA, dpsResultB, dpsResultC;

        // UI Controls for each slot
        private ComboBox cmbBuildA, cmbBuildB, cmbBuildC;
        private Button btnCalculateA, btnCalculateB, btnCalculateC;
        private Button btnLoadA, btnLoadB, btnLoadC;
        private Button btnClearAll, btnExportComparison;
        private Panel pnlBuildA, pnlBuildB, pnlBuildC, pnlGraphDamage, pnlGraphDPS;
        private Label lblResultsA, lblResultsB, lblResultsC;

        private void InitializeComparisonTab(TabPage tab)
        {
            tab.AutoScroll = true;
            int colWidth = 290;
            int col1 = 15, col2 = col1 + colWidth + 10, col3 = col2 + colWidth + 10;
            int yPos = 20;

            
            var lblTitle = new Label
            {
                Text = "BUILD COMPARISON (Side-by-Side)",
                Location = new Point(15, yPos),
                Size = new Size(880, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(240, 240, 255)
            };
            tab.Controls.Add(lblTitle);
            yPos += 40;

            // === BUILD A ===
            pnlBuildA = CreateBuildSlot(tab, col1, yPos, "BUILD A", Color.FromArgb(255, 240, 240),
                out cmbBuildA, out btnLoadA, out btnCalculateA, out lblResultsA);
            btnLoadA.Click += (s, e) => LoadBuildToSlot('A');
            btnCalculateA.Click += (s, e) => CalculateBuildSlot('A');

            // === BUILD B ===
            pnlBuildB = CreateBuildSlot(tab, col2, yPos, "BUILD B", Color.FromArgb(240, 255, 240),
                out cmbBuildB, out btnLoadB, out btnCalculateB, out lblResultsB);
            btnLoadB.Click += (s, e) => LoadBuildToSlot('B');
            btnCalculateB.Click += (s, e) => CalculateBuildSlot('B');

            // === BUILD C ===
            pnlBuildC = CreateBuildSlot(tab, col3, yPos, "BUILD C", Color.FromArgb(240, 240, 255),
                out cmbBuildC, out btnLoadC, out btnCalculateC, out lblResultsC);
            btnLoadC.Click += (s, e) => LoadBuildToSlot('C');
            btnCalculateC.Click += (s, e) => CalculateBuildSlot('C');

            yPos += 480;

            
            var lblDamageGraph = new Label
            {
                Text = "AVERAGE DAMAGE COMPARISON",
                Location = new Point(15, yPos),
                Size = new Size(880, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            tab.Controls.Add(lblDamageGraph);
            yPos += 30;

            pnlGraphDamage = new Panel
            {
                Location = new Point(15, yPos),
                Size = new Size(880, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            pnlGraphDamage.Paint += PnlGraphDamage_Paint;
            tab.Controls.Add(pnlGraphDamage);
            yPos += 130;

            
            var lblDPSGraph = new Label
            {
                Text = "COMBINED DPS COMPARISON",
                Location = new Point(15, yPos),
                Size = new Size(880, 25),
                Font = new Font("Arial", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            tab.Controls.Add(lblDPSGraph);
            yPos += 30;

            pnlGraphDPS = new Panel
            {
                Location = new Point(15, yPos),
                Size = new Size(880, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            pnlGraphDPS.Paint += PnlGraphDPS_Paint;
            tab.Controls.Add(pnlGraphDPS);
            yPos += 130;

           
            btnClearAll = new Button
            {
                Text = "Clear All Builds",
                Location = new Point(15, yPos),
                Size = new Size(150, 35),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnClearAll.Click += BtnClearAll_Click;
            tab.Controls.Add(btnClearAll);

            btnExportComparison = new Button
            {
                Text = "Export Comparison",
                Location = new Point(175, yPos),
                Size = new Size(150, 35),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnExportComparison.Click += BtnExportComparison_Click;
            tab.Controls.Add(btnExportComparison);

            
            RefreshComparisonBuilds();
        }

        private Panel CreateBuildSlot(TabPage tab, int x, int y, string title, Color bgColor,
            out ComboBox buildCombo, out Button loadBtn, out Button calcBtn, out Label resultsLabel)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(290, 470),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = bgColor
            };

          
            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(10, 10),
                Size = new Size(270, 25),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblTitle);

            buildCombo = new ComboBox
            {
                Location = new Point(10, 45),
                Width = 270,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            panel.Controls.Add(buildCombo);

           
            loadBtn = new Button
            {
                Text = "Load Build",
                Location = new Point(10, 75),
                Size = new Size(270, 30),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            panel.Controls.Add(loadBtn);

            // Results display
            resultsLabel = new Label
            {
                Location = new Point(10, 115),
                Size = new Size(270, 310),
                Font = new Font("Consolas", 8),
                Text = "No build loaded.\n\nSelect and load a build to compare.",
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(resultsLabel);

            // Calculate button
            calcBtn = new Button
            {
                Text = "Calculate",
                Location = new Point(10, 430),
                Size = new Size(270, 30),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            panel.Controls.Add(calcBtn);

            tab.Controls.Add(panel);
            return panel;
        }

        private void RefreshComparisonBuilds()
        {
            try
            {
                var files = Directory.GetFiles(buildsDirectory, "*.json");
                var buildNames = files.Select(f => Path.GetFileNameWithoutExtension(f))
                                     .OrderBy(n => n).ToArray();

                cmbBuildA.Items.Clear();
                cmbBuildB.Items.Clear();
                cmbBuildC.Items.Clear();

                cmbBuildA.Items.Add("-- Select Build --");
                cmbBuildB.Items.Add("-- Select Build --");
                cmbBuildC.Items.Add("-- Select Build --");

                foreach (var name in buildNames)
                {
                    cmbBuildA.Items.Add(name);
                    cmbBuildB.Items.Add(name);
                    cmbBuildC.Items.Add(name);
                }

                cmbBuildA.SelectedIndex = 0;
                cmbBuildB.SelectedIndex = 0;
                cmbBuildC.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadBuildToSlot(char slot)
        {
            try
            {
                ComboBox cmb = slot == 'A' ? cmbBuildA : slot == 'B' ? cmbBuildB : cmbBuildC;
                Label lbl = slot == 'A' ? lblResultsA : slot == 'B' ? lblResultsB : lblResultsC;

                if (cmb.SelectedIndex <= 0)
                {
                    MessageBox.Show("Please select a build first.", "No Build Selected");
                    return;
                }

                string buildName = cmb.SelectedItem.ToString();
                string path = Path.Combine(buildsDirectory, $"{buildName}.json");

                if (!File.Exists(path))
                {
                    MessageBox.Show("Build file not found.", "Error");
                    return;
                }

                var profile = JsonSerializer.Deserialize<BuildProfile>(File.ReadAllText(path));

                if (slot == 'A') buildA = profile;
                else if (slot == 'B') buildB = profile;
                else buildC = profile;

                // Display build info
                lbl.Text = GenerateBuildSummary(profile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading build: {ex.Message}", "Error");
            }
        }

        private string GenerateBuildSummary(BuildProfile p)
        {
            if (p == null) return "No build loaded.";

            var summary = $"【 {p.Name} 】\n\n";
            summary += "STATS:\n";
            summary += $"  Base ATK: {p.BaseAttack}\n";
            summary += $"  Weapon ATK: {p.WeaponAttack}\n";
            summary += $"  ATK %: {p.AttackPercent}%\n";
            summary += $"  ATK Flat: +{p.AttackFlat}\n";
            summary += $"  Primary: {p.PrimaryStat}\n";
            summary += $"  Secondary: {p.SecondaryStat}\n";
            summary += $"  Multiplier: {p.DamageMultiplier}%\n\n";

            summary += "CRIT:\n";
            summary += $"  Rate: {p.CritRate}%\n";
            summary += $"  Damage: {p.CritDamage}%\n\n";

            summary += "BONUSES:\n";
            summary += $"  Elemental: +{p.ElementalBonus}%\n";
            summary += $"  Skill: +{p.SkillBonus}%\n";
            summary += $"  Unbalance: +{p.UnbalanceBonus}%\n";
            summary += $"  Other: +{p.OtherBonus}%\n\n";

            summary += "TARGET:\n";
            summary += $"  Defense: {p.TargetDefense}\n";
            summary += $"  Resistance: {p.TargetResistance}%\n";
            summary += $"  Type: {p.DamageType}\n";

            summary += "\n(Click Calculate to compute damage)";

            return summary;
        }

        private void CalculateBuildSlot(char slot)
        {
            try
            {
                BuildProfile profile = slot == 'A' ? buildA : slot == 'B' ? buildB : buildC;
                Label lbl = slot == 'A' ? lblResultsA : slot == 'B' ? lblResultsB : lblResultsC;

                if (profile == null)
                {
                    MessageBox.Show($"Please load a build into slot {slot} first.", "No Build");
                    return;
                }

                // Calculate damage using the profile
                var result = DamageCalculator.Calculate(
                    (double)profile.BaseAttack, (double)profile.WeaponAttack,
                    (double)profile.AttackPercent, (double)profile.AttackFlat,
                    (double)profile.PrimaryStat, (double)profile.SecondaryStat,
                    (double)profile.DamageMultiplier,
                    (double)profile.CritRate, (double)profile.CritDamage,
                    (double)profile.Level, (double)profile.SourceStoneArtistry,
                    (double)profile.ElementalBonus, (double)profile.SkillBonus,
                    (double)profile.UnbalanceBonus, (double)profile.OtherBonus,
                    (double)profile.TargetDefense, (double)profile.TargetResistance,
                    profile.IsUnbalanced, profile.IsTrueDamage,
                    profile.AnomalyType ?? "None", (int)profile.AnomalyLevel,
                    (double)profile.Vulnerability, (double)profile.Amplification,
                    (double)profile.Sanctuary, (double)profile.Fragility,
                    (double)profile.DamageReduction, (double)profile.SpecialMultiplier,
                    profile.IsCritical
                );

                // Store result
                if (slot == 'A') resultA = result;
                else if (slot == 'B') resultB = result;
                else resultC = result;

                // Calculate DPS as well (using default 4-hit combo values)
                var dpsResult = DPSCalculator.Calculate(
                    result.MinDamage, result.MaxDamage, result.AverageDamage,
                    36, 54, 56, 88, 0,  // Default combo values
                    false,  // 4-hit combo
                    3.0,    // Default sequence time
                    (double)profile.DamageMultiplier,
                    50,     // Default skill bonus
                    30,     // Default skill uptime
                    80,     // Default ult bonus
                    50      // Default ult uptime
                );

                if (slot == 'A') dpsResultA = dpsResult;
                else if (slot == 'B') dpsResultB = dpsResult;
                else dpsResultC = dpsResult;

                // Display results
                lbl.Text = GenerateResultSummary(profile, result, dpsResult);

                // Refresh graphs
                pnlGraphDamage.Invalidate();
                pnlGraphDPS.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating: {ex.Message}", "Error");
            }
        }

        private string GenerateResultSummary(BuildProfile p, DamageCalculator.DamageResult r, DPSCalculator.DPSResult dps)
        {
            var summary = $"【 {p.Name} 】\n\n";
            summary += "CALCULATED RESULTS:\n\n";
            summary += $"Final ATK: {Math.Floor(r.FinalAttack):N0}\n\n";

            summary += "DAMAGE:\n";
            summary += $"  Min: {Math.Floor(r.MinDamage):N0}\n";
            summary += $"  Avg: {Math.Floor(r.AverageDamage):N0}\n";
            summary += $"  Max: {Math.Floor(r.MaxDamage):N0}\n";
            summary += $"  Variance: {Math.Floor(r.MaxDamage - r.MinDamage):N0}\n\n";

            summary += "DPS (Combined):\n";
            summary += $"  Base: {Math.Floor(dps.AvgBaseDPS):N0}\n";
            summary += $"  +Skill: {Math.Floor(dps.AvgDPSWithSkill):N0}\n";
            summary += $"  +Ult: {Math.Floor(dps.AvgCombinedDPS):N0}\n\n";

            summary += "MULTIPLIERS:\n";
            summary += $"  Crit: ×{r.CritZone:F2}\n";
            summary += $"  Defense: ×{r.DefenseZone:F3}\n";
            summary += $"  Resistance: ×{r.ResistanceZone:F2}\n";

            return summary;
        }

        private void PnlGraphDamage_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            // Title
            var titleFont = new Font("Arial", 10, FontStyle.Bold);
            g.DrawString("Average Damage per Hit", titleFont, Brushes.DarkSlateGray, 10, 10);

            if (resultA == null && resultB == null && resultC == null)
            {
                g.DrawString("Calculate builds to see comparison", new Font("Arial", 9),
                    Brushes.Gray, 300, 50);
                return;
            }

            // Find max value for scaling
            double maxDmg = 0;
            if (resultA != null) maxDmg = Math.Max(maxDmg, resultA.AverageDamage);
            if (resultB != null) maxDmg = Math.Max(maxDmg, resultB.AverageDamage);
            if (resultC != null) maxDmg = Math.Max(maxDmg, resultC.AverageDamage);

            if (maxDmg == 0) return;

            // Draw bars
            int barHeight = 20;
            int startY = 40;
            int startX = 120;
            int maxBarWidth = 700;

            DrawBar(g, "Build A", resultA?.AverageDamage ?? 0, maxDmg, startX, startY,
                maxBarWidth, barHeight, Color.FromArgb(255, 150, 150), buildA?.Name);
            DrawBar(g, "Build B", resultB?.AverageDamage ?? 0, maxDmg, startX, startY + 30,
                maxBarWidth, barHeight, Color.FromArgb(150, 255, 150), buildB?.Name);
            DrawBar(g, "Build C", resultC?.AverageDamage ?? 0, maxDmg, startX, startY + 60,
                maxBarWidth, barHeight, Color.FromArgb(150, 150, 255), buildC?.Name);
        }

        private void PnlGraphDPS_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            // Title
            var titleFont = new Font("Arial", 10, FontStyle.Bold);
            g.DrawString("Combined DPS (with Skills & Ultimate)", titleFont, Brushes.DarkSlateGray, 10, 10);

            if (dpsResultA == null && dpsResultB == null && dpsResultC == null)
            {
                g.DrawString("Calculate builds to see DPS comparison", new Font("Arial", 9),
                    Brushes.Gray, 300, 50);
                return;
            }

            // Find max value for scaling
            double maxDPS = 0;
            if (dpsResultA != null) maxDPS = Math.Max(maxDPS, dpsResultA.AvgCombinedDPS);
            if (dpsResultB != null) maxDPS = Math.Max(maxDPS, dpsResultB.AvgCombinedDPS);
            if (dpsResultC != null) maxDPS = Math.Max(maxDPS, dpsResultC.AvgCombinedDPS);

            if (maxDPS == 0) return;

            // Draw bars
            int barHeight = 20;
            int startY = 40;
            int startX = 120;
            int maxBarWidth = 700;

            DrawBar(g, "Build A", dpsResultA?.AvgCombinedDPS ?? 0, maxDPS, startX, startY,
                maxBarWidth, barHeight, Color.FromArgb(255, 150, 150), buildA?.Name);
            DrawBar(g, "Build B", dpsResultB?.AvgCombinedDPS ?? 0, maxDPS, startX, startY + 30,
                maxBarWidth, barHeight, Color.FromArgb(150, 255, 150), buildB?.Name);
            DrawBar(g, "Build C", dpsResultC?.AvgCombinedDPS ?? 0, maxDPS, startX, startY + 60,
                maxBarWidth, barHeight, Color.FromArgb(150, 150, 255), buildC?.Name);
        }

        private void DrawBar(Graphics g, string label, double value, double maxValue,
            int x, int y, int maxWidth, int height, Color color, string buildName)
        {
            if (value == 0 || string.IsNullOrEmpty(buildName))
            {
                g.DrawString($"{label}: Not calculated", new Font("Arial", 8),
                    Brushes.Gray, 10, y + 5);
                return;
            }

            // Calculate bar width
            int barWidth = (int)((value / maxValue) * maxWidth);

            // Draw bar
            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, x, y, barWidth, height);
            }
            g.DrawRectangle(Pens.Black, x, y, barWidth, height);

            // Draw label and value
            string text = $"{label}: {Math.Floor(value):N0}";
            g.DrawString(text, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, 10, y + 5);

            // Mark winner with star
            if (value == maxValue)
            {
                g.DrawString("⭐", new Font("Arial", 12), Brushes.Gold, x + barWidth + 5, y);
            }
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            buildA = buildB = buildC = null;
            resultA = resultB = resultC = null;
            dpsResultA = dpsResultB = dpsResultC = null;

            lblResultsA.Text = "No build loaded.\n\nSelect and load a build to compare.";
            lblResultsB.Text = "No build loaded.\n\nSelect and load a build to compare.";
            lblResultsC.Text = "No build loaded.\n\nSelect and load a build to compare.";

            cmbBuildA.SelectedIndex = 0;
            cmbBuildB.SelectedIndex = 0;
            cmbBuildC.SelectedIndex = 0;

            pnlGraphDamage.Invalidate();
            pnlGraphDPS.Invalidate();
        }

        private void BtnExportComparison_Click(object sender, EventArgs e)
        {
            try
            {
                var summary = "=== BUILD COMPARISON EXPORT ===\n\n";

                if (buildA != null && resultA != null && dpsResultA != null)
                {
                    summary += $"BUILD A - {buildA.Name}\n";
                    summary += $"  Avg Damage: {Math.Floor(resultA.AverageDamage):N0}\n";
                    summary += $"  Combined DPS: {Math.Floor(dpsResultA.AvgCombinedDPS):N0}\n\n";
                }

                if (buildB != null && resultB != null && dpsResultB != null)
                {
                    summary += $"BUILD B - {buildB.Name}\n";
                    summary += $"  Avg Damage: {Math.Floor(resultB.AverageDamage):N0}\n";
                    summary += $"  Combined DPS: {Math.Floor(dpsResultB.AvgCombinedDPS):N0}\n\n";
                }

                if (buildC != null && resultC != null && dpsResultC != null)
                {
                    summary += $"BUILD C - {buildC.Name}\n";
                    summary += $"  Avg Damage: {Math.Floor(resultC.AverageDamage):N0}\n";
                    summary += $"  Combined DPS: {Math.Floor(dpsResultC.AvgCombinedDPS):N0}\n\n";
                }

                // Find winners
                double maxDmg = 0, maxDPS = 0;
                string dmgWinner = "", dpsWinner = "";

                if (resultA != null) { maxDmg = resultA.AverageDamage; dmgWinner = buildA.Name; }
                if (resultB != null && resultB.AverageDamage > maxDmg) { maxDmg = resultB.AverageDamage; dmgWinner = buildB.Name; }
                if (resultC != null && resultC.AverageDamage > maxDmg) { maxDmg = resultC.AverageDamage; dmgWinner = buildC.Name; }

                if (dpsResultA != null) { maxDPS = dpsResultA.AvgCombinedDPS; dpsWinner = buildA.Name; }
                if (dpsResultB != null && dpsResultB.AvgCombinedDPS > maxDPS) { maxDPS = dpsResultB.AvgCombinedDPS; dpsWinner = buildB.Name; }
                if (dpsResultC != null && dpsResultC.AvgCombinedDPS > maxDPS) { maxDPS = dpsResultC.AvgCombinedDPS; dpsWinner = buildC.Name; }

                summary += "WINNERS:\n";
                if (!string.IsNullOrEmpty(dmgWinner)) summary += $"  Highest Damage: {dmgWinner} ({maxDmg:N0})\n";
                if (!string.IsNullOrEmpty(dpsWinner)) summary += $"  Highest DPS: {dpsWinner} ({maxDPS:N0})\n";

                Clipboard.SetText(summary);
                MessageBox.Show("Comparison copied to clipboard!", "Exported");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Error");
            }
        }
    }
}