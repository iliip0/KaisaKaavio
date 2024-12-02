using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class Form1 : Form, IStatusRivi
    {
        private Kilpailu kilpailu = new Kilpailu();
        private Asetukset asetukset = new Asetukset();
        private Ranking.Ranking ranking = new Ranking.Ranking();

        private Rahanjako rahanjako = new Rahanjako();
        private Loki loki = null;

        private string kansio = string.Empty;
        private string varmuuskopioKansio = string.Empty;

        private Font isoPaksuFontti = new Font(FontFamily.GenericSansSerif, 14.0f, FontStyle.Bold);
        private Font paksuFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Bold);
        private Font ohutFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Regular);
        private Font paksuPieniFontti = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Bold);
        private Font ohutPieniFontti = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Regular);
        private Color pelatunPelinVari = Color.FromArgb(255, 230, 230, 230);
        private Color valmiinPelinVari = Color.FromArgb(255, 244, 255, 244);
        private Color keskeneraisenPelinVari = Color.FromArgb(255, 255, 240, 200);
        private Color virhePelinVari = Color.LightPink;
        private Color arpomattomanPelaajanVäri = Color.FromArgb(255, 255, 240, 200);
        private Color sbilKeskusteluTausta = Color.FromArgb(255, 225, 235, 242);

        private Brush rajaHarja = null;
        private Pen rajaKyna = null;

        private HakuAlgoritmi haku = null;

        public Form1()
        {
            this.kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot");
            Directory.CreateDirectory(this.kansio);

            this.varmuuskopioKansio = Path.Combine(this.kansio, "Varmuuskopiot");
            Directory.CreateDirectory(this.varmuuskopioKansio);

            this.loki = new Loki(this.kansio);
            this.kilpailu.Loki = this.loki;
            this.ranking.Loki = this.loki;
            this.ranking.Asetukset = this.asetukset.RankingAsetuksetKaisa;

            this.rajaHarja = new SolidBrush(Color.Black);
            this.rajaKyna = new Pen(this.rajaHarja, 1);

            InitializeComponent();

            kilpailu.Osallistujat.ListChanged += Osallistujat_ListChanged;

            asetukset.Lataa();

            if (!string.IsNullOrEmpty(asetukset.ViimeisinKilpailu))
            {
                AvaaKilpailu(asetukset.ViimeisinKilpailu);
            }

            this.kilpailuBindingSource.DataSource = this.kilpailu;
            this.pelaajaBindingSource.DataSource = this.kilpailu.Osallistujat;
            this.jalkiIlmoBindingSource.DataSource = this.kilpailu.JalkiIlmoittautuneet;
            this.peliBindingSource.DataSource = this.kilpailu.Pelit;
            this.kaavioBindingSource.DataSource = this.kilpailu.OsallistujatJarjestyksessa;
            this.poytaBindingSource.DataSource = this.asetukset.Sali.Poydat;
            this.linkkiBindingSource.DataSource = this.asetukset.Sali.Linkit;
            this.rankingBindingSource.DataSource = this.ranking;

            this.kaavioTyyppiComboBox.DataSource = Enum.GetValues(typeof(KaavioTyyppi));

            this.rankingKisaTyyppiComboBox.DataSource = Enum.GetValues(typeof(Ranking.RankingSarjanPituus));
            this.rankingVuosiComboBox.Items.AddRange(this.ranking.Vuodet.Select(x => (object)x).ToArray());
            this.rankingPituusComboBox.DataSource = Enum.GetValues(typeof(Ranking.RankingSarjanPituus));
            this.rankingSarjaComboBox.DataSource = this.ranking.SarjatBindingSource;
            this.rankingOsakilpailuComboBox.DataSource = this.ranking.KilpailutBindingSource;

            this.openFileDialog1.InitialDirectory = this.kansio;

            Shown += Form1_Shown;
            FormClosing += Form1_FormClosing;
            
            this.tuloksetRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.pelitRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.alkavatPelitRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.kilpailuKutsuRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.rankingKokonaistilanneRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.rankingOsakilpailuRichTextBox.BackColor = this.sbilKeskusteluTausta;

            PaivitaIkkunanNimi();

            this.hakuBackgroundWorker.DoWork += hakuBackgroundWorker_DoWork;
            this.hakuBackgroundWorker.RunWorkerCompleted += hakuBackgroundWorker_RunWorkerCompleted;

            this.sbilJakoTyyppiComboBox.SelectedIndex = 0;
            this.jarjestajaJakotyyppiComboBox.SelectedIndex = 0;

            this.paivitaOhjelmaAutomaattisestiSuljettaessaToolStripMenuItem.Checked = this.asetukset.PaivitaAutomaattisesti;

#if DEBUG
            this.testaaToolStripMenuItem.Visible = true;
#else
            this.testaaToolStripMenuItem.Visible = false;
#endif

            this.loki.Kirjoita("KaisaKaavio käynnistetty onnistuneesti", null, false);
        }

        // ========={( Kaavioiden luominen, tallennus ja lataus )}============================================= //
        #region Kaavio I/O

        public static string ToValidFileName(string s)
        {
            string fileName = s;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private void Tallenna()
        {
            try
            {
                if (this.kilpailu.RankingKisa)
                {
                    this.ranking.LisaaKilpailu(this.kilpailu);
                    this.ranking.TallennaAvatutSarjat();
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Rankingsarjojen tallennus epäonnistui", ex, false);
            }

            try
            {
                this.kilpailu.Tallenna();
                this.loki.Kirjoita(string.Format("Tallennettu onnistuneesti!{0}{1}", Environment.NewLine, this.kilpailu.Tiedosto), null, false);
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaisakaavion tallennus epäonnistui", ex, false);
            }

            try
            {
                if (!string.IsNullOrEmpty(this.kilpailu.Tiedosto) && File.Exists(this.kilpailu.Tiedosto))
                {
                    this.asetukset.ViimeisinKilpailu = this.kilpailu.Tiedosto;
                }

                this.asetukset.TallennaPelaajat(this.kilpailu);
                this.asetukset.Tallenna();
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Asetusten tallennus epäonnistui", ex, false);
            }

            try
            {
                if (!string.IsNullOrEmpty(this.kilpailu.Tiedosto) && File.Exists(this.kilpailu.Tiedosto))
                {
                    // Luo varmuuskopio
                    FileInfo tiedosto = new FileInfo(this.kilpailu.Tiedosto);

                    var now = DateTime.Now;
                    string aika = string.Format("{0}-{1}-{2}_{3}-{4}-{5}",
                        now.Year,
                        now.Month,
                        now.Day,
                        now.Hour,
                        now.Minute,
                        now.Second);

                    string varmuuskopionNimi = Path.Combine(
                        this.varmuuskopioKansio, aika + "_" + tiedosto.Name);

                    File.Copy(this.kilpailu.Tiedosto, varmuuskopionNimi);
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Varmuuskopiointi epäonnistui", ex, false);
            }
        }

        private void AvaaKilpailu(string tiedosto)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.kilpailu.Tiedosto))
                {
                    this.kilpailu.Tallenna();
                }

                this.kilpailu.Avaa(tiedosto);
                this.asetukset.ViimeisinKilpailu = tiedosto;

                PaivitaIkkunanNimi();
                PaivitaKilpailuTyyppi();
            }
            catch (Exception e)
            {
                this.loki.Kirjoita(string.Format("KaisaKaavion avaaminen epäonnistui tiedostosta{0}", tiedosto), e, true);
            }
        }

        private bool kysyKilpailunPaalleKirjoitus(string tiedosto)
        {
            if (File.Exists(tiedosto))
            {
                if (MessageBox.Show(
                    string.Format("Tiedosto {0} on jo olemassa?{1}Tallennetaanko siitä huolimatta?{1}{1}OK <> tallenna aiemman kisan päälle{1}Cancel <> peruuta kilpailun luominen", tiedosto, Environment.NewLine),
                    "Tallenna päälle?",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        // Uusi kilpailu
        private void uusiKilpailuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeskeytaHaku();
            Tallenna();

            using(var popup = new UusiKilpailuPopup())
            {
                var result = popup.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string nimi = popup.Nimi;
                    string tiedosto = Path.Combine(this.kansio, ToValidFileName(nimi) + ".xml");

                    if (kysyKilpailunPaalleKirjoitus(tiedosto))
                    {
                        this.tabControl1.SelectedTab = this.kisaInfoTabPage;

                        SuspenAllDataBinding();

                        this.kilpailu.Tiedosto = tiedosto;

                        this.kilpailu.Osallistujat.Clear();
                        this.kilpailu.OsallistujatJarjestyksessa.Clear();
                        this.kilpailu.PoistaKaikkiPelit();

                        this.kilpailu.Nimi = popup.Nimi;
                        this.kilpailu.Laji = popup.Laji;

                        switch (this.kilpailu.Laji)
                        {
                            case Laji.Kaisa:
                                this.kilpailu.TavoitePistemaara = 60;
                                break;

                            case Laji.Kara:
                                this.kilpailu.TavoitePistemaara = 30;
                                break;

                            default:
                                this.kilpailu.TavoitePistemaara = 4;
                                break;
                        }

                        this.kilpailu.AlkamisAika = DateTime.Today;
                        this.kilpailu.Palkinnot = string.Empty;
                        this.kilpailu.Ilmoittautuminen = string.Empty;

                        if (popup.LuoViikkokisa)
                        {
                            this.kilpailu.KaavioTyyppi = KaavioTyyppi.Pudari3Kierros;
                            this.kilpailu.PeliAika = 40;
                            this.kilpailu.RankkareidenMaara = 3;
                            this.kilpailu.KellonAika = "18:00";
                            this.kilpailu.LisenssiVaatimus = string.Empty;
                            this.kilpailu.MaksuTapa = string.Empty;
                            this.kilpailu.OsallistumisOikeus = string.Empty;
                            this.kilpailu.OsallistumisMaksu = string.Empty;
                            this.kilpailu.Pukeutuminen = string.Empty;
                            this.kilpailu.Yksipaivainen = true;
                            this.kilpailu.KilpailuOnViikkokisa = true;
                            this.kilpailu.RankingKisa = popup.RankingKisa;
                            this.kilpailu.RankingKisaTyyppi = popup.RankingKisatyyppi;

                            if (this.kilpailu.PelaajiaEnintaan < 48)
                            {
                                this.kilpailu.PelaajiaEnintaan = 48;
                            }
                        }
                        else
                        {
                            this.kilpailu.KaavioTyyppi = KaavioTyyppi.TuplaKaavio;
                            this.kilpailu.PeliAika = 60;
                            this.kilpailu.RankkareidenMaara = 5;
                            this.kilpailu.KellonAika = "10:00";
                            this.kilpailu.LisenssiVaatimus = string.Empty; // TODO, linkit 
                            this.kilpailu.MaksuTapa = "Etukäteen biljardi.org kautta";
                            this.kilpailu.OsallistumisOikeus = "SBiL:n jäsenseurojen jäsenillä";
                            this.kilpailu.OsallistumisMaksu = "Aikuiset 50€ / junnut 25€";
                            this.kilpailu.Pukeutuminen = "SBiL EB-taso";
                            this.kilpailu.Yksipaivainen = false;
                            this.kilpailu.KilpailuOnViikkokisa = false;
                            this.kilpailu.RankingKisa = false;

                            if (this.kilpailu.PelaajiaEnintaan < 256)
                            {
                                this.kilpailu.PelaajiaEnintaan = 256;
                            }
                        }

                        PaivitaKilpailuTyyppi();
                        ResumeAllDataBinding();

                        Tallenna();
                    }
                }
            }
        }

        /// <summary>
        /// Piilottaa ylimääräiset käyttöliittymän osat riippuen kilpailun lajista ja tyypistä
        /// </summary>
        private void PaivitaKilpailuTyyppi()
        {
            switch (this.kilpailu.Laji)
            {
                case Laji.Kaisa: this.ranking.Asetukset = this.asetukset.RankingAsetuksetKaisa; break;
                case Laji.Pool: this.ranking.Asetukset = this.asetukset.RankingAsetuksetPool; break;
                case Laji.Snooker: this.ranking.Asetukset = this.asetukset.RankingAsetuksetSnooker; break;
                case Laji.Kara: this.ranking.Asetukset = this.asetukset.RankingAsetuksetKara; break;
                case Laji.Pyramidi: this.ranking.Asetukset = this.asetukset.RankingAsetuksetPyramidi; break;
                case Laji.Heyball: this.ranking.Asetukset = this.asetukset.RankingAsetuksetHeyball; break;
                default: this.ranking.Asetukset = this.asetukset.RankingAsetuksetKaisa; break;
            }

            if (this.kilpailu.KilpailuOnViikkokisa)
            {
                this.sijoitettuDataGridViewTextBoxColumn.HeaderText = "Ranking";
                this.sijoitettuDataGridViewTextBoxColumn.Visible = this.kilpailu.RankingKisa;
            }
            else
            {
                this.sijoitettuDataGridViewTextBoxColumn.HeaderText = "Sijoitet- tu";
                this.sijoitettuDataGridViewTextBoxColumn.Visible = true;
            }

            if (this.kilpailu.Laji == Laji.Pool)
            {
                this.peliAikaLabel.Visible = false;
                this.peliAikaLabel2.Visible = false;
                this.peliaikaNumericUpDown.Visible = false;
                this.tavoitePistemaaraLabel.Text = "voittoon";
            }
            else
            {
                this.peliAikaLabel.Visible = true;
                this.peliAikaLabel2.Visible = true;
                this.peliaikaNumericUpDown.Visible = true;
                this.tavoitePistemaaraLabel.Text = "pisteeseen";
            }

            if (this.kilpailu.Laji == Laji.Kara)
            {
                this.peliAikaLabel.Text = "Lyöntivuoroja:";
                this.peliAikaLabel2.Text = string.Empty;
                this.tavoitePistemaaraLabel.Text = "karaan";
            }
            else
            {
                this.peliAikaLabel.Text = "Peliaika:";
                this.peliAikaLabel2.Text = "minuuttia";
            }

            this.yksipaivainenCheckBox.Visible = !this.kilpailu.KilpailuOnViikkokisa;
            this.kisaDetaljitGroupBox.Visible = !this.kilpailu.KilpailuOnViikkokisa;
            this.tulostaToolStripMenuItem.Visible = !this.kilpailu.KilpailuOnViikkokisa;

            this.rankkarienMaaraLabel.Visible = this.kilpailu.Laji == Laji.Kaisa;
            this.rankkarienMaaraNumericUpDown.Visible = this.kilpailu.Laji == Laji.Kaisa;

            this.kabikeMaksuDataGridViewTextBoxColumn.Visible = (this.kilpailu.Laji == Laji.Kaisa) && (!this.kilpailu.KilpailuOnViikkokisa);
            this.piilotaToinenKierrosCheckBox.Visible = this.kilpailu.KilpailuOnViikkokisa;

            this.rankingKisaCheckBox.Visible = this.kilpailu.KilpailuOnViikkokisa;

            // Todo: Näitä ei kai nykyään tarvita ikinä
            this.seuranJasenMaksuDataGridViewTextBoxColumn.Visible = false;
            this.lisenssiMaksuDataGridViewTextBoxColumn.Visible = false;

            if (this.kilpailu.KilpailuOnViikkokisa)
            {
                if (this.tabControl1.Contains(this.kilpailuKutsuTabPage))
                {
                    this.tabControl1.Controls.Remove(this.kilpailuKutsuTabPage);
                }

                if (this.tabControl1.Contains(this.saliInfoTabPage))
                {
                    this.tabControl1.Controls.Remove(this.saliInfoTabPage);
                }

                if (!this.tabControl1.Contains(this.rankingTabPage))
                {
                    this.tabControl1.Controls.Add(this.rankingTabPage);
                }
            }
            else
            {
                if (!this.tabControl1.Contains(this.kilpailuKutsuTabPage))
                {
                    this.tabControl1.Controls.Add(this.kilpailuKutsuTabPage);
                }

                if (!this.tabControl1.Contains(this.saliInfoTabPage))
                {
                    this.tabControl1.Controls.Add(this.saliInfoTabPage);
                }

                if (this.tabControl1.Contains(this.rankingTabPage))
                {
                    this.tabControl1.Controls.Remove(this.rankingTabPage);
                }
            }
        }

        private void avaaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.openFileDialog1.FileName = string.Empty;

                if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(this.openFileDialog1.FileName))
                    {
                        this.tabControl1.SelectedTab = this.kisaInfoTabPage;

                        SuspenAllDataBinding();

                        this.kilpailu.Avaa(this.openFileDialog1.FileName);
                        this.asetukset.ViimeisinKilpailu = this.kilpailu.Tiedosto;

                        ResumeAllDataBinding();
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaavion avaaminen epäonnistui", ex, true);
            }
        }

        // Tallenna
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(kilpailu.Tiedosto) || !File.Exists(kilpailu.Tiedosto))
                {
                    tallennaToolStripMenuItem_Click(sender, e);
                }
                else
                {
                    Tallenna();
                }

                this.loki.Kirjoita(string.Format("Tallennettu onnistuneesti!{0}{1}", Environment.NewLine, this.kilpailu.Tiedosto), null, true);
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaavion tallennus epäonnistui", ex, true);
            }
        }

        // Tallenna nimella
        private void tallennaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = ToValidFileName(this.kilpailu.Nimi);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "KaisaKilpailu_" + DateTime.Now.ToShortDateString();
                }

                this.openFileDialog1.FileName = fileName + ".xml";
                if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.kilpailu.TallennaNimella(this.openFileDialog1.FileName);
                    this.asetukset.ViimeisinKilpailu = this.kilpailu.Tiedosto;
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaavion tallennus epäonnistui", ex, true);
            }
        }

        #endregion

        // ========={( Ikkunan päivitys ja välilehdet )}======================================================= //
        #region Ikkuna ja täbit

        void Form1_Shown(object sender, EventArgs e)
        {
            PaivitaStatusRivi(string.Empty, false, 0, 0);
            PaivitaArvontaTabi();

            switch (this.kilpailu.Tilanne)
            {
                case KilpailunTilanne.Kaynnissa:
                    this.tabControl1.SelectedTab = this.pelitTabPage;
                    break;

                case KilpailunTilanne.Paattynyt:
                    this.tabControl1.SelectedTab = this.tuloksetTabPage;
                    break;

                case KilpailunTilanne.Arvonta:
                default:
                    this.tabControl1.SelectedTab = this.arvontaTabPage;
                    break;
            }
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Tallenna();

#if !DEBUG // Päivitetään ohjelma uusimpaan versioon suljettaessa
            try
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    if ((this.asetukset.PaivitaAutomaattisesti && this.asetukset.ViimeisimmanPaivityksenPaiva != DateTime.Now.Day) ||
                        this.haePaivityksiaOhjelmanSulkeuduttuaToolStripMenuItem.Checked)
                    {
                        this.loki.Kirjoita("Haetaan päivityksiä ohjelmaan...");

                        this.asetukset.ViimeisimmanPaivityksenPaiva = DateTime.Now.Day;

                        string exe = Assembly.GetExecutingAssembly().Location;
                        string kansio = Path.GetDirectoryName(exe);
                        string paivittaja = Path.Combine(kansio, "KaisaKaavioUpdater.exe");
                        if (Program.PuraResurssi("KaisaKaavio.Resources.KaisaKaavioUpdater.exe", paivittaja, this.loki))
                        {
                            string komentorivi = exe + " " + Process.GetCurrentProcess().Id;
                            Process process = Process.Start(paivittaja, komentorivi);

                            this.loki.Kirjoita("Käynnistettiin päivittäjä onnistuneesti", null, false);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                this.loki.Kirjoita("Ohjelman päivitys epäonnistui", ee, false);
            }
#endif
        }

        private void PaivitaIkkunanNimi()
        {
            string software = string.Format("KaisaKaavio v{0}", Assembly.GetEntryAssembly().GetName().Version);

            if (string.IsNullOrEmpty(this.kilpailu.Nimi))
            {
                this.Text = software;
            }
            else if (this.kilpailu.Nimi.Contains(DateTime.Now.Year.ToString()))
            {
                this.Text = string.Format("{0} - {1}", software, this.kilpailu.Nimi);
            }
            else
            {
                this.Text = string.Format("{0} - {1} - {2}.{3}.{4}",
                    software,
                    this.kilpailu.Nimi,
                    this.kilpailu.AlkamisAika.Day,
                    this.kilpailu.AlkamisAika.Month,
                    this.kilpailu.AlkamisAika.Year);
            }
        }

        public void PaivitaStatusRivi(string teksti, bool mittariNakyvissa, int mittari, int mittariMax)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<string, bool, int, int>(PaivitaStatusRivi), teksti, mittariNakyvissa, mittari, mittariMax);
                }
                else
                {
                    if (string.IsNullOrEmpty(teksti))
                    {
                        this.toolStripStatusLabel1.Text = string.Empty;
                    }
                    else
                    {
                        this.toolStripStatusLabel1.Text = teksti;
                    }

                    if (mittariNakyvissa)
                    {
                        this.toolStripProgressBar1.Enabled = true;
                        this.toolStripProgressBar1.Visible = true;
                        this.toolStripProgressBar1.Maximum = mittariMax;
                        this.toolStripProgressBar1.Value = mittari;
                    }
                    else
                    {
                        this.toolStripProgressBar1.Visible = false;
                        this.toolStripProgressBar1.Enabled = false;
                    }
                }
            }
            catch (Exception ee)
            {
                loki.Kirjoita("Statusrivin päivitys epäonnistui", ee, false);
            }
        }

        private void SuspenAllDataBinding()
        {
            this.kaavioBindingSource.SuspendBinding();
            this.kilpailuBindingSource.SuspendBinding();
            this.pelaajaBindingSource.SuspendBinding();
            this.peliBindingSource.SuspendBinding();
        }

        private void ResumeAllDataBinding()
        {
            this.kaavioBindingSource.ResetBindings(false);
            this.kilpailuBindingSource.ResetBindings(false);
            this.pelaajaBindingSource.ResetBindings(false);
            this.peliBindingSource.ResetBindings(false);

            this.kaavioBindingSource.ResumeBinding();
            this.kilpailuBindingSource.ResumeBinding();
            this.pelaajaBindingSource.ResumeBinding();
            this.peliBindingSource.ResumeBinding();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedTab == this.arvontaTabPage)
            {
                this.kilpailu.PoistaTyhjatOsallistujat();

                this.kilpailuBindingSource.ResetBindings(false);
                this.pelaajaBindingSource.ResetBindings(false);

                try
                {
                    this.dateTimePicker2.Value = this.kilpailu.AlkamisAika;
                }
                catch (Exception ee)
                {
                    this.loki.Kirjoita("Alkamisajan asetus epäonnistui", ee, false);
                }

                PaivitaArvontaTabi();
            }
            else if (this.tabControl1.SelectedTab == this.kisaInfoTabPage)
            {
                this.rankingKisaTyyppiComboBox.Visible = this.kilpailu.RankingKisa;

                if (this.kilpailu.KilpailuAlkanut)
                {
                    this.pelaajiaEnintaanNumericUpDown.Increment = 0;
                    this.tavoitePistemaaraNumericUpDown.Increment = 0;
                }
                else
                {
                    this.pelaajiaEnintaanNumericUpDown.Increment = 1;
                    this.tavoitePistemaaraNumericUpDown.Increment = 1;
                }
            }
            else if (this.tabControl1.SelectedTab == this.pelitTabPage)
            {
                lock (this.kilpailu)
                {
                    this.kilpailu.HakuTarvitaan = false;

                    this.pelitDataGridView.SuspendLayout();
                    this.peliBindingSource.SuspendBinding();

                    this.kilpailu.PaivitaPelienTulokset();

                    var algoritmi = this.kilpailu.Haku(this);
                    if (algoritmi != null)
                    {
                        algoritmi.Hae();
                        PaivitaHaku(algoritmi);
                    }

                    this.peliBindingSource.ResetBindings(false);

                    this.peliBindingSource.ResumeBinding();
                    this.pelitDataGridView.ResumeLayout();

                    if (this.pelitDataGridView.RowCount > 3)
                    {
                        this.pelitDataGridView.FirstDisplayedScrollingRowIndex = this.pelitDataGridView.RowCount - 1;
                    }

                    if (this.piilotaToinenKierrosCheckBox.Visible)
                    {
                        if (this.kilpailu.ToinenKierrosAlkanut)
                        {
                            this.piilotaToinenKierrosCheckBox.Visible = false;
                        }
                    }
                }
            }
            else if (this.tabControl1.SelectedTab == this.tuloksetTabPage)
            {
                PaivitaTuloksetTeksti();
                PaivitaPelitTeksti();
            }
            else if (this.tabControl1.SelectedTab == this.kilpailuKutsuTabPage)
            {
                PaivitaKilpailukutsuTeksti();
                PaivitaAlkavatPelit();
            }
            else if (this.tabControl1.SelectedTab == this.kaavioTabPage)
            {
                this.kaavioDataGridView.SuspendLayout();
                this.kaavioBindingSource.SuspendBinding();

                this.kilpailu.PaivitaKaavioData();

                // Piilota seura sarake jos se olisi tyhjä
                if (this.kilpailu.OsallistujatJarjestyksessa.Any(x => !string.IsNullOrEmpty(x.Seura)))
                {
                    this.kaavioDataGridView.Columns[2].Visible = true;
                }
                else
                {
                    this.kaavioDataGridView.Columns[2].Visible = false;
                }

                // Piilotetaan turhat sarakkeet kaavio taulukosta
                for (int i = 3; i < this.Voitot.Index; ++i)
                {
                    int kierros = (i - 3) / 2;

                    if (kierros >= this.kilpailu.MaxKierros + 1)
                    {
                        this.kaavioDataGridView.Columns[i].Visible = false;
                    }
                    else
                    {
                        this.kaavioDataGridView.Columns[i].Visible = true;
                    }
                }

                this.kaavioBindingSource.ResetBindings(false);
                this.kaavioBindingSource.ResumeBinding();
                this.kaavioDataGridView.ResumeLayout();
            }
            else if (this.tabControl1.SelectedTab == this.rahanJakoTabPage)
            {
                AlustaRahanjako();
                PaivitaRahanjako();
            }
            else if (this.tabControl1.SelectedTab == this.rankingTabPage)
            {
                if (this.kilpailu.RankingKisa)
                {
                    this.ranking.LisaaKilpailu(this.kilpailu);
                }

                this.rankingVuosiComboBox.Items.Clear();
                this.rankingVuosiComboBox.Items.AddRange(this.ranking.Vuodet.Select(x => (object)x).ToArray());

                PaivitaRankingPistetytysLiuut();

                this.ranking.PaivitaValitutSarjat();

                this.rankingVuosiComboBox.SelectedIndex = 0;
            }

            bool pelitTabilla = this.tabControl1.SelectedTab == this.pelitTabPage;

            this.hakuTimer.Enabled = pelitTabilla;  
            if (!pelitTabilla) // Peruuta käynnissä oleva haku jos poistutaan Pelit tabilta
            {
                KeskeytaHaku();
                PaivitaStatusRivi(string.Empty, false, 0, 100);
            }
        }

        #endregion

        // ========={( Hakualgoritmin pyöritys ja kaavion päivitys )}========================================== //
        #region Hakualgoritmi

        /// <summary>
        /// Tämä funktio tikittää sekunnin välein kun pelit taulukko on näkyvissä.
        /// Tässä funktiossa suoritetaan kaavion ja hakujen päivitys sekä
        /// automaattinen tallentaminen
        /// </summary>
        private void hakuTimer_Tick(object sender, EventArgs e)
        {
            // Lykkää päivityksiä jos käyttäjä editoi pelit taulukkoa parhaillaan
            if (this.pelitDataGridView.IsCurrentCellInEditMode)
            {
                return;
            }

            if (this.kilpailu.PelinTulosMuuttunutNumerolla != Int32.MaxValue)
            {
                this.kilpailu.PoistaTyhjatPelitAlkaenNumerosta(this.kilpailu.PelinTulosMuuttunutNumerolla);
                this.kilpailu.PelinTulosMuuttunutNumerolla = Int32.MaxValue;
                this.kilpailu.HakuTarvitaan = true;
                return;
            }

            if (this.kilpailu.HakuTarvitaan)
            {
                this.kilpailu.HakuTarvitaan = false;
                KaynnistaHaku();

                return; // < Vältetään tekemästä montaa päivitysoperaatiota samalla kertaa, jotta UI pysyy virkeänä
            }

            if (this.kilpailu.PelienTilannePaivitysTarvitaan)
            {
                this.kilpailu.PelienTilannePaivitysTarvitaan = false;
                PaivitaPeliTilanteet();
                return; // < Vältetään tekemästä montaa päivitysoperaatiota samalla kertaa, jotta UI pysyy virkeänä
            }

            if (this.kilpailu.KilpailuPaattyiJuuri)
            {
                this.kilpailu.KilpailuPaattyiJuuri = false;
                var voittaja = this.kilpailu.Voittaja();
                if (voittaja != null)
                {
                    MessageBox.Show(
                        string.Format("Kilpailu on päättynyt! {0} voitti", voittaja.Nimi),
                        "Kilpailu on päättynyt!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }

            if (this.kilpailu.TallennusAjastin >= 1)
            {
                this.kilpailu.TallennusAjastin -= 1;
            }

            if ((this.kilpailu.TallennusAjastin <= 1) && this.kilpailu.TallennusTarvitaan)
            {
                Tallenna();
            }
        }

        private void KeskeytaHaku()
        {
            try
            {
                lock (this.kilpailu)
                {
                    if (this.haku != null)
                    {
                        this.haku.PeruutaHaku = true;
                    }
                }

                if (this.hakuBackgroundWorker.IsBusy)
                {
                    loki.Kirjoita("Keskeytetään aiempi haku");

                    this.hakuBackgroundWorker.CancelAsync();

                    var begin = DateTime.Now;

                    while (this.hakuBackgroundWorker.IsBusy)
                    {
                        Thread.Sleep(10);

                        if ((DateTime.Now - begin).Seconds > 2) // Varmuuden vuoksi lakataan odottamasta 2s. jälkeen jottei jäädä jumiin
                        {
                            loki.Kirjoita("Aiemman haun keskeytys epäonnistui");
                            break;
                        }
                    }
                }

                lock (this.kilpailu)
                {
                    this.haku = null;
                }
            }
            catch (Exception e)
            {
                loki.Kirjoita("Haun keskeytys epäonnistui", e, false);
            }
        }

        private void KaynnistaHaku()
        {
            KeskeytaHaku();

            if (this.hakuBackgroundWorker.IsBusy) // Hakutyöskentelijä on kiireinen, yritetään hetken päästä uudelleen
            {
                this.loki.Kirjoita("Hakutyöskentelijä on kiireinen. Yritetään hakua myöhemmin uudelleen...");

                this.kilpailu.HakuTarvitaan = true;
                return;
            }

            try
            {
                var algoritmi = this.kilpailu.Haku(this);
                if (algoritmi != null)
                {
                    lock (this.kilpailu)
                    {
                        this.haku = algoritmi;
                        this.hakuBackgroundWorker.RunWorkerAsync(algoritmi);
                    }
                }
            }
            catch (Exception e)
            {
                loki.Kirjoita("Haun käynnistys epäonnistui", e, false);
            }
        }

        void hakuBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                HakuAlgoritmi algoritmi = (HakuAlgoritmi)e.Argument;
                algoritmi.Hae();
                e.Result = algoritmi;
            }
            catch (Exception ee)
            {
                this.loki.Kirjoita("Haku taustasäikeessä epäonnistui", ee, false);
            }
        }

        void hakuBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HakuAlgoritmi algoritmi = (HakuAlgoritmi)e.Result;

            lock (this.kilpailu)
            {
                if (this.haku != algoritmi)
                {
                    // Hylkää tämän haun tulos. Uudempi haku on jo käynnissä tai haku on keskeytetty
                    return;
                }
            }

            if (algoritmi != null && !algoritmi.PeruutaHaku)
            {
                PaivitaHaku(algoritmi);
            }

            this.haku = null;
        }

        private void PaivitaHaku(HakuAlgoritmi algoritmi)
        {
            if (algoritmi == null ||
                algoritmi.HakuVirhe != null ||
                !algoritmi.HakuValmis ||
                algoritmi.UudetPelit.Count == 0)
            {
                return;
            }

            try
            {
                int currentRow = -1;
                int currentColumn = -1;

                if (this.pelitDataGridView.ContainsFocus && this.pelitDataGridView.CurrentCell != null)
                {
                    currentRow = this.pelitDataGridView.CurrentCell.RowIndex;
                    currentColumn = this.pelitDataGridView.CurrentCell.ColumnIndex;
                }

                this.pelitDataGridView.Enabled = false;
                this.pelitDataGridView.SuspendLayout();

                lock (this.kilpailu)
                {
                    foreach (var peli in algoritmi.UudetPelit)
                    {
                        this.kilpailu.LisaaPeli(peli.Pelaaja1, peli.Pelaaja2);
                    }

                    if (algoritmi.UusiHakuTarvitaan)
                    {
                        this.kilpailu.HakuTarvitaan = true;
                    }
                }

                this.pelitDataGridView.ResumeLayout();
                this.pelitDataGridView.Enabled = true;

                if (currentRow >= 0 && currentColumn >= 0)
                {
                    if (currentRow < this.pelitDataGridView.Rows.Count &&
                        currentColumn < this.pelitDataGridView.Columns.Count)
                    {
                        try
                        {
                            this.pelitDataGridView.CurrentCell = this.pelitDataGridView.Rows[currentRow].Cells[currentColumn];
                            this.pelitDataGridView.FirstDisplayedScrollingRowIndex = currentRow;
                            this.pelitDataGridView.Focus();
                        }
                        catch (Exception exx)
                        {
                            this.loki.Kirjoita("Kursorin palautus epäonnistui haun jälkeen", exx, false);
                        }
                    }
                }

                this.loki.Kirjoita("Haku valmis");
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Ajastettu haku epäonnistui", ex, false);
                this.kilpailu.HakuTarvitaan = true;
            }
        }

        #endregion

        // ========={( Kisainfo-sivun päivitys )}============================================================== //
        #region Kisainfo sivu

        private void kilpailunNimiTextBox_TextChanged(object sender, EventArgs e)
        {
            PaivitaIkkunanNimi();
        }

        private void comboBox1_Format(object sender, ListControlConvertEventArgs e)
        {
            KaavioTyyppi t = (KaavioTyyppi)e.ListItem;

            var field = typeof(KaavioTyyppi).GetField(t.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            e.Value = attributes.Length == 0 ? t.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            this.kilpailu.AlkamisAika = this.dateTimePicker2.Value;
        }

        #endregion

        // ========={( Arvonta-sivun päivitys )}=============================================================== //
        #region Osallistujat/arvonta taulukko

        private void Osallistujat_ListChanged(object sender, ListChangedEventArgs e)
        {
            PaivitaOsallistujaLista();
        }

        private void PaivitaOsallistujaLista()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(PaivitaOsallistujaLista));
            }
            else
            {
                try
                {
                    this.osallistujatDataGridView.SuspendLayout();

                    int i = 1;
                    foreach (var o in this.kilpailu.Osallistujat)
                    {
                        if (!string.IsNullOrEmpty(o.Nimi))
                        {
                            o.IlmoittautumisNumero = i.ToString();
                            i++;
                        }
                        else
                        {
                            o.IlmoittautumisNumero = string.Empty;
                        }
                    }

                    if (this.kilpailu.Osallistujat.Count() == 1)
                    {
                        osallistujaMaaraRichTextBox.Text = string.Format("{0} Osallistuja", this.kilpailu.Osallistujat.Count);
                    }
                    else
                    {
                        osallistujaMaaraRichTextBox.Text = string.Format("{0} Osallistujaa", this.kilpailu.Osallistujat.Count);
                    }

                    this.pelaajaBindingSource.ResetBindings(false);
                    this.kilpailuBindingSource.ResetBindings(false);
                    this.osallistujatDataGridView.ResumeLayout();
                }
                catch (Exception e)
                {
                    this.loki.Kirjoita("Ilmoittautumislistan päivitys epäonnistui", e, false);
                }
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            osallistujatDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = string.Empty;
            e.ThrowException = false;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                {
                    PaivitaOsallistujaLista();
                }

                if (e.ColumnIndex == this.nimiDataGridViewTextBoxColumn.Index)
                {
                    Pelaaja pelaaja = (Pelaaja)this.osallistujatDataGridView.Rows[e.RowIndex].DataBoundItem;

                    if (this.kilpailu.KilpailuOnViikkokisa && this.kilpailu.RankingKisa)
                    {
                        pelaaja.Sijoitettu = string.Empty;

                        if (!string.IsNullOrEmpty(pelaaja.Nimi))
                        { 
                            int sijoitus = 0;
                            if (this.ranking.HaeNykyinenRankingSijoitus(this.kilpailu.RankingKisaTyyppi, pelaaja.Nimi, out sijoitus))
                            {
                                int pisteita = this.ranking.Asetukset.PisteitaVoitosta(sijoitus);
                                if (pisteita > 1)
                                {
                                    pelaaja.Sijoitettu = string.Format("{0}P", pisteita);
                                }
                            }
                        }
                    }

                    PelaajaTietue tietue = this.asetukset.Pelaajat.FirstOrDefault(x => string.Equals(x.Nimi, pelaaja.Nimi, StringComparison.OrdinalIgnoreCase));
                    if (tietue != null)
                    {
                        if (!string.Equals(pelaaja.Nimi, tietue.Nimi))
                        {
                            pelaaja.Nimi = tietue.Nimi;
                        }

                        if (string.IsNullOrEmpty(pelaaja.Seura))
                        {
                            pelaaja.Seura = tietue.Seura;
                        }
                    }
                }
            }
            catch
            { 
            }
        }

        private void PaivitaArvontaTabi()
        {
            try
            {
                if (this.kilpailu.KilpailuOnViikkokisa)
                {
                    this.splitContainer10.Panel2Collapsed = this.kilpailu.Pelit.Count() > 0;
                }
                else 
                {
                    this.splitContainer10.Panel2Collapsed = true;
                }
            }
            catch
            { 
            }
        }

        private void arvoKaavioButton_Click(object sender, EventArgs e)
        {
            try
            {
                bool siirrtyPelitValilehdelle = this.kilpailu.Pelit.Count() == 0;

                this.loki.Kirjoita("Arvotaan kaavio...", null, false);

                string virhe = string.Empty;

                if (this.kilpailu.ArvoKaavio(out virhe))
                {
                    if (siirrtyPelitValilehdelle)
                    {
                        this.tabControl1.SelectedTab = this.pelitTabPage;
                    }

                    this.kilpailu.SiirraJalkiIlmoittautuneetOsallistujiin(this.asetukset);

                    this.loki.Kirjoita("Kaavio arvottu", null, false);
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Kaavion arpominen ei onnistunut: {0}", virhe), 
                        "Virhe", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                }

                PaivitaArvontaTabi();
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaavion arpominen epäonnistui", ex, true);
            }
        }

        private void osallistujatDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Rivin muotoilu
            try
            {
                var row = this.osallistujatDataGridView.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];

                Pelaaja pelaaja = (Pelaaja)row.DataBoundItem;
                if (pelaaja != null && !string.IsNullOrEmpty(pelaaja.Nimi))
                {
                    if (this.kilpailu.ToinenKierrosAlkanut)
                    {
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        if (pelaaja.Id < 0)
                        {
                            e.CellStyle.BackColor = this.arpomattomanPelaajanVäri;

                            if (e.ColumnIndex == this.arvontaIdDataGridViewTextBoxColumn.Index)
                            {
                                e.CellStyle.ForeColor = Color.Red;
                                e.CellStyle.Font = this.paksuFontti;
                            }
                            else
                            {
                                e.CellStyle.ForeColor = Color.Black;
                                e.CellStyle.Font = this.ohutFontti;
                            }
                        }
                        else
                        {
                            e.CellStyle.BackColor = Color.White;
                            e.CellStyle.ForeColor = Color.Black;

                            if (e.ColumnIndex == nimiDataGridViewTextBoxColumn.Index)
                            {
                                e.CellStyle.Font = this.paksuFontti;
                            }
                            else
                            {
                                e.CellStyle.Font = this.ohutFontti;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Nimien ehdotus osallistujat-taulukossa
        /// </summary>
        private void osallistujatDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
#if !DEBUG
            try
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    int column = osallistujatDataGridView.CurrentCell.ColumnIndex;
                    if (column == this.nimiDataGridViewTextBoxColumn.Index)
                    {
                        if (this.asetukset.Pelaajat.Any(x => !string.IsNullOrEmpty(x.Nimi)))
                        {
                            AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
                            acsc.AddRange(this.asetukset.Pelaajat.Select(x => x.Nimi).ToArray());

                            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                            tb.AutoCompleteCustomSource = acsc;
                            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        }
                        else
                        {
                            tb.AutoCompleteMode = AutoCompleteMode.None;
                        }
                    }
                    else
                    {
                        tb.AutoCompleteMode = AutoCompleteMode.None;
                    }
                }
            }
            catch
            { 
            }
#endif
        }

        private void osallistujatDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                Pelaaja pelaaja = (Pelaaja)e.Row.DataBoundItem;
                if (pelaaja.Id >= 0)
                {
                    e.Cancel = true; // Ei sovi poistaa pelaajaa joka on jo arvottu kaavioon
                }
            }
            catch
            { 
            }
        }

        private void siirraJalkiIlmoittautuneetButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.kilpailu.SiirraJalkiIlmoittautuneetOsallistujiin(this.asetukset);
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Jälki-ilmoittautuneiden siirto osallistujalistalle epäonnistui!", ex, false);
            }
        }

        #endregion

        // ========={( Pelit-sivun päivitys )}================================================================= //
        #region Pelit taulukko

        /// <summary>
        /// Päivittää pelien "valmiina alkamaan" tilan
        /// </summary>
        private void PaivitaPeliTilanteet()
        {
            try
            {
                lock (this.kilpailu)
                {
                    this.kilpailu.PaivitaPelitValmiinaAlkamaan();
                }

                this.pelitDataGridView.Refresh();
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Pelitilanteiden päivitys epäonnistui", ex, false);
                this.kilpailu.PelienTilannePaivitysTarvitaan = true;
            }
        }

        private void pelitDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var row = this.pelitDataGridView.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];

            // Pelin tilannekuvake
            if (e.ColumnIndex == this.Tilanne.Index)
            {
                if (e.Value is PelinTilanne)
                {
                    var tilanne = (PelinTilanne)e.Value;

                    switch (tilanne)
                    {
                        case PelinTilanne.ValmiinaAlkamaan: e.Value = this.imageList1.Images[0]; break;
                        case PelinTilanne.AiempiPeliMenossa: e.Value = this.imageList1.Images[3]; break;
                        case PelinTilanne.Kaynnissa: e.Value = this.imageList1.Images[1]; break;
                        case PelinTilanne.Pelattu: e.Value = this.imageList1.Images[2]; break;
                        default: e.Value = this.imageList1.Images[4]; break;
                    }
                }
            }

            // Rivin muotoilu
            try
            {
                bool editoitavaSarake = e.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index ||
                                e.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index ||
                                e.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index;
                bool pisteSarake = e.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index ||
                                e.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index;

                cell.ToolTipText = string.Empty;

                Peli peli = (Peli)row.DataBoundItem;
                if (peli != null)
                {
                    // Toisen kierroksen piilotus viikkokisoissa
                    if (this.piilotaToinenKierrosCheckBox.Checked && 
                        (peli.Kierros == 2) &&
                        e.ColumnIndex != this.kierrosDataGridViewTextBoxColumn.Index)
                    {
                        e.CellStyle.ForeColor = Color.White;
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.SelectionBackColor = Color.LightBlue;
                        e.CellStyle.SelectionForeColor = Color.LightBlue;
                        return;
                    }

                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionForeColor = Color.Black;

                    // Rivin taustaväri ja tooltip
                    if (peli.Tulos == PelinTulos.Virheellinen)
                    {
                        e.CellStyle.BackColor = this.virhePelinVari;
                        e.CellStyle.SelectionBackColor = editoitavaSarake ? Color.LightBlue : this.virhePelinVari;
                        cell.ToolTipText = peli.VirheTuloksessa;
                    }
                    else if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        e.CellStyle.ForeColor = Color.Gray;
                        e.CellStyle.SelectionForeColor = Color.Gray;

                        if (this.kilpailu.VoiMuokataPelia(peli))
                        {
                            e.CellStyle.BackColor = pisteSarake ? this.keskeneraisenPelinVari : this.pelatunPelinVari;
                            e.CellStyle.SelectionBackColor = editoitavaSarake ? Color.LightBlue : this.pelatunPelinVari;
                        }
                        else
                        {
                            e.CellStyle.BackColor = this.pelatunPelinVari;
                            e.CellStyle.SelectionBackColor = this.pelatunPelinVari;
                        }
                    }
                    else if (peli.Tilanne == PelinTilanne.Kaynnissa)
                    {
                        e.CellStyle.BackColor = this.keskeneraisenPelinVari;
                        e.CellStyle.SelectionBackColor = editoitavaSarake ? Color.LightBlue : this.keskeneraisenPelinVari;
                    }
                    else if (peli.Tilanne == PelinTilanne.ValmiinaAlkamaan)
                    {
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.SelectionBackColor = editoitavaSarake ? Color.LightBlue : Color.White;
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.SelectionBackColor = Color.White;
                    }

                    // Pelaajan 1 nimi
                    if (cell.ColumnIndex == this.pelaaja1DataGridViewTextBoxColumn.Index)
                    {
                        int tappiot1 = peli.TappiotPeliRivilla1();
                        if (tappiot1 >= 2)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Black;
                        }

                        if (tappiot1 == 1)
                        {
                            e.CellStyle.Font = this.ohutFontti;
                        }
                        else
                        {
                            e.CellStyle.Font = this.paksuFontti;
                        }
                    }

                    // Pelaajan 1 pisteet
                    if (cell.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index)
                    {
                        if (peli.Tilanne == PelinTilanne.Pelattu)
                        {
                            if (peli.Tulos == PelinTulos.Pelaaja1Voitti)
                            {
                                e.CellStyle.ForeColor = Color.Black;
                                e.CellStyle.Font = this.paksuFontti;
                            }
                            else if (peli.Tulos == PelinTulos.Pelaaja2Voitti || peli.Tulos == PelinTulos.MolemmatHavisi)
                            {
                                e.CellStyle.ForeColor = Color.Red;
                                e.CellStyle.Font = this.paksuFontti;
                            }
                            else
                            {
                                e.CellStyle.ForeColor = Color.Black;
                                e.CellStyle.Font = this.ohutFontti;
                            }
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Black;
                            e.CellStyle.Font = this.ohutFontti;
                        }
                    }

                    // Pelaajan 2 nimi
                    if (e.ColumnIndex == this.pelaaja2DataGridViewTextBoxColumn.Index)
                    {
                        int tappiot2 = peli.TappiotPeliRivilla2();
                        if (tappiot2 >= 2)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Black;
                        }

                        if (tappiot2 == 1)
                        {
                            e.CellStyle.Font = this.ohutFontti;
                        }
                        else
                        {
                            e.CellStyle.Font = this.paksuFontti;
                        }
                    }

                    // Pelaajan 2 pisteet
                    if (cell.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index)
                    {
                        if (peli.Tilanne == PelinTilanne.Pelattu)
                        {
                            if (peli.Tulos == PelinTulos.Pelaaja2Voitti)
                            {
                                e.CellStyle.ForeColor = Color.Black;
                                e.CellStyle.Font = this.paksuFontti;
                            }
                            else if (peli.Tulos == PelinTulos.Pelaaja1Voitti || peli.Tulos == PelinTulos.MolemmatHavisi)
                            {
                                e.CellStyle.ForeColor = Color.Red;
                                e.CellStyle.Font = this.paksuFontti;
                            }
                            else
                            {
                                e.CellStyle.ForeColor = Color.Black;
                                e.CellStyle.Font = this.ohutFontti;
                            }
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Black;
                            e.CellStyle.Font = this.ohutFontti;
                        }
                    }

                    // Pelin tilanne painike
                    if (cell.ColumnIndex == this.TilanneTeksti.Index)
                    {
                        if (peli.Tilanne == PelinTilanne.Kaynnissa)
                        {
                            e.CellStyle.Font = this.isoPaksuFontti;
                        }
                        else
                        {
                            e.CellStyle.Font = this.ohutFontti;
                        }
                    }

                    if (cell.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index)
                    {
                        if (peli.Tilanne == PelinTilanne.Kaynnissa)
                        {
                            e.CellStyle.ForeColor = Color.Black;
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Gray;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void pelitDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Pelin "Käynnistä" painike
                if (e.ColumnIndex == 1)
                {
                    Peli peli = (Peli)this.pelitDataGridView.Rows[e.RowIndex].DataBoundItem;

                    if (peli.Kierros == 2 && this.piilotaToinenKierrosCheckBox.Checked)
                    {
                        return;
                    }

                    if (this.kilpailu.VoiMuokataPelia(peli))
                    {
                        // Jos 2.kierroksen peli yritetään alottaa kun listalla on jälki-ilmoittautuneita, kysytään varmistus
                        if (peli.Kierros == 2 && this.kilpailu.Osallistujat.Any(x => !string.IsNullOrEmpty(x.Nimi) && x.Id < 0))
                        {
                            if (VarmistaToisenKierroksenAloitus())
                            {
                                this.kilpailu.PoistaEiMukanaOlevatPelaajat();
                            }
                            else
                            {
                                return;
                            }
                        }

                        switch (peli.Tilanne)
                        {
                            case PelinTilanne.ValmiinaAlkamaan:
                                peli.KaynnistaPeli();
                                break;

                            case PelinTilanne.Pelattu:
                                break;

                            case PelinTilanne.Kaynnissa:
                                break;

                            default:
                                break;
                        }
                    }

                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        using (var popup = new PelinTiedotPopup(this.kilpailu, peli))
                        {
                            popup.ShowDialog();
                            pelitDataGridView.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Pelitilanteen päivitys epäonnistui", ex, true);
            }
        }

        /// <summary>
        /// Siirtää keyboard fokuksen järkeviin soluihin nuolinapeilla navigoidessa
        /// </summary>
        private void pelitDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                foreach (DataGridViewCell cell in this.pelitDataGridView.SelectedCells)
                {
                    Peli peli = (Peli)cell.OwningRow.DataBoundItem;
                    //if (!this.kilpailu.VoiMuokataPelia(peli))
                    {
                        bool editoitavaSarake = 
                            cell.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index ||
                            cell.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index ||
                            cell.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index;

                        // Siirry lähimpään editoitavaan peliin valitun solun yläpuolelle
                        if (e.KeyCode == Keys.Up)
                        {
                            int r = cell.RowIndex;
                            while (r > 0)
                            {
                                r--;
                                Peli p = (Peli)this.pelitDataGridView.Rows[r].DataBoundItem;
                                if (this.kilpailu.VoiMuokataPelia(p))
                                {
                                    if (editoitavaSarake)
                                    {
                                        this.pelitDataGridView.Rows[r].Cells[cell.ColumnIndex].Selected = true;
                                    }
                                    else 
                                    {
                                        this.pelitDataGridView.Rows[r].Cells[this.pisteet1DataGridViewTextBoxColumn.Index].Selected = true;
                                    }
                                    break;
                                }
                            }
                            
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                        }

                        // Siirry lähimpään editoitavaan peliin valitun solun alapuolella
                        else if (e.KeyCode == Keys.Down)
                        {
                            int r = cell.RowIndex;
                            while (r < this.pelitDataGridView.Rows.Count - 1)
                            {
                                r++;
                                Peli p = (Peli)this.pelitDataGridView.Rows[r].DataBoundItem;
                                if (this.kilpailu.VoiMuokataPelia(p))
                                {
                                    if (editoitavaSarake)
                                    {
                                        this.pelitDataGridView.Rows[r].Cells[cell.ColumnIndex].Selected = true;
                                    }
                                    else
                                    {
                                        this.pelitDataGridView.Rows[r].Cells[this.pisteet1DataGridViewTextBoxColumn.Index].Selected = true;
                                    }
                                    break;
                                }
                            }

                            e.SuppressKeyPress = false;
                            e.Handled = true;
                        }
                    }

                    if (cell.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index)
                    {
                        if (e.KeyCode == Keys.Left)
                        {
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                        else if (e.KeyCode == Keys.Right)
                        {
                            cell.OwningRow.Cells[this.pisteet1DataGridViewTextBoxColumn.Index].Selected = true;
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                    }

                    else if (cell.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index)
                    {
                        if (e.KeyCode == Keys.Left)
                        {
                            cell.OwningRow.Cells[this.poytaDataGridViewTextBoxColumn.Index].Selected = true;
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                        else if (e.KeyCode == Keys.Right)
                        {
                            cell.OwningRow.Cells[this.pisteet2DataGridViewTextBoxColumn.Index].Selected = true;
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                    }

                    else if (cell.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index)
                    {
                        if (e.KeyCode == Keys.Left)
                        {
                            cell.OwningRow.Cells[this.pisteet1DataGridViewTextBoxColumn.Index].Selected = true;
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                        if (e.KeyCode == Keys.Right)
                        {
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                    }

                    else // Keyboard fokus on jossain readonly sarakkeessa. Siirretään "pisteet 1" sarakkeeseen
                    {
                        if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                        {
                            cell.OwningRow.Cells[this.pisteet1DataGridViewTextBoxColumn.Index].Selected = true;
                            e.SuppressKeyPress = false;
                            e.Handled = true;
                            return;
                        }
                    }

                    return;
                }
            }
            catch
            {
            }
        }

        private void pelitDataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                foreach (DataGridViewCell cell in this.pelitDataGridView.SelectedCells)
                {
                    Peli peli = (Peli)cell.OwningRow.DataBoundItem;

                    if (e.KeyChar == (char)Keys.Back ||
                        e.KeyChar == (char)Keys.Delete)
                    {
                        if (cell.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index)
                        {
                            peli.Pisteet1 = string.Empty;
                        }

                        if (cell.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index)
                        {
                            peli.Pisteet2 = string.Empty;
                        }

                        if (cell.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index)
                        {
                            peli.Poyta = string.Empty;
                        }
                    }

                    return;
                }
            }
            catch
            {
            }
        }

        private void piilotaToinenKierrosCheckBox_VisibleChanged(object sender, EventArgs e)
        {
            if (this.piilotaToinenKierrosCheckBox.Visible == false)
            {
                this.piilotaToinenKierrosCheckBox.Checked = false;
            }
        }

        private bool VarmistaToisenKierroksenAloitus()
        {
            return MessageBox.Show(
                "Arvonta osiossa on jälki-ilmoittautuneita pelaajia joita ei ole vielä arvottu " +
                "kaavioon." + Environment.NewLine +
                "Paina Cancel jos haluat vielä arpoa pelaajat mukaan ennen toisen kierroksen aloittamista." + Environment.NewLine +
                "Paina OK jos haluat käynnistää toisen kierroksen ilman jälki-ilmoittautuneita.",
                "Varoitus",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Cancel;
        }

        private void pelitDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                var row = this.pelitDataGridView.Rows[e.RowIndex];

                Peli peli = (Peli)row.DataBoundItem;

                if (this.kilpailu.VoiMuokataPelia(peli))
                {
                    // Kun toinen kierros on piilotettu, ei pysty muokkaamaan 2.kierroksen pelejä
                    if (peli.Kierros == 2 && this.piilotaToinenKierrosCheckBox.Checked)
                    {
                        e.Cancel = true;
                        return;
                    }

                    // Jos 2.kierroksen peli yritetään alottaa kun listalla on jälki-ilmoittautuneita, kysytään varmistus
                    if (peli.Kierros == 2 && this.kilpailu.Osallistujat.Any(x => !string.IsNullOrEmpty(x.Nimi) && x.Id < 0))
                    {
                        if (VarmistaToisenKierroksenAloitus())
                        {
                            this.kilpailu.PoistaEiMukanaOlevatPelaajat();
                        }
                        else
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch
            {
            }
        }

        private void pelitDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        /// <summary>
        /// Tämä funktio tekee pelit taulukkoon visuaalisen raon eri kierrosten pelien välille
        /// </summary>
        private void pelitDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                var row = this.pelitDataGridView.Rows[e.RowIndex];
                if (e.RowIndex < this.pelitDataGridView.Rows.Count - 1)
                {
                    var nextRow = this.pelitDataGridView.Rows[e.RowIndex + 1];

                    Peli peli = (Peli)row.DataBoundItem;
                    Peli seuraavaPeli = (Peli)nextRow.DataBoundItem;

                    if (peli.Kierros != seuraavaPeli.Kierros)
                    {
                        row.DividerHeight = 4;
                        return;
                    }
                }

                row.DividerHeight = 0;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Piirtää erikoiskehykset semmosien solujen ympärille joita pystyy muokkaamaan
        /// </summary>
        private void pelitDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return;
                }

                if (e.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index ||
                    e.ColumnIndex == this.pisteet1DataGridViewTextBoxColumn.Index ||
                    e.ColumnIndex == this.pisteet2DataGridViewTextBoxColumn.Index)
                {
                    var row = this.pelitDataGridView.Rows[e.RowIndex];
                    var cell = row.Cells[e.ColumnIndex];

                    Peli peli = (Peli)row.DataBoundItem;

                    if (this.piilotaToinenKierrosCheckBox.Checked &&
                        peli.Kierros == 2)
                    {
                        return;
                    }

                    if (this.kilpailu.VoiMuokataPelia(peli))
                    {
                        e.PaintBackground(e.ClipBounds, true);

                        Rectangle rectDimensions = e.CellBounds;

                        rectDimensions.Width -= 4;
                        rectDimensions.Height -= 4;
                        rectDimensions.X = rectDimensions.Left + 1;
                        rectDimensions.Y = rectDimensions.Top + 1;

                        rectDimensions.Width -= cell.OwningColumn.DividerWidth;
                        rectDimensions.Height -= row.DividerHeight;

                        e.Graphics.DrawRectangle(this.rajaKyna, rectDimensions);

                        e.Handled = true;

                        e.PaintContent(e.ClipBounds);
                        e.Handled = true;
                    } 
                }
            }
            catch
            {
            }
        }

        private void pelitDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return;
                }

                var row = this.pelitDataGridView.Rows[e.RowIndex];
                Peli peli = (Peli)row.DataBoundItem;

                if (this.piilotaToinenKierrosCheckBox.Visible)
                {
                    if (peli.Kierros > 1)
                    {
                        this.piilotaToinenKierrosCheckBox.Visible = false;
                    }
                }
            }
            catch
            {
            }
        }

        private void kaavioDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var row = this.kaavioDataGridView.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];

            // Rivin muotoilu
            try
            {
                Pelaaja pelaaja = (Pelaaja)row.DataBoundItem;
                if (pelaaja != null)
                {
                    if (e.ColumnIndex == 1)
                    {
                        if (pelaaja.Pudotettu)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                        else 
                        {
                            e.CellStyle.ForeColor = Color.Black;
                        }
                    }

                    if (e.ColumnIndex >= 3 && e.ColumnIndex < this.Voitot.Index)
                    {
                        int pelinNumero = (e.ColumnIndex - 3) / 2;

                        if (pelinNumero >= pelaaja.Pelit.Count)
                        {
                            if (pelaaja.Pudotettu)
                            {
                                e.CellStyle.BackColor = Color.DarkGray;
                            }
                            else if (pelinNumero >= this.kilpailu.MaxKierros)
                            {
                                if (e.ColumnIndex % 2 == 1)
                                {
                                    e.CellStyle.BackColor = Color.Yellow;
                                }
                                else
                                {
                                    e.CellStyle.BackColor = Color.White;
                                }
                            }
                            else 
                            {
                                if (e.ColumnIndex % 2 == 1)
                                {
                                    e.CellStyle.BackColor = Color.Yellow;
                                }
                                else
                                {
                                    e.CellStyle.BackColor = Color.White;
                                }
                            }
                        }
                        else
                        {
                            Pelaaja.PeliTietue peli = pelaaja.Pelit[pelinNumero];

                            if (e.ColumnIndex % 2 == 1)
                            {
                                e.Value = peli.Vastustaja;
                                //e.CellStyle.BackColor = Color.Yellow; // Id
                            }
                            else
                            {
                                if (peli.Pelattu)
                                {
                                    cell.Style.Font = this.paksuFontti;
                                    cell.Style.BackColor = Color.White;

                                    if (peli.Voitto)
                                    {
                                        cell.Style.ForeColor = Color.Black;
                                        cell.Value = peli.Pisteet.ToString() + "V";
                                    }
                                    else
                                    {
                                        cell.Style.ForeColor = Color.Red;
                                        cell.Value = peli.Pisteet.ToString();
                                    }
                                }
                                else
                                {
                                    cell.Style.Font = this.ohutFontti;

                                    e.Value = string.Empty;
                                    e.CellStyle.BackColor = Color.White;
                                    e.CellStyle.ForeColor = Color.Black;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void piilotaToinenKierrosCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.pelitDataGridView.Refresh();
        }

        #endregion

        // ========={( Sbil keskustelupalstalle kopioituvat tekstit )}========================================= //
        #region Sbil keskustelupalsta

        private void RtfInfoRivi(string otsikko, string sisalto, StringBuilder rtf, StringBuilder sbil)
        {
            if (!string.IsNullOrEmpty(sisalto))
            {
                rtf.Append(@"\b " + otsikko + @" \b0 : " + sisalto);
                rtf.Append(@" \line ");
                sbil.AppendLine("[b]" + otsikko + "[/b] : " + sisalto);
            }
        }

        private void RtfLinkki(string otsikko, string sisalto, StringBuilder rtf, StringBuilder sbil)
        {
            if (!string.IsNullOrEmpty(otsikko))
            {
                rtf.Append(otsikko + " : ");
                sbil.Append(otsikko + " : ");
            }

            if (!string.IsNullOrEmpty(sisalto))
            {
                if (sisalto.Length > 32)
                {
                    rtf.Append(sisalto.Substring(0, 15) + "..." + sisalto.Substring(sisalto.Length - 15));
                }
                else
                {
                    rtf.Append(sisalto);
                }

                rtf.Append(@" \line ");
                sbil.AppendLine("[url]" + sisalto + "[/url]");
            }
        }

        private void RtfRivinvaihto(StringBuilder rtf, StringBuilder sbil)
        {
            rtf.Append(@" \line ");
            sbil.AppendLine();
        }

        private void PaivitaKilpailukutsuTeksti()
        {
            StringBuilder rtf = new StringBuilder();
            StringBuilder sbil = new StringBuilder();

            AloitaRtfTeksti(rtf);

            RtfInfoRivi("Kilpailun nimi", this.kilpailu.Nimi, rtf, sbil);
            RtfInfoRivi("Järjestäjä", this.kilpailu.JarjestavaSeura, rtf, sbil);

            RtfRivinvaihto(rtf, sbil);

            RtfInfoRivi("Osallistumismaksu", this.kilpailu.OsallistumisMaksu, rtf, sbil);
            RtfInfoRivi("Lisenssivaatimus", this.kilpailu.LisenssiVaatimus, rtf, sbil);
            RtfInfoRivi("Maksutapa", this.kilpailu.MaksuTapa, rtf, sbil);
            RtfInfoRivi("Osallistumisoikeus", this.kilpailu.OsallistumisOikeus, rtf, sbil);
            RtfInfoRivi("Pukeutuminen", this.kilpailu.Pukeutuminen, rtf, sbil);
            RtfInfoRivi("Palkinnot", this.kilpailu.Palkinnot, rtf, sbil);
            RtfInfoRivi("Ilmoittautuminen", this.kilpailu.Ilmoittautuminen, rtf, sbil);
            RtfInfoRivi("Alkamisaika", this.kilpailu.AlkamisAikaString(), rtf, sbil);
            RtfInfoRivi("Kesto", this.kilpailu.Yksipaivainen ? "Kilpailu on yksipäiväinen" : "Kilpailu on kaksipäiväinen", rtf, sbil);
            RtfInfoRivi("Pelipaikka", this.asetukset.Sali.Nimi, rtf, sbil);
            RtfInfoRivi("Pelipaikan osoite", this.asetukset.Sali.Osoite, rtf, sbil);
            RtfInfoRivi("Pelipaikan puhelinnumero", this.asetukset.Sali.PuhelinNumero, rtf, sbil);

            RtfRivinvaihto(rtf, sbil);

            if (this.asetukset.Sali.Poydat.Count > 0)
            {
                RtfInfoRivi("Käytössä", string.Format("{0} pöytää", this.asetukset.Sali.Poydat.Count), rtf, sbil);
            }

            if (this.asetukset.Sali.Linkit.Count > 0)
            {
                RtfRivinvaihto(rtf, sbil);
                RtfInfoRivi("Linkit", " ", rtf, sbil);

                foreach (var linkki in this.asetukset.Sali.Linkit)
                {
                    RtfLinkki(linkki.Teksti, linkki.Osoite, rtf, sbil);
                }
            }

            RtfRivinvaihto(rtf, sbil);

            RtfInfoRivi("Kilpailunjohtaja", this.kilpailu.KilpailunJohtaja, rtf, sbil);
            RtfInfoRivi("Puhelinnumero", this.kilpailu.PuhelinNumero, rtf, sbil);

            if (!string.IsNullOrEmpty(this.kilpailu.LisaTietoa))
            {
                RtfRivinvaihto(rtf, sbil);
                RtfInfoRivi("Lisätietoa", " ", rtf, sbil);

                rtf.Append(this.kilpailu.LisaTietoa);
                sbil.Append(this.kilpailu.LisaTietoa);

                RtfRivinvaihto(rtf, sbil);
            }

            var osallistujat = this.kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
            if (osallistujat.Count() > 0)
            {
                RtfRivinvaihto(rtf, sbil);
                RtfInfoRivi("Ilmoittautuneet", " ", rtf, sbil);

                int i = 1;

                foreach (var osallistuja in osallistujat)
                {
                    if (i == (this.kilpailu.PelaajiaEnintaan + 1))
                    {
                        rtf.Append(@"Varalla: \line ");
                        sbil.AppendLine("Varalla:");
                    }

                    RtfInfoRivi(i.ToString(), (osallistuja.Nimi + " " + osallistuja.Seura).Trim(), rtf, sbil);
                    i++;
                }
            }

            LopetaRtfTeksti(rtf);

            this.kilpailuKutsuRichTextBox.Rtf = rtf.ToString();
            this.kilpailuKutsuRichTextBox.Tag = sbil.ToString();
        }

        private void PaivitaAlkavatPelit()
        {
            StringBuilder rtf = new StringBuilder();
            StringBuilder sbil = new StringBuilder();

            AloitaRtfTeksti(rtf);

            int kierros = 0;

            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Kierros <= 2).ToArray())
            {
                if (peli.Kierros != kierros)
                {
                    kierros = peli.Kierros;

                    if (kierros != 1)
                    {
                        rtf.Append(@" \line ");
                        sbil.Append(Environment.NewLine);
                    }

                    rtf.Append(@"\b " + kierros.ToString() + @". Kierros \b0 ");
                    sbil.Append("[b]" + kierros.ToString() + ". Kierros[/b]");

                    if (peli.OnPudotusPeli())
                    {
                        rtf.Append(" (pudari)");
                        sbil.Append(" (pudari)");
                    }

                    rtf.Append(@" \line ");
                    rtf.Append(@" \line ");
                    sbil.Append(Environment.NewLine);
                    sbil.Append(Environment.NewLine);
                }

                peli.RichTextKuvaus(this.asetukset.Sali, rtf, sbil);
            }

            LopetaRtfTeksti(rtf);

            this.alkavatPelitRichTextBox.Rtf = rtf.ToString();
            this.alkavatPelitRichTextBox.Tag = sbil.ToString();
        }

        private void AloitaRtfTeksti(StringBuilder s)
        {
            s.AppendLine(@"{\rtf1\ansi");
            s.AppendLine(@"{\colortbl;\red0\green0\blue0;\red255\green0\blue0;\red0\green0\blue255;}");
            s.AppendLine(@" \line ");
        }

        private void LopetaRtfTeksti(StringBuilder s)
        {
            s.Append(@"}");
        }

        private void PaivitaPelitTeksti()
        {
            StringBuilder rtf = new StringBuilder();
            StringBuilder sbil = new StringBuilder();

            AloitaRtfTeksti(rtf);

            if (!this.kilpailu.KilpailuOnViikkokisa)
            {
                if (this.asetukset.Sali.Linkit.Count > 0)
                {
                    RtfInfoRivi("Linkit", " ", rtf, sbil);

                    foreach (var linkki in this.asetukset.Sali.Linkit)
                    {
                        RtfLinkki(linkki.Teksti, linkki.Osoite, rtf, sbil);
                    }

                    RtfRivinvaihto(rtf, sbil);
                }

                if (this.asetukset.Sali.Poydat.Any(x => !string.IsNullOrEmpty(x.StriimiLinkki)))
                {
                    RtfInfoRivi("Striimit", " ", rtf, sbil);

                    foreach (var poyta in this.asetukset.Sali.Poydat.Where(x => !string.IsNullOrEmpty(x.StriimiLinkki)))
                    {
                        RtfLinkki(string.Format("Pöytä {0}", poyta.Numero), poyta.StriimiLinkki, rtf, sbil);
                    }

                    RtfRivinvaihto(rtf, sbil);
                }

                if (this.asetukset.Sali.Poydat.Any(x => !string.IsNullOrEmpty(x.TulosLinkki)))
                {
                    RtfInfoRivi("Pöytäkohtaiset tilanneseurantalinkit", " ", rtf, sbil);

                    foreach (var poyta in this.asetukset.Sali.Poydat.Where(x => !string.IsNullOrEmpty(x.TulosLinkki)))
                    {
                        RtfLinkki(string.Format("Pöytä {0}", poyta.Numero), poyta.TulosLinkki, rtf, sbil);
                    }

                    RtfRivinvaihto(rtf, sbil);
                }
            }

            int kierros = 0;

            foreach (var peli in this.kilpailu.Pelit.ToArray())
            {
                if (peli.Kierros != kierros)
                {
                    kierros = peli.Kierros;

                    if (kierros != 1)
                    {
                        rtf.Append(@" \line ");
                        sbil.Append(Environment.NewLine);
                    }

                    rtf.Append(@"\b " + kierros.ToString() + @". Kierros \b0 ");
                    sbil.Append("[b]" + kierros.ToString() + ". Kierros[/b]");

                    if (peli.OnPudotusPeli())
                    {
                        rtf.Append(" (pudari)");
                        sbil.Append(" (pudari)");
                    }

                    var ekapeli = this.kilpailu.Pelit.Where(x => x.Kierros == kierros && !string.IsNullOrEmpty(x.Alkoi)).FirstOrDefault();
                    if (ekapeli != null)
                    {
                        rtf.Append(" alkoi " + ekapeli.Alkoi);
                        sbil.Append(" alkoi " + ekapeli.Alkoi);
                    }

                    rtf.Append(@" \line ");
                    rtf.Append(@" \line ");
                    sbil.Append(Environment.NewLine);
                    sbil.Append(Environment.NewLine);
                }

                peli.RichTextKuvaus(this.asetukset.Sali, rtf, sbil);
            }

            // TODO! Sarjat, ehkä lisää tuki sarjoille jotenkin

            LopetaRtfTeksti(rtf);

            this.pelitRichTextBox.Rtf = rtf.ToString();
            this.pelitRichTextBox.Tag = sbil.ToString();
        }

        private void PaivitaTuloksetTeksti()
        {
            StringBuilder rtf = new StringBuilder();
            StringBuilder sbil = new StringBuilder();

            AloitaRtfTeksti(rtf);

            RtfInfoRivi("Tulokset", " ", rtf, sbil);
            RtfRivinvaihto(rtf, sbil);

            int sijoitus = 1;
            bool ekaMukana = false;
            bool tokaMukana = false;

            string voittajaTulos = string.Empty;
            List<string> tulokset = new List<string>();

            Dictionary<string, List<int>> sarjat = new Dictionary<string, List<int>>();
            foreach (var peli in this.kilpailu.Pelit)
            {
                var detaljit = this.kilpailu.PelienDetaljit.FirstOrDefault(x => 
                    (x.Kierros == peli.Kierros) &&
                    (x.Pelaaja1 == peli.Id1) &&
                    (x.Pelaaja2 == peli.Id2));

                if (!sarjat.ContainsKey(peli.Pelaaja1))
                {
                    sarjat.Add(peli.Pelaaja1, new List<int>());
                }

                if (!sarjat.ContainsKey(peli.Pelaaja2))
                {
                    sarjat.Add(peli.Pelaaja2, new List<int>());
                }

                if (detaljit != null)
                {
                    int sarja = 0;
                    if (Int32.TryParse(detaljit.PisinSarja1, out sarja) && sarja > 0)
                    {
                        sarjat[peli.Pelaaja1].Add(sarja);
                    }
                    if (Int32.TryParse(detaljit.ToiseksiPisinSarja1, out sarja) && sarja > 0)
                    {
                        sarjat[peli.Pelaaja1].Add(sarja);
                    }
                    if (Int32.TryParse(detaljit.PisinSarja2, out sarja) && sarja > 0)
                    {
                        sarjat[peli.Pelaaja2].Add(sarja);
                    }
                    if (Int32.TryParse(detaljit.ToiseksiPisinSarja2, out sarja) && sarja > 0)
                    {
                        sarjat[peli.Pelaaja2].Add(sarja);
                    }
                }
            }

            var eiTyhjatSarjat = sarjat.Where(x => x.Value.Count > 0);
            foreach (var s in eiTyhjatSarjat)
            {
                var tempSarjat = s.Value.OrderByDescending(x => x).ToArray();
                s.Value.Clear();
                s.Value.AddRange(tempSarjat);
            }

            foreach (var osallistuja in this.kilpailu.Tulokset())
            {
                int tappiot = this.kilpailu.LaskeTappiot(osallistuja.Id, 999);
                if (sijoitus == 1)
                {
                    ekaMukana = tappiot < 2;
                    voittajaTulos = string.Format("{0}. {1} - {2}/{3}",
                        sijoitus,
                        osallistuja.Nimi,
                        this.kilpailu.LaskeVoitot(osallistuja.Id, 999),
                        this.kilpailu.LaskePisteet(osallistuja.Id, 999));
                }
                else if (tappiot > 1)
                {
                    tulokset.Add(string.Format("{0}. {1} - {2}/{3}",
                        sijoitus,
                        osallistuja.Nimi,
                        this.kilpailu.LaskeVoitot(osallistuja.Id, 999),
                        this.kilpailu.LaskePisteet(osallistuja.Id, 999)));
                }
                else
                {
                    tulokset.Add(string.Format("{0}.", sijoitus));
                }

                if (sijoitus == 2)
                {
                    tokaMukana = tappiot < 2;
                }

                sijoitus++;
            }

            if (tulokset.Count > 2)
            {
                if (ekaMukana && !tokaMukana)
                {
                    rtf.Append(voittajaTulos);
                    rtf.Append(@" \line ");
                    sbil.AppendLine(voittajaTulos);
                }
                else
                {
                    rtf.Append("1.");
                    rtf.Append(@" \line ");
                    sbil.AppendLine("1.");
                }

                foreach (var tulos in tulokset)
                {
                    rtf.Append(tulos);
                    rtf.Append(@" \line ");
                    sbil.AppendLine(tulos);
                }
            }

            if (eiTyhjatSarjat.Count() > 0)
            {
                RtfRivinvaihto(rtf, sbil);
                RtfInfoRivi("Sarjat", " ", rtf, sbil);
                RtfRivinvaihto(rtf, sbil);

                foreach (var s in eiTyhjatSarjat.OrderByDescending(x => x.Value.First()))
                {
                    string sarjaTeksti = string.Format("{0} {1}", s.Key, string.Join(",", s.Value.ToArray()));
                    rtf.Append(sarjaTeksti);
                    rtf.Append(@" \line ");
                    sbil.AppendLine(sarjaTeksti);
                }
            }

            LopetaRtfTeksti(rtf);

            this.tuloksetRichTextBox.Rtf = rtf.ToString();
            this.tuloksetRichTextBox.Tag = sbil.ToString();
        }

        private void KopioiLeikepoydalle(object teksti)
        {
            try
            {
                if (teksti != null && teksti is string)
                {
                    System.Windows.Forms.Clipboard.SetText((string)teksti);
                    this.loki.Kirjoita("Teksti kopioitu leikepöydälle", null, true);
                }
                else
                {
                    System.Windows.Forms.Clipboard.Clear();
                    this.loki.Kirjoita("Leikepöytä tyhjennetty", null, true);
                }
            }
            catch (Exception e)
            {
                this.loki.Kirjoita("Tekstin kopiointi epäonnistui", e, true);
            }
        }

        private void kilpailuKutsuButton_Click(object sender, EventArgs e)
        {
            KopioiLeikepoydalle(this.kilpailuKutsuRichTextBox.Tag);
        }

        private void alkavatPelitButton_Click(object sender, EventArgs e)
        {
            KopioiLeikepoydalle(this.alkavatPelitRichTextBox.Tag);
        }

        private void tuloksetButton_Click(object sender, EventArgs e)
        {
            KopioiLeikepoydalle(this.tuloksetRichTextBox.Tag);
        }

        private void pelitButton_Click(object sender, EventArgs e)
        {
            KopioiLeikepoydalle(this.pelitRichTextBox.Tag);
        }

        #endregion

        // ========={( Rahanjako-sivun päivitys )}============================================================= //
        #region Rahanjako

        private void AlustaRahanjako()
        {
            this.rahanjako.AlustaRahanjako(this.kilpailu, this.loki);

            this.sbilJakoTyyppiComboBox.SelectedIndex = this.rahanjako.SbilOsuusOnProsentteja ? 0 : 1;
            this.jarjestajaJakotyyppiComboBox.SelectedIndex = this.rahanjako.SeuranOsuusOnProsentteja ? 0 : 1;
            this.sbilOsuusNumericUpDown.Value = (decimal)this.rahanjako.SbilOsuus;
            this.jarjestajanOsuusNumericUpDown.Value = (decimal)this.rahanjako.SeuranOsuus;
            this.palkittujenMaaraNumericUpDown.Value = (decimal)this.rahanjako.PalkittujenMaara;
            this.voittajanOsuusTrackBar.Value = this.rahanjako.VoittajanOsuus;

            this.osallistumisMaksutTextBox.Text = ((int)this.rahanjako.OsallistumisMaksut).ToString();
            this.kabikeMaksutTextBox.Text = ((int)this.rahanjako.KabikeMaksut).ToString();
            this.lisenssiMaksutTextBox.Text = ((int)this.rahanjako.LisenssiMaksut).ToString();
            this.seurojenJasenMaksutTextBox.Text = ((int)this.rahanjako.SeuranJasenMaksut).ToString();
            this.maksutYhteensaTextBox.Text = ((int)this.rahanjako.RahaaYhteensa).ToString();
        }

        private void PaivitaRahanjako()
        {
            this.rahanjako.PaivitaRahanjako(this.rahanJakoDataGridView, this.loki);

            this.sbilOsuusTextBox.Text = ((int)this.rahanjako.SbilOsuusSumma).ToString();
            this.seuranOsuusTextBox.Text = ((int)this.rahanjako.SeuranOsuusSumma).ToString();
            this.palkintoihinTextBox.Text = ((int)this.rahanjako.Palkintoihin).ToString();
        }

        private void sbilOsuusNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.rahanjako.SbilOsuus = (int)this.sbilOsuusNumericUpDown.Value;
            PaivitaRahanjako();
        }

        private void jarjestajanOsuusNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.rahanjako.SeuranOsuus = (int)this.jarjestajanOsuusNumericUpDown.Value;
            PaivitaRahanjako();
        }

        private void sbilJakoTyyppiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.rahanjako.SbilOsuusOnProsentteja = this.sbilJakoTyyppiComboBox.SelectedIndex == 0;
            PaivitaRahanjako();
        }

        private void jarjestajaJakotyyppiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.rahanjako.SeuranOsuusOnProsentteja = this.jarjestajaJakotyyppiComboBox.SelectedIndex == 0;
            PaivitaRahanjako();
        }

        private void palkittujenMaaraNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.rahanjako.PalkittujenMaara = (int)this.palkittujenMaaraNumericUpDown.Value;
            PaivitaRahanjako();
        }

        private void voittajanOsuusTrackBar_Scroll(object sender, EventArgs e)
        {
            this.rahanjako.VoittajanOsuus = this.voittajanOsuusTrackBar.Value;
            PaivitaRahanjako();
        }

        private void palkittujenMaaraNumericUpDown_Leave(object sender, EventArgs e)
        {
            this.rahanjako.PalkittujenMaara = (int)this.palkittujenMaaraNumericUpDown.Value;
            PaivitaRahanjako();
        }

        private void sbilOsuusNumericUpDown_Leave(object sender, EventArgs e)
        {
            this.rahanjako.SbilOsuus = (int)this.sbilOsuusNumericUpDown.Value;
            PaivitaRahanjako();
        }

        private void jarjestajanOsuusNumericUpDown_Leave(object sender, EventArgs e)
        {
            this.rahanjako.SeuranOsuus = (int)this.jarjestajanOsuusNumericUpDown.Value;
            PaivitaRahanjako();
        }

        #endregion

        // ========={( Ylävalikon painikkeet )}================================================================ //
        #region Valikko

        // About
        private void kaisaKaavioOhjelmanTiedotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string teksti = string.Format("KaisaKaavio v.{0}{1}Copyright © Ilari Nieminen 2024{1}{1}" +
                "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.{1}{1}" +
                "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.{1}{1}" +
                "You should have received a copy of the GNU General Public License along with this program. If not, see https://www.gnu.org/licenses/",
                Assembly.GetEntryAssembly().GetName().Version,
                Environment.NewLine);

            MessageBox.Show(teksti, "Tietoa ohjelmasta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tietoaOhjelmastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string teksti = string.Format("KaisaKaavio v.{0}{1}Copyright © Ilari Nieminen 2024{1}{1}" +
                "Ohjelman suunnittelu ja toteutus: Ilari Nieminen{1}{1}" +
                "Kaisa kaavioiden asiantuntija: Jarmo Tainio{1}{1}" +
                "Testaus: Ilari Nieminen ja Jarmo Tainio{1}{1}" +
                "Ohjelman käyttöopas löytyy samasta kansiosta ohjelman kanssa, sekä 'tietoa' valikon kautta",
                Assembly.GetEntryAssembly().GetName().Version,
                Environment.NewLine);

            MessageBox.Show(teksti, "Tietoa ohjelmasta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void kayttoopasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Käyttöopas ei ole vielä valmis", "Työn alla", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ottelupoytakirjalappujaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ottelupöytäkirjojen tulostusominaisuus ei ole vielä valmis", "Työn alla", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void kuittejaPelaajilleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kuittien tulostusominaisuus ei ole vielä valmis", "Työn alla", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void suljeOhjelmaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void paivitaOhjelmaAutomaattisestiSuljettaessaToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.asetukset.PaivitaAutomaattisesti = this.paivitaOhjelmaAutomaattisestiSuljettaessaToolStripMenuItem.Checked;
        }

        #endregion

        // ========={( Ohjelman testaus )}===================================================================== //
        #region Testaus

        private void pelaaTestikaaviotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo kansio = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                var juuri = kansio.Parent.Parent;
                var testiKansio = Path.Combine(juuri.FullName, "TestiData");
                if (!Directory.Exists(testiKansio))
                {
                    this.folderBrowserDialog1.SelectedPath = juuri.FullName;
                    if (this.folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                    testiKansio = this.folderBrowserDialog1.SelectedPath;
                }

                if (!Directory.Exists(testiKansio))
                {
                    return;
                }

                Testaus.TestiAjo testi = new Testaus.TestiAjo(testiKansio, this.loki, this);

                int testattu = testi.Aja();

                MessageBox.Show(
                    string.Format("Testi onnistui: {0} kaaviota pelattu oikein läpi", testattu),
                    "Testi valmis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("Testi epäonnistui: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace),
                    "Virhe",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        // ========={( Ranking )}============================================================================== //
        #region Ranking

        private bool rankingPisteliukujenPaivitysKesken = false;

        private void PaivitaRankingPistetytysLiuut()
        {
            this.rankingPisteliukujenPaivitysKesken = true;

            try
            {
                PaivitaRankingPisteytysPelistaLiuku(Ranking.RankingPisteetPelista.JokaisestaVoitosta, this.rpJokaPeliNumericUpDown);
                PaivitaRankingPisteytysPelistaLiuku(Ranking.RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari, this.rpEkaKierrosNumericUpDown);
                PaivitaRankingPisteytysPelistaLiuku(Ranking.RankingPisteetPelista.RankingYkkosenVoitosta, this.rpRgYkkonenNumericUpDown);
                PaivitaRankingPisteytysPelistaLiuku(Ranking.RankingPisteetPelista.RankingKakkosenVoitosta, this.rpRgKakkonenNumericUpDown);
                PaivitaRankingPisteytysPelistaLiuku(Ranking.RankingPisteetPelista.RankingKolmosenVoitosta, this.rpRgKolmonenNumericUpDown);

                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Voittajalle, this.rp1NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Kakkoselle, this.rp2NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Kolmoselle, this.rp3NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Neloselle, this.rp4NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Vitoselle, this.rp5NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Kutoselle, this.rp6NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Seiskalle, this.rp7NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Kasille, this.rp8NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Ysille, this.rp9NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.Kymmenennelle, this.rp10NumericUpDown);
                PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta.KaikilleOsallistujille, this.rpOsallistuminenNumericUpDown);
            }
            catch
            {
            }
            finally 
            {
                this.rankingPisteliukujenPaivitysKesken = false;
            }
        }

        private void PaivitaRankingPisteytysPelistaLiuku(Ranking.RankingPisteetPelista peliEhto, NumericUpDown liuku)
        {
            var asetus = this.ranking.Asetukset.PistetytysPeleista.FirstOrDefault(x => x.Ehto == peliEhto);
            if (asetus != null)
            {
                liuku.Value = (decimal)asetus.Pisteet;
            }
            else
            {
                liuku.Value = 0;
            }
        }

        private void PaivitaRankingPisteytysSijoituksestaLiuku(Ranking.RankingPisteetSijoituksesta ehto, NumericUpDown liuku)
        {
            var asetus = this.ranking.Asetukset.PisteytysSijoituksista.FirstOrDefault(x => x.Ehto == ehto);
            if (asetus != null)
            {
                liuku.Value = (decimal)asetus.Pisteet;
            }
            else
            {
                liuku.Value = 0;
            }
        }

        private void RankingPisteytys_ValueChanged(object sender, EventArgs e)
        {
            if (!this.rankingPisteliukujenPaivitysKesken)
            {
                PaivitaRankingPisteytysPelista(Ranking.RankingPisteetPelista.JokaisestaVoitosta, this.rpJokaPeliNumericUpDown);
                PaivitaRankingPisteytysPelista(Ranking.RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari, this.rpEkaKierrosNumericUpDown);
                PaivitaRankingPisteytysPelista(Ranking.RankingPisteetPelista.RankingYkkosenVoitosta, this.rpRgYkkonenNumericUpDown);
                PaivitaRankingPisteytysPelista(Ranking.RankingPisteetPelista.RankingKakkosenVoitosta, this.rpRgKakkonenNumericUpDown);
                PaivitaRankingPisteytysPelista(Ranking.RankingPisteetPelista.RankingKolmosenVoitosta, this.rpRgKolmonenNumericUpDown);

                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Voittajalle, this.rp1NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Kakkoselle, this.rp2NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Kolmoselle, this.rp3NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Neloselle, this.rp4NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Vitoselle, this.rp5NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Kutoselle, this.rp6NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Seiskalle, this.rp7NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Kasille, this.rp8NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Ysille, this.rp9NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.Kymmenennelle, this.rp10NumericUpDown);
                PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta.KaikilleOsallistujille, this.rpOsallistuminenNumericUpDown);
            }

            NumericUpDown liuku = (NumericUpDown)sender;
            if (liuku != null)
            {
                if (liuku.Value > 0)
                {
                    liuku.Font = this.paksuPieniFontti;
                    liuku.BackColor = Color.LightGreen;
                }
                else
                {
                    liuku.Font = this.ohutPieniFontti;
                    liuku.BackColor = Color.White;
                }
            }
        }

        private void PaivitaRankingPisteytysSijoituksesta(Ranking.RankingPisteetSijoituksesta ehto, NumericUpDown liuku)
        {
            var asetus = this.ranking.Asetukset.PisteytysSijoituksista.FirstOrDefault(x => x.Ehto == ehto);

            int pisteet = (int)liuku.Value;
            if (pisteet > 0)
            {
                if (asetus == null)
                {
                    this.ranking.Asetukset.PisteytysSijoituksista.Add(new Ranking.RankingPisteytysSijoituksesta()
                    {
                        Ehto = ehto,
                        Pisteet = pisteet
                    });
                }
                else
                {
                    asetus.Pisteet = pisteet;
                }
            }
            else
            {
                if (asetus != null)
                {
                    this.ranking.Asetukset.PisteytysSijoituksista.Remove(asetus);
                }
            }
        }

        private void PaivitaRankingPisteytysPelista(Ranking.RankingPisteetPelista peliEhto, NumericUpDown liuku)
        {
            var asetus = this.ranking.Asetukset.PistetytysPeleista.FirstOrDefault(x => x.Ehto == peliEhto);

            int pisteet = (int)liuku.Value;
            if (pisteet > 0)
            {
                if (asetus == null)
                {
                    this.ranking.Asetukset.PistetytysPeleista.Add(new Ranking.RankingPisteytysPelista()
                    {
                        Ehto = peliEhto,
                        Pisteet = pisteet
                    });
                }
                else
                {
                    asetus.Pisteet = pisteet;
                }
            }
            else
            {
                if (asetus != null)
                {
                    this.ranking.Asetukset.PistetytysPeleista.Remove(asetus);
                }
            }
        }

        private void rankingKisaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.rankingKisaTyyppiComboBox.Visible = this.rankingKisaCheckBox.Checked;
        }

        private void RankingComboBoxEditBegin(bool reset)
        {
            if (reset)
            {
            }
        }

        private void RankingComboBoxEditEnd(bool reset)
        {
            if (reset)
            {
                this.rankingSarjaComboBox.SelectedItem = this.ranking.ValittuSarja;
                this.rankingOsakilpailuComboBox.SelectedItem = this.ranking.ValittuOsakilpailu;
            }
        }

        private void rankingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RankingComboBoxEditBegin(true);
            try
            {
                this.rankingKilpailutLabel.Focus();
                ((ComboBox)sender).Focus();
            }
            catch
            {
            }
        }

        private void rankingComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RankingComboBoxEditEnd(true);
        }

        private void rankingComboBox_SelectedIndexChanged2(object sender, EventArgs e)
        {
            RankingComboBoxEditBegin(false);
            try
            {
                this.rankingKilpailutLabel.Focus();
                ((ComboBox)sender).Focus();
            }
            catch
            {
            }
        }

        private void rankingComboBox_SelectionChangeCommitted2(object sender, EventArgs e)
        {
            RankingComboBoxEditEnd(false);
        }

        private void rankingSarjaComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem == null)
            {
                e.Value = string.Empty;
            }
            else if (e.ListItem is Ranking.RankingSarja)
            {
                e.Value = ((Ranking.RankingSarja)e.ListItem).Nimi;
            }
            else if (e.ListItem is Ranking.RankingOsakilpailu)
            {
                e.Value = ((Ranking.RankingOsakilpailu)e.ListItem).Nimi;
            }
        }

        #endregion
    }
}
