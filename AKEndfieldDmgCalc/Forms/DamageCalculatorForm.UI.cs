using System;
using System.Windows.Forms;
using System.Drawing;

namespace EndfieldCalculator
{

    public partial class DamageCalculatorForm
    {
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

        private NumericUpDown AddNumeric(Control parent, string label, int x, ref int y, decimal min, decimal max, decimal val, int decimalPlaces = 0)
        {
            AddLabel(parent, label, x, y);
            var nud = new NumericUpDown
            {
                Location = new Point(x + 180, y - 3),
                Width = 90,
                Minimum = min,
                Maximum = max,
                Value = val,
                DecimalPlaces = decimalPlaces,
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
    }
}