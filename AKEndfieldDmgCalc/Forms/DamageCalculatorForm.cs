using AKEndfieldDmgCalc.Data;
using AKEndfieldDmgCalc.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using static EndfieldCalculator.DamageCalculator.DamageResult;

namespace EndfieldCalculator
{
    public partial class DamageCalculatorForm : Form
    {
        private TabControl tabControl;

        private BaselineService _baselineService;


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

        // Gear Tab Controls
        private ComboBox cmbGearSet;
        private ComboBox cmbStandaloneItem;
        private Label lblGearSetInfo;
        private Label lblStandaloneInfo;
        private Panel pnlGearInfo;

        // Gear system - store selected items
        private GearSet selectedGearSet = null;
        private StandaloneItem selectedStandaloneItem = null;

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
            this.Text = "Arknights: Endfield Damage Calculator v1.2";
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
            var tabGear = new TabPage("Gear & Weapons");
            var tabComparison = new TabPage("Comparison");
            var tabBuilds = new TabPage("Build Manager");

            tabControl.TabPages.Add(tabDamage);
            tabControl.TabPages.Add(tabDPS);
            tabControl.TabPages.Add(tabGear);
            tabControl.TabPages.Add(tabComparison);
            tabControl.TabPages.Add(tabBuilds);

            InitializeDamageTab(tabDamage);
            InitializeDPSTab(tabDPS);
            InitializeGearTab(tabGear);
            InitializeComparisonTab(tabComparison);
            InitializeBuildTab(tabBuilds);
        }

            private DamageCalculator.DamageResult CalculateFromUi(double damageMultiplierOverride, bool forceCriticalOverrideUsesUi = true)
        {
            var gearBonuses = CalculateGearBonuses();

            double staggeredDamageBonus = (double)nudStaggeredBonus.Value;
            bool isStaggered = chkStaggered.Checked;

            return DamageCalculator.Calculate(
                (double)nudBaseAttack.Value,
                (double)nudWeaponAttack.Value,
                (double)nudAttackPercent.Value + gearBonuses.AttackPercent,
                (double)nudAttackFlat.Value + gearBonuses.AttackFlat,
                (double)nudPrimaryStat.Value,
                (double)nudSecondaryStat.Value,
                damageMultiplierOverride,
                (double)nudCritRate.Value + gearBonuses.CritRate,
                (double)nudCritDamage.Value + gearBonuses.CritDamage,
                (double)nudLevel.Value,
                (double)nudSourceStoneArtistry.Value,
                (double)nudElementalBonus.Value + gearBonuses.ElementalDamageBonus,
                (double)nudSkillBonus.Value + gearBonuses.SkillDamageBonus + gearBonuses.AllDamageBonus,
                staggeredDamageBonus,
                (double)nudOtherBonus.Value,
                (double)nudTargetDefense.Value,
                (double)nudTargetResistance.Value,
                isStaggered,
                chkIsTrueDamage.Checked,
                cmbAnomalyType.SelectedItem?.ToString() ?? "None",
                (int)nudAnomalyLevel.Value,
                (double)nudVulnerability.Value,
                (double)nudAmplification.Value,
                (double)nudSanctuary.Value,
                (double)nudFragility.Value,
                (double)nudDamageReduction.Value + gearBonuses.DamageReduction,
                (double)nudSpecialMultiplier.Value,
                forceCriticalOverrideUsesUi ? chkIsCritical.Checked : false
            );
        }

        public DamageCalculator.DamageResult CalculateFromUiForRotation(
                 double damageMultiplier,
                 double elementalBonusOverride,
                 double skillBonusOverride,
                 double otherBonusOverride)
        {
            var gearBonuses = CalculateGearBonuses();

            double staggeredDamageBonus = (double)nudStaggeredBonus.Value;
            bool isStaggered = chkStaggered.Checked;

            return DamageCalculator.Calculate(
                (double)nudBaseAttack.Value,
                (double)nudWeaponAttack.Value,
                (double)nudAttackPercent.Value + gearBonuses.AttackPercent,
                (double)nudAttackFlat.Value + gearBonuses.AttackFlat,
                (double)nudPrimaryStat.Value,
                (double)nudSecondaryStat.Value,
                damageMultiplier,
                (double)nudCritRate.Value + gearBonuses.CritRate,
                (double)nudCritDamage.Value + gearBonuses.CritDamage,
                (double)nudLevel.Value,
                (double)nudSourceStoneArtistry.Value,
                elementalBonusOverride,
                skillBonusOverride,
                staggeredDamageBonus,
                otherBonusOverride,
                (double)nudTargetDefense.Value,
                (double)nudTargetResistance.Value,
                chkStaggered.Checked,
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
        }


        private DamageCalcContext ComputeDamageContextFromUi()
        {
            // 100% baseline (for rotation scaling)
            var r100 = CalculateFromUi(100, forceCriticalOverrideUsesUi: true);

            // current multiplier (what the user sees in the damage tab)
            var rCur = CalculateFromUi((double)nudDamageMultiplier.Value, forceCriticalOverrideUsesUi: true);

            return new DamageCalcContext
            {
                Result100 = r100,
                ResultCurrent = rCur
            };
        }

        private void ApplyDamageResultToUi(DamageCalculator.DamageResult result)
        {
            
            txtFinalAttack.Text = result.FinalAttack.ToString("F2");
            txtBaseDamage.Text = result.BaseDamage.ToString("F2");
            txtFinalDamage.Text = result.FinalDamage.ToString("F2") + (chkIsCritical.Checked ? " (CRIT!)" : "");

            if (txtMinDamage != null) txtMinDamage.Text = result.MinDamage.ToString("F2");
            if (txtMaxDamage != null) txtMaxDamage.Text = result.MaxDamage.ToString("F2");
            if (txtAvgDamage != null) txtAvgDamage.Text = result.AverageDamage.ToString("F2");

            // Crit chance display (including gear)
            var gearBonuses = CalculateGearBonuses();
            if (txtCritChance != null) txtCritChance.Text = $"{nudCritRate.Value + (decimal)gearBonuses.CritRate:F2}%";

          
        }

    }

}
