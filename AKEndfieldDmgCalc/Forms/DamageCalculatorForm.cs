using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Linq;
using AKEndfieldDmgCalc.Helpers;
using AKEndfieldDmgCalc.Calculators;
using AKEndfieldDmgCalc.Models;


namespace EndfieldCalculator
{
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
            this.Text = "Arknights: Endfield Damage Calculator v3.1";
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

    
     
    }
}