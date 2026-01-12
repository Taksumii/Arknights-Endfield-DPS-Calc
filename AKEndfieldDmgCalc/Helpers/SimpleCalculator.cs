using System;
using System.Windows.Forms;
using System.Drawing;

namespace AKEndfieldDmgCalc.Helpers
{
  
    public class SimpleCalculator : Panel
    {
        private TextBox txtDisplay;
        private double firstNumber = 0;
        private double secondNumber = 0;
        private string operation = "";
        private bool operationPressed = false;

        public SimpleCalculator(int x, int y)
        {
            this.Location = new Point(x, y);
            this.Size = new Size(200, 270);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(240, 240, 240);

            InitializeCalculator();
        }

        private void InitializeCalculator()
        {
            // Title
            var lblTitle = new Label
            {
                Text = "Quick Calc",
                Location = new Point(5, 5),
                Size = new Size(190, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Display
            txtDisplay = new TextBox
            {
                Location = new Point(10, 30),
                Size = new Size(180, 30),
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = true,
                Text = "0",
                BackColor = Color.White
            };
            this.Controls.Add(txtDisplay);

            // Button layout (4x4 grid)
            int btnSize = 40;
            int spacing = 5;
            int startX = 10;
            int startY = 70;

            // Row 1: 7, 8, 9, /
            CreateButton("7", startX, startY, btnSize, NumberClick);
            CreateButton("8", startX + (btnSize + spacing), startY, btnSize, NumberClick);
            CreateButton("9", startX + (btnSize + spacing) * 2, startY, btnSize, NumberClick);
            CreateButton("/", startX + (btnSize + spacing) * 3, startY, btnSize, OperatorClick, Color.LightBlue);

            // Row 2: 4, 5, 6, *
            startY += btnSize + spacing;
            CreateButton("4", startX, startY, btnSize, NumberClick);
            CreateButton("5", startX + (btnSize + spacing), startY, btnSize, NumberClick);
            CreateButton("6", startX + (btnSize + spacing) * 2, startY, btnSize, NumberClick);
            CreateButton("*", startX + (btnSize + spacing) * 3, startY, btnSize, OperatorClick, Color.LightBlue);

            // Row 3: 1, 2, 3, -
            startY += btnSize + spacing;
            CreateButton("1", startX, startY, btnSize, NumberClick);
            CreateButton("2", startX + (btnSize + spacing), startY, btnSize, NumberClick);
            CreateButton("3", startX + (btnSize + spacing) * 2, startY, btnSize, NumberClick);
            CreateButton("-", startX + (btnSize + spacing) * 3, startY, btnSize, OperatorClick, Color.LightBlue);

            // Row 4: C, 0, =, +
            startY += btnSize + spacing;
            CreateButton("C", startX, startY, btnSize, ClearClick, Color.LightCoral);
            CreateButton("0", startX + (btnSize + spacing), startY, btnSize, NumberClick);
            CreateButton("=", startX + (btnSize + spacing) * 2, startY, btnSize, EqualsClick, Color.LightGreen);
            CreateButton("+", startX + (btnSize + spacing) * 3, startY, btnSize, OperatorClick, Color.LightBlue);
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

        private void NumberClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            if (txtDisplay.Text == "0" || operationPressed)
            {
                txtDisplay.Text = btn.Text;
                operationPressed = false;
            }
            else
            {
                txtDisplay.Text += btn.Text;
            }
        }

        private void OperatorClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            firstNumber = double.Parse(txtDisplay.Text);
            operation = btn.Text;
            operationPressed = true;
        }

        private void EqualsClick(object sender, EventArgs e)
        {
            secondNumber = double.Parse(txtDisplay.Text);
            double result = 0;

            switch (operation)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;
                case "-":
                    result = firstNumber - secondNumber;
                    break;
                case "*":
                    result = firstNumber * secondNumber;
                    break;
                case "/":
                    if (secondNumber != 0)
                        result = firstNumber / secondNumber;
                    else
                    {
                        txtDisplay.Text = "Error";
                        return;
                    }
                    break;
            }

            txtDisplay.Text = result.ToString("F2");
            operationPressed = true;
        }

        private void ClearClick(object sender, EventArgs e)
        {
            txtDisplay.Text = "0";
            firstNumber = 0;
            secondNumber = 0;
            operation = "";
            operationPressed = false;
        }
    }
}