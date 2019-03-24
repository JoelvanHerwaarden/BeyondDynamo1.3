using System;
using System.Drawing;
using System.Windows.Forms;

namespace BeyondDynamo.UI.About
{
    public partial class About : Form
    {
        /// <summary>
        ///  Public constructor for the about Form
        /// </summary>
        public About(double version)
        {
            InitializeComponent();
            versionLabel.Text = "Version " + version.ToString();
        }
        

        #region LinkedIn Label Events
        private void LinkedIn_Label_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(LinkedIn_Label.Text);
        }

        private void LinkedIn_Label_MouseEnter(object sender, EventArgs e)
        {
            LinkedIn_Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFC8C8C8");
        }

        private void LinkedIn_Label_MouseLeave(object sender, EventArgs e)
        {
            LinkedIn_Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
        }
        #endregion
                

        #region GitHub Label Events
        private void GitHub_Label_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("www.github.com/JoelvanHerwaarden/BeyondDynamo1.3");
        }

        private void GitHub_Label_MouseEnter(object sender, EventArgs e)
        {
            GitHub_Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFC8C8C8");
        }

        private void GitHub_Label_MouseLeave(object sender, EventArgs e)
        {
            GitHub_Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
        }
        #endregion
        
    }
}
