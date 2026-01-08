using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Linq;

public class BuildProfile
{
    public string Name { get; set; }
    public string Notes { get; set; }
    public decimal BaseAttack { get; set; }
    public decimal WeaponAttack { get; set; }
    public decimal AttackPercent { get; set; }
    public decimal AttackFlat { get; set; }
    public decimal PrimaryStat { get; set; }
    public decimal SecondaryStat { get; set; }
    public decimal DamageMultiplier { get; set; }
    public decimal CritRate { get; set; }
    public decimal CritDamage { get; set; }
    public decimal Level { get; set; }
    public decimal SourceStoneArtistry { get; set; }
    public decimal ElementalBonus { get; set; }
    public decimal SkillBonus { get; set; }
    public decimal UnbalanceBonus { get; set; }
    public decimal OtherBonus { get; set; }
    public decimal TargetDefense { get; set; }
    public decimal TargetResistance { get; set; }
    public string DamageType { get; set; }
    public bool IsUnbalanced { get; set; }
    public bool IsCritical { get; set; }
    public bool IsTrueDamage { get; set; }
    public string AnomalyType { get; set; }
    public decimal AnomalyLevel { get; set; }
    public decimal Vulnerability { get; set; }
    public decimal Amplification { get; set; }
    public decimal Sanctuary { get; set; }
    public decimal Fragility { get; set; }
    public decimal DamageReduction { get; set; }
    public decimal SpecialMultiplier { get; set; }
}

public static class InputDialog
{
    public static string Show(string text, string caption)
    {
        Form prompt = new Form()
        {
            Width = 400,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen
        };
        Label textLabel = new Label() { Left = 20, Top = 20, Width = 340, Text = text };
        TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
        Button confirmation = new Button()
        {
            Text = "OK",
            Left = 260,
            Width = 100,
            Top = 80,
            DialogResult = DialogResult.OK
        };
        confirmation.Click += (s, e) => { prompt.Close(); };
        prompt.Controls.Add(textBox);
        prompt.Controls.Add(confirmation);
        prompt.Controls.Add(textLabel);
        prompt.AcceptButton = confirmation;
        return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }
}

public partial class DamageCalculatorForm : Form
{
    private NumericUpDown nudBaseAttack, nudWeaponAttack, nudAttackPercent, nudAttackFlat;
    private NumericUpDown nudPrimaryStat, nudSecondaryStat, nudDamageMultiplier;
    private NumericUpDown nudCritRate, nudCritDamage, nudLevel, nudSourceStoneArtistry;
    private NumericUpDown nudElementalBonus, nudSkillBonus, nudUnbalanceBonus, nudOtherBonus;
    private NumericUpDown nudTargetDefense, nudTargetResistance, nudAnomalyLevel;
    private NumericUpDown nudVulnerability, nudAmplification, nudSanctuary;
    private NumericUpDown nudFragility, nudDamageReduction, nudSpecialMultiplier;
    private ComboBox cmbDamageType, cmbAnomalyType, cmbBuildProfiles;
    private CheckBox chkIsUnbalanced, chkIsCritical, chkIsTrueDamage;
    private TextBox txtFinalAttack, txtBaseDamage, txtFinalDamage, txtBreakdown;
    private TextBox txtBuildNotes;
    private Button btnCalculate;

