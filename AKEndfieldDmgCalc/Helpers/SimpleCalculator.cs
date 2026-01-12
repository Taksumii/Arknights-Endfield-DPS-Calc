using System;
using System.Windows.Forms;
using System.Drawing;

namespace AKEndfieldDmgCalc.Helpers
{
    public class SimpleCalculatorPanel : Panel
    {
        private TextBox txtDisplay;
        private decimal firstNumber = 0;
        private decimal secondNumber = 0;
        private string operation = "";
        private bool operationPressed = false;

        public SimpleCalculatorPanel(int x, int y)
        {
            this.Location = new Point(x, y);
            this.Size = new Size(200, 270);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(240, 240, 240);

            InitializeCalculator();
        }

        private void InitializeCalculator()
        {
            // Display
            txtDisplay = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(180, 40),
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = true,
                Text = "0",
                BackColor = Color.White
            };
            this.Controls.Add(txtDisplay);

            int btnSize = 40;
            int spacing = 5;
            int startX = 10;
            int startY = 60;

            // Buttons
            string[,] buttons = new string[5, 4]
            {
                { "7", "8", "9", "/" },
                { "4", "5", "6", "*" },
                { "1", "2", "3", "-" },
                { "C", "0", ".", "+" },
                { "=", "=", "=", "=" }
            };

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    string txt = buttons[row, col];
                    Color? color = null;

                    if (txt == "/" || txt == "*" || txt == "-" || txt == "+") color = Color.LightBlue;
                    if (txt == "C") color = Color.LightCoral;
                    if (txt == ".") color = Color.LightGray;

                    CreateButton(txt, startX + col * (btnSize + spacing), startY + row * (btnSize + spacing), btnSize, NumberOrOperatorClick, color);
                }
            }

            // Equals button
            var equalsBtn = new Button
            {
                Text = "=",
                Location = new Point(startX, startY + 4 * (btnSize + spacing)),
                Size = new Size((btnSize + spacing) * 4 - spacing, btnSize),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            equalsBtn.FlatAppearance.BorderColor = Color.Gray;
            equalsBtn.Click += EqualsClick;
            this.Controls.Add(equalsBtn);
        }

        private void CreateButton(string text, int x, int y, int size, EventHandler clickHandler, Color? bgColor = null)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(size, size),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = bgColor ?? Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderColor = Color.Gray;
            btn.Click += clickHandler;
            this.Controls.Add(btn);
        }

        private void NumberOrOperatorClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            if ("0123456789".Contains(btn.Text))
            {
                if (txtDisplay.Text == "0" || operationPressed)
                {
                    txtDisplay.Text = btn.Text;
                    operationPressed = false;
                }
                else txtDisplay.Text += btn.Text;
            }
            else
            {
                firstNumber = decimal.Parse(txtDisplay.Text);
                operation = btn.Text;
                operationPressed = true;
            }

            if (btn.Text == "C") Clear();
            if (btn.Text == ".") DecimalClick();
        }

        private void DecimalClick()
        {
            if (operationPressed) { txtDisplay.Text = "0"; operationPressed = false; }
            if (!txtDisplay.Text.Contains(".")) txtDisplay.Text += ".";
        }

        private void EqualsClick(object sender, EventArgs e)
        {
            secondNumber = decimal.Parse(txtDisplay.Text);
            decimal result = 0;

            switch (operation)
            {
                case "+": result = firstNumber + secondNumber; break;
                case "-": result = firstNumber - secondNumber; break;
                case "*": result = firstNumber * secondNumber; break;
                case "/": result = (secondNumber != 0) ? firstNumber / secondNumber : 0; break;
            }

            txtDisplay.Text = result.ToString();
            operationPressed = true;
        }

        private void Clear()
        {
            txtDisplay.Text = "0";
            firstNumber = 0;
            secondNumber = 0;
            operation = "";
            operationPressed = false;
        }
    }
}
