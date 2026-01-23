using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Linq;
using AKEndfieldDmgCalc.Helpers;
using AKEndfieldDmgCalc.Models;
using AKEndfieldDmgCalc.Calculators;

namespace EndfieldCalculator
{

    

    public partial class DamageCalculatorForm
    {
        private void InitializeBuildTab(TabPage tab)
        {
            int leftCol = 30, rightCol = 450, yPos = 30;

            AddLabel(tab, "=== SAVED BUILDS ===", leftCol, yPos, true);
            yPos += 35;

            lstBuilds = new ListBox
            {
                Location = new Point(leftCol, yPos),
                Size = new Size(380, 400),
                Font = new Font("Arial", 10)
            };
            lstBuilds.SelectedIndexChanged += LstBuilds_SelectedIndexChanged;
            tab.Controls.Add(lstBuilds);

            int btnY = yPos + 420;
            int btnX = leftCol;

            btnNewBuild = new Button { Text = "New Build", Location = new Point(btnX, btnY), Size = new Size(90, 35) };
            btnNewBuild.Click += BtnNew_Click;
            tab.Controls.Add(btnNewBuild);
            btnX += 95;

            btnSaveBuild = new Button { Text = "Save", Location = new Point(btnX, btnY), Size = new Size(90, 35) };
            btnSaveBuild.Click += BtnSave_Click;
            tab.Controls.Add(btnSaveBuild);
            btnX += 95;

            btnLoadBuild = new Button { Text = "Load", Location = new Point(btnX, btnY), Size = new Size(90, 35) };
            btnLoadBuild.Click += BtnLoad_Click;
            tab.Controls.Add(btnLoadBuild);
            btnX += 95;

            btnDeleteBuild = new Button { Text = "Delete", Location = new Point(btnX, btnY), Size = new Size(90, 35) };
            btnDeleteBuild.Click += BtnDelete_Click;
            tab.Controls.Add(btnDeleteBuild);

            btnY += 45;
            btnX = leftCol;

            btnExportBuild = new Button { Text = "Export Build...", Location = new Point(btnX, btnY), Size = new Size(185, 35) };
            btnExportBuild.Click += BtnExport_Click;
            tab.Controls.Add(btnExportBuild);
            btnX += 190;

            btnImportBuild = new Button { Text = "Import Build...", Location = new Point(btnX, btnY), Size = new Size(185, 35) };
            btnImportBuild.Click += BtnImport_Click;
            tab.Controls.Add(btnImportBuild);

            // Build Details
            AddLabel(tab, "=== BUILD DETAILS ===", rightCol, yPos, true);
            yPos += 35;

            lblBuildInfo = new Label
            {
                Location = new Point(rightCol, yPos),
                Size = new Size(400, 100),
                Font = new Font("Arial", 9),
                Text = "Select a build to view details..."
            };
            tab.Controls.Add(lblBuildInfo);
            yPos += 110;

            AddLabel(tab, "Build Notes:", rightCol, yPos);
            yPos += 25;
            txtBuildNotes = new TextBox
            {
                Location = new Point(rightCol, yPos),
                Size = new Size(400, 350),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 9),
                PlaceholderText = "Add notes: weapon, gear, team comp, strategy..."
            };
            tab.Controls.Add(txtBuildNotes);
        }

