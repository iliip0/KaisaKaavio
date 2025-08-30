using System;
using System.IO;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class UusiKilpailuPopup : Form
    {
        private bool avattu = false;
        private string oletusNimi = string.Empty;
        private bool nimeaMuokattuManuaalisesti = false;
        private Asetukset.KisaOletusasetukset asetukset = null;

        public bool LuoTestikilpailu { get; private set; }

        public UusiKilpailuPopup()
        {
            InitializeComponent();

            this.uusiKilpailuLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));
            this.uusiKilpailuLajiComboBox.SelectedIndex = 0;

            this.kilpaSarjaComboBox.DataSource = Enum.GetValues(typeof(KilpaSarja));
            this.kilpaSarjaComboBox.SelectedIndex = 0;

            this.kilpailunTyyppiComboBox.SelectedIndex = 0;
            this.kaavioComboBox.SelectedIndex = 2;

            this.rankingComboBox.SelectedIndex = 0;
            this.rankingCheckBox.Checked = false;
            this.rankingComboBox.Visible = false;

            this.alkamisAikaDateTimePicker.Value = DateTime.Today;

            this.virheLabel.Text = string.Empty;
            this.virheLabel.Visible = false;

            this.LuoTestikilpailu = false;

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
                        this.rankingCheckBox.Checked = viikkokisa;
                        this.rankingComboBox.SelectedIndex = 0;
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
                        this.rankingCheckBox.Checked = viikkokisa;
                        this.rankingComboBox.SelectedIndex = 0;
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
                        this.rankingCheckBox.Checked = viikkokisa;
                        this.rankingComboBox.SelectedIndex = 0;
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
                        this.rankingCheckBox.Checked = false;
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
                        this.rankingCheckBox.Checked = false;
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
                        this.rankingCheckBox.Checked = false;
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
                }
                else
                {
                    this.Text = "Luo uusi kilpailu";
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
                }
                else
                {
                    this.kaavioComboBox.SelectedIndex = 0;
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
                this.nimeaMuokattuManuaalisesti = false;
                PaivitaKilpailunOletusNimi();
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

        public string Kansio
        {
            get { return this.kansioTextBox.Text; }
        }

        public bool RankingKisa { get { return this.rankingCheckBox.Checked; } }
        public string Aika { get { return Tyypit.Aika.DateTimeToString(this.alkamisAikaDateTimePicker.Value); } }
        public decimal Tavoite { get { return this.tavoiteNumericUpDown.Value; } }

        public decimal Peliaika 
        {
            get 
            { 
                return this.peliAikaCheckBox.Checked ? this.peliAikaNumericUpDown.Value : 0; 
            }
        }

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
            if (this.asetukset != null && 
                this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa &&
                !this.LuoTestikilpailu)
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

                this.kaavioComboBox.SelectedIndex = 0;
            }

            PaivitaKilpailunOletusNimi();
            PaivitaOletusKansio(this.Laji);
            TarkistaTiedot();
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
            this.avattu = true;
            this.oletusNimi = this.kilpailunNimiTextBox.Text;
            PaivitaUiLajille((KaisaKaavio.Laji)uusiKilpailuLajiComboBox.SelectedItem, true);
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

        private bool NaytaVirhe(string teksti)
        {
            this.uusiKilpailuButton.Enabled = false;
            this.virheLabel.Text = teksti;
            this.virheLabel.Visible = true;
            return false;
        }

        private bool TarkistaTiedot()
        {
            this.uusiKilpailuButton.Enabled = true;
            this.virheLabel.Visible = false;

            switch (this.KilpaSarja)
            {
                case KilpaSarja.Joukkuekilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa ||
                        this.KilpailunTyyppi != KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                    {
                        return NaytaVirhe("Joukkuekilpailuominaisuus toimii ainoastaan Kaisan Joukkue SM-kilpailussa!");
                    }
                    break;

                case KaisaKaavio.KilpaSarja.MixedDoubles:
                case KaisaKaavio.KilpaSarja.Parikilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa ||
                        this.KilpailunTyyppi != KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                    {
                        return NaytaVirhe("Parikilpailuominaisuus toimii ainoastaan Kaisan Pari- ja Mixed Doubles SM-kilpailuissa!");
                    }
                    break;
            }

            switch (this.KilpailunTyyppi)
            {
                case KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa)
                    {
                        return NaytaVirhe("SBiL SM-kilpailuominaisuus toimii ainoastaan Kaisa-lajissa!");
                    }
                    break;

                case KaisaKaavio.KilpailunTyyppi.KaisanRGKilpailu:
                    if (this.Laji != KaisaKaavio.Laji.Kaisa)
                    {
                        return NaytaVirhe("SBiL RG-kilpailuominaisuus toimii ainoastaan Kaisa-lajissa!");
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
                        return NaytaVirhe("Kaisan RG-kilpailussa täytyy pelata tuplakaaviolla loppuun asti!");
                    }

                    if (this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.KaisanSMKilpailu)
                    {
                        return NaytaVirhe("Kaisan SM-kilpailussa täytyy pelata tuplakaaviolla loppuun asti!");
                    }
                    break;
            }

            return true;
        }

        private void kaavioComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TarkistaTiedot();
        }
    }
}