    private string buildsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "EndfieldCalculator", "Builds");

    public DamageCalculatorForm()
    {
        InitializeComponent();
        LoadBuildsList();
    }

    private void InitializeComponent()
    {
        this.Text = "Arknights: Endfield Damage Calculator v1.0";
        this.Size = new Size(900, 850);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.AutoScroll = true;

        int leftCol = 20, rightCol = 450, yPos = 20;

        // Build Management
        AddLabel("=== BUILD PROFILES ===", leftCol, yPos, true);
        yPos += 30;

        AddLabel("Current Build:", leftCol, yPos);
        cmbBuildProfiles = new ComboBox
        {
            Location = new Point(leftCol + 120, yPos),
            Width = 250,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        this.Controls.Add(cmbBuildProfiles);
        yPos += 35;

        int btnX = leftCol;
        var btnNew = new Button { Text = "New", Location = new Point(btnX, yPos), Size = new Size(70, 30) };
        btnNew.Click += BtnNew_Click;
        this.Controls.Add(btnNew);
        btnX += 75;

        var btnSave = new Button { Text = "Save", Location = new Point(btnX, yPos), Size = new Size(70, 30) };
        btnSave.Click += BtnSave_Click;
        this.Controls.Add(btnSave);
        btnX += 75;

        var btnLoad = new Button { Text = "Load", Location = new Point(btnX, yPos), Size = new Size(70, 30) };
        btnLoad.Click += BtnLoad_Click;
        this.Controls.Add(btnLoad);
        btnX += 75;

        var btnDel = new Button { Text = "Delete", Location = new Point(btnX, yPos), Size = new Size(70, 30) };
        btnDel.Click += BtnDelete_Click;
        this.Controls.Add(btnDel);
        yPos += 40;

        btnX = leftCol;
        var btnExp = new Button { Text = "Export...", Location = new Point(btnX, yPos), Size = new Size(120, 30) };
        btnExp.Click += BtnExport_Click;
        this.Controls.Add(btnExp);
        btnX += 125;

        var btnImp = new Button { Text = "Import...", Location = new Point(btnX, yPos), Size = new Size(120, 30) };
        btnImp.Click += BtnImport_Click;
        this.Controls.Add(btnImp);
        yPos += 50;

        // Build Notes
        AddLabel("Build Notes:", leftCol, yPos);
        yPos += 25;
        txtBuildNotes = new TextBox
        {
            Location = new Point(leftCol, yPos),
            Size = new Size(370, 60),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Arial", 9),
            PlaceholderText = "Add notes about this build (weapon, gear, strategy, etc.)"
        };
        this.Controls.Add(txtBuildNotes);
        yPos += 70;

        // Attacker Stats
        AddLabel("=== ATTACKER STATS ===", leftCol, yPos, true);
        yPos += 30;

        nudBaseAttack = AddNumeric("Operator Base Attack:", leftCol, ref yPos, 0, 10000, 100);
        nudWeaponAttack = AddNumeric("Weapon Base Attack:", leftCol, ref yPos, 0, 10000, 50);
        nudAttackPercent = AddNumeric("Attack % Bonus:", leftCol, ref yPos, 0, 500, 0, true);
        nudAttackFlat = AddNumeric("Attack Flat Bonus:", leftCol, ref yPos, 0, 5000, 0);
        nudPrimaryStat = AddNumeric("Primary Stat:", leftCol, ref yPos, 0, 2000, 100);
        nudSecondaryStat = AddNumeric("Secondary Stat:", leftCol, ref yPos, 0, 2000, 50);
        nudDamageMultiplier = AddNumeric("Damage Multiplier %:", leftCol, ref yPos, 0, 10000, 100, true);
        nudCritRate = AddNumeric("Critical Rate %:", leftCol, ref yPos, 0, 100, 5, true);
        nudCritDamage = AddNumeric("Critical Damage %:", leftCol, ref yPos, 0, 500, 50, true);
        nudLevel = AddNumeric("Operator Level:", leftCol, ref yPos, 1, 99, 99);
        nudSourceStoneArtistry = AddNumeric("Source Stone Artistry:", leftCol, ref yPos, 0, 1000, 0);

        yPos += 10;
        AddLabel("=== DAMAGE BONUSES ===", leftCol, yPos, true);
        yPos += 30;

        nudElementalBonus = AddNumeric("Elemental Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
        nudSkillBonus = AddNumeric("Skill Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
        nudUnbalanceBonus = AddNumeric("Unbalance Bonus %:", leftCol, ref yPos, 0, 500, 0, true);
        nudOtherBonus = AddNumeric("Other Bonus %:", leftCol, ref yPos, 0, 500, 0, true);

        // Target Stats
        yPos = 300;
        AddLabel("=== TARGET STATS ===", rightCol, yPos, true);
        yPos += 30;

        nudTargetDefense = AddNumeric("Target Defense:", rightCol, ref yPos, -1000, 5000, 100);
        nudTargetResistance = AddNumeric("Target Resistance:", rightCol, ref yPos, 0, 100, 20);

        AddLabel("Damage Type:", rightCol, yPos);
        cmbDamageType = new ComboBox
        {
            Location = new Point(rightCol + 200, yPos),
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbDamageType.Items.AddRange(new object[] {
            "Physical", "Pyro", "Electromagnetic", "Cryo", "Geo", "Transcendent", "True Damage"
        });
        cmbDamageType.SelectedIndex = 0;
        this.Controls.Add(cmbDamageType);
        yPos += 35;

        chkIsUnbalanced = new CheckBox
        {
            Text = "Target is Unbalanced (1.3x)",
            Location = new Point(rightCol, yPos),
            Width = 300
        };
        this.Controls.Add(chkIsUnbalanced);
        yPos += 30;

        chkIsCritical = new CheckBox
        {
            Text = "Force Critical Hit",
            Location = new Point(rightCol, yPos),
            Width = 200
        };
        this.Controls.Add(chkIsCritical);
        yPos += 30;

        chkIsTrueDamage = new CheckBox
        {
            Text = "True Damage",
            Location = new Point(rightCol, yPos),
            Width = 200
        };
        this.Controls.Add(chkIsTrueDamage);
        yPos += 35;

        AddLabel("Anomaly Type:", rightCol, yPos);
        cmbAnomalyType = new ComboBox
        {
            Location = new Point(rightCol + 200, yPos),
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbAnomalyType.Items.AddRange(new object[] {
            "None", "Knockback", "Knockdown", "Armor Shatter", "Smash",
            "Electrification", "Corrosion", "Burning", "Freeze", "Shatter Ice", "Spell Burst"
        });
        cmbAnomalyType.SelectedIndex = 0;
        this.Controls.Add(cmbAnomalyType);
        yPos += 35;

        nudAnomalyLevel = AddNumeric("Anomaly Level (1-4):", rightCol, ref yPos, 1, 4, 1);

        yPos += 10;
        AddLabel("=== MULTIPLIER ZONES ===", rightCol, yPos, true);
        yPos += 30;

        nudVulnerability = AddNumeric("Vulnerability %:", rightCol, ref yPos, 0, 200, 0, true);
        nudAmplification = AddNumeric("Amplification %:", rightCol, ref yPos, 0, 200, 0, true);
        nudSanctuary = AddNumeric("Sanctuary %:", rightCol, ref yPos, 0, 100, 0, true);
        nudFragility = AddNumeric("Fragility %:", rightCol, ref yPos, 0, 200, 0, true);
        nudDamageReduction = AddNumeric("Damage Reduction %:", rightCol, ref yPos, 0, 100, 0, true);
        nudSpecialMultiplier = AddNumeric("Special Multiplier %:", rightCol, ref yPos, 0, 200, 0, true);

        // Calculate Button
        int calcButtonY = Math.Max(yPos, 700); 
        btnCalculate = new Button
        {
            Text = "Calculate Damage",
            Location = new Point(leftCol, calcButtonY),
            Size = new Size(200, 40),
            Font = new Font("Arial", 12, FontStyle.Bold),
            BackColor = Color.LightGreen
        };
        btnCalculate.Click += BtnCalculate_Click;
        this.Controls.Add(btnCalculate);

        // Results
        int resultsY = calcButtonY + 60;
        AddLabel("=== RESULTS ===", leftCol, resultsY, true);
        resultsY += 30;

        txtFinalAttack = AddResult("Final Attack:", leftCol, ref resultsY);
        txtBaseDamage = AddResult("Base Damage:", leftCol, ref resultsY);
        txtFinalDamage = AddResult("Final Damage:", leftCol, ref resultsY);

        AddLabel("Breakdown:", leftCol, resultsY);
        resultsY += 25;
        txtBreakdown = new TextBox
        {
            Location = new Point(leftCol, resultsY),
            Size = new Size(820, 150),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 9)
        };
        this.Controls.Add(txtBreakdown);
    }

    private Label AddLabel(string text, int x, int y, bool bold = false)
    {
        var lbl = new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = bold ? new Font("Arial", 10, FontStyle.Bold) : new Font("Arial", 9)
        };
        this.Controls.Add(lbl);
        return lbl;
    }

    private NumericUpDown AddNumeric(string label, int x, ref int y, decimal min, decimal max, decimal val, bool percent = false)
    {
        AddLabel(label, x, y);
        var nud = new NumericUpDown
        {
            Location = new Point(x + 200, y),
            Width = 100,
            Minimum = min,
            Maximum = max,
            Value = val,
            DecimalPlaces = percent ? 1 : 0
        };
        this.Controls.Add(nud);
        y += 35;
        return nud;
    }

    private TextBox AddResult(string label, int x, ref int y)
    {
        AddLabel(label, x, y);
        var txt = new TextBox
        {
            Location = new Point(x + 250, y - 3),
            Width = 200,
            ReadOnly = true,
            BackColor = Color.LightYellow,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        this.Controls.Add(txt);
        y += 35;
        return txt;
    }

    private void BtnCalculate_Click(object sender, EventArgs e)
    {
        try
        {
            double statBonus = (double)nudPrimaryStat.Value * 0.005 + (double)nudSecondaryStat.Value * 0.002;
            double finalAtk = ((double)nudBaseAttack.Value + (double)nudWeaponAttack.Value) *
                             (1 + (double)nudAttackPercent.Value / 100) + (double)nudAttackFlat.Value;
            finalAtk *= (1 + statBonus);
            txtFinalAttack.Text = Math.Floor(finalAtk).ToString("F0");

            double baseDmg = (double)nudDamageMultiplier.Value / 100 * finalAtk;
            double critZone = chkIsCritical.Checked ? (1 + (double)nudCritDamage.Value / 100) : 1.0;
            double dmgBonus = 1.0 + ((double)nudElementalBonus.Value + (double)nudSkillBonus.Value +
                                     (double)nudUnbalanceBonus.Value + (double)nudOtherBonus.Value) / 100;
            double vulnZone = 1.0 + (double)nudVulnerability.Value / 100;
            double ampZone = 1.0 + (double)nudAmplification.Value / 100;
            double sanctZone = 1.0 - (double)nudSanctuary.Value / 100;
            double fragZone = 1.0 + (double)nudFragility.Value / 100;
            double dmgRedZone = 1.0 - (double)nudDamageReduction.Value / 100;
            double specZone = 1.0 + (double)nudSpecialMultiplier.Value / 100;

            double defZone = 1.0;
            if (!chkIsTrueDamage.Checked)
            {
                double def = (double)nudTargetDefense.Value;
                defZone = def >= 0 ? 100.0 / (def + 100.0) : 2.0 - Math.Pow(0.99, -def);
            }

            double unbalZone = chkIsUnbalanced.Checked ? 1.3 : 1.0;
            double resZone = 1.0 - (double)nudTargetResistance.Value / 100;

            double anomMult = 1.0;
            int anomLvl = (int)nudAnomalyLevel.Value;
            string anomType = cmbAnomalyType.SelectedItem.ToString();

            switch (anomType)
            {
                case "Knockback":
                case "Knockdown": anomMult = 1.2; break;
                case "Armor Shatter": anomMult = 0.5 + 0.5 * anomLvl; break;
                case "Smash": anomMult = 1.5 + 1.5 * anomLvl; break;
                case "Electrification":
                case "Corrosion": anomMult = 0.8 + 0.8 * anomLvl; break;
                case "Burning": anomMult = 0.12 + 0.12 * anomLvl; break;
                case "Freeze": anomMult = 1.3; break;
                case "Shatter Ice": anomMult = 1.2 + 1.2 * anomLvl; break;
                case "Spell Burst": anomMult = 1.6; break;
            }

            double spellLvlZone = 1.0;
            if (anomType != "None" && !anomType.Contains("Knock") &&
                anomType != "Armor Shatter" && anomType != "Smash")
            {
                spellLvlZone = 1.0 + (5.0 / 980.0) * ((double)nudLevel.Value - 1);
            }

            double stoneZone = 1.0 + (double)nudSourceStoneArtistry.Value / 100;

            double preDmg = baseDmg * anomMult * critZone * dmgBonus * dmgRedZone *
                           vulnZone * ampZone * sanctZone * fragZone * unbalZone *
                           specZone * spellLvlZone * stoneZone;

            txtBaseDamage.Text = Math.Floor(preDmg).ToString("F0");
            double finalDmg = preDmg * defZone * resZone;
            txtFinalDamage.Text = Math.Floor(finalDmg).ToString("F0") +
                                 (chkIsCritical.Checked ? " (CRIT!)" : "");

            txtBreakdown.Text = $@"Attack Calculation:
  Final Attack: {Math.Floor(finalAtk)}

        Damage Zones:
        Base Damage: {baseDmg:F2}
        Anomaly: ×{anomMult:F2}
        Critical: ×{critZone:F2}
        Damage Bonus: ×{dmgBonus:F2}
        Spell Level: ×{spellLvlZone:F4}
        Source Stone: ×{stoneZone:F2}
        Vulnerability: ×{vulnZone:F2}
        Amplification: ×{ampZone:F2}
        Sanctuary: ×{sanctZone:F2}
        Fragility: ×{fragZone:F2}
        Unbalanced: ×{unbalZone:F2}
        Special: ×{specZone:F2}
        Damage Reduction: ×{dmgRedZone:F2}
  
        Pre-Defense: {preDmg:F2}

        Defense & Resistance:
        Defense Zone: ×{defZone:F4}
        Resistance Zone: ×{resZone:F2}
  
         FINAL DAMAGE: {Math.Floor(finalDmg)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new DamageCalculatorForm());
    }



private void LoadBuildsList()
    {
        try
        {
            Directory.CreateDirectory(buildsDirectory);
            cmbBuildProfiles.Items.Clear();
            cmbBuildProfiles.Items.Add("-- New Build --");

            var files = Directory.GetFiles(buildsDirectory, "*.json");
            foreach (var file in files.OrderByDescending(f => File.GetLastWriteTime(f)))
            {
                cmbBuildProfiles.Items.Add(Path.GetFileNameWithoutExtension(file));
            }

            cmbBuildProfiles.SelectedIndex = cmbBuildProfiles.Items.Count > 1 ? 1 : 0;
        }
        catch { }
    }

   

    private BuildProfile GetCurrentProfile()
    {
        return new BuildProfile
        {
            Name = cmbBuildProfiles.SelectedItem?.ToString() ?? "Build",
            Notes = txtBuildNotes.Text,
            BaseAttack = nudBaseAttack.Value,
            WeaponAttack = nudWeaponAttack.Value,
            AttackPercent = nudAttackPercent.Value,
            AttackFlat = nudAttackFlat.Value,
            PrimaryStat = nudPrimaryStat.Value,
            SecondaryStat = nudSecondaryStat.Value,
            DamageMultiplier = nudDamageMultiplier.Value,
            CritRate = nudCritRate.Value,
            CritDamage = nudCritDamage.Value,
            Level = nudLevel.Value,
            SourceStoneArtistry = nudSourceStoneArtistry.Value,
            ElementalBonus = nudElementalBonus.Value,
            SkillBonus = nudSkillBonus.Value,
            UnbalanceBonus = nudUnbalanceBonus.Value,
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
            SpecialMultiplier = nudSpecialMultiplier.Value
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
    nudDamageMultiplier.Value = p.DamageMultiplier;
    nudCritRate.Value = p.CritRate;
    nudCritDamage.Value = p.CritDamage;
    nudLevel.Value = p.Level;
    nudSourceStoneArtistry.Value = p.SourceStoneArtistry;
    nudElementalBonus.Value = p.ElementalBonus;
    nudSkillBonus.Value = p.SkillBonus;
    nudUnbalanceBonus.Value = p.UnbalanceBonus;
    nudOtherBonus.Value = p.OtherBonus;
    nudTargetDefense.Value = p.TargetDefense;
    nudTargetResistance.Value = p.TargetResistance;

    int idx = cmbDamageType.Items.IndexOf(p.DamageType);
    cmbDamageType.SelectedIndex = idx >= 0 ? idx : 0;

    chkIsUnbalanced.Checked = p.IsUnbalanced;
    chkIsCritical.Checked = p.IsCritical;
    chkIsTrueDamage.Checked = p.IsTrueDamage;

    idx = cmbAnomalyType.Items.IndexOf(p.AnomalyType);
    cmbAnomalyType.SelectedIndex = idx >= 0 ? idx : 0;

    nudAnomalyLevel.Value = p.AnomalyLevel;
    nudVulnerability.Value = p.Vulnerability;
    nudAmplification.Value = p.Amplification;
    nudSanctuary.Value = p.Sanctuary;
    nudFragility.Value = p.Fragility;
    nudDamageReduction.Value = p.DamageReduction;
    nudSpecialMultiplier.Value = p.SpecialMultiplier;
}

private void BtnNew_Click(object sender, EventArgs e)
    {
        string name = InputDialog.Show("Enter build name:", "New Build");
        if (!string.IsNullOrWhiteSpace(name))
        {
            cmbBuildProfiles.Items.Insert(1, name);
            cmbBuildProfiles.SelectedIndex = 1;
            BtnSave_Click(sender, e);
        }
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string name = cmbBuildProfiles.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(name) || name == "-- New Build --")
                name = InputDialog.Show("Enter build name:", "Save Build");

            if (string.IsNullOrWhiteSpace(name)) return;

            var profile = GetCurrentProfile();
            profile.Name = name;

            string path = Path.Combine(buildsDirectory, $"{name}.json");
            File.WriteAllText(path, JsonSerializer.Serialize(profile,
                new JsonSerializerOptions { WriteIndented = true }));

            MessageBox.Show($"Saved '{name}'!", "Success");
            LoadBuildsList();

            int idx = cmbBuildProfiles.Items.IndexOf(name);
            if (idx >= 0) cmbBuildProfiles.SelectedIndex = idx;
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
            string name = cmbBuildProfiles.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(name) || name == "-- New Build --")
            {
                MessageBox.Show("Select a build first", "No Build");
                return;
            }

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
            string name = cmbBuildProfiles.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(name) || name == "-- New Build --")
            {
                MessageBox.Show("Select a build first", "No Build");
                return;
            }

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

                    LoadProfile(profile);
                    LoadBuildsList();

                    int idx = cmbBuildProfiles.Items.IndexOf(name);
                    if (idx >= 0) cmbBuildProfiles.SelectedIndex = idx;

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
