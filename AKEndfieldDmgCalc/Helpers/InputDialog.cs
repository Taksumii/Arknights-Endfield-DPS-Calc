using System.Windows.Forms;
using System.Drawing;

namespace AKEndfieldDmgCalc.Helpers
{
   
    /// Input dialog for getting text input from user
    
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

            Label textLabel = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 340,
                Text = text
            };

            TextBox textBox = new TextBox()
            {
                Left = 20,
                Top = 50,
                Width = 340
            };

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
}