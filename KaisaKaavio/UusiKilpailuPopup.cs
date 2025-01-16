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
        private bool avattu = false;
        private string oletusNimi = string.Empty;
        private bool nimeaMuokattuManuaalisesti = false;
        private Asetukset.KisaOletusasetukset asetukset = null;

        public UusiKilpailuPopup()
        {
            InitializeComponent();

            this.uusiKilpailuLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));
            this.uusiKilpailuLajiComboBox.SelectedIndex = 0;

            this.kilpaSarjaComboBox.DataSource = Enum.GetValues(typeof(KilpaSarja));
            this.kilpaSarjaComboBox.SelectedIndex = 0;

            this.kilpailunTyyppiComboBox.SelectedIndex = 0;
            this.kaavioComboBox.SelectedIndex = 0;

            this.rankingComboBox.SelectedIndex = 0;
            this.rankingCheckBox.Checked = false;
            this.rankingComboBox.Visible = false;

            this.alkamisAikaDateTimePicker.Value = DateTime.Today;

            PaivitaKilpailunOletusNimi();
        }

        private void PaivitaUiLajille(Laji laji, bool oletusArvot)
        {
            switch (laji)
            {
                case KaisaKaavio.Laji.Kara:
                    this.alaLajiComboBox.DataSource = new string[] { "Kolmen vallin kara", "Suora kara" };
                    this.tavoiteLabel.Text = "karaan";

                    if (oletusArvot)
                    {
                        this.peliAikaCheckBox.Checked = true;
                        this.peliAikaNumericUpDown.Value = 40;
                        this.tavoiteNumericUpDown.Value = 20;
                        this.rankingCheckBox.Checked = true;
                        this.rankingComboBox.SelectedIndex = 0;
                    }
                    break;

                case KaisaKaavio.Laji.Kaisa:
                    this.alaLajiComboBox.DataSource = null;
                    this.tavoiteLabel.Text = "pisteeseen";

                    if (oletusArvot)
                    {
                        this.peliAikaCheckBox.Checked = true;
                        this.peliAikaNumericUpDown.Value = 40;
                        this.tavoiteNumericUpDown.Value = 60;
                        this.rankingCheckBox.Checked = true;
                        this.rankingComboBox.SelectedIndex = 0;
                    }
                    break;

                case KaisaKaavio.Laji.Pyramidi:
                    this.alaLajiComboBox.DataSource = new string[] { "Amerikanka", "Nevskaja", "Moskovskaja", "Corona", "Straight (15 ball)" };
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 3;
                        this.peliAikaCheckBox.Checked = true;
                        this.peliAikaNumericUpDown.Value = 40;
                        this.rankingCheckBox.Checked = true;
                        this.rankingComboBox.SelectedIndex = 0;
                    }
                    break;

                case KaisaKaavio.Laji.Pool:
                    this.alaLajiComboBox.DataSource = new string[] 
                    { 
                        "9-ball", "10-ball", "8-ball", "14.1 (straightpool)", 
                        "One pocket", "Rotation", "Bankpool (15 balls)", "Bankpool (9 balls)",
                        "Multiball", "7-ball", "Taiwanese carom"
                    };
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 4;
                        this.peliAikaCheckBox.Checked = false;
                        this.peliAikaNumericUpDown.Value = 0;
                        this.rankingCheckBox.Checked = false;
                        this.rankingComboBox.SelectedIndex = 0;
                    }
                    break;

                case KaisaKaavio.Laji.Heyball:
                    this.alaLajiComboBox.DataSource = null;
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 3;
                        this.peliAikaCheckBox.Checked = false;
                        this.peliAikaNumericUpDown.Value = 0;
                        this.rankingCheckBox.Checked = false;
                        this.rankingComboBox.SelectedIndex = 0;
                    }
                    break;

                case KaisaKaavio.Laji.Snooker:
                    this.alaLajiComboBox.DataSource = new string[] { "Snooker", "Six reds", "Ten reds" };
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 2;
                        this.peliAikaCheckBox.Checked = false;
                        this.peliAikaNumericUpDown.Value = 0;
                        this.rankingCheckBox.Checked = false;
                        this.rankingComboBox.SelectedIndex = 0;
                    }
                    break;
            }
        }

        public void AsetaOletusarvot(Asetukset.KisaOletusasetukset asetukset, Laji laji, KilpailunTyyppi kilpailunTyyppi, bool salliVaihtaa)
        {
            try
            {
                this.asetukset = asetukset;

                this.uusiKilpailuLajiComboBox.SelectedItem = laji;
                this.uusiKilpailuLajiComboBox.Enabled = salliVaihtaa;

                if (kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa)
                {
                    this.kaavioComboBox.SelectedIndex = 2;
                }
                else
                {
                    this.kaavioComboBox.SelectedIndex = 0;
                }

                this.kilpailunTyyppiComboBox.SelectedIndex = (int) kilpailunTyyppi;
                this.kilpailunTyyppiComboBox.Enabled = false;

                if (kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu ||
                    kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                {
                    this.kilpasarjaLabel.Visible = true;
                    this.kilpaSarjaComboBox.Visible = true;
                }
                else
                {
                    this.kilpasarjaLabel.Visible = false;
                    this.kilpaSarjaComboBox.Visible = false;
                }

                this.alaLajiComboBox.DataSource = null;

                switch (laji)
                {
                    case KaisaKaavio.Laji.Kara:
                        this.alaLajiComboBox.DataSource = new string[] { "Kolmen vallin kara", "Suora kara" };
                        this.tavoiteLabel.Text = "karaan";
                        break;
                }

                if (this.asetukset != null)
                {
                    this.tavoiteNumericUpDown.Value = this.asetukset.Tavoite;
                    this.rankingCheckBox.Checked = this.asetukset.RankingSarja;
                    this.peliAikaCheckBox.Checked = this.asetukset.PeliaikaRajattu;
                    this.peliAikaNumericUpDown.Value = this.asetukset.Peliaika;
                    this.rankingComboBox.SelectedIndex = (int)this.asetukset.RankingSarjanTyyppi;
                    this.kaavioComboBox.SelectedIndex = (int)this.asetukset.KaavioTyyppi;
                    try
                    {
                        this.alaLajiComboBox.SelectedItem = asetukset.Alalaji;
                    }
                    catch
                    { 
                    }
                }

                this.lajiSplitContainer.Panel2Collapsed = this.alaLajiComboBox.DataSource == null;
            }
            catch
            { 
            }
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

        public KaavioTyyppi KaavioTyyppi
        {
            get
            {
                if (this.kaavioComboBox.SelectedIndex >= 0)
                {
                    return (KaavioTyyppi)Enum.GetValues(typeof(KaavioTyyppi)).GetValue(this.kaavioComboBox.SelectedIndex);
                }
                else
                {
                    return KaavioTyyppi.Pudari3Kierros;
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
            if (this.asetukset != null)
            {
                try
                {
                    this.asetukset.Alalaji = this.alaLajiComboBox.SelectedItem != null ? this.alaLajiComboBox.SelectedItem.ToString() : string.Empty;
                    this.asetukset.Peliaika = (int)this.peliAikaNumericUpDown.Value;
                    this.asetukset.PeliaikaRajattu = this.peliAikaCheckBox.Checked;
                    this.asetukset.RankingSarja = this.rankingCheckBox.Checked;
                    this.asetukset.Tavoite = (int)this.tavoiteNumericUpDown.Value;
                    this.asetukset.RankingSarjanTyyppi = this.RankingKisatyyppi;
                    this.asetukset.KaavioTyyppi = this.KaavioTyyppi;
                }
                catch
                { 
                }
            }

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
                this.rankingCheckBox.Visible = true;
                this.rankingComboBox.Visible = this.rankingCheckBox.Checked;

                this.kilpasarjaLabel.Visible = false;
                this.kilpaSarjaComboBox.Visible = false;
            }
            
            // Avoin kisa tai SBiL kisa
            else
            {
                this.rankingCheckBox.Visible = false;
                this.rankingComboBox.Visible = false;

                this.kilpasarjaLabel.Visible = true;
                this.kilpaSarjaComboBox.Visible = true;
            }

            PaivitaKilpailunOletusNimi();
        }

        private void rankingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
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
            if (this.avattu)
            {
                if (!string.Equals(this.kilpailunNimiTextBox.Text, this.oletusNimi))
                {
                    this.nimeaMuokattuManuaalisesti = true;
                }
            }
        }

        private void alkamisAikaDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            PaivitaKilpailunOletusNimi();
        }

        private void kilpaSarjaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaivitaKilpailunOletusNimi();
        }

        private void UusiKilpailuPopup_Shown(object sender, EventArgs e)
        {
            this.avattu = true;
            this.oletusNimi = this.kilpailunNimiTextBox.Text;
        }
    }
}