        private void LoadBuildsList()
        {
            try
            {
                Directory.CreateDirectory(buildsDirectory);

                cmbBuildProfiles.Items.Clear();
                cmbBuildProfiles.Items.Add("-- Select Build --");
                lstBuilds.Items.Clear();

                var files = Directory.GetFiles(buildsDirectory, "*.json");
                foreach (var file in files.OrderByDescending(f => File.GetLastWriteTime(f)))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    cmbBuildProfiles.Items.Add(name);
                    lstBuilds.Items.Add(name);
                }

                cmbBuildProfiles.SelectedIndex = 0;
            }
            catch { }
        }

        // Update these methods in DamageCalculatorForm.BuildTab.cs:

        private BuildProfile GetCurrentProfile()
        {
            return new BuildProfile
            {
                Name = "Build",
                Notes = txtBuildNotes.Text,
                BaseAttack = nudBaseAttack.Value,
                WeaponAttack = nudWeaponAttack.Value,
                AttackPercent = nudAttackPercent.Value,
                AttackFlat = nudAttackFlat.Value,
                PrimaryStat = nudPrimaryStat.Value,
                SecondaryStat = nudSecondaryStat.Value,

                // Save stat types
                PrimaryStatType = cmbPrimaryStatType?.SelectedItem?.ToString() ?? "STR",
                SecondaryStatType = cmbSecondaryStatType?.SelectedItem?.ToString() ?? "STR",

                DamageMultiplier = nudDamageMultiplier.Value,
                CritRate = nudCritRate.Value,
                CritDamage = nudCritDamage.Value,
                Level = nudLevel.Value,
                SourceStoneArtistry = nudSourceStoneArtistry.Value,
                ElementalBonus = nudElementalBonus.Value,
                SkillBonus = nudSkillBonus.Value,

                // CHANGED: Save staggered state instead of unbalance bonus
                IsStaggered = chkStaggered.Checked,
                UnbalanceBonus = 0, // Keep at 0 for compatibility

                OtherBonus = nudOtherBonus.Value,
                TargetDefense = nudTargetDefense.Value,
                TargetResistance = nudTargetResistance.Value,
                DamageType = cmbDamageType.SelectedItem?.ToString() ?? "Physical",
                IsUnbalanced = chkIsUnbalanced.Checked,
                IsCritical = chkIsCritical.Checked,
                IsTrueDamage = chkIsTrueDamage.Checked,
                AnomalyType = cmbAnomalyType.SelectedItem?.ToString() ?? "None",
                AnomalyLevel = nudAnomalyLevel.Value,
                Vulnerability = nudVulnerability.Value,
                Amplification = nudAmplification.Value,
                Sanctuary = nudSanctuary.Value,
                Fragility = nudFragility.Value,
                DamageReduction = nudDamageReduction.Value,
                SpecialMultiplier = nudSpecialMultiplier.Value,
                GearSetName = cmbGearSet?.SelectedItem?.ToString() ?? "None (No Set Bonus)"
            };
        }

        private void LoadProfile(BuildProfile p)
        {
            txtBuildNotes.Text = p.Notes ?? "";
            nudBaseAttack.Value = p.BaseAttack;
            nudWeaponAttack.Value = p.WeaponAttack;
            nudAttackPercent.Value = p.AttackPercent;
            nudAttackFlat.Value = p.AttackFlat;
            nudPrimaryStat.Value = p.PrimaryStat;
            nudSecondaryStat.Value = p.SecondaryStat;

            // Load stat types
            if (cmbPrimaryStatType != null && !string.IsNullOrEmpty(p.PrimaryStatType))
            {
                int idx = cmbPrimaryStatType.Items.IndexOf(p.PrimaryStatType);
                if (idx >= 0) cmbPrimaryStatType.SelectedIndex = idx;
            }

            if (cmbSecondaryStatType != null && !string.IsNullOrEmpty(p.SecondaryStatType))
            {
                int idx = cmbSecondaryStatType.Items.IndexOf(p.SecondaryStatType);
                if (idx >= 0) cmbSecondaryStatType.SelectedIndex = idx;
            }

            nudDamageMultiplier.Value = p.DamageMultiplier;
            nudCritRate.Value = p.CritRate;
            nudCritDamage.Value = p.CritDamage;
            nudLevel.Value = p.Level;
            nudSourceStoneArtistry.Value = p.SourceStoneArtistry;
            nudElementalBonus.Value = p.ElementalBonus;
            nudSkillBonus.Value = p.SkillBonus;

           
            if (p.UnbalanceBonus > 0)
            {
                chkStaggered.Checked = true;
            }
            else
            {
                chkStaggered.Checked = p.IsStaggered;
            }

            nudOtherBonus.Value = p.OtherBonus;
            nudTargetDefense.Value = p.TargetDefense;
            nudTargetResistance.Value = p.TargetResistance;

            if (cmbGearSet != null && !string.IsNullOrEmpty(p.GearSetName))
            {
                int gearIdx = cmbGearSet.Items.IndexOf(p.GearSetName);
                if (gearIdx >= 0) cmbGearSet.SelectedIndex = gearIdx;
            }

            int idx2 = cmbDamageType.Items.IndexOf(p.DamageType);
            cmbDamageType.SelectedIndex = idx2 >= 0 ? idx2 : 0;

            chkIsUnbalanced.Checked = p.IsUnbalanced;
            chkIsCritical.Checked = p.IsCritical;
            chkIsTrueDamage.Checked = p.IsTrueDamage;

            idx2 = cmbAnomalyType.Items.IndexOf(p.AnomalyType);
            cmbAnomalyType.SelectedIndex = idx2 >= 0 ? idx2 : 0;

            nudAnomalyLevel.Value = p.AnomalyLevel;
            nudVulnerability.Value = p.Vulnerability;
            nudAmplification.Value = p.Amplification;
            nudSanctuary.Value = p.Sanctuary;
            nudFragility.Value = p.Fragility;
            nudDamageReduction.Value = p.DamageReduction;
            nudSpecialMultiplier.Value = p.SpecialMultiplier;
        }

        private void QuickLoadBuild()
        {
            try
            {
                string name = cmbBuildProfiles.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(name) || name == "-- Select Build --") return;

                string path = Path.Combine(buildsDirectory, $"{name}.json");
                if (!File.Exists(path)) return;

                var profile = JsonSerializer.Deserialize<BuildProfile>(File.ReadAllText(path));
                if (profile != null)
                {
                    LoadProfile(profile);
                }
            }
            catch { }
        }

        private void LstBuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstBuilds.SelectedIndex < 0)
                {
                    
                    lstBuilds.Tag = null;
                    return;
                }

                string name = lstBuilds.SelectedItem.ToString();
                string path = Path.Combine(buildsDirectory, $"{name}.json");

                if (File.Exists(path))
                {
                    var profile = JsonSerializer.Deserialize<BuildProfile>(File.ReadAllText(path));
                    if (profile != null)
                    {
                        var fileInfo = new FileInfo(path);

                        // Enhanced display with stat types
                        string statInfo = "";
                        if (!string.IsNullOrEmpty(profile.PrimaryStatType) || !string.IsNullOrEmpty(profile.SecondaryStatType))
                        {
                            statInfo = $"\nStats: {profile.PrimaryStat} {profile.PrimaryStatType ?? "STR"} / {profile.SecondaryStat} {profile.SecondaryStatType ?? "STR"}";
                        }

                        lblBuildInfo.Text = $"Build: {name}\nLast Modified: {fileInfo.LastWriteTime:g}\nSize: {fileInfo.Length} bytes{statInfo}";
                        txtBuildNotes.Text = profile.Notes ?? "";

                        
                        lstBuilds.Tag = null;
                    }
                }
            }
            catch { }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            string name = InputDialog.Show("Enter build name:", "New Build");
            if (!string.IsNullOrWhiteSpace(name))
            {
                
                lstBuilds.SelectedIndex = -1;

             
                txtBuildNotes.Text = "";

                
                lblBuildInfo.Text = $"New Build: {name}\nNot yet saved.\n\nClick 'Save' to create this build.";

                
                lstBuilds.Tag = name;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = null;

                
                if (lstBuilds.Tag is string newBuildName && !string.IsNullOrWhiteSpace(newBuildName))
                {
                    name = newBuildName;
                }
         
                else if (lstBuilds.SelectedIndex >= 0)
                {
                    name = lstBuilds.SelectedItem?.ToString();
                }

                
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = InputDialog.Show("Enter build name:", "Save Build");
                }

                if (string.IsNullOrWhiteSpace(name)) return;

                var profile = GetCurrentProfile();
                profile.Name = name;

                string path = Path.Combine(buildsDirectory, $"{name}.json");
                File.WriteAllText(path, JsonSerializer.Serialize(profile,
                    new JsonSerializerOptions { WriteIndented = true }));

                // Clear the temporary new build name
                lstBuilds.Tag = null;

                MessageBox.Show($"Saved '{name}'!", "Success");
                LoadBuildsList();

                int idx = lstBuilds.Items.IndexOf(name);
                if (idx >= 0) lstBuilds.SelectedIndex = idx;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstBuilds.SelectedIndex < 0)
                {
                    MessageBox.Show("Select a build first", "No Build");
                    return;
                }

                string name = lstBuilds.SelectedItem.ToString();
                string path = Path.Combine(buildsDirectory, $"{name}.json");

                if (!File.Exists(path))
                {
                    MessageBox.Show("Build not found", "Error");
                    return;
                }

                var profile = JsonSerializer.Deserialize<BuildProfile>(File.ReadAllText(path));
                if (profile != null)
                {
                    LoadProfile(profile);
                    tabControl.SelectedIndex = 0;
                    MessageBox.Show($"Loaded '{name}'!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstBuilds.SelectedIndex < 0)
                {
                    MessageBox.Show("Select a build first", "No Build");
                    return;
                }

                string name = lstBuilds.SelectedItem.ToString();

                if (MessageBox.Show($"Delete '{name}'?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string path = Path.Combine(buildsDirectory, $"{name}.json");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        MessageBox.Show($"Deleted '{name}'!", "Success");
                        LoadBuildsList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    FileName = GetCurrentProfile().Name + ".json"
                };

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(GetCurrentProfile(),
                        new JsonSerializerOptions { WriteIndented = true }));
                    MessageBox.Show("Exported!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog { Filter = "JSON files (*.json)|*.json" };

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var profile = JsonSerializer.Deserialize<BuildProfile>(File.ReadAllText(dlg.FileName));
                    if (profile != null)
                    {
                        string name = Path.GetFileNameWithoutExtension(dlg.FileName);
                        profile.Name = name;

                        string dest = Path.Combine(buildsDirectory, $"{name}.json");
                        File.WriteAllText(dest, JsonSerializer.Serialize(profile,
                            new JsonSerializerOptions { WriteIndented = true }));

                        LoadBuildsList();
                        MessageBox.Show($"Imported '{name}'!", "Success");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }
    }
}