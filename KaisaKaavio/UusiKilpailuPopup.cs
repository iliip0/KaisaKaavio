using KaisaKaavio.Ranking;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class UusiKilpailuPopup : Form
    {
        private bool avattu = false;
        private string oletusNimi = string.Empty;
        private bool nimeaMuokattuManuaalisesti = false;
        private Asetukset.KisaOletusasetukset asetukset = null;
        private Asetukset yleisAsetukset = null;
        private AutoCompleteStringCollection pelipaikkojenNimet = null;

        public bool LuoTestikilpailu { get; private set; }

        public UusiKilpailuPopup(Asetukset asetukset)
        {
            this.yleisAsetukset = asetukset;

            InitializeComponent();

            this.uusiKilpailuLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));
            this.uusiKilpailuLajiComboBox.SelectedIndex = 0;

            this.kilpaSarjaComboBox.DataSource = Enum.GetValues(typeof(KilpaSarja));
            this.kilpaSarjaComboBox.SelectedIndex = 0;

            this.nakyvyysComboBox.DataSource = Enum.GetValues(typeof(Nakyvyys));
            this.nakyvyysComboBox.SelectedIndex = 0;

            this.kilpailunTyyppiComboBox.SelectedIndex = 0;

            this.rankingComboBox.DataSource = Enum.GetValues(typeof(RankingSarjanTyyppi));
            this.rankingComboBox.SelectedIndex = 0;

            this.kaavioComboBox.DataSource = Enum.GetValues(typeof(KaavioTyyppi));
            this.kaavioComboBox.SelectedIndex = 2;

            this.alkamisAikaDateTimePicker.Value = DateTime.Today;

            this.virheLabel.Text = string.Empty;
            this.virheLabel.Visible = false;

            this.LuoTestikilpailu = false;

            this.pelipaikkojenNimet = new AutoCompleteStringCollection();

            PaivitaKilpailunOletusNimi();
        }

        private void PaivitaUiLajille(Laji laji, bool oletusArvot)
        {
            this.alaLajiComboBox.DataSource = null;
            bool viikkokisa = this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa;

            if (laji == KaisaKaavio.Laji.Kara)
            {
                peliAikaCheckBox.Text = "Lyöntimäärä rajattu?";
                peliaikaLabel.Text = "lyöntivuoroa";
            }
            else
            {
                peliAikaCheckBox.Text = "Peliaika rajattu?";
                peliaikaLabel.Text = "minuuttia";
            }

            switch (laji)
            {
                case KaisaKaavio.Laji.Kara:
                    this.lajiPictureBox.BackgroundImage = Properties.Resources.KaraMV;
                    this.alaLajiComboBox.DataSource = new string[] { "Kolmen vallin kara", "Suora kara" };
                    this.tavoiteLabel.Text = "karaan";

                    if (oletusArvot)
                    {
                        this.peliAikaCheckBox.Checked = true;
                        this.peliAikaNumericUpDown.Value = viikkokisa ? 20 : 40;
                        this.tavoiteNumericUpDown.Value = viikkokisa ? 20 : 30;
                        this.rankingComboBox.SelectedIndex = viikkokisa ? 1 : 0;
                        this.kaavioComboBox.SelectedIndex = viikkokisa ? 2 : 0;
                    }
                    break;

                case KaisaKaavio.Laji.Kaisa:
                    this.lajiPictureBox.BackgroundImage = Properties.Resources.KaisaMV;
                    this.alaLajiComboBox.DataSource = null;
                    this.tavoiteLabel.Text = "pisteeseen";

                    if (oletusArvot)
                    {
                        this.peliAikaCheckBox.Checked = true;
                        this.peliAikaNumericUpDown.Value = viikkokisa ? 40 : 60;
                        this.tavoiteNumericUpDown.Value = 60;
                        this.rankingComboBox.SelectedIndex = viikkokisa ? 1 : 0;
                        this.kaavioComboBox.SelectedIndex = viikkokisa ? 2 : 0;
                    }
                    break;

                case KaisaKaavio.Laji.Pyramidi:
                    this.lajiPictureBox.BackgroundImage = Properties.Resources.PyramidiMV;
                    this.alaLajiComboBox.DataSource = new string[] { "Amerikanka", "Nevskaja", "Moskovskaja", "Corona", "Straight (15 ball)" };
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 3;
                        this.peliAikaCheckBox.Checked = true;
                        this.peliAikaNumericUpDown.Value = 40;
                        this.rankingComboBox.SelectedIndex = viikkokisa ? 1 : 0;
                        this.kaavioComboBox.SelectedIndex = 2;
                    }
                    break;

                case KaisaKaavio.Laji.Pool:
                    this.lajiPictureBox.BackgroundImage = Properties.Resources.PoolMV;
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
                        this.rankingComboBox.SelectedIndex = 0;
                        this.kaavioComboBox.SelectedIndex = 2;
                    }
                    break;

                case KaisaKaavio.Laji.Heyball:
                    this.lajiPictureBox.BackgroundImage = Properties.Resources.HeyballMV;
                    this.alaLajiComboBox.DataSource = new string[] { "Chinese 8-ball", "Chinese Snooker (10 reds)", "Chinese Snooker (6 reds)" };
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 3;
                        this.peliAikaCheckBox.Checked = false;
                        this.peliAikaNumericUpDown.Value = 0;
                        this.rankingComboBox.SelectedIndex = 0;
                        this.kaavioComboBox.SelectedIndex = 2;
                    }
                    break;

                case KaisaKaavio.Laji.Snooker:
                    this.lajiPictureBox.BackgroundImage = Properties.Resources.SnookerMV;
                    this.alaLajiComboBox.DataSource = new string[] { "Snooker", "Six reds", "Ten reds" };
                    this.tavoiteLabel.Text = "erävoittoon";

                    if (oletusArvot)
                    {
                        this.tavoiteNumericUpDown.Value = 2;
                        this.peliAikaCheckBox.Checked = false;
                        this.peliAikaNumericUpDown.Value = 0;
                        this.rankingComboBox.SelectedIndex = 0;
                        this.kaavioComboBox.SelectedIndex = 2;
                    }
                    break;
            }

            this.lajiSplitContainer.Panel2Collapsed = this.alaLajiComboBox.DataSource == null;
            this.peliAikaNumericUpDown.Visible = this.peliAikaCheckBox.Checked;
            this.peliaikaLabel.Visible = this.peliAikaCheckBox.Checked;

            PaivitaOletusKansio(laji);
        }

        private void PaivitaOletusKansio(Laji laji)
        {
            switch (this.KilpailunTyyppi)
            {
                case KaisaKaavio.KilpailunTyyppi.Viikkokisa:
                    this.kansioTextBox.Text = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "KaisaKaaviot",
                        string.Format("Viikkokisat {0} {1}", laji, DateTime.Now.Year));
                    break;

                case KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu:
                case KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu:
                    this.kansioTextBox.Text = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "KaisaKaaviot",
                        string.Format("SBiL kilpailut {0}", DateTime.Now.Year));
                    break;

                case KaisaKaavio.KilpailunTyyppi.AvoinKilpailu:
                    this.kansioTextBox.Text = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "KaisaKaaviot",
                        string.Format("Avoimet kilpailut {0}", DateTime.Now.Year));
                    break;

                default:
                    this.kansioTextBox.Text = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "KaisaKaaviot",
                        string.Format("Muut kilpailut {0}", DateTime.Now.Year));
                    break;
            }
        }

        public void AsetaOletusarvot(Asetukset.KisaOletusasetukset asetukset, Laji laji, KilpailunTyyppi kilpailunTyyppi, bool salliVaihtaa, bool luoTestiKilpailu)
        {
            try
            {
                if (luoTestiKilpailu)
                {
                    this.Text = "Luo uusi testikilpailu";
#if DEBUG
                    this.nakyvyysComboBox.SelectedIndex = (int)Nakyvyys.Kaikille;
                    this.onlineGroupBox.Visible = true;
#else
                    this.nakyvyysComboBox.SelectedIndex = (int)Nakyvyys.Offline;
                    this.onlineGroupBox.Visible = false;
#endif

                    this.Paikka = this.yleisAsetukset.TestiPelipaikka;
                }
                else
                {
                    this.Text = "Luo uusi kilpailu";
                    this.nakyvyysComboBox.SelectedIndex = (int)Nakyvyys.Kaikille;
                    this.nakyvyysComboBox.Enabled = true;
                    this.onlineGroupBox.Visible = true;
                    this.Paikka = this.yleisAsetukset.Pelipaikka;
                }

                this.kansioTextBox.Visible = !luoTestiKilpailu;
                this.kansioButton.Visible = !luoTestiKilpailu;
                this.kansioPictureBox.Visible = !luoTestiKilpailu;
                this.kansioLabel.Visible = !luoTestiKilpailu;

                this.asetukset = asetukset;

                this.LuoTestikilpailu = luoTestiKilpailu;

                this.uusiKilpailuLajiComboBox.SelectedItem = laji;
                this.uusiKilpailuLajiComboBox.Enabled = luoTestiKilpailu || salliVaihtaa;

                if (kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa)
                {
                    this.kaavioComboBox.SelectedIndex = 2;
                    this.kelloTextBox.Text = "18:00";
                }
                else
                {
                    this.kaavioComboBox.SelectedIndex = 0;
                    this.kelloTextBox.Text = "10:00";
                }

                if (laji == KaisaKaavio.Laji.Kaisa &&
                    (kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu ||
                    kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu))
                {
                    this.kaavioComboBox.Enabled = false;
                }

                this.kilpailunTyyppiComboBox.SelectedIndex = (int) kilpailunTyyppi;
                this.kilpailunTyyppiComboBox.Enabled = luoTestiKilpailu;

                if (luoTestiKilpailu ||
                    kilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu ||
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

                PaivitaUiLajille(laji, true);

                if (this.asetukset != null && 
                    this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa &&
                    !this.LuoTestikilpailu)
                {
                    this.tavoiteNumericUpDown.Value = this.asetukset.Tavoite;
                    this.peliAikaCheckBox.Checked = this.asetukset.PeliaikaRajattu;
                    this.peliAikaNumericUpDown.Value = this.asetukset.Peliaika;
                    this.rankingComboBox.SelectedIndex = (int)this.asetukset.RankingSarjanTyyppi;
                    this.nakyvyysComboBox.SelectedIndex = (int)this.asetukset.Nakyvyys;
                    this.kelloTextBox.Text = this.asetukset.AlkamisAika;
                    
                    if (this.rankingComboBox.SelectedIndex == (int)RankingSarjanTyyppi.Vapaamuotoinen)
                    {
                        this.rankingSarjanNimiTextBox.Text = this.asetukset.RankingSarjanNimi;
                    }

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
                this.nimeaMuokattuManuaalisesti = false;

                PaivitaKilpailunOletusNimi();

                PaivitaOnlineKenttienNakyvyys();
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
                return (KaavioTyyppi)this.kaavioComboBox.SelectedItem;
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

        public string Kansio
        {
            get { return this.kansioTextBox.Text; }
        }

        public string Paikka
        {
            get { return this.peliPaikkaComboBox.Text; }
            private set { this.peliPaikkaComboBox.Text = value != null ? value : string.Empty; }
        }

        public Nakyvyys Nakyvyys
        {
            get { return (Nakyvyys)Enum.GetValues(typeof(Nakyvyys)).GetValue(this.nakyvyysComboBox.SelectedIndex); }
        }

        public string Kellonaika
        {
            get
            {
                return this.kelloTextBox.Text;
            }
        }

        public bool RankingKisa { get { return this.rankingComboBox.SelectedIndex != (int)RankingSarjanTyyppi.EiRankingSarjaa; } }
        public string Aika { get { return Tyypit.Aika.DateTimeToString(this.alkamisAikaDateTimePicker.Value); } }
        public decimal Tavoite { get { return this.tavoiteNumericUpDown.Value; } }

        public decimal Peliaika 
        {
            get 
            { 
                return this.peliAikaCheckBox.Checked ? this.peliAikaNumericUpDown.Value : 0; 
            }
        }

        public Ranking.RankingSarjanTyyppi RankingKisatyyppi 
        {
            get
            {
                return (Ranking.RankingSarjanTyyppi)this.rankingComboBox.SelectedItem;
            }
        }

        public string RankingSarjanNimi
        {
            get
            {
                return this.rankingSarjanNimiTextBox.Text;
            }
        }

        public Laji Laji
        {
            get
            {
                return (Laji)this.uusiKilpailuLajiComboBox.SelectedItem;
            }
        }

        public string Alalaji
        {
            get
            {
                var result = this.alaLajiComboBox.SelectedItem;
                if (result != null)
                {
                    return result.ToString();
                }
                else 
                {
                    return string.Empty;
                } 
            }
        }

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void uusiKilpailuButton_Click(object sender, EventArgs e)
        {
            if (this.asetukset != null && 
                this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa)
            {
                try
                {
                    if (!this.LuoTestikilpailu)
                    {
                        this.asetukset.Alalaji = this.alaLajiComboBox.SelectedItem != null ? this.alaLajiComboBox.SelectedItem.ToString() : string.Empty;
                        this.asetukset.Peliaika = (int)this.peliAikaNumericUpDown.Value;
                        this.asetukset.PeliaikaRajattu = this.peliAikaCheckBox.Checked;
                        this.asetukset.Tavoite = (int)this.tavoiteNumericUpDown.Value;
                        this.asetukset.RankingSarjanTyyppi = this.RankingKisatyyppi;
                        this.asetukset.Nakyvyys = this.Nakyvyys;
                        this.asetukset.AlkamisAika = this.kelloTextBox.Text;

                        if (this.RankingKisatyyppi == RankingSarjanTyyppi.Vapaamuotoinen)
                        {
                            this.asetukset.RankingSarjanNimi = this.RankingSarjanNimi;
                        }

                        this.asetukset.KaavioTyyppi = this.KaavioTyyppi;
                        this.yleisAsetukset.Pelipaikka = this.Paikka;
                    }
                    else
                    {
                        this.yleisAsetukset.TestiPelipaikka = this.Paikka;
                    }
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
            TarkistaTiedot();
        }

        private void kilpailunTyyppiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Viikkokisa
            if (this.kilpailunTyyppiComboBox.SelectedIndex == 0)
            {
                this.rankingLabel.Visible = true;
                this.rankingComboBox.Visible = true;
                this.rankingSarjanNimiLabel.Visible = this.rankingComboBox.SelectedIndex == (int)RankingSarjanTyyppi.Vapaamuotoinen;
                this.rankingSarjanNimiTextBox.Visible = this.rankingComboBox.SelectedIndex == (int)RankingSarjanTyyppi.Vapaamuotoinen;

                this.kilpasarjaLabel.Visible = false;
                this.kilpaSarjaComboBox.Visible = false;

                this.kaavioComboBox.Enabled = true;
            }
            
            // Avoin kisa tai SBiL kisa
            else
            {
                this.rankingLabel.Visible = false;
                this.rankingComboBox.Visible = false;
                this.rankingSarjanNimiTextBox.Visible = false;
                this.rankingSarjanNimiLabel.Visible = false;

                this.kilpasarjaLabel.Visible = true;
                this.kilpaSarjaComboBox.Visible = true;

                this.kaavioComboBox.SelectedIndex = 0;
                this.kaavioComboBox.Enabled = this.Laji != Laji.Kaisa || this.kilpailunTyyppiComboBox.SelectedIndex <= 1;
                
                if (this.Laji == Laji.Kaisa)
                {
                    this.peliAikaNumericUpDown.Value = 60;
                    this.tavoiteNumericUpDown.Value = 60;
                }
            }

            PaivitaKilpailunOletusNimi();
            PaivitaOletusKansio(this.Laji);
            TarkistaTiedot();
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

                if (this.LuoTestikilpailu)
                {
                    this.kilpailunNimiTextBox.Text += " (TESTI)";
                }
            }
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
            TarkistaTiedot();
        }

        private void UusiKilpailuPopup_Shown(object sender, EventArgs e)
        {
            var nimet = this.yleisAsetukset.SaliTietueet
                .Where(x => !string.IsNullOrEmpty(x.Lyhenne))
                .OrderBy(x => x.Lyhenne)
                .Select(x => x.Lyhenne)
                .Distinct()
                .ToArray();

            this.pelipaikkojenNimet.Clear();
            this.pelipaikkojenNimet.AddRange(nimet);

            this.peliPaikkaComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.peliPaikkaComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.peliPaikkaComboBox.DataSource = pelipaikkojenNimet;
            //this.peliPaikkaComboBox.AutoCompleteCustomSource = this.pelipaikkojenNimet;

            this.peliPaikkaComboBox.Text = this.yleisAsetukset.Pelipaikka;

            this.avattu = true;
            this.oletusNimi = this.kilpailunNimiTextBox.Text;

            PaivitaOnlineKenttienNakyvyys();

            PaivitaUiLajille((KaisaKaavio.Laji)uusiKilpailuLajiComboBox.SelectedItem, false);
            TarkistaTiedot();
        }

        private void uusiKilpailuLajiComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PaivitaUiLajille((Laji)this.uusiKilpailuLajiComboBox.SelectedItem, true);
            PaivitaKilpailunOletusNimi();
            TarkistaTiedot();
        }

        private void peliAikaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.peliAikaNumericUpDown.Visible = this.peliAikaCheckBox.Checked;
            this.peliaikaLabel.Visible = this.peliAikaCheckBox.Checked;
        }

        private void kansioTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void kansioButton_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(this.kansioTextBox.Text);
                this.folderBrowserDialog1.SelectedPath = this.kansioTextBox.Text;
            }
            catch
            {
                this.folderBrowserDialog1.SelectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot");
            }

            var valinta = this.folderBrowserDialog1.ShowDialog();
            if (valinta == System.Windows.Forms.DialogResult.OK)
            {
                this.kansioTextBox.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void uusiKilpailuLajiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaivitaOletusKansio(this.Laji);
            TarkistaTiedot();
        }

        private bool NaytaVirhe(Control namiska, string teksti)
        {
            this.uusiKilpailuButton.Enabled = false;
            this.virheLabel.Text = teksti;
            this.virheLabel.Visible = true;

            if (namiska != null)
            {
                this.errorProvider1.SetError(namiska, teksti);
            }

            return false;
        }

        private bool TarkistaTiedot()
        {
            this.uusiKilpailuButton.Enabled = true;
            this.virheLabel.Visible = false;
            this.errorProvider1.SetError(this.peliPaikkaComboBox, string.Empty);
            this.errorProvider1.SetError(this.kilpailunTyyppiComboBox, string.Empty);
            this.errorProvider1.SetError(this.kilpaSarjaComboBox, string.Empty);
            this.errorProvider1.SetError(this.kaavioComboBox, string.Empty);
            this.errorProvider1.SetError(this.kilpailunNimiTextBox, string.Empty);
            this.errorProvider1.SetError(this.rankingSarjanNimiTextBox, string.Empty);
            this.errorProvider1.SetError(this.kelloTextBox, string.Empty);

            if (string.IsNullOrEmpty(this.kelloTextBox.Text))
            {
                return NaytaVirhe(this.kelloTextBox, "Kilpailun alkamisaika tarvitaan");
            }

            if (string.IsNullOrEmpty(this.peliPaikkaComboBox.Text))
            {
                return NaytaVirhe(this.peliPaikkaComboBox, "Kirjoita pelipaikan lyhenne \"Pelipaikka\" kenttään (esim EBK tai PVK)");
            }

            if (string.IsNullOrEmpty(this.kilpailunNimiTextBox.Text))
            {
                return NaytaVirhe(this.kilpailunNimiTextBox, "Kilpailun nimi ei voi olla tyhjä");
            }

            switch (this.KilpaSarja)
            {
                case KilpaSarja.Joukkuekilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa ||
                        this.KilpailunTyyppi != KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                    {
                        return NaytaVirhe(this.kilpaSarjaComboBox, "Joukkuekilpailuominaisuus toimii ainoastaan Kaisan Joukkue SM-kilpailussa!");
                    }
                    break;

                case KaisaKaavio.KilpaSarja.MixedDoubles:
                case KaisaKaavio.KilpaSarja.Parikilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa ||
                        this.KilpailunTyyppi != KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                    {
                        return NaytaVirhe(this.kilpaSarjaComboBox, "Parikilpailuominaisuus toimii ainoastaan Kaisan Pari- ja Mixed Doubles SM-kilpailuissa!");
                    }
                    break;
            }

            switch (this.KilpailunTyyppi)
            {
                case KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa)
                    {
                        return NaytaVirhe(this.kilpailunTyyppiComboBox, "SBiL SM-kilpailuominaisuus toimii ainoastaan Kaisa-lajissa!");
                    }
                    break;

                case KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa)
                    {
                        return NaytaVirhe(this.kilpailunTyyppiComboBox, "SBiL RG-kilpailuominaisuus toimii ainoastaan Kaisa-lajissa!");
                    }
                    break;
            }

            switch (this.KaavioTyyppi)
            {
                case KaisaKaavio.KaavioTyyppi.TuplaKaavio:
                    break;

                case KaisaKaavio.KaavioTyyppi.Pudari4Kierros:
                case KaisaKaavio.KaavioTyyppi.Pudari3Kierros:
                case KaisaKaavio.KaavioTyyppi.Pudari2Kierros:
                default:
                    if (this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu)
                    {
                        return NaytaVirhe(this.kaavioComboBox, "Kaisan RG-kilpailussa täytyy pelata tuplakaaviolla loppuun asti!");
                    }

                    if (this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                    {
                        return NaytaVirhe(this.kaavioComboBox, "Kaisan SM-kilpailussa täytyy pelata tuplakaaviolla loppuun asti!");
                    }
                    break;
            }

            if (this.RankingKisatyyppi == RankingSarjanTyyppi.Vapaamuotoinen)
            {
                if (string.IsNullOrEmpty(this.RankingSarjanNimi))
                {
                    return NaytaVirhe(this.rankingSarjanNimiTextBox, "Anna rankingsarjalle nimi!");
                }
            }

            return true;
        }

        private void kaavioComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TarkistaTiedot();
        }

        private void nakyvyysComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            Nakyvyys n = (Nakyvyys)e.ListItem;

            var field = typeof(Nakyvyys).GetField(n.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            e.Value = attributes.Length == 0 ? n.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }

        private void peliPaikkaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TarkistaTiedot();
        }

        private void peliPaikkaComboBox_TextUpdate(object sender, EventArgs e)
        {
            TarkistaTiedot();
        }

        private void peliPaikkaComboBox_Validating(object sender, CancelEventArgs e)
        {
            var teksti = this.peliPaikkaComboBox.Text;

            if (!string.IsNullOrEmpty(teksti))
            {
                var sali = this.yleisAsetukset.SaliTietueet.FirstOrDefault(x => string.Equals(x.Lyhenne, teksti, StringComparison.OrdinalIgnoreCase));
                if (sali != null)
                {
                    if (!string.Equals(teksti, sali.Lyhenne))
                    {
                        this.peliPaikkaComboBox.Text = sali.Lyhenne;
                    }
                }

                var alias = this.yleisAsetukset.SaliTietueet.FirstOrDefault(x => string.Equals(x.Alias, teksti, StringComparison.OrdinalIgnoreCase));
                if (alias != null)
                {
                    this.peliPaikkaComboBox.Text = alias.Lyhenne;
                }
            }

            TarkistaTiedot();
        }

        private void peliPaikkaComboBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void ilmoAlkaaComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            try
            {
                IlmoittautumisenAlkaminen s = (IlmoittautumisenAlkaminen)e.ListItem;

                var field = typeof(IlmoittautumisenAlkaminen).GetField(s.ToString());
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                e.Value = attributes.Length == 0 ? s.ToString() : ((DescriptionAttribute)attributes[0]).Description;
            }
            catch
            {
            }
        }

        private void ilmoPaattyyComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            try
            {
                IlmoittautumisenPaattyminen s = (IlmoittautumisenPaattyminen)e.ListItem;

                var field = typeof(IlmoittautumisenPaattyminen).GetField(s.ToString());
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                e.Value = attributes.Length == 0 ? s.ToString() : ((DescriptionAttribute)attributes[0]).Description;
            }
            catch
            {
            }
        }

        private void nakyvyysComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nakyvyysComboBox.SelectedIndex == 2)
            {
            }

            PaivitaOnlineKenttienNakyvyys();
        }

        private void PaivitaOnlineKenttienNakyvyys()
        {
        }

        private void onlineIlmoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaivitaOnlineKenttienNakyvyys();
        }

        private void kaksiosainenArvontaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PaivitaOnlineKenttienNakyvyys();
        }

        private void rankingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.rankingSarjanNimiLabel.Visible = this.rankingComboBox.SelectedIndex == (int)RankingSarjanTyyppi.Vapaamuotoinen;
            this.rankingSarjanNimiTextBox.Visible = this.rankingComboBox.SelectedIndex == (int)RankingSarjanTyyppi.Vapaamuotoinen;

            TarkistaTiedot();
        }

        private void rankingSarjanNimiTextBox_TextChanged(object sender, EventArgs e)
        {
            TarkistaTiedot();
        }

        private void rankingComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            RankingSarjanTyyppi r = (RankingSarjanTyyppi)e.ListItem;

            var field = typeof(RankingSarjanTyyppi).GetField(r.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            e.Value = attributes.Length == 0 ? r.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }

        private void kelloTextBox_TextChanged(object sender, EventArgs e)
        {
            TarkistaTiedot();
        }

        private void kaavioComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            KaavioTyyppi k = (KaavioTyyppi)e.ListItem;

            var field = typeof(KaavioTyyppi).GetField(k.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            e.Value = attributes.Length == 0 ? k.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
