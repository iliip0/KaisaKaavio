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
        private bool nimeaMuokattuManuaalisesti = false;

        public UusiKilpailuPopup()
        {
            InitializeComponent();

            this.uusiKilpailuLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));
            this.uusiKilpailuLajiComboBox.SelectedIndex = 0;

            this.kilpaSarjaComboBox.DataSource = Enum.GetValues(typeof(KilpaSarja));
            this.kilpaSarjaComboBox.SelectedIndex = 0;

            this.kilpailunTyyppiComboBox.SelectedIndex = 0;
            this.rankingComboBox.SelectedIndex = 0;
            this.rankingCheckBox.Checked = false;
            this.rankingComboBox.Visible = false;

            this.alkamisAikaDateTimePicker.Value = DateTime.Today;

            PaivitaKilpailunOletusNimi();
        }

        public string Nimi { get { return this.kilpailunNimiTextBox.Text; } }
        public bool LuoViikkokisa { get { return this.kilpailunTyyppiComboBox.SelectedIndex == 0; } }
        public KilpailunTyyppi KilpailunTyyppi 
        { 
            get 
            {
                if (this.kilpailunTyyppiComboBox.SelectedIndex >= 0)
                {
                    return (KilpailunTyyppi)Enum.GetValues(typeof(KilpailunTyyppi)).GetValue(this.kilpailunTyyppiComboBox.SelectedIndex);
                }
                else
                {
                    return KaisaKaavio.KilpailunTyyppi.Viikkokisa;
                }
            }
        }

        public KilpaSarja KilpaSarja 
        { 
            get 
            {
                if (this.kilpaSarjaComboBox.SelectedIndex >= 0)
                {
                    return (KilpaSarja)Enum.GetValues(typeof(KilpaSarja)).GetValue(this.kilpaSarjaComboBox.SelectedIndex);
                }
                else 
                {
                    return KaisaKaavio.KilpaSarja.Yleinen;
                }
            }
        }

        public bool RankingKisa { get { return this.rankingCheckBox.Checked; } }
        public string Aika { get { return Tyypit.Aika.DateTimeToString(this.alkamisAikaDateTimePicker.Value); } }

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

                this.kilpasarjaLabel.Visible = false;
                this.kilpaSarjaComboBox.Visible = false;
            }
            
            // Avoin kisa tai SBiL kisa
            else
            {
                this.rankingLabel.Visible = false;
                this.rankingCheckBox.Visible = false;
                this.rankingComboBox.Visible = false;

                this.kilpasarjaLabel.Visible = true;
                this.kilpaSarjaComboBox.Visible = true;
            }

            PaivitaKilpailunOletusNimi();
        }

        private void rankingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.rankingLabel.Visible = this.rankingCheckBox.Checked;
            this.rankingComboBox.Visible = this.rankingCheckBox.Checked;
        }

        private void PaivitaKilpailunOletusNimi()
        {
            if (!this.nimeaMuokattuManuaalisesti)
            {
                string kilpatyyppi = "kilpailu";

                switch (this.KilpailunTyyppi)
                {
                    case KilpailunTyyppi.Viikkokisa: kilpatyyppi = "viikkokisa"; break;
                    case KaisaKaavio.KilpailunTyyppi.AvoinKilpailu: kilpatyyppi = "avoin kilpailu"; break;
                    case KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu: kilpatyyppi = "RG osakilpailu"; break;
                    case KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu: kilpatyyppi = "SM kilpailu"; break;
                }

                if (this.KilpaSarja != KaisaKaavio.KilpaSarja.Yleinen)
                { 
                    kilpatyyppi = kilpatyyppi + " (" + Enum.GetName(typeof(KilpaSarja), this.KilpaSarja) +")";
                }

                string aika = Tyypit.Aika.DateTimeToString(this.alkamisAikaDateTimePicker.Value);
                string laji = "Kaisan";

                switch (Laji)
                {
                    case KaisaKaavio.Laji.Kaisa: laji = "Kaisan"; break;
                    case KaisaKaavio.Laji.Kara: laji = "Karan"; break;
                    case KaisaKaavio.Laji.Heyball: laji = "Heyball"; break;
                    case KaisaKaavio.Laji.Pool: laji = "Poolin"; break;
                    case KaisaKaavio.Laji.Pyramidi: laji = "Pyramidin"; break;
                    case KaisaKaavio.Laji.Snooker: laji = "Snookerin"; break;
                }

                this.kilpailunNimiTextBox.Text = string.Format("{0} {1} {2}", laji, kilpatyyppi, aika);
            }
        }

        private void uusiKilpailuLajiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaivitaKilpailunOletusNimi();
        }

        private void kilpailunNimiTextBox_Validated(object sender, EventArgs e)
        {
            this.nimeaMuokattuManuaalisesti = true;
        }

        private void alkamisAikaDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            PaivitaKilpailunOletusNimi();
        }

        private void kilpaSarjaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaivitaKilpailunOletusNimi();
        }
    }
}
