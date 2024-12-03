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

            this.uusiKilpailuLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));
            this.uusiKilpailuLajiComboBox.SelectedIndex = 0;
            this.kilpailunTyyppiComboBox.SelectedIndex = 0;
            this.rankingComboBox.SelectedIndex = 0;
            this.rankingCheckBox.Checked = true;

            this.kilpailunNimiTextBox.Text = string.Format("Kaisan viikkokilpailu {0}.{1}.{2}",
                DateTime.Now.Day,
                DateTime.Now.Month,
                DateTime.Now.Year);
        }

        public string Nimi { get { return this.kilpailunNimiTextBox.Text; } }
        public bool LuoViikkokisa { get { return this.kilpailunTyyppiComboBox.SelectedIndex == 0; } }
        public bool RankingKisa { get { return this.rankingCheckBox.Checked; } }
        public Ranking.RankingSarjanPituus RankingKisatyyppi 
        {
            get
            {
                switch (this.rankingComboBox.SelectedIndex)
                {
                    case 0: default: return Ranking.RankingSarjanPituus.Kuukausi;
                    case 1: return Ranking.RankingSarjanPituus.Vuodenaika;
                    case 2: return Ranking.RankingSarjanPituus.Puolivuotta;
                    case 3: return Ranking.RankingSarjanPituus.Vuosi;
                }
            }
        }
        public Laji Laji
        {
            get
            {
                return (Laji)this.uusiKilpailuLajiComboBox.SelectedItem;
            }
        }

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

        private void kilpailunTyyppiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Viikkokisa
            if (this.kilpailunTyyppiComboBox.SelectedIndex == 0)
            {
                this.rankingLabel.Visible = this.rankingCheckBox.Checked;
                this.rankingCheckBox.Visible = true;
                this.rankingComboBox.Visible = this.rankingCheckBox.Checked;
            }
            
            // SBiL kisa
            else
            {
                this.rankingLabel.Visible = false;
                this.rankingCheckBox.Visible = false;
                this.rankingComboBox.Visible = false;
            }
        }

        private void rankingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.rankingLabel.Visible = this.rankingCheckBox.Checked;
            this.rankingComboBox.Visible = this.rankingCheckBox.Checked;
        }
    }
}
