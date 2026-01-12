using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace AKEndfieldDmgCalc.Helpers
{
  
    public class CollapsibleSection
    {
        private Panel headerPanel;
        private Panel contentPanel;
        private Label lblTitle;
        private Label lblToggle;
        private bool isExpanded = true;
        private int collapsedHeight = 35;
        private int expandedHeight;
        private Action onToggleCallback;

        public Panel Panel { get; private set; }
        public bool IsExpanded => isExpanded;

        public CollapsibleSection(Control parent, string title, int x, int y, int width, Color headerColor, Action onToggle = null)
        {
            onToggleCallback = onToggle;

         
            Panel = new Panel
            {
                Location = new Point(x, y),
                Width = width,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

          
            headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(width - 2, collapsedHeight),
                BackColor = headerColor,
                Cursor = Cursors.Hand
            };
            headerPanel.Click += Toggle;

            lblTitle = new Label
            {
                Text = title,
                Location = new Point(35, 8),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            lblTitle.Click += Toggle;
            headerPanel.Controls.Add(lblTitle);

            lblToggle = new Label
            {
                Text = "▼",
                Location = new Point(10, 8),
                Size = new Size(20, 20),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            lblToggle.Click += Toggle;
            headerPanel.Controls.Add(lblToggle);

          
            contentPanel = new Panel
            {
                Location = new Point(0, collapsedHeight),
                Width = width - 2,
                AutoSize = false,
                BackColor = Color.White
            };

            Panel.Controls.Add(headerPanel);
            Panel.Controls.Add(contentPanel);
            parent.Controls.Add(Panel);

            isExpanded = true;
        }

        public void AddControl(Control control)
        {
            contentPanel.Controls.Add(control);
            RecalculateHeight();
        }

        public void AddControls(params Control[] controls)
        {
            foreach (var control in controls)
            {
                contentPanel.Controls.Add(control);
            }
            RecalculateHeight();
        }

        private void Toggle(object sender, EventArgs e)
        {
            isExpanded = !isExpanded;

            if (isExpanded)
            {
                Expand();
            }
            else
            {
                Collapse();
            }

           
            onToggleCallback?.Invoke();
        }

      
        public void Expand()
        {
            isExpanded = true;
            lblToggle.Text = "▼";
            contentPanel.Visible = true;
            Panel.Height = expandedHeight;
        }

        public void Collapse()
        {
            isExpanded = false;
            lblToggle.Text = "▶";
            contentPanel.Visible = false;
            Panel.Height = collapsedHeight;
        }

        private void RecalculateHeight()
        {
            if (contentPanel.Controls.Count == 0)
            {
                expandedHeight = collapsedHeight;
                contentPanel.Height = 0;
                return;
            }

         
            int maxBottom = 0;
            foreach (Control ctrl in contentPanel.Controls)
            {
                int bottom = ctrl.Location.Y + ctrl.Height;
                if (bottom > maxBottom)
                    maxBottom = bottom;
            }

            contentPanel.Height = maxBottom + 10; 
            expandedHeight = collapsedHeight + contentPanel.Height;
            Panel.Height = isExpanded ? expandedHeight : collapsedHeight;
        }

        public void SetY(int y)
        {
            Panel.Location = new Point(Panel.Location.X, y);
        }

   
        public int GetBottom()
        {
            return Panel.Location.Y + Panel.Height;
        }
    }

    public class CollapsibleSectionManager
    {
        private List<CollapsibleSection> sections = new List<CollapsibleSection>();
        private int spacing = 10;

        public void AddSection(CollapsibleSection section)
        {
            sections.Add(section);
            section.Panel.LocationChanged += (s, e) => RepositionSections();
            section.Panel.SizeChanged += (s, e) => RepositionSections();
        }

        
        public void RepositionSections()
        {
            if (sections.Count == 0) return;

            int currentY = sections[0].Panel.Location.Y;

            foreach (var section in sections)
            {
                section.SetY(currentY);
                currentY = section.GetBottom() + spacing;
            }
        }

        public void ExpandAll()
        {
            foreach (var section in sections)
            {
                section.Expand();
            }
            RepositionSections();
        }

     
        public void CollapseAll()
        {
            foreach (var section in sections)
            {
                section.Collapse();
            }
            RepositionSections();
        }
    }
}