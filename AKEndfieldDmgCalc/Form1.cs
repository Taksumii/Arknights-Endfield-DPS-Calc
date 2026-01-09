using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Linq;

/*
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
    private TabControl tabControl;

    // Damage Calculator Tab Controls
    private NumericUpDown nudBaseAttack, nudWeaponAttack, nudAttackPercent, nudAttackFlat;
    private NumericUpDown nudPrimaryStat, nudSecondaryStat, nudDamageMultiplier;
    private NumericUpDown nudCritRate, nudCritDamage, nudLevel, nudSourceStoneArtistry;
    private NumericUpDown nudElementalBonus, nudSkillBonus, nudUnbalanceBonus, nudOtherBonus;
    private NumericUpDown nudTargetDefense, nudTargetResistance, nudAnomalyLevel;
    private NumericUpDown nudVulnerability, nudAmplification, nudSanctuary;
    private NumericUpDown nudFragility, nudDamageReduction, nudSpecialMultiplier;
    private ComboBox cmbDamageType, cmbAnomalyType;
    private CheckBox chkIsUnbalanced, chkIsCritical, chkIsTrueDamage;
    private TextBox txtFinalAttack, txtBaseDamage, txtFinalDamage, txtBreakdown;
    private TextBox txtMinDamage, txtMaxDamage, txtAvgDamage, txtCritChance;
    private Button btnCalculate;

    // DPS Calculator Tab Controls
    private NumericUpDown nudSeq1, nudSeq2, nudSeq3, nudSeq4, nudSeq5;
    private NumericUpDown nudSeqTime, nudSkillDamageBonus, nudSkillUptime;
    private NumericUpDown nudUltDamageBonus, nudUltUptime;
    private TextBox txtBaseDPS, txtWithSkillDPS, txtWithUltDPS, txtCombinedDPS, txtDPSBreakdown;
    private TextBox txtMinDPS, txtMaxDPS, txtAvgDPSRange;
    private CheckBox chkUse5Hits;
    private Button btnCalculateDPS;

    // Build Manager Tab Controls
    private ComboBox cmbBuildProfiles;
    private TextBox txtBuildNotes;
    private ListBox lstBuilds;
    private Button btnNewBuild, btnSaveBuild, btnLoadBuild, btnDeleteBuild;
    private Button btnExportBuild, btnImportBuild;
    private Label lblBuildInfo;

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
        this.Text = "Arknights: Endfield Damage Calculator v1.1";
        this.Size = new Size(950, 800);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create TabControl
        tabControl = new TabControl
        {
            Location = new Point(10, 10),
            Size = new Size(910, 740),
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        this.Controls.Add(tabControl);

        // Create Tabs
        var tabDamage = new TabPage("Damage Calculator");
        var tabDPS = new TabPage("DPS Calculator");
        var tabBuilds = new TabPage("Build Manager");

        tabControl.TabPages.Add(tabDamage);
        tabControl.TabPages.Add(tabDPS);
        tabControl.TabPages.Add(tabBuilds);

        InitializeDamageTab(tabDamage);
        InitializeDPSTab(tabDPS);
        InitializeBuildTab(tabBuilds);
    }

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
        cmbBuildProfiles.SelectedIndexChanged += (s, e) => {
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

        // Results
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

    private Label AddLabel(Control parent, string text, int x, int y, bool bold = false)
    {
        var lbl = new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = bold ? new Font("Arial", 10, FontStyle.Bold) : new Font("Arial", 9)
        };
        parent.Controls.Add(lbl);
        return lbl;
    }

    private NumericUpDown AddNumeric(Control parent, string label, int x, ref int y, decimal min, decimal max, decimal val, bool percent = false)
    {
        AddLabel(parent, label, x, y);
        var nud = new NumericUpDown
        {
            Location = new Point(x + 180, y - 3),
            Width = 90,
            Minimum = min,
            Maximum = max,
            Value = val,
            DecimalPlaces = percent ? 1 : 0,
            Font = new Font("Arial", 9)
        };
        parent.Controls.Add(nud);
        y += 35;
        return nud;
    }

    private TextBox AddResult(Control parent, string label, int x, ref int y, int labelWidth = 200)
    {
        AddLabel(parent, label, x, y);
        var txt = new TextBox
        {
            Location = new Point(x + labelWidth, y - 3),
            Width = 180,
            ReadOnly = true,
            BackColor = Color.LightYellow,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        parent.Controls.Add(txt);
        y += 35;
        return txt;
    }
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
            DecimalPlaces = 1,
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
            DecimalPlaces = 1,
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
            DecimalPlaces = 1,
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
            DecimalPlaces = 1,
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
            DecimalPlaces = 1,
            Value = 119,
            Font = new Font("Arial", 9),
            Enabled = false
        };
        tab.Controls.Add(nudSeq5);
        AddLabel(tab, "%", seqX + 135, yPos);

        yPos += 40;
        nudSeqTime = AddNumeric(tab, "Full Sequence Time (seconds):", leftCol, ref yPos, 0.1m, 30, 3, true);

        yPos += 20;
        AddLabel(tab, "=== BATTLE SKILL ===", leftCol, yPos, true);
        yPos += 35;

        nudSkillDamageBonus = AddNumeric(tab, "Skill Damage Bonus % (avg):", leftCol, ref yPos, 0, 1000, 50, true);
        nudSkillUptime = AddNumeric(tab, "Skill Uptime % (active time):", leftCol, ref yPos, 0, 100, 30, true);

        yPos += 20;
        AddLabel(tab, "=== ULTIMATE ===", leftCol, yPos, true);
        yPos += 35;

        nudUltDamageBonus = AddNumeric(tab, "Ultimate Damage Bonus %:", leftCol, ref yPos, 0, 1000, 80, true);
        nudUltUptime = AddNumeric(tab, "Ultimate Uptime % (active time):", leftCol, ref yPos, 0, 100, 50, true);

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
                nudSeq5.Value = 0; // Reset to 0 when disabled
            }
            else
            {
                nudSeq5.Value = 119; 
            }
        }
    }

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

            // Calculate both non-crit and crit damage
            double nonCritZone = 1.0;
            double critZone = 1.0 + (double)nudCritDamage.Value / 100;
            double actualCritZone = chkIsCritical.Checked ? critZone : 1.0;

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
            string anomType = cmbAnomalyType.SelectedItem?.ToString() ?? "None";

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

            // Calculate min damage (non-crit)
            double preDmgMin = baseDmg * anomMult * nonCritZone * dmgBonus * dmgRedZone *
                           vulnZone * ampZone * sanctZone * fragZone * unbalZone *
                           specZone * spellLvlZone * stoneZone;
            double minDamage = preDmgMin * defZone * resZone;

            // Calculate max damage (crit)
            double preDmgMax = baseDmg * anomMult * critZone * dmgBonus * dmgRedZone *
                           vulnZone * ampZone * sanctZone * fragZone * unbalZone *
                           specZone * spellLvlZone * stoneZone;
            double maxDamage = preDmgMax * defZone * resZone;

            // Calculate average damage based on crit rate
            double critRate = (double)nudCritRate.Value / 100.0;
            double avgDamage = (minDamage * (1 - critRate)) + (maxDamage * critRate);

            // Current damage
            double preDmg = baseDmg * anomMult * actualCritZone * dmgBonus * dmgRedZone *
                           vulnZone * ampZone * sanctZone * fragZone * unbalZone *
                           specZone * spellLvlZone * stoneZone;
            txtBaseDamage.Text = Math.Floor(preDmg).ToString("F0");
            double finalDmg = preDmg * defZone * resZone;
            txtFinalDamage.Text = Math.Floor(finalDmg).ToString("F0") +
                                 (chkIsCritical.Checked ? " (CRIT!)" : "");

            // Display damage range
            if (txtMinDamage != null) txtMinDamage.Text = Math.Floor(minDamage).ToString("F0");
            if (txtMaxDamage != null) txtMaxDamage.Text = Math.Floor(maxDamage).ToString("F0");
            if (txtAvgDamage != null) txtAvgDamage.Text = Math.Floor(avgDamage).ToString("F0");
            if (txtCritChance != null) txtCritChance.Text = $"{nudCritRate.Value}%";

            txtBreakdown.Text = $@"Attack Calculation:
              Final Attack: {Math.Floor(finalAtk)}
            
            Damage Zones:
              Base: {baseDmg:F2} | Anomaly: ×{anomMult:F2} | Crit: ×{actualCritZone:F2}
              Damage Bonus: ×{dmgBonus:F2} | Spell Level: ×{spellLvlZone:F4}
              Source Stone: ×{stoneZone:F2} | Vulnerability: ×{vulnZone:F2}
              Amplification: ×{ampZone:F2} | Sanctuary: ×{sanctZone:F2}
              Fragility: ×{fragZone:F2} | Unbalanced: ×{unbalZone:F2}
              Special: ×{specZone:F2} | Dmg Reduction: ×{dmgRedZone:F2}
              Pre-Defense: {preDmg:F2}
            
            Defense & Resistance:
              Defense: ×{defZone:F4} | Resistance: ×{resZone:F2}
              
            CURRENT DAMAGE: {Math.Floor(finalDmg)}
            
            Damage Range ({nudCritRate.Value}% crit rate):
              Min (No Crit): {Math.Floor(minDamage)}
              Max (Crit): {Math.Floor(maxDamage)}
              Average: {Math.Floor(avgDamage)}
              Variance: {Math.Floor(maxDamage - minDamage)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nStack: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Calculate sequence total multiplier (4 or 5 hits based on toggle)
            double seqTotal = (double)(nudSeq1.Value + nudSeq2.Value + nudSeq3.Value + nudSeq4.Value);
            string hitSequence = $"{nudSeq1.Value}% + {nudSeq2.Value}% + {nudSeq3.Value}% + {nudSeq4.Value}%";

            if (chkUse5Hits.Checked && nudSeq5.Enabled)
            {
                seqTotal += (double)nudSeq5.Value;
                hitSequence += $" + {nudSeq5.Value}%";
            }

            seqTotal /= 100.0;
            double seqTime = (double)nudSeqTime.Value;
            double dmgMultiplier = (double)nudDamageMultiplier.Value / 100.0;

            // Calculate damage per sequence for min, max, and average
            double minSeqDamage = minHitDamage * seqTotal / dmgMultiplier;
            double maxSeqDamage = maxHitDamage * seqTotal / dmgMultiplier;
            double avgSeqDamage = avgHitDamage * seqTotal / dmgMultiplier;

            // Base DPS (one sequence)
            double minBaseDPS = minSeqDamage / seqTime;
            double maxBaseDPS = maxSeqDamage / seqTime;
            double avgBaseDPS = avgSeqDamage / seqTime;

            // With Battle Skill
            double skillBonus = (double)nudSkillDamageBonus.Value / 100.0;
            double skillUptime = (double)nudSkillUptime.Value / 100.0;
            double avgSkillBonus = 1.0 + (skillBonus * skillUptime);

            double minDpsWithSkill = minBaseDPS * avgSkillBonus;
            double maxDpsWithSkill = maxBaseDPS * avgSkillBonus;
            double avgDpsWithSkill = avgBaseDPS * avgSkillBonus;

            // With Ultimate
            double ultBonus = (double)nudUltDamageBonus.Value / 100.0;
            double ultUptime = (double)nudUltUptime.Value / 100.0;
            double avgUltBonus = 1.0 + (ultBonus * ultUptime);

            double minDpsWithUlt = minBaseDPS * avgUltBonus;
            double maxDpsWithUlt = maxBaseDPS * avgUltBonus;
            double avgDpsWithUlt = avgBaseDPS * avgUltBonus;

            // Combined (Skill + Ultimate)
            double minCombinedDPS = minBaseDPS * avgSkillBonus * avgUltBonus;
            double maxCombinedDPS = maxBaseDPS * avgSkillBonus * avgUltBonus;
            double avgCombinedDPS = avgBaseDPS * avgSkillBonus * avgUltBonus;

            // Display base DPS results (using average)
            txtBaseDPS.Text = Math.Floor(avgBaseDPS).ToString("N0");
            txtWithSkillDPS.Text = Math.Floor(avgDpsWithSkill).ToString("N0");
            txtWithUltDPS.Text = Math.Floor(avgDpsWithUlt).ToString("N0");
            txtCombinedDPS.Text = Math.Floor(avgCombinedDPS).ToString("N0");

            // Display DPS range
            if (txtMinDPS != null) txtMinDPS.Text = Math.Floor(minCombinedDPS).ToString("N0");
            if (txtMaxDPS != null) txtMaxDPS.Text = Math.Floor(maxCombinedDPS).ToString("N0");
            if (txtAvgDPSRange != null) txtAvgDPSRange.Text = Math.Floor(avgCombinedDPS).ToString("N0");

            txtDPSBreakdown.Text = $@"=== DPS BREAKDOWN ===

            Basic Attack Sequence ({(chkUse5Hits.Checked ? "5-hit" : "4-hit")} combo):
              Hits: {hitSequence}
              Total: {seqTotal * 100:F1}% over {seqTime} seconds
              
            Damage per Sequence:
              Min: {minSeqDamage:F0} | Max: {maxSeqDamage:F0} | Avg: {avgSeqDamage:F0}
              
            Base DPS (no buffs):
              Min: {minBaseDPS:F0} | Max: {maxBaseDPS:F0} | Avg: {avgBaseDPS:F0}
            
            Battle Skill:
              Damage Bonus: +{nudSkillDamageBonus.Value}%
              Active Time: {nudSkillUptime.Value}% uptime
              Avg Multiplier: ×{avgSkillBonus:F2}
              DPS: Min {minDpsWithSkill:F0} | Max {maxDpsWithSkill:F0} | Avg {avgDpsWithSkill:F0}
            
            Ultimate:
              Damage Bonus: +{nudUltDamageBonus.Value}%
              Active Time: {nudUltUptime.Value}% uptime
              Avg Multiplier: ×{avgUltBonus:F2}
              DPS: Min {minDpsWithUlt:F0} | Max {maxDpsWithUlt:F0} | Avg {avgDpsWithUlt:F0}
              
            Combined (Skill + Ult):
              Min DPS: {minCombinedDPS:F0}
              Max DPS: {maxCombinedDPS:F0}
              Avg DPS: {avgCombinedDPS:F0}
              Variance: {maxCombinedDPS - minCombinedDPS:F0}
            
            30 Second Damage: {avgCombinedDPS * 30:F0}
            60 Second Damage: {avgCombinedDPS * 60:F0}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nStack: {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
            if (lstBuilds.SelectedIndex < 0) return;

            string name = lstBuilds.SelectedItem.ToString();
            string path = Path.Combine(buildsDirectory, $"{name}.json");

            if (File.Exists(path))
            {
                var profile = JsonSerializer.Deserialize<BuildProfile>(File.ReadAllText(path));
                if (profile != null)
                {
                    var fileInfo = new FileInfo(path);
                    lblBuildInfo.Text = $"Build: {name}\nLast Modified: {fileInfo.LastWriteTime:g}\nSize: {fileInfo.Length} bytes";
                    txtBuildNotes.Text = profile.Notes ?? "";
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
            txtBuildNotes.Text = "";
            lblBuildInfo.Text = $"New Build: {name}\nNot yet saved.";
        }
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string name = lstBuilds.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(name))
                name = InputDialog.Show("Enter build name:", "Save Build");

            if (string.IsNullOrWhiteSpace(name)) return;

            var profile = GetCurrentProfile();
            profile.Name = name;

            string path = Path.Combine(buildsDirectory, $"{name}.json");
            File.WriteAllText(path, JsonSerializer.Serialize(profile,
                new JsonSerializerOptions { WriteIndented = true }));

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

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new DamageCalculatorForm());
    }

}

*/