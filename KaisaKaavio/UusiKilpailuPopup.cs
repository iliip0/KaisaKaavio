using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class UusiKilpailuPopup : Form
    {
        public UusiKilpailuPopup()
        {
            InitializeComponent();

            this.kilpailunTyyppiComboBox.SelectedIndex = 0;

            this.kilpailunNimiTextBox.Text = string.Format("Kaisan viikkokilpailu {0}.{1}.{2}",
                DateTime.Now.Day,
                DateTime.Now.Month,
                DateTime.Now.Year);
        }

        public string Nimi { get { return this.kilpailunNimiTextBox.Text; } }
        public bool LuoViikkokisa { get { return this.kilpailunTyyppiComboBox.SelectedIndex == 0; } }

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void uusiKilpailuButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void kilpailunNimiTextBox_TextChanged(object sender, EventArgs e)
        {
            this.uusiKilpailuButton.Enabled = !string.IsNullOrEmpty(this.kilpailunNimiTextBox.Text);
        }
    }
}
