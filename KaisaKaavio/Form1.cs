using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
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

#if DEBUG
        private Testaus.UudelleenPelaaminen uudelleenPelaaminen = null;
#endif

        private string kansio = string.Empty;
        private string varmuuskopioKansio = string.Empty;

        private Font isoPaksuFontti = new Font(FontFamily.GenericSansSerif, 14.0f, FontStyle.Bold);
        private Font isoOhutFontti = new Font(FontFamily.GenericSansSerif, 14.0f, FontStyle.Regular);
        private Font paksuFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Bold);
        private Font ohutFontti = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Regular);
        private Font paksuPieniFontti = new Font(FontFamily.GenericSerif, 10.0f, FontStyle.Bold);
        private Font ohutPieniFontti = new Font(FontFamily.GenericSerif, 10.0f, FontStyle.Regular);
        private Color pelatunPelinVari = Color.FromArgb(255, 230, 230, 230);
        private Color valmiinPelinVari = Color.FromArgb(255, 244, 255, 244);
        private Color keskeneraisenPelinVari = Color.FromArgb(255, 255, 240, 200);
        private Color virhePelinVari = Color.LightPink;
        private Color arpomattomanPelaajanVäri = Color.FromArgb(255, 255, 240, 200);
        private Color sbilKeskusteluTausta = Color.FromArgb(255, 225, 235, 242);
        private Color rankingRivinVari0 = Color.FromArgb(255, 235, 235, 235);
        private Color rankingRivinVari1 = Color.FromArgb(255, 255, 255, 255);
        private Color tummaHarmaa = Color.FromArgb(255, 100, 100, 100);
        private Color biljardiOrgVari = Color.FromArgb(255, 180, 22, 111);

        private AutoCompleteStringCollection pelaajienNimet = null;
        private AutoCompleteStringCollection pelipaikkojenNimet = null;

        private Brush rajaHarja = null;
        private Brush kultaHarja = null;
        private Brush hopeaHarja = null;
        private Brush pronssiHarja = null;
        private Pen rajaKyna = null;
        private Pen paksuRajaKyna = null;

        private IHakuAlgoritmi haku = null;

        private bool kilpailunLatausKaynnissa = false; // 
        private bool arvontaKaynnissa = false;

        private bool suljeOhjelmaHeti = false;

        public Form1()
        {
            this.kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot");
            Directory.CreateDirectory(this.kansio);

            this.varmuuskopioKansio = Path.Combine(this.kansio, "Varmuuskopiot");
            Directory.CreateDirectory(this.varmuuskopioKansio);

            Tyypit.Tiedosto.PoistaVanhimmatTiedostotKansiosta(this.varmuuskopioKansio, 50);

            this.loki = new Loki(this.kansio);
            this.kilpailu.Loki = this.loki;
            this.kilpailu.Sali = this.asetukset.Sali;

            this.ranking.Loki = this.loki;
            this.ranking.Asetukset = this.asetukset;
            this.ranking.PropertyChanged += ranking_PropertyChanged;

            this.rajaHarja = new SolidBrush(Color.Black);
            this.kultaHarja = new SolidBrush(Color.Gold);
            this.hopeaHarja = new SolidBrush(Color.Silver);
            this.pronssiHarja = new SolidBrush(Color.Orange);

            this.rajaKyna = new Pen(this.rajaHarja, 1);
            this.paksuRajaKyna = new Pen(this.rajaHarja, 1.5f);

#if DEBUG
            this.uudelleenPelaaminen = new Testaus.UudelleenPelaaminen(this.kilpailu, this.loki);
#endif

            InitializeComponent();

            kilpailu.Osallistujat.ListChanged += Osallistujat_ListChanged;

            asetukset.Lataa();

            NaytaAloitussivu();

            this.kilpailuBindingSource.DataSource = this.kilpailu;
            this.pelaajaBindingSource.DataSource = this.kilpailu.Osallistujat;
            this.jalkiIlmoBindingSource.DataSource = this.kilpailu.JalkiIlmoittautuneet;
            this.peliBindingSource.DataSource = this.kilpailu.Pelit;
            this.kaavioBindingSource.DataSource = this.kilpailu.OsallistujatJarjestyksessa;
            this.saliBindingSource.DataSource = this.asetukset.Sali;
            this.salitBindingSource.DataSource = this.kilpailu.PeliPaikat;
            this.poytaBindingSource.DataSource = this.asetukset.Sali.Poydat;
            this.linkkiBindingSource.DataSource = this.asetukset.Sali.Linkit;
            this.rankingBindingSource.DataSource = this.ranking;

            this.kaavioTyyppiComboBox.DataSource = Enum.GetValues(typeof(KaavioTyyppi));
            this.kilpasarjaComboBox.DataSource = Enum.GetValues(typeof(KilpaSarja));
            this.sijoitustenMaaraytyminenComboBox.DataSource = Enum.GetValues(typeof(SijoitustenMaaraytyminen));
            this.tuloksetSijoitustenMaaraytyminenComboBox.DataSource = Enum.GetValues(typeof(SijoitustenMaaraytyminen));

            this.rankingKisaTyyppiComboBox.DataSource = Enum.GetValues(typeof(Ranking.RankingSarjanPituus));
            this.rankingVuosiComboBox.Items.AddRange(this.ranking.Vuodet.Select(x => (object)x).ToArray());
            this.rankingPituusComboBox.DataSource = Enum.GetValues(typeof(Ranking.RankingSarjanPituus));
            this.rankingOsakilpailuComboBox.DataSource = this.ranking.KilpailutBindingSource;
            this.rankingSarjanLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));
            this.rankingHakuLajiComboBox.DataSource = Enum.GetValues(typeof(Laji));

            this.kaavioidenYhdistaminenComboBox.SelectedIndex = 1;

            this.haeBiljardOrgSivultaButton.BackColor = this.biljardiOrgVari;

            this.openFileDialog1.InitialDirectory = this.kansio;

            Shown += Form1_Shown;
            FormClosing += Form1_FormClosing;
            
            this.tuloksetRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.pelitRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.alkavatPelitRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.kilpailuKutsuRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.rankingKokonaistilanneRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.rankingOsakilpailuRichTextBox.BackColor = this.sbilKeskusteluTausta;
            this.pelaajaId1DataGridViewTextBoxColumn.DefaultCellStyle.Font = this.ohutPieniFontti;
            this.pelaajaId2DataGridViewTextBoxColumn.DefaultCellStyle.Font = this.ohutPieniFontti;

            PaivitaIkkunanNimi();

            this.hakuBackgroundWorker.DoWork += hakuBackgroundWorker_DoWork;
            this.hakuBackgroundWorker.RunWorkerCompleted += hakuBackgroundWorker_RunWorkerCompleted;

            this.sbilJakoTyyppiComboBox.SelectedIndex = 0;
            this.jarjestajaJakotyyppiComboBox.SelectedIndex = 0;

            this.paivitaOhjelmaAutomaattisestiSuljettaessaToolStripMenuItem.Checked = this.asetukset.PaivitaAutomaattisesti;

#if DEBUG
            this.testaaToolStripMenuItem.Visible = true;
            this.testiajoBackgroundWorker1.DoWork += testiajoBackgroundWorker1_DoWork;
            this.testiajoBackgroundWorker1.RunWorkerCompleted += testiajoBackgroundWorker1_RunWorkerCompleted;
#else
            this.testaaToolStripMenuItem.Visible = false;
#endif

            Kayttoliittyma.Mukauttaja.Mukauta(this);

            this.loki.Kirjoita("KaisaKaavio käynnistetty onnistuneesti", null, false);
        }

        // =========(( Aloitussivu ))========================================================================== //
        #region Aloitussivu

        private void NaytaAloitussivu()
        {
            try
            {
                using (var aloitussivu = new Aloitussivu.Aloitussivu(this, this.asetukset, this.kilpailu))
                {
                    var valinta = aloitussivu.ShowDialog();
                    if (valinta != System.Windows.Forms.DialogResult.Cancel)
                    {
                        PaivitaIkkunanNimi();
                        PaivitaKilpailuTyyppi();
                    }
                    else
                    {
                        this.suljeOhjelmaHeti = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Aloitussivulla tapahtui virhe", ex, true);
            }
        }

        #endregion

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
                this.ranking.TallennaAvatutSarjat();
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
                if (!this.kilpailu.TestiKilpailu)
                {
                    this.asetukset.LisaaViimeisimpiinKilpailuihin(this.kilpailu);

                    if (!string.IsNullOrEmpty(this.kilpailu.Tiedosto) && File.Exists(this.kilpailu.Tiedosto))
                    {
                        this.asetukset.ViimeisinKilpailu = this.kilpailu.Tiedosto;
                    }

                    this.asetukset.TallennaPelaajat(this.kilpailu);
                }

                this.asetukset.Tallenna();
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Asetusten tallennus epäonnistui", ex, false);
            }

            if (!this.kilpailu.TestiKilpailu)
            {
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
        }

        public void AvaaKilpailu(string tiedosto)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.AvaaKilpailu"))
#endif
            {
                try
                {
                    this.kilpailunLatausKaynnissa = true;

                    if (!string.IsNullOrEmpty(this.kilpailu.Tiedosto))
                    {
                        this.kilpailu.Tallenna();
                    }

                    this.kilpailu.Avaa(tiedosto, true);

                    if (this.kilpailu.KilpailuOnViikkokisa)
                    {
                        this.kilpailu.RankingOsakilpailu = this.ranking.AvaaRankingTietueKilpailulle(this.kilpailu);
                    }
                    else
                    {
                        this.kilpailu.RankingOsakilpailu = null;
                    }

                    this.asetukset.ViimeisinKilpailu = tiedosto;

                    PaivitaIkkunanNimi();
                    PaivitaKilpailuTyyppi();
                    PaivitaPelaajienRankingPisteetOsallistujalistaan();

                    this.asetukset.LisaaViimeisimpiinKilpailuihin(this.kilpailu);
                    PaivitaViimeisimmatTiedostot();

#if DEBUG
                    if (this.kilpailu.Nimi.Contains("(Uusinta)"))
                    {
                        this.uudelleenPelausButton.Visible = true;
                    }
                    else
                    {
                        this.uudelleenPelausButton.Visible = false;
                    }
#endif
                }
                catch (Exception e)
                {
                    this.loki.Kirjoita(string.Format("KaisaKaavion avaaminen epäonnistui tiedostosta{0}", tiedosto), e, true);
                }
                finally
                {
                    this.kilpailunLatausKaynnissa = false;
                }
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
            try
            {
                this.kilpailunLatausKaynnissa = true;

                KeskeytaHaku();
                Tallenna();

                using (var popup = new UusiKilpailuPopup())
                {
                    var result = popup.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        LuoKilpailu(popup);
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Uuden kilpailun luonti epäonnistui", ex, true);
            }
            finally
            {
                this.kilpailunLatausKaynnissa = false;
            }
        }

        public void LuoKilpailu(UusiKilpailuPopup popup)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.LuoKilpailu"))
#endif
            {
                this.ranking.TyhjennaSarjatMuistista();

                string nimi = popup.Nimi;
                string tiedosto = Path.Combine(this.kansio, ToValidFileName(nimi) + ".xml");

                if (popup.LuoTestikilpailu)
                {
                    Directory.CreateDirectory(Path.Combine(this.kansio, "TestiKilpailut"));
                    tiedosto = Path.Combine(this.kansio, "TestiKilpailut", ToValidFileName(nimi) + ".xml");
                }

                if (kysyKilpailunPaalleKirjoitus(tiedosto))
                {
                    this.tabControl1.SelectedTab = this.kisaInfoTabPage;

                    SuspenAllDataBinding();

                    this.kilpailu.Tiedosto = tiedosto;

                    this.kilpailu.Id = string.Empty;
                    this.kilpailu.VarmistaEttaKilpailullaOnId();

                    this.kilpailu.Osallistujat.Clear();
                    this.kilpailu.OsallistujatJarjestyksessa.Clear();
                    this.kilpailu.PoistaKaikkiPelit();

                    this.kilpailu.Nimi = popup.Nimi;
                    this.kilpailu.Laji = popup.Laji;
                    this.kilpailu.KilpailunTyyppi = popup.KilpailunTyyppi;
                    this.kilpailu.KaavioTyyppi = popup.KaavioTyyppi;
                    this.kilpailu.KilpaSarja = popup.KilpaSarja;
                    this.kilpailu.AlkamisAika = popup.Aika;
                    this.kilpailu.TavoitePistemaara = popup.Tavoite;
                    this.kilpailu.PeliAika = popup.Peliaika;
                    this.kilpailu.PeliaikaOnRajattu = popup.Peliaika > 0;
                    this.kilpailu.TestiKilpailu = popup.LuoTestikilpailu;

                    this.kilpailu.Palkinnot = string.Empty;
                    this.kilpailu.Ilmoittautuminen = string.Empty;

                    if (popup.LuoViikkokisa)
                    {
                        this.kilpailu.RankkareidenMaara = 3;
                        this.kilpailu.KellonAika = "18:00";
                        this.kilpailu.LisenssiVaatimus = string.Empty;
                        this.kilpailu.MaksuTapa = string.Empty;
                        this.kilpailu.OsallistumisOikeus = string.Empty;
                        this.kilpailu.OsallistumisMaksu = string.Empty;
                        this.kilpailu.Pukeutuminen = string.Empty;
                        this.kilpailu.Yksipaivainen = true;
                        this.kilpailu.Sijoittaminen = Sijoittaminen.EiSijoittamista;
                        this.kilpailu.RankingOsakilpailu = this.ranking.AvaaRankingTietueKilpailulle(this.kilpailu);
                        this.kilpailu.SijoitustenMaaraytyminen = SijoitustenMaaraytyminen.KolmeParastaKierroksistaLoputPisteista;

                        this.kilpailu.OsallistumisMaksu = "10€";

                        if (this.kilpailu.PelaajiaEnintaan < 48)
                        {
                            this.kilpailu.PelaajiaEnintaan = 48;
                        }
                    }
                    else
                    {
                        this.kilpailu.RankkareidenMaara = 5;
                        this.kilpailu.KellonAika = "10:00";
                        this.kilpailu.LisenssiVaatimus = string.Empty; // TODO, linkit 
                        this.kilpailu.SijoitustenMaaraytyminen = SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista;

                        if (this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.AvoinKilpailu)
                        {
                            this.kilpailu.Yksipaivainen = true;

                            this.kilpailu.MaksuTapa = string.Empty;
                            this.kilpailu.OsallistumisOikeus = string.Empty;
                            this.kilpailu.Pukeutuminen = string.Empty;

                            this.kilpailu.Sijoittaminen = Sijoittaminen.EiSijoittamista;
                        }
                        else
                        {
                            this.kilpailu.Yksipaivainen = false;

                            this.kilpailu.MaksuTapa = "Etukäteen biljardi.org kautta";

                            // TODO!!! Päivitä osallistumisoikeus teksti ja osmaksu kilpasarjan mukaan
                            this.kilpailu.OsallistumisOikeus = "SBiL:n jäsenseurojen jäsenillä";

                            this.kilpailu.Pukeutuminen = "SBiL EB-taso";

                            if (this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanSMKilpailu)
                            {
                                this.kilpailu.Sijoittaminen = Sijoittaminen.Sijoitetaan24Pelaajaa;
                            }
                            else
                            {
                                this.kilpailu.Sijoittaminen = Sijoittaminen.Sijoitetaan8Pelaajaa;
                            }
                        }

                        this.kilpailu.OsallistumisMaksu = "Aikuiset 50€ / junnut 25€";

                        this.kilpailu.RankingOsakilpailu = null;

                        if (this.kilpailu.PelaajiaEnintaan < 256)
                        {
                            this.kilpailu.PelaajiaEnintaan = 256;
                        }
                    }

                    this.kilpailu.RankingKisa = popup.RankingKisa;
                    this.kilpailu.RankingKisaTyyppi = popup.RankingKisatyyppi;
                    this.kilpailu.RankingKisaLaji = popup.Laji;

                    PaivitaKilpailuTyyppi();
                    ResumeAllDataBinding();

                    Tallenna();
                }
            }
        }

        private void PaivitaSijoitettuSarake()
        {
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
        }

        /// <summary>
        /// Piilottaa ylimääräiset käyttöliittymän osat riippuen kilpailun lajista ja tyypistä
        /// </summary>
        private void PaivitaKilpailuTyyppi()
        {
            this.osallistujatDataGridView.Enabled = !this.kilpailu.Tyhja;
            this.jalkiIlmoittautuneetDataGridView.Enabled = !this.kilpailu.Tyhja;
            this.pelitDataGridView.Enabled = !this.kilpailu.Tyhja;
            this.kilpailunNimiTextBox.Enabled = !this.kilpailu.Tyhja;
            this.alkamisAikaDateTimePicker.Enabled = !this.kilpailu.Tyhja;
            this.kellonAikaTextBox.Enabled = !this.kilpailu.Tyhja;
            this.yksipaivainenCheckBox.Enabled = !this.kilpailu.Tyhja;
            this.kaavioGroupBox.Enabled = !this.kilpailu.Tyhja;
            this.rankingGroupBox.Enabled = !this.kilpailu.Tyhja;
            this.kisaDetaljitGroupBox.Enabled = !this.kilpailu.Tyhja;
            this.osMaksuYlaTextBox.Enabled = !this.kilpailu.Tyhja;

            PaivitaSijoitettuSarake();

            if (this.kilpailu.Laji == Laji.Pool)
            {
                //this.peliAikaLabel.Visible = false;
                //this.peliAikaLabel2.Visible = false;
                //this.peliaikaNumericUpDown.Visible = false;
                this.tavoitePistemaaraLabel.Text = "voittoon";
            }
            else
            {
                //this.peliAikaLabel.Visible = true;
                //this.peliAikaLabel2.Visible = true;
                //this.peliaikaNumericUpDown.Visible = true;
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

            this.rankingJaKilpailuKutsuSplitContainer.Panel1Collapsed = !this.kilpailu.KilpailuOnViikkokisa;
            this.rankingJaKilpailuKutsuSplitContainer.Panel2Collapsed = this.kilpailu.KilpailuOnViikkokisa;
            
            this.tulostaToolStripMenuItem.Visible = !this.kilpailu.KilpailuOnViikkokisa;

            this.osMaksuYlaLabel.Visible = this.kilpailu.KilpailuOnViikkokisa;
            this.osMaksuYlalabel2.Visible = this.kilpailu.KilpailuOnViikkokisa;
            this.osMaksuYlaTextBox.Visible = this.kilpailu.KilpailuOnViikkokisa;

            this.rankkarienMaaraLabel.Visible = this.kilpailu.Laji == Laji.Kaisa;
            this.rankkarienMaaraNumericUpDown.Visible = this.kilpailu.Laji == Laji.Kaisa;

            this.kabikeMaksuDataGridViewTextBoxColumn.Visible =
                (this.kilpailu.Laji == Laji.Kaisa) &&
                (this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanRGKilpailu ||
                this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanSMKilpailu);
            this.piilotaToinenKierrosCheckBox.Visible = this.kilpailu.KilpailuOnViikkokisa;

            this.rankingKisaCheckBox.Visible = this.kilpailu.KilpailuOnViikkokisa;
            PaivitaRankingKontrollienNakyvyys();

            // Todo: Näitä ei kai nykyään tarvita ikinä
            this.seuranJasenMaksuDataGridViewTextBoxColumn.Visible = false;
            this.lisenssiMaksuDataGridViewTextBoxColumn.Visible = false;

            if (this.kilpailu.KilpailuOnViikkokisa)
            {
                if (this.tabControl1.Contains(this.kilpailuKutsuTabPage))
                {
                    this.tabControl1.Controls.Remove(this.kilpailuKutsuTabPage);
                }

                if (this.tabControl1.Contains(this.pelipaikatTabPage))
                {
                    this.tabControl1.Controls.Remove(this.pelipaikatTabPage);
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

                if (this.tabControl1.Contains(this.rankingTabPage))
                {
                    this.tabControl1.Controls.Remove(this.rankingTabPage);
                }

                if (!this.tabControl1.Contains(this.pelipaikatTabPage))
                {
                    this.tabControl1.Controls.Add(this.pelipaikatTabPage);
                }
            }

            this.TasoitusColumn.Visible = this.kilpailu.KilpailuOnTasurikisa;
        }

        public bool AvaaTiedostoDialogista()
        {
            try
            {
                this.openFileDialog1.FileName = string.Empty;

                if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AvaaKilpailuTiedostosta(this.openFileDialog1.FileName);

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaavion avaaminen epäonnistui", ex, true);
            }

            return false;
        }

        private void avaaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AvaaTiedostoDialogista();
        }

        private void AvaaKilpailuTiedostosta(string tiedosto)
        {
            if (File.Exists(tiedosto))
            {
                this.tabControl1.SelectedTab = this.kisaInfoTabPage;

                SuspenAllDataBinding();

                AvaaKilpailu(tiedosto);

                ResumeAllDataBinding();
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
                    this.kilpailu.TallennaNimella(this.openFileDialog1.FileName, true);
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
            if (this.suljeOhjelmaHeti)
            {
                this.Close();
                return;
            }

            this.pelaajienNimet = new AutoCompleteStringCollection();
            this.pelaajienNimet.AddRange(this.asetukset.Pelaajat.Select(x => x.Nimi).ToArray());

            this.pelipaikkojenNimet = new AutoCompleteStringCollection();

            PaivitaStatusRivi(string.Empty, false, 0, 0);
            PaivitaArvontaTabi();
            PaivitaViimeisimmatTiedostot();

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

            this.BringToFront();
            this.Focus();
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

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            SuspendLayout();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            ResumeLayout();
            Refresh();
        }

        private void PaivitaViimeisimmatTiedostot()
        { 
            List<ToolStripMenuItem> tiedostot = new List<ToolStripMenuItem>();

            foreach (var t in this.asetukset.ViimeisimmatKilpailut)
            {
                var viimeisimmatTiedostotItem = new ToolStripMenuItem() { Text = t.Nimi, Tag = t.Polku };
                viimeisimmatTiedostotItem.Click += viimeisimmatTiedostotItem_Click;
                tiedostot.Add(viimeisimmatTiedostotItem);
            }

            this.viimeisimmatKisatToolStripMenuItem.DropDownItems.Clear();
            this.viimeisimmatKisatToolStripMenuItem.DropDownItems.AddRange(tiedostot.ToArray());
        }

        private void viimeisimmatTiedostotItem_Click(object sender, EventArgs e)
        {
            try
            {
                AvaaKilpailuTiedostosta((string)((ToolStripMenuItem)sender).Tag);
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Viimeisimmän kilpailun lataus epäonnistui", ex, true);
            }
        }

        private void PaivitaIkkunanNimi()
        {
            string software = string.Format("KaisaKaavio v{0}", Assembly.GetEntryAssembly().GetName().Version);

            if (string.IsNullOrEmpty(this.kilpailu.Nimi))
            {
                this.Text = software;
            }
            else if (this.kilpailu.Nimi.Contains(this.kilpailu.AlkamisAikaDt.Year.ToString()))
            {
                this.Text = string.Format("{0} - {1}", software, this.kilpailu.Nimi);
            }
            else
            {
                this.Text = string.Format("{0} - {1} - {2}",
                    software,
                    this.kilpailu.Nimi,
                    this.kilpailu.AlkamisAika);
            }

            if (this.kilpailu.TestiKilpailu && !this.kilpailu.Nimi.Contains("TESTI"))
            {
                this.Text = this.Text + " (TESTI)";
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
#if PROFILE
            using (new Testaus.Profileri("Form1.SuspenAllDataBinding"))
#endif
            {
                this.kaavioBindingSource.SuspendBinding();
                this.kilpailuBindingSource.SuspendBinding();
                this.pelaajaBindingSource.SuspendBinding();
                this.peliBindingSource.SuspendBinding();
            }
        }

        private void ResumeAllDataBinding()
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.ResumeAllDataBinding"))
#endif
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
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.tabControl1_SelectedIndexChanged"))
#endif
            {
                if (this.tabControl1.SelectedTab == this.arvontaTabPage)
                {
                    this.kilpailu.PoistaTyhjatOsallistujat();

                    PaivitaPelaajienRankingPisteetOsallistujalistaan();
                    PaivitaSijoitusSarakkeenNakyvyys();
                    PaivitaPelipaikkaSarakkeenNakyvyys();

                    this.kilpailuBindingSource.ResetBindings(false);
                    this.pelaajaBindingSource.ResetBindings(false);

                    PaivitaArvontaTabi();
                }
                else if (this.tabControl1.SelectedTab == this.kisaInfoTabPage)
                {
                    try
                    {
                        this.alkamisAikaDateTimePicker.Value = this.kilpailu.AlkamisAikaDt;
                    }
                    catch (Exception ee)
                    {
                        this.loki.Kirjoita("Alkamisajan asetus epäonnistui", ee, false);
                    }

                    PaivitaRankingKontrollienNakyvyys();

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

                        this.PelinPaikkaColumn.Visible = this.kilpailu.OnUseanPelipaikanKilpailu;

                        this.kilpailu.PaivitaPelienTulokset();

                        var algoritmi = this.kilpailu.Haku(this);
                        if (algoritmi != null)
                        {
                            algoritmi.Hae();
                            PaivitaHaku(algoritmi);
                        }

                        this.pelitDataGridView.ResumeLayout();

                        ScrollaaPelitListanLoppuun();

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

                    PaivitaKaavioSolut();

                    this.kaavioBindingSource.ResetBindings(false);
                    this.kaavioBindingSource.ResumeBinding();
                    this.kaavioDataGridView.ResumeLayout();

                    PaivitaKaavioMitaliKuvat();
                }
                else if (this.tabControl1.SelectedTab == this.rahanJakoTabPage)
                {
                    AlustaRahanjako();
                    PaivitaRahanjako();
                }
                else if (this.tabControl1.SelectedTab == this.rankingTabPage)
                {
                    AlustaRankingTab();
                }
                else if (this.tabControl1.SelectedTab == this.pelipaikatTabPage)
                {
                    PaivitaPelipaikatUI();
                }

                bool pelitTabilla = this.tabControl1.SelectedTab == this.pelitTabPage;

                this.hakuTimer.Enabled = pelitTabilla;
                if (!pelitTabilla) // Peruuta käynnissä oleva haku jos poistutaan Pelit tabilta
                {
                    KeskeytaHaku();
                    PaivitaStatusRivi(string.Empty, false, 0, 100);
                }
            }
        }

        private void ScrollaaPelitListanLoppuun()
        {
            if (this.pelitDataGridView.RowCount > 3)
            {
                this.pelitDataGridView.FirstDisplayedScrollingRowIndex = this.pelitDataGridView.RowCount - 1;
            }
        }

        private Ranking.RankingAsetukset RankingAsetuksetAvatulleKilpailulle()
        {
            var sarja = this.ranking.ValittuSarja;
            if (sarja == null && this.kilpailu.RankingOsakilpailu != null)
            {
                sarja = this.ranking.ValitseRankingSarjaKilpailulle(this.kilpailu);
            }

            if (sarja != null && 
                this.kilpailu.RankingOsakilpailu != null &&
                this.ranking.ValittuSarja.SisaltaaOsakilpailun(this.kilpailu.RankingOsakilpailu))
            {
                return this.ranking.ValittuSarja.Asetukset;
            }

            return null;
        }

        private void PaivitaSijoitusSarakkeenNakyvyys()
        {
            var asetukset = RankingAsetuksetAvatulleKilpailulle();

            if ((asetukset != null && asetukset.RankingKarjetRelevantteja) ||
                this.kilpailu.Sijoittaminen != Sijoittaminen.EiSijoittamista)
            {
                this.sijoitettuDataGridViewTextBoxColumn.Visible = true;
                PaivitaSijoitettuSarake();
            }
            else
            {
                this.sijoitettuDataGridViewTextBoxColumn.Visible = false;
            }

            this.sijoitettuDataGridViewTextBoxColumn.ReadOnly =
                this.kilpailu.Sijoittaminen != Sijoittaminen.EiSijoittamista &&
                this.kilpailu.KaavioArvottu;
        }

        private void PaivitaPelipaikkaSarakkeenNakyvyys()
        {
            this.PeliPaikkaColumn.Visible = 
                !this.kilpailu.KilpailuOnViikkokisa &&
                this.kilpailu.PeliPaikat.Any(x => !x.Tyhja);

            this.PeliPaikkaColumn.ReadOnly =
                this.kilpailu.KaavioArvottu;
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
                this.kilpailu.PelienTilannePaivitysTarvitaan = false;
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
                IHakuAlgoritmi algoritmi = (IHakuAlgoritmi)e.Argument;
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
            IHakuAlgoritmi algoritmi = (IHakuAlgoritmi)e.Result;

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

        private void PaivitaHaku(IHakuAlgoritmi algoritmi)
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

                this.kilpailu.Pelit.RaiseListChangedEvents = false;

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

                this.kilpailu.Pelit.RaiseListChangedEvents = true;
                this.kilpailu.Pelit.ResetBindings();

                this.pelitDataGridView.ResumeLayout();
                this.pelitDataGridView.Enabled = true;

                ScrollaaPelitListanLoppuun();

                if (currentRow >= 0 && currentColumn >= 0)
                {
                    if (currentRow < this.pelitDataGridView.Rows.Count &&
                        currentColumn < this.pelitDataGridView.Columns.Count)
                    {
                        if (currentRow >= this.pelitDataGridView.RowCount - this.pelitDataGridView.DisplayedRowCount(true))
                        {
                            try
                            {
                                this.pelitDataGridView.CurrentCell = this.pelitDataGridView.Rows[currentRow].Cells[currentColumn];
                                this.pelitDataGridView.Focus();
                            }
                            catch (Exception exx)
                            {
                                this.loki.Kirjoita("Kursorin palautus epäonnistui haun jälkeen", exx, false);
                            }
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

        // ========={( Kisainfo-sivun ja kilpailukutsu-sivun päivitys )}======================================= //
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

        private void sijoitustenMaaraytyminenComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            try
            {
                SijoitustenMaaraytyminen s = (SijoitustenMaaraytyminen)e.ListItem;

                var field = typeof(SijoitustenMaaraytyminen).GetField(s.ToString());
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                e.Value = attributes.Length == 0 ? s.ToString() : ((DescriptionAttribute)attributes[0]).Description;
            }
            catch
            {
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (!DateTime.Equals(this.kilpailu.AlkamisAikaDt, this.alkamisAikaDateTimePicker.Value))
            {
                // Ranking tietueita tulee päivittää jos kilpailun päivämäärä muuttuu
                bool rankingKisa = this.kilpailu.RankingOsakilpailu != null;
                
                if (rankingKisa)
                {
                    this.loki.Kirjoita(string.Format("Rankingosakilpailun {0} päivämäärää muutettiin manuaalisesti {1} => {2}. Päivitetään rankingtietueita",
                        this.kilpailu.Nimi,
                        this.kilpailu.AlkamisAikaDt,
                        this.alkamisAikaDateTimePicker.Value));

                    this.ranking.PoistaRankingTietue(this.kilpailu.RankingOsakilpailu, this.kilpailu.TestiKilpailu);
                    this.kilpailu.RankingOsakilpailu = null;
                }

                this.kilpailu.AlkamisAikaDt = this.alkamisAikaDateTimePicker.Value;

                if (rankingKisa)
                {
                    this.kilpailu.RankingOsakilpailu = this.ranking.AvaaRankingTietueKilpailulle(this.kilpailu);
                }
            }
        }

        private void alkavatPelitPoytiaNmericUpDown_ValueChanged(object sender, EventArgs e)
        {
            PaivitaAlkavatPelit();
        }

        private void kilpailuKutsuAlkamisAikaTextBox_TextChanged(object sender, EventArgs e)
        {
            PaivitaAlkavatPelit();
        }

        private void kilpailuKutsuAlkamisAikaTextBox_Validated(object sender, EventArgs e)
        {
            PaivitaAlkavatPelit();
        }

        #endregion

        // ========={( Arvonta-sivun päivitys )}=============================================================== //
        #region Osallistujat/arvonta taulukko

        private void Osallistujat_ListChanged(object sender, ListChangedEventArgs e)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.Osallistujat_ListChanged"))
#endif
            {
                if (!this.kilpailunLatausKaynnissa &&
                    !this.arvontaKaynnissa)
                {
                    PaivitaOsallistujaLista();
                }
            }
        }

        private void PaivitaOsallistujaLista()
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.PaivitaOsallistujaLista"))
#endif
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(PaivitaOsallistujaLista));
                }
                else
                {
                    try
                    {
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

                        if (this.kilpailu.Tyhja)
                        {
                            osallistujaMaaraRichTextBox.Text = string.Empty;
                        }
                        else
                        {
                            var osallistujat = this.kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
                            if (osallistujat.Count() == 1)
                            {
                                osallistujaMaaraRichTextBox.Text = string.Format("{0} Osallistuja", osallistujat.Count());
                            }
                            else
                            {
                                osallistujaMaaraRichTextBox.Text = string.Format("{0} Osallistujaa", osallistujat.Count());
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.loki.Kirjoita("Ilmoittautumislistan päivitys epäonnistui", e, false);
                    }
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
                if (e.RowIndex >= 0)
                {
                    var rivi = this.osallistujatDataGridView.Rows[e.RowIndex];

                    var pelaaja = (Pelaaja)rivi.DataBoundItem;
                    if (pelaaja != null)
                    {
                        if (e.ColumnIndex == this.nimiDataGridViewTextBoxColumn.Index)
                        {
                            PaivitaOsallistujaLista();
                        }

                        if (e.ColumnIndex == this.nimiDataGridViewTextBoxColumn.Index)
                        {
                            PaivitaPelaajienRankingPisteetOsallistujalistaan();

                            var tietue = this.asetukset.Pelaajat.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, pelaaja.Nimi));
                            if (tietue != null)
                            {
                                pelaaja.Nimi = Tyypit.Nimi.MuotoileNimi(pelaaja.Nimi);
                                pelaaja.Seura = tietue.Seura;

                                if (this.kilpailu.KilpailuOnTasurikisa)
                                {
                                    pelaaja.Tasoitus = tietue.Tasoitus(this.kilpailu.Laji);
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
            catch
            { 
            }
        }

        private void PaivitaPelaajanRankingPisteetOsallistujalistaan(Ranking.RankingSarja sarja, List<Ranking.RankingPelaajaTietue> rankingKarki, Pelaaja pelaaja)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.PaivitaPelaajanRankingPisteetOsallistujalistaan"))
#endif
            {
                try
                {
                    pelaaja.Sijoitettu = string.Empty;

                    if (this.kilpailu.KilpailuOnViikkokisa && this.kilpailu.RankingKisa)
                    {
                        if (!string.IsNullOrEmpty(pelaaja.Nimi))
                        {
                            var karki = rankingKarki.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, pelaaja.Nimi));
                            if (karki != null)
                            {
                                if (karki.KumulatiivinenSijoitus >= 1 && karki.KumulatiivinenSijoitus <= 3)
                                {
                                    int pisteita = sarja.Asetukset.PisteitaVoitosta(karki.KumulatiivinenSijoitus);
                                    if (pisteita > 1)
                                    {
                                        pelaaja.Sijoitettu = string.Format("{0}P", pisteita);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    this.loki.Kirjoita(string.Format("Pelaajan {0} ranking pisteiden päivitys osallistujalistaan epäonnistui", pelaaja.Nimi), e, false);
                }
            }
        }

        private void PaivitaPelaajienRankingPisteetOsallistujalistaan()
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.PaivitaPelaajienRankingPisteetOsallistujalistaan"))
#endif
            {
                if (this.kilpailu.KilpailuOnViikkokisa && this.kilpailu.RankingKisa)
                {
                    List<Ranking.RankingPelaajaTietue> karki = null;

                    var sarja = this.ranking.AvaaRankingSarja(this.kilpailu, false);
                    if (sarja != null && sarja.Asetukset.RankingKarjetRelevantteja)
                    {
                        karki = sarja.HaeRankingKarjetKilpailulle(this.ranking, this.kilpailu);

                        this.osallistujatDataGridView.SuspendLayout();
                        this.kilpailu.Osallistujat.RaiseListChangedEvents = false;

                        foreach (var p in this.kilpailu.Osallistujat)
                        {
                            PaivitaPelaajanRankingPisteetOsallistujalistaan(sarja, karki, p);
                        }

                        this.kilpailu.Osallistujat.RaiseListChangedEvents = true;
                        this.kilpailu.Osallistujat.ResetBindings();
                        this.osallistujatDataGridView.ResumeLayout();
                    }
                }
            }
        }

        private void PaivitaArvontaTabi()
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.PaivitaArvontaTabi"))
#endif
            {
                try
                {
                    PaivitaSijoitusSarakkeenNakyvyys();
                    PaivitaOsallistujaLista();

                    if (this.kilpailu.KilpailuOnViikkokisa)
                    {
                        this.splitContainer10.Panel2Collapsed = this.kilpailu.Pelit.Count() > 0;
                    }
                    else
                    {
                        this.splitContainer10.Panel2Collapsed = true;
                    }

                    if (!this.kilpailu.KaavioArvottu &&
                        (this.kilpailu.TestiKilpailu ||
                        this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanRGKilpailu ||
                        this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanSMKilpailu))
                    {
                        this.biljardiOrgSplitContainer.Panel1Collapsed = !(
                            this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanRGKilpailu || 
                            this.kilpailu.KilpailunTyyppi == KilpailunTyyppi.KaisanSMKilpailu);
                        this.biljardiOrgSplitContainer.Panel2Collapsed = !this.kilpailu.TestiKilpailu;
                    }
                    else
                    {
                        this.biljardiOrgSplitContainer.Panel1Collapsed = false;
                        this.biljardiOrgSplitContainer.Panel2Collapsed = false;
                        this.osallistujatSplitContainer.Panel1Collapsed = true;
                    }

                    this.arvoKaavioButton.Visible = !this.kilpailu.ToinenKierrosAlkanut;
                }
                catch
                {
                }
            }
        }

        private void arvoKaavioButton_Click(object sender, EventArgs e)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.arvoKaavioButton_Click"))
#endif
            {
                bool siirrtyPelitValilehdelle = this.kilpailu.Pelit.Count() == 0;

                try
                {
                    if (this.kilpailu.OnUseanPelipaikanKilpailu ||
                        this.kilpailu.Sijoittaminen != Sijoittaminen.EiSijoittamista)
                    { 
                        using (var arvontaPopup = new ArvontaPopup(this.asetukset.Sali, this.kilpailu, this.loki))
                        {
                            if (arvontaPopup.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                            {
                                this.loki.Kirjoita("Arvonta peruttu tietoja tarkistettaessa");
                                return;
                            }
                        }
                    }

                    this.arvontaKaynnissa = true;

                    this.kilpailu.Osallistujat.RaiseListChangedEvents = false;
                    this.kilpailu.Pelit.RaiseListChangedEvents = false;

                    this.osallistujatDataGridView.SuspendLayout();
                    this.pelitDataGridView.SuspendLayout();
                    
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
                        this.kilpailu.PeruutaArvonta();

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
                finally
                {
                    this.kilpailu.Osallistujat.RaiseListChangedEvents = true;
                    this.kilpailu.Osallistujat.ResetBindings();

                    this.kilpailu.Pelit.RaiseListChangedEvents = true;
                    this.kilpailu.Pelit.ResetBindings();

                    this.pelitDataGridView.ResumeLayout();
                    this.osallistujatDataGridView.ResumeLayout();

                    if (!siirrtyPelitValilehdelle)
                    {
                        this.osallistujatDataGridView.Refresh();
                    }

                    this.arvontaKaynnissa = false;
                }
            }
        }

        private void osallistujatDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Testaus.Profileri.KirjaaKutsu("Form1.osallistujatDataGridView_CellFormatting");

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
            try
            {
                if (osallistujatDataGridView.CurrentCell != null)
                {
                    if (osallistujatDataGridView.CurrentCell.ColumnIndex == this.PeliPaikkaColumn.Index)
                    {
                        EhdotaPelipaikkoja(e, osallistujatDataGridView.CurrentCell.ColumnIndex, this.PeliPaikkaColumn.Index);
                    }
                    else if (osallistujatDataGridView.CurrentCell.ColumnIndex == this.nimiDataGridViewTextBoxColumn.Index)
                    {
                        EhdotaPelaajienNimia(e, osallistujatDataGridView.CurrentCell.ColumnIndex, this.nimiDataGridViewTextBoxColumn.Index);
                    }
                }
            }
            catch
            { 
            }
        }

        private void jalkiIlmoittautuneetDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
#if !DEBUG
            try
            {
                EhdotaPelaajienNimia(e, jalkiIlmoittautuneetDataGridView.CurrentCell.ColumnIndex, this.nimiDataGridViewTextBoxColumn2.Index);
            }
            catch
            { 
            }
#endif
        }

        private void EhdotaPelipaikkoja(DataGridViewEditingControlShowingEventArgs e, int nykySarake, int nimiSarake)
        {
            try
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    int column = nykySarake;
                    if (column == nimiSarake)
                    {
                        if (this.kilpailu.PeliPaikat.Any(x => !x.Tyhja))
                        {
                            List<string> nimet = new List<string>();

                            nimet.Add(this.asetukset.Sali.LyhytNimi);

                            foreach (var sali in this.kilpailu.PeliPaikat)
                            {
                                nimet.Add(sali.LyhytNimi);
                            }

                            this.pelipaikkojenNimet.Clear();
                            this.pelipaikkojenNimet.AddRange(nimet.ToArray());

                            tb.AutoCompleteMode = AutoCompleteMode.Suggest;
                            tb.AutoCompleteCustomSource = this.pelipaikkojenNimet;
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
        }

        private void EhdotaPelaajienNimia(DataGridViewEditingControlShowingEventArgs e, int nykySarake, int nimiSarake)
        {
            try
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    int column = nykySarake;
                    if (column == nimiSarake)
                    {
                        if (this.asetukset.Pelaajat.Any(x => !string.IsNullOrEmpty(x.Nimi)))
                        {
                            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                            tb.AutoCompleteCustomSource = this.pelaajienNimet;
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

        private void osallistujatDataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                foreach (DataGridViewCell cell in this.osallistujatDataGridView.SelectedCells)
                {
                    Pelaaja pelaaja = (Pelaaja)cell.OwningRow.DataBoundItem;

                    if (e.KeyChar == (char)Keys.Back ||
                        e.KeyChar == (char)Keys.Delete)
                    {
                        if (cell.ColumnIndex == this.nimiDataGridViewTextBoxColumn.Index)
                        {
                            pelaaja.Nimi = string.Empty;
                            PaivitaPelaajienRankingPisteetOsallistujalistaan();
                        }

                        if (cell.ColumnIndex == this.seuraDataGridViewTextBoxColumn.Index)
                        {
                            pelaaja.Seura = string.Empty;
                        }

                        if (cell.ColumnIndex == this.TasoitusColumn.Index)
                        {
                            pelaaja.Tasoitus = string.Empty;
                        }

                        if (cell.ColumnIndex == this.sijoitettuDataGridViewTextBoxColumn.Index)
                        {
                            pelaaja.Sijoitettu = string.Empty;
                        }

                        if (cell.ColumnIndex == this.osMaksuDataGridViewTextBoxColumn.Index)
                        {
                            pelaaja.OsMaksu = string.Empty;
                        }

                        if (cell.ColumnIndex == this.veloitettuDataGridViewTextBoxColumn.Index)
                        {
                            pelaaja.Veloitettu = string.Empty;
                        }

                        if (cell.ColumnIndex == this.kabikeMaksuDataGridViewTextBoxColumn.Index)
                        {
                            pelaaja.KabikeMaksu = string.Empty;
                        }
                    }

                    return;
                }
            }
            catch
            {
            }
        }

        private void jalkiIlmoittautuneetDataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                foreach (DataGridViewCell cell in this.jalkiIlmoittautuneetDataGridView.SelectedCells)
                {
                    Pelaaja pelaaja = (Pelaaja)cell.OwningRow.DataBoundItem;

                    if (e.KeyChar == (char)Keys.Back ||
                        e.KeyChar == (char)Keys.Delete)
                    {
                        if (cell.ColumnIndex == this.nimiDataGridViewTextBoxColumn2.Index)
                        {
                            pelaaja.Nimi = string.Empty;
                        }
                    }

                    return;
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

        /// <summary>
        /// Automaattinen osallistumismaksun täydennys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void osallistujatDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
#if PROFILE
            using (new Testaus.Profileri("Form1.osallistujatDataGridView_RowValidated"))
#endif
            {
                try
                {
                    if (e.RowIndex >= 0 && e.RowIndex < this.osallistujatDataGridView.Rows.Count)
                    {
                        var rivi = this.osallistujatDataGridView.Rows[e.RowIndex];
                        var pelaaja = (Pelaaja)rivi.DataBoundItem;
                        if (pelaaja != null)
                        {
                            if (string.IsNullOrEmpty(pelaaja.OsMaksu))
                            {
                                int maksu = 0;
                                if (Tyypit.Luku.ParsiKokonaisluku(kilpailu.OsallistumisMaksu, out maksu) && maksu > 0)
                                {
                                    pelaaja.OsMaksu = maksu.ToString();
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
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
#if PROFILE
            using (new Testaus.Profileri("Form1.PaivitaPeliTilanteet"))
#endif
            {
                try
                {
                    this.pelitDataGridView.SuspendLayout();

                    lock (this.kilpailu)
                    {
                        this.kilpailu.Pelit.RaiseListChangedEvents = false;

                        this.kilpailu.PaivitaPelitValmiinaAlkamaan();

                        this.kilpailu.Pelit.RaiseListChangedEvents = true;
                    }

                    this.pelitDataGridView.ResumeLayout();
                    this.pelitDataGridView.Refresh();
                }
                catch (Exception ex)
                {
                    this.loki.Kirjoita("Pelitilanteiden päivitys epäonnistui", ex, false);
                    this.kilpailu.PelienTilannePaivitysTarvitaan = true;
                }
            }
        }

        private void pelitDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Testaus.Profileri.KirjaaKutsu("Form1.pelitDataGridView_CellFormatting");

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
                return;
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

                    // Pelin kierros
                    if (e.ColumnIndex == this.kierrosDataGridViewTextBoxColumn.Index)
                    {
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.SelectionBackColor = Color.White;

                        if (peli.Tilanne == PelinTilanne.Kaynnissa)
                        {
                            e.CellStyle.ForeColor = Color.Black;
                            e.CellStyle.SelectionForeColor = Color.Black;
                            e.CellStyle.Font = this.paksuPieniFontti;
                        }
                        else if (peli.Tilanne == PelinTilanne.Pelattu)
                        {
                            e.CellStyle.ForeColor = Color.Gray;
                            e.CellStyle.SelectionForeColor = Color.Gray;
                            e.CellStyle.Font = this.ohutPieniFontti;
                        }
                        else 
                        {
                            e.CellStyle.ForeColor = Color.Black;
                            e.CellStyle.SelectionForeColor = Color.Black;
                            e.CellStyle.Font = this.ohutPieniFontti;
                        }

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

        private int virheellinenPeliPopupNaytetty = 0;

        private void pelitDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < this.pelitDataGridView.Rows.Count)
                {
                    Peli peli = (Peli)this.pelitDataGridView.Rows[e.RowIndex].DataBoundItem;

                    // Näytä popup jos käyttäjä yrittää päivittää muita pelejä kun yhdessä pelissä on virhe
                    if (this.virheellinenPeliPopupNaytetty < 2)
                    {
                        var virheellinenPeli = this.kilpailu.Pelit.FirstOrDefault(x => x != peli && x.Tulos == PelinTulos.Virheellinen);
                        if (virheellinenPeli != null &&
                            (e.ColumnIndex == pisteet1DataGridViewTextBoxColumn.Index ||
                            e.ColumnIndex == pisteet2DataGridViewTextBoxColumn.Index ||
                            e.ColumnIndex == Tilanne.Index ||
                            e.ColumnIndex == poytaDataGridViewTextBoxColumn.Index))
                        {
                            this.virheellinenPeliPopupNaytetty++;

                            if (this.virheellinenPeliPopupNaytetty == 2)
                            {
                                MessageBox.Show(
                                    string.Format(
                                        "Kaaviossa on virheellinen pelin tulos (punainen rivi) pelissä:\n{0}\nKorjaa tämän pelin tulos ennen kuin voit jatkaa muita pelejä.",
                                        virheellinenPeli.Kuvaus()),
                                    "Virhe",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }

                            return;
                        }
                    }

                    // Pelin "Käynnistä" painike
                    if (e.ColumnIndex == this.TilanneTeksti.Index)
                    {
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
                                    peli.KaynnistaPeli(this.asetukset, true);
                                    return;

                                case PelinTilanne.Pelattu:
                                    break;

                                case PelinTilanne.Kaynnissa:
                                    break;

                                default:
                                    break;
                            }
                        }

                        if ((peli.Tilanne == PelinTilanne.Pelattu || peli.Tilanne == PelinTilanne.Kaynnissa) && peli.Tulos != PelinTulos.Virheellinen)
                        {
                            KeskeytaHaku();

                            using (var popup = new PelinTiedotPopup(this.kilpailu, peli, this.loki))
                            {
                                if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    pelitDataGridView.Refresh();
                                }
                            }
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
            //Testaus.Profileri.KirjaaKutsu("Form1.pelitDataGridView_RowPrePaint");

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
            //Testaus.Profileri.KirjaaKutsu("Form1.pelitDataGridView_CellPainting");

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

        private void piilotaToinenKierrosCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.pelitDataGridView.Refresh();
        }

        private Point GetControlLocation(Control control)
        {
            Control parent = control.Parent;

            Point offset = control.Location;            

            while (parent != null)
            {
                offset.X += parent.Left;
                offset.Y += parent.Top;                
                parent = parent.Parent;
            }

            offset.X -= this.Left;
            offset.Y -= this.Top;

            return offset;
        }

        private void pelitDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var peli = (Peli)(this.pelitDataGridView.Rows[e.RowIndex].DataBoundItem);

                    if (e.ColumnIndex == this.poytaDataGridViewTextBoxColumn.Index)
                    {
                        if (peli.Tilanne == PelinTilanne.ValmiinaAlkamaan)
                        {
                            var p = GetControlLocation(this.pelitDataGridView);
                            var r = this.pelitDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                            var borderWidth = (this.Size.Width - this.ClientSize.Width) / 2;
                            var borderHeight = (this.Size.Height - this.ClientSize.Height) / 2 - borderWidth;

                            var poydat = this.kilpailu.VapaatPoydat(peli, this.asetukset);
                            if (poydat.Any())
                            {
                                if (poydat.Count() == 1)
                                {
                                    this.toolTip1.Show(
                                        string.Format("Vapaana pöytä {0}", poydat.First()),
                                        this,
                                        p.X + r.X + borderWidth,
                                        p.Y + r.Y + borderHeight,
                                        5000);
                                }
                                else
                                {
                                    this.toolTip1.Show(
                                        string.Format("Vapaana pöydät {0}", string.Join(",", poydat)),
                                        this,
                                        p.X + r.X + borderWidth,
                                        p.Y + r.Y + borderHeight,
                                        5000);
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

        #endregion

        // ========={( Kaavio tab )}=========================================================================== //
        #region Kaavio

        private int PelinNumeroKaaviossa(int sarake)
        {
            if (sarake >= 3 && sarake < this.KaavioKuvaSarake.Index)
            {
                return (sarake - 3) / 2;
            }

            return -1;
        }

        private void PaivitaKaavioSolut()
        {
            try
            {
                // Piilota seura sarake jos se olisi tyhjä
                if (this.kilpailu.OsallistujatJarjestyksessa.Any(x => !string.IsNullOrEmpty(x.Seura)))
                {
                    this.kaavioDataGridView.Columns[2].Visible = true;
                }
                else
                {
                    this.kaavioDataGridView.Columns[2].Visible = false;
                }

                int ylimSarakkeita = this.kilpailu.KilpailuOnPaattynyt ? 0 : 1;

                // Piilotetaan turhat sarakkeet kaavio taulukosta
                for (int i = 3; i < this.Voitot.Index; ++i)
                {
                    int kierros = (i - 3) / 2;

                    if (kierros >= this.kilpailu.MaxKierros + ylimSarakkeita)
                    {
                        this.kaavioDataGridView.Columns[i].Visible = false;
                    }
                    else
                    {
                        this.kaavioDataGridView.Columns[i].Visible = true;
                    }
                }

                foreach (var c in this.kaavioDataGridView.Columns)
                {
                    DataGridViewColumn column = (DataGridViewColumn)c;
                    if (column != null)
                    {
                        if (column.Index % 2 == 1 &&
                            PelinNumeroKaaviossa(column.Index) >= 2)
                        {
                            column.ReadOnly = false;
                        }
                        else
                        {
                            column.ReadOnly = true;
                        }

                        if (column.Index == KaavioKuvaSarake.Index)
                        {
                            column.Visible = this.kilpailu.KilpailuOnPaattynyt;
                        }
                    }
                }
            }
            catch
            { 
            }
        }

        private void kaavioDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Rivin muotoilu
            try
            {
                var row = this.kaavioDataGridView.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];

                Pelaaja pelaaja = (Pelaaja)row.DataBoundItem;
                if (pelaaja != null)
                {
                    if (e.ColumnIndex == 1)
                    {
                        if (pelaaja.Sijoitus.Pudotettu)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Black;
                        }
                    }

                    int pelinNumero = PelinNumeroKaaviossa(e.ColumnIndex);
                    if (pelinNumero >= 0)
                    {
                        if (pelinNumero >= (pelaaja.Pelit.Count + 1))
                        {
                            if (pelaaja.Sijoitus.Pudotettu)
                            {
                                e.CellStyle.BackColor = Color.DarkGray;
                            }
                            else
                            {
                                e.CellStyle.BackColor = Color.White;
                            }
                        }
                        else if (pelinNumero == pelaaja.Pelit.Count)
                        {
                            if (pelaaja.Sijoitus.Pudotettu)
                            {
                                e.CellStyle.BackColor = Color.DarkGray;
                            }
                            else if (e.ColumnIndex % 2 == 1 &&
                                KaavioPeliEditoitavissa(pelaaja, pelinNumero))
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
                            e.CellStyle.BackColor = Color.White;
                        }

                        if (pelinNumero < pelaaja.Pelit.Count)
                        {
                            Pelaaja.PeliTietue peli = pelaaja.Pelit[pelinNumero];

                            if (e.ColumnIndex % 2 == 1)
                            {
                                e.Value = peli.Vastustaja;
                            }
                            else
                            {
                                if (peli.Pelattu)
                                {
                                    cell.Style.Font = this.paksuFontti;

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
                                    e.CellStyle.Font = this.ohutFontti;
                                    e.Value = string.Empty;
                                    e.CellStyle.ForeColor = Color.Black;
                                }
                            }
                        }
                        else
                        {
                            e.CellStyle.Font = this.ohutFontti;
                            e.Value = string.Empty;
                            e.CellStyle.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void PaivitaKaavioMitaliKuvat()
        {
            try
            {
                for (int r = 0; r < this.kaavioDataGridView.Rows.Count; ++r)
                {
                    var row = this.kaavioDataGridView.Rows[r];
                    Pelaaja pelaaja = (Pelaaja)((DataGridViewRow)row).DataBoundItem;
                    if (pelaaja != null)
                    {
                        if (pelaaja.Sijoitus.Sijoitus == 1)
                        {
                            row.Cells[KaavioKuvaSarake.Index].Value = Properties.Resources.Gold;
                        }
                        else if (pelaaja.Sijoitus.Sijoitus == 2)
                        {
                            row.Cells[KaavioKuvaSarake.Index].Value = Properties.Resources.Silver;
                        }
                        else if (pelaaja.Sijoitus.Sijoitus == 3)
                        {
                            row.Cells[KaavioKuvaSarake.Index].Value = Properties.Resources.Bronze;
                        }
                        else
                        {
                            row.Cells[KaavioKuvaSarake.Index].Value = Properties.Resources.Lapinakuva;
                        }
                    }
                    else
                    {
                        row.Cells[KaavioKuvaSarake.Index].Value = Properties.Resources.Lapinakuva;
                    }
                }
            }
            catch
            { 
            }
        }

        private bool KaavioPeliEditoitavissa(Pelaaja pelaaja, int pelinNumero)
        {
            if (pelinNumero < 2) // Ekan ja tokan kierroksen pelejä ei sovi editoida
            {
                return false;
            }

            if (pelinNumero == pelaaja.Pelit.Count)
            {
                if (pelaaja.Sijoitus.Pudotettu)
                {
                    return false;
                }

                if (pelaaja.Sijoitus.Tappiot > 1)
                {
                    return false;
                }

                if (pelaaja.Pelit.Any(x => !x.Pelattu && x.Pudari))
                {
                    return false;
                }

                if ((pelaaja.Pelit.Count(x => !x.Pelattu) + pelaaja.Sijoitus.Tappiot) > 1)
                {
                    return false;
                }

                if (this.kilpailu.KilpailuOnPaattynyt)
                {
                    return false;
                }

                return true;
            }
            else if (pelinNumero == pelaaja.Pelit.Count - 1)
            { 
                var peli = pelaaja.Pelit[pelinNumero];
                if (peli.Tilanne != PelinTilanne.Pelattu &&
                    peli.Tilanne != PelinTilanne.Kaynnissa)
                {
                    return true; // Sallitaan poistaa haettu peli 
                }
            }

            return false;
        }

        private void kaavioDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return;
                }

                int pelinNumero = PelinNumeroKaaviossa(e.ColumnIndex);
                if (pelinNumero >= 0)
                {
                    var row = this.kaavioDataGridView.Rows[e.RowIndex];
                    var cell = row.Cells[e.ColumnIndex];

                    Pelaaja pelaaja = (Pelaaja)row.DataBoundItem;

                    if (KaavioPeliEditoitavissa(pelaaja, pelinNumero) &&
                        (e.ColumnIndex % 2 == 1)) 
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

        private void kaavioDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int pelinNumero = PelinNumeroKaaviossa(e.ColumnIndex);
                if (pelinNumero > 1)
                {
                    var row = this.kaavioDataGridView.Rows[e.RowIndex];

                    Pelaaja pelaaja = (Pelaaja)row.DataBoundItem;
                    if (pelaaja != null)
                    {
                        if (KaavioPeliEditoitavissa(pelaaja, pelinNumero))
                        {
                            var value = (string)row.Cells[e.ColumnIndex].Value;
                            if (string.IsNullOrEmpty(value))
                            {
                                if (VoiPoistaaPelinKaaviosta(pelinNumero, pelaaja))
                                {
                                    PoistaPeliKaaviosta(pelinNumero, pelaaja);
                                }
                            }
                            else
                            {
                                if (pelinNumero > pelaaja.Pelit.Count - 1)
                                {
                                    int id = -1;
                                    if (Int32.TryParse(value, out id))
                                    {
                                        string virhe = string.Empty;
                                        if (VoiHakeaPelinKaavioonManuaalisesti(pelaaja, id, out virhe))
                                        {
                                            LisaaPeliKaavioon(pelaaja.Id, id);
                                        }
                                        else
                                        {
                                            MessageBox.Show(virhe, "Pelin hakeminen ei ole mahdollista", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
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

        private void kaavioDataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                foreach (DataGridViewCell cell in this.kaavioDataGridView.SelectedCells)
                {
                    int pelinNumero = PelinNumeroKaaviossa(cell.ColumnIndex);
                    if (pelinNumero > 1)
                    {
                        Pelaaja pelaaja = (Pelaaja)cell.OwningRow.DataBoundItem;

                        if (e.KeyChar == (char)Keys.Back ||
                            e.KeyChar == (char)Keys.Delete)
                        {
                            if (KaavioPeliEditoitavissa(pelaaja, pelinNumero) &&
                                VoiPoistaaPelinKaaviosta(pelinNumero, pelaaja))
                            {
                                PoistaPeliKaaviosta(pelinNumero, pelaaja);
                            }
                        }

                        return;
                    }
                }
            }
            catch
            {
            }
        }

        private bool VoiPoistaaPelinKaaviosta(int pelinNumero, Pelaaja pelaaja)
        {
            if (pelinNumero <= pelaaja.Pelit.Count)
            {
                var pelitietue = pelaaja.Pelit[pelinNumero];
                var peli = this.kilpailu.Pelit.LastOrDefault(x => 
                    x.Kierros == pelitietue.Kierros && 
                    x.SisaltaaPelaajat(pelaaja.Id, pelitietue.Vastustaja));

                if (peli != null && 
                    peli.Tilanne != PelinTilanne.Pelattu &&
                    peli.Tilanne != PelinTilanne.Kaynnissa)
                {
                    return true;
                }
            }

            return false;
        }

        private bool VoiHakeaPelinKaavioonManuaalisesti(Pelaaja pelaaja, int vastustajaId, out string virhe)
        {
            virhe = string.Empty;

            if (pelaaja.Id == vastustajaId)
            {
                virhe = "Pelaaja ei voi hakea itseään!";
                return false;
            }

            var vastustaja = this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == vastustajaId);
            if (vastustaja == null)
            {
                virhe = string.Format("Kaaviosta ei löydy pelaajaa numerolla {0}!", vastustajaId);
                return false;
            }

            if (vastustaja.Sijoitus.Pudotettu ||
                vastustaja.Sijoitus.Tappiot > 1)
            {
                virhe = string.Format("Ei voi hakea! Vastustaja {0} on jo pudonnut kilpailusta!", vastustajaId);
                return false;
            }

            if (vastustaja.Pelit.Any(x => !x.Pelattu && x.Pudari))
            {
                virhe = string.Format("Ei voi hakea! Vastustajalla on haettuja pelejä pelaamatta", vastustajaId);
                return false;
            }

            if ((pelaaja.Pelit.Count(x => !x.Pelattu) + pelaaja.Sijoitus.Tappiot) > 1)
            {
                virhe = string.Format("Ei voi hakea! Vastustajalla on haettuja pelejä pelaamatta", vastustajaId);
                return false;
            }

            return true;
        }

        private void PoistaPeliKaaviosta(int pelinNumero, Pelaaja pelaaja)
        {
            try
            {
                if (pelinNumero < pelaaja.Pelit.Count)
                {
                    var peli = pelaaja.Pelit[pelinNumero];

                    var poistettavaPeli = this.kilpailu.Pelit.LastOrDefault(x =>
                        x.Kierros == peli.Kierros &&
                        x.SisaltaaPelaajat(peli.Vastustaja, pelaaja.Id));

                    if (poistettavaPeli != null &&
                        poistettavaPeli.Tilanne != PelinTilanne.Pelattu &&
                        poistettavaPeli.Tilanne != PelinTilanne.Kaynnissa)
                    {
                        this.kilpailu.PoistaPeli(poistettavaPeli, true);

                        this.loki.Kirjoita(string.Format("Poistettiin peli {0} kaaviosta manuaalisesti", poistettavaPeli.Kuvaus()));

                        this.kaavioDataGridView.SuspendLayout();

                        this.kilpailu.PaivitaKaavioData();
                        PaivitaKaavioSolut();

                        this.kaavioDataGridView.ResumeLayout();
                        this.kaavioDataGridView.Refresh();
                    }
                }
            }
            catch
            {
            }
        }

        private void LisaaPeliKaavioon(int pelaaja, int vastustaja)
        {
            try
            {
                this.kilpailu.LisaaPeli(
                    this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelaaja),
                    this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == vastustaja));

                var peli = this.kilpailu.Pelit.LastOrDefault(x => x.SisaltaaPelaajat(pelaaja, vastustaja));
                if (peli != null)
                {
                    this.loki.Kirjoita(string.Format("Lisättiin peli {0} kaavioon manuaalisesti", peli.Kuvaus()));
                }

                this.kaavioDataGridView.SuspendLayout();

                this.kilpailu.PaivitaKaavioData();
                PaivitaKaavioSolut();

                this.kaavioDataGridView.ResumeLayout();
                this.kaavioDataGridView.Refresh();
            }
            catch
            {
            }
        }

        private void kaavioDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        #endregion

        // ========={( Sbil keskustelupalstalle kopioituvat tekstit )}========================================= //
        #region Sbil keskustelupalsta

        private void PaivitaKilpailukutsuTeksti()
        {
            Tyypit.Teksti teksti = new Tyypit.Teksti();

            teksti.InfoRivi("Kilpailun nimi", this.kilpailu.Nimi);
            teksti.InfoRivi("Järjestäjä", this.kilpailu.JarjestavaSeura);
            teksti.RivinVaihto();
            teksti.InfoRivi("Osallistumismaksu", this.kilpailu.OsallistumisMaksu);
            teksti.InfoRivi("Lisenssivaatimus", this.kilpailu.LisenssiVaatimus);
            teksti.InfoRivi("Maksutapa", this.kilpailu.MaksuTapa);
            teksti.InfoRivi("Osallistumisoikeus", this.kilpailu.OsallistumisOikeus);
            teksti.InfoRivi("Pukeutuminen", this.kilpailu.Pukeutuminen);
            teksti.InfoRivi("Palkinnot", this.kilpailu.Palkinnot);
            teksti.InfoRivi("Ilmoittautuminen", this.kilpailu.Ilmoittautuminen);
            teksti.InfoRivi("Alkamisaika", this.kilpailu.AlkamisAikaString());
            teksti.InfoRivi("Kesto", this.kilpailu.Yksipaivainen ? "Kilpailu on yksipäiväinen" : "Kilpailu on kaksipäiväinen");
            teksti.RivinVaihto();

            List<Sali> salit = new List<Sali>();

            salit.Add(this.asetukset.Sali);

            foreach (var s in this.kilpailu.PeliPaikat)
            {
                if (s.Kaytossa || !kilpailu.KaavioArvottu)
                {
                    salit.Add(s);
                }
            }

            bool ekasali = true;

            foreach (var sali in salit)
            {
                KirjoitaPelipaikanTiedot(teksti, sali, ekasali);
                ekasali = false;
            }

            teksti.RivinVaihto();
            teksti.OsionVaihto();
            teksti.RivinVaihto();
            teksti.InfoRivi("Kilpailunjohtaja", this.kilpailu.KilpailunJohtaja);
            teksti.InfoRivi("Puhelinnumero", this.kilpailu.PuhelinNumero);
            teksti.RivinVaihto();

            teksti.InfoRivi("Lisätietoa", this.kilpailu.LisaTietoa);

            var osallistujat = this.kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
            if (osallistujat.Count() > 0)
            {
                teksti.RivinVaihto();
                teksti.Otsikko("Ilmoittautuneet");
                teksti.RivinVaihto();

                int i = 1;

                foreach (var osallistuja in osallistujat)
                {
                    if (i == (this.kilpailu.PelaajiaEnintaan + 1))
                    {
                        teksti.NormaaliRivi("Varalla:");
                    }

                    teksti.NormaaliRivi(string.Format("{0}. {1} {2}", i, osallistuja.Nimi, osallistuja.Seura));
                    i++;
                }
            }

            this.kilpailuKutsuRichTextBox.Rtf = teksti.Rtf;
            this.kilpailuKutsuRichTextBox.Tag = teksti.Sbil;
        }

        private void KirjoitaPelipaikanTiedot(Tyypit.Teksti teksti, Sali sali, bool varsinainenPelipaikka)
        {
            if (this.kilpailu.KaavioArvottu && !sali.Kaytossa)
            {
                return;
            }

            teksti.RivinVaihto();
            teksti.OsionVaihto();

            if (varsinainenPelipaikka)
            {
                teksti.InfoRivi("Pelipaikka", sali.Nimi);
            }
            else
            {
                if (this.kilpailu.KaavioArvottu)
                {
                    teksti.InfoRivi("Varasali", sali.Nimi);
                }
                else 
                {
                    teksti.InfoRivi("Varasali (tarvittaessa)", sali.Nimi);
                }
            }

            teksti.InfoRivi("Pelipaikan osoite", sali.Osoite);
            teksti.InfoRivi("Pelipaikan puhelinnumero", sali.PuhelinNumero);
            teksti.RivinVaihto();

            int poytia = sali.Poydat.Where(x => x.Kaytossa).Count();
            if (poytia > 0)
            {
                teksti.InfoRivi("Käytössä", string.Format("{0} pöytää", poytia));
            }

            if (sali.Linkit.Any())
            {
                teksti.RivinVaihto();
                teksti.Otsikko("Linkit");
                teksti.RivinVaihto();

                foreach (var linkki in sali.Linkit)
                {
                    teksti.Linkki(linkki.Teksti, linkki.Osoite, null);
                }
            }

            if (sali.Toimitsijat.Any(x => !string.IsNullOrEmpty(x.Nimi)))
            {
                teksti.RivinVaihto();
                teksti.Otsikko("Pelipaikan vastuuhenkilöt");
                teksti.RivinVaihto();

                foreach (var toimitsija in sali.Toimitsijat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
                {
                    teksti.NormaaliTeksti(string.Format("{0} ({1})", toimitsija.Nimi, toimitsija.Rooli));

                    if (!string.IsNullOrEmpty(toimitsija.PuhelinNumero))
                    {
                        teksti.NormaaliTeksti(string.Format(" - puh {0}", toimitsija.PuhelinNumero));
                    }

                    teksti.RivinVaihto();
                }
            }
        }

        private void PaivitaAlkavatPelit()
        {
            Tyypit.Teksti teksti = new Tyypit.Teksti();

            teksti.InfoRivi(this.kilpailu.Nimi, "Ensimmäisten kierrosten pelit");
            teksti.RivinVaihto();

            int kierros = 0;
            string pelipaikka = string.Empty;

            Dictionary<string, int> peliNumerot = new Dictionary<string,int>();

            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Kierros <= 2).ToArray())
            {
                if (peli.Kierros != kierros ||
                    !string.Equals(peli.Paikka, pelipaikka))
                {
                    if (peli.Kierros != kierros && peli.Kierros != 1)
                    {
                        teksti.RivinVaihto();
                    }
                    else 
                    {
                        if (!string.Equals(peli.Paikka, pelipaikka))
                        {
                            teksti.RivinVaihto();
                        }
                    }

                    kierros = peli.Kierros;
                    pelipaikka = peli.Paikka;

                    if (!string.IsNullOrEmpty(pelipaikka))
                    {
                        teksti.PaksuTeksti(string.Format("{0}. kierros - {1}", kierros, pelipaikka));
                    }
                    else
                    {
                        teksti.PaksuTeksti(string.Format("{0}. kierros", kierros));
                    }

                    if (peli.OnPudotusPeli())
                    {
                        teksti.NormaaliTeksti(" (pudotuspeli)");
                    }

                    teksti.RivinVaihto();
                    teksti.RivinVaihto();
                }

                var salit = this.kilpailu.Salit();
                var sali = salit.FirstOrDefault(x => string.Equals(x.LyhytNimi, peli.Paikka));

                int pelinumero = 1;

                if (sali != null)
                {
                    if (peliNumerot.ContainsKey(sali.LyhytNimi))
                    {
                        peliNumerot[sali.LyhytNimi]++;
                        pelinumero = peliNumerot[sali.LyhytNimi];
                    }
                    else
                    {
                        peliNumerot.Add(sali.LyhytNimi, 1);
                    }

                    var poydat = sali.Poydat.Where(x => x.Kaytossa);

                    if (pelinumero <= poydat.Count())
                    {
                        peli.RichTextKuvausAlkaviinPeleihin(teksti,
                            poydat.ElementAt(pelinumero - 1).Numero,
                            this.kilpailu.KellonAika);
                    }
                    else 
                    {
                        peli.RichTextKuvausAlkaviinPeleihin(teksti, null, null);
                    }
                }
                else
                {
                    peli.RichTextKuvausAlkaviinPeleihin(teksti, null, null);
                }
            }

            teksti.RivinVaihto();
            teksti.OsionVaihto();
            teksti.LoppuMainos();

            this.alkavatPelitRichTextBox.Rtf = teksti.Rtf;
            this.alkavatPelitRichTextBox.Tag = teksti.Sbil;
        }

        private void PaivitaPelitTeksti()
        {
            Tyypit.Teksti teksti = new Tyypit.Teksti();

            teksti.Otsikko(this.kilpailu.Nimi);
            teksti.OsionVaihto();

            List<int> pelienKestot = new List<int>();
            int keskimaarainenPelinKesto = 0;
            int pelejaKeskimaaranLaskemiseksi = 0;

            if (!this.kilpailu.KilpailuOnViikkokisa && !this.kilpailu.KilpailuOnPaattynyt)
            {
                bool linkkeja = this.asetukset.Sali.Linkit.Count > 0;
                bool striimilinkkeja = this.asetukset.Sali.Poydat.Any(x => !string.IsNullOrEmpty(x.StriimiLinkki));
                bool tuloslinkkeja = this.asetukset.Sali.Poydat.Any(x => !string.IsNullOrEmpty(x.TulosLinkki));

                if (linkkeja || striimilinkkeja || tuloslinkkeja)
                {
                    if (linkkeja)
                    {
                        teksti.RivinVaihto();
                        teksti.Otsikko("Linkit");
                        teksti.RivinVaihto();

                        foreach (var linkki in this.asetukset.Sali.Linkit)
                        {
                            teksti.NormaaliTeksti(string.Format("{0} : ", linkki.Teksti));
                            teksti.Linkki(null, linkki.Osoite, null);
                        }

                        teksti.RivinVaihto();
                    }

                    if (striimilinkkeja || tuloslinkkeja)
                    {
                        teksti.RivinVaihto();
                        teksti.Otsikko("Striimit ja tulostaulut");
                        teksti.RivinVaihto();

                        foreach (var poyta in this.asetukset.Sali.Poydat.Where(x => !string.IsNullOrEmpty(x.StriimiLinkki) || !string.IsNullOrEmpty(x.TulosLinkki)))
                        {
                            teksti.NormaaliTeksti(string.Format("Pöytä {0} :", poyta.Numero));

                            if (!string.IsNullOrEmpty(poyta.StriimiLinkki))
                            {
                                teksti.NormaaliTeksti(" (");
                                teksti.Linkki(null, "striimi", poyta.StriimiLinkki);
                                teksti.NormaaliTeksti(")");
                            }

                            if (!string.IsNullOrEmpty(poyta.TulosLinkki))
                            {
                                teksti.NormaaliTeksti(" (");
                                teksti.Linkki(null, "tulostaulu", poyta.TulosLinkki);
                                teksti.NormaaliTeksti(")");
                            }

                            teksti.RivinVaihto();
                        }
                    }

                    teksti.RivinVaihto();
                    teksti.OsionVaihto();
                }
            }

            // Laitetaan tulokset heti viestin alkuun jos kilpailu on jo päättynyt
            if (this.kilpailu.KilpailuOnPaattynyt)
            {
                teksti.RivinVaihto();
                KirjoitaTuloksetTeksti(teksti);
                teksti.RivinVaihto();
                teksti.OsionVaihto();
            }

            int kierros = 0;
            string pelipaikka = string.Empty;

            if (this.kilpailu.Pelit.Count > 0)
            {
                teksti.RivinVaihto();

                foreach (var peli in this.kilpailu.Pelit.ToArray())
                {
                    if (peli.Kierros != kierros || !string.Equals(peli.Paikka, pelipaikka))
                    {
                        if (peli.Kierros != kierros && peli.Kierros != 1)
                        {
                            teksti.RivinVaihto();
                        }
                        else if (!string.Equals(peli.Paikka, pelipaikka))
                        {
                            teksti.RivinVaihto();
                        }

                        kierros = peli.Kierros;
                        pelipaikka = peli.Paikka;

                        if (this.kilpailu.OnUseanPelipaikanKilpailu)
                        {
                            teksti.PaksuTeksti(string.Format("{0}. Kierros - {1}", kierros, pelipaikka));
                        }
                        else
                        {
                            teksti.PaksuTeksti(string.Format("{0}. Kierros", kierros));
                        }

                        var mukana = this.kilpailu.MukanaOlevatPelaajatEnnenPelia(peli);
                        if (mukana.Count() == 2)
                        {
                            teksti.NormaaliTeksti(" (finaali)");
                        }
                        else if (peli.OnPudotusPeli())
                        {
                            teksti.NormaaliTeksti(" (pudari)");
                        }

                        if (kierros != 1)
                        {
                            string alkoi = string.Empty;
                            string paattyi = string.Empty;

                            var ekapeli = this.kilpailu.Pelit.Where(x => 
                                x.Kierros == kierros && 
                                !string.IsNullOrEmpty(x.Alkoi) &&
                                string.Equals(x.Paikka, pelipaikka)).FirstOrDefault();

                            if (ekapeli != null)
                            {
                                alkoi = ekapeli.Alkoi;
                            }

                            if (!this.kilpailu.Pelit.Any(x => x.Kierros <= kierros && x.Tilanne != PelinTilanne.Pelattu))
                            {
                                var vikapeli = this.kilpailu.Pelit.Where(x => 
                                    x.Kierros == kierros && 
                                    !string.IsNullOrEmpty(x.Alkoi) &&
                                    string.Equals(x.Paikka, pelipaikka)).LastOrDefault();

                                if (vikapeli != null)
                                {
                                    paattyi = vikapeli.Paattyi;
                                }
                            }

                            if (!string.IsNullOrEmpty(alkoi))
                            {
                                teksti.NormaaliTeksti(" ");

                                if (!string.IsNullOrEmpty(paattyi))
                                {
                                    teksti.PieniTeksti(string.Format(" {0} - {1}", alkoi, paattyi));
                                }
                                else
                                {
                                    teksti.PieniTeksti(string.Format(" alkoi {0}", alkoi));
                                }
                            }
                        }

                        teksti.RivinVaihto();
                        teksti.RivinVaihto();
                    }

                    Sali sali = this.asetukset.Sali;

                    if (this.kilpailu.OnUseanPelipaikanKilpailu)
                    {
                        var s = this.kilpailu.Salit().FirstOrDefault(x => string.Equals(x.LyhytNimi, pelipaikka));
                        if (s != null)
                        {
                            sali = s;
                        }
                    }

                    peli.RichTextKuvaus(sali, teksti);

                    try
                    {
                        if (!string.IsNullOrEmpty(peli.Alkoi) &&
                            !string.IsNullOrEmpty(peli.Paattyi))
                        {
                            int kesto = 0;
                            if (Tyypit.Aika.AikaeroMinuutteina(peli.Alkoi, peli.Paattyi, out kesto) && kesto > 0)
                            {
                                keskimaarainenPelinKesto += kesto;
                                pelejaKeskimaaranLaskemiseksi++;
                                pelienKestot.Add(kesto);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                teksti.RivinVaihto();
                teksti.OsionVaihto();
            }

            if (kierros > 2 && !this.kilpailu.KilpailuOnPaattynyt)
            {
                teksti.RivinVaihto();
                KirjoitaTuloksetTeksti(teksti);
                teksti.RivinVaihto();
                teksti.OsionVaihto();
            }

            if (pelejaKeskimaaranLaskemiseksi > 2)
            {
                teksti.RivinVaihto();
                teksti.InfoRivi("Pelien keskimääräinen kesto", string.Format("{0} minuuttia", keskimaarainenPelinKesto / pelejaKeskimaaranLaskemiseksi));
                teksti.RivinVaihto();
                teksti.OsionVaihto();
            }

            if (this.kilpailu.KilpailuOnPaattynyt &&
                this.kilpailu.RankingKisa)
            {
                // Päivittää ranking pisteet avatulle kilpailulle
                this.ranking.TallennaAvatutSarjat();
                var rankingSarja = this.ranking.ValitseRankingSarjaKilpailulle(this.kilpailu);
                if (rankingSarja != null)
                {
                    teksti.RivinVaihto();
                    rankingSarja.KirjoitaTilanne(teksti);
                    teksti.RivinVaihto();
                    teksti.OsionVaihto();
                }
            }

            teksti.LoppuMainos();

            this.pelitRichTextBox.Rtf = teksti.Rtf;
            this.pelitRichTextBox.Tag = teksti.Sbil;
        }

        private void KirjoitaTuloksetTeksti(Tyypit.Teksti teksti)
        {
            teksti.PaksuTeksti("Tulokset");

            if (this.kilpailu.SijoitustenMaaraytyminen != SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista)
            {
                teksti.NormaaliTeksti(" ");
                switch (this.kilpailu.SijoitustenMaaraytyminen)
                {
                    case SijoitustenMaaraytyminen.KaksiParastaKierroksistaLoputPisteista:
                        teksti.PieniTeksti(" (Finaalin hävinnyt sijoittuu toiseksi)");
                        break;
                    
                    case SijoitustenMaaraytyminen.KolmeParastaKierroksistaLoputPisteista: 
                        teksti.PieniTeksti(" (Sijat 1-3 kierrosten perusteella)");
                        break;
                }
            }

            teksti.RivinVaihto();
            teksti.RivinVaihto();

            Dictionary<string, List<int>> sarjat = new Dictionary<string, List<int>>();
            foreach (var peli in this.kilpailu.Pelit)
            {
                if (!sarjat.ContainsKey(peli.Pelaaja1))
                {
                    sarjat.Add(peli.Pelaaja1, new List<int>());
                }

                if (!sarjat.ContainsKey(peli.Pelaaja2))
                {
                    sarjat.Add(peli.Pelaaja2, new List<int>());
                }

                int sarja = 0;
                if (Int32.TryParse(peli.PisinSarja1, out sarja) && sarja > 0)
                {
                    sarjat[peli.Pelaaja1].Add(sarja);
                }
                if (Int32.TryParse(peli.ToiseksiPisinSarja1, out sarja) && sarja > 0)
                {
                    sarjat[peli.Pelaaja1].Add(sarja);
                }
                if (Int32.TryParse(peli.PisinSarja2, out sarja) && sarja > 0)
                {
                    sarjat[peli.Pelaaja2].Add(sarja);
                }
                if (Int32.TryParse(peli.ToiseksiPisinSarja2, out sarja) && sarja > 0)
                {
                    sarjat[peli.Pelaaja2].Add(sarja);
                }
            }

            var eiTyhjatSarjat = sarjat.Where(x => x.Value.Count > 0);
            foreach (var s in eiTyhjatSarjat)
            {
                var tempSarjat = s.Value.OrderByDescending(x => x).ToArray();
                s.Value.Clear();
                s.Value.AddRange(tempSarjat);
            }

            bool paattynyt = this.kilpailu.KilpailuOnPaattynyt;

            var tulosluettelo = this.kilpailu.Tulokset();

            int maxKierros = 1;

            if (paattynyt)
            {
                var pudonneet = tulosluettelo.Where(x => x.Pudotettu);
                if (pudonneet.Count() > 0)
                {
                    maxKierros = pudonneet.Select(x => x.PudonnutKierroksella).Max() + 1;
                }
            }

            int voittajanKierros = -1;
            int kakkosenKierros = -1;
            int kolmostenKierros = -1;
            int kolmosia = 0;

            foreach (var osallistuja in tulosluettelo)
            {
                if (osallistuja.SijoitusOnVarma)
                {
                    teksti.NormaaliTeksti(string.Format("{0}. {1} - {2}/{3}",
                        osallistuja.Sijoitus,
                        this.kilpailu.PelaajanNimiTulosluettelossa(osallistuja.Pelaaja.Id.ToString()),
                        osallistuja.Voitot,
                        osallistuja.Pisteet));

                    if (this.kilpailu.SijoitustenMaaraytyminen != SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista &&
                        osallistuja.Sijoitus == 1)
                    {
                        voittajanKierros = maxKierros;
                    }
                    else if (this.kilpailu.SijoitustenMaaraytyminen != SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista &&
                        osallistuja.Sijoitus == 2)
                    {
                        kakkosenKierros = osallistuja.PudonnutKierroksella;
                        teksti.NormaaliTeksti(" ");
                        teksti.PieniTeksti(" (*)");
                    }
                    else if (this.kilpailu.SijoitustenMaaraytyminen == SijoitustenMaaraytyminen.KolmeParastaKierroksistaLoputPisteista &&
                        osallistuja.Sijoitus == 3)
                    {
                        kolmostenKierros = osallistuja.PudonnutKierroksella;
                        kolmosia++;
                        teksti.NormaaliTeksti(" ");
                        teksti.PieniTeksti(" (**)");
                    }
                }
                else 
                {
                    teksti.NormaaliTeksti(string.Format("{0}.", osallistuja.Sijoitus));
                }

                teksti.RivinVaihto();
            }

            if (voittajanKierros >= 0 || kakkosenKierros >= 0 || kolmostenKierros >= 0)
            {
                teksti.RivinVaihto();

                if (kakkosenKierros >= 0)
                {
                    teksti.PieniTeksti(string.Format("(*) = Finalisti, pudonnut {0}. kierroksella", kakkosenKierros));
                    teksti.RivinVaihto();
                }

                if (kolmostenKierros >= 0)
                {
                    if (kolmosia == 1)
                    {
                        teksti.PieniTeksti(string.Format("(**) = Kolmonen, pudonnut {0}. kierroksella", kolmostenKierros));
                    }
                    else
                    {
                        teksti.PieniTeksti(string.Format("(**) = Kolmoset, pudonneet {0}. kierroksella", kolmostenKierros));
                    }
                    teksti.RivinVaihto();
                }
            }

            if (eiTyhjatSarjat.Count() > 0)
            {
                teksti.RivinVaihto();
                teksti.Otsikko("Pisimmät sarjat");
                teksti.RivinVaihto();

                foreach (var s in eiTyhjatSarjat.OrderByDescending(x => x.Value.First()))
                {
                    teksti.NormaaliRivi(string.Format("{0} {1}", s.Key, string.Join(", ", s.Value.ToArray())));
                }
            }
        }

        private void PaivitaTuloksetTeksti()
        {
            Tyypit.Teksti teksti = new Tyypit.Teksti();

            teksti.Otsikko(this.kilpailu.Nimi);
            teksti.RivinVaihto();

            KirjoitaTuloksetTeksti(teksti);

            teksti.RivinVaihto();
            teksti.OsionVaihto();
            teksti.LoppuMainos();

            this.tuloksetRichTextBox.Rtf = teksti.Rtf;
            this.tuloksetRichTextBox.Tag = teksti.Sbil;
        }

        private void tuloksetSijoitustenMaaraytyminenComboBox_Validated(object sender, EventArgs e)
        {
            PaivitaPelitTeksti();
            PaivitaTuloksetTeksti();
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

        private void AvaaTiedosto(string tiedosto)
        {
            try
            {
                string kansio = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(kansio, tiedosto));
                psi.UseShellExecute = true;
                Process.Start(psi);
            }
            catch (Exception e)
            {
                this.loki.Kirjoita(string.Format("Tiedoston {0} avaaminen epäonnistui!", tiedosto), e, true);
            }
        }

        private void kaisaKaavioOhjelmanTiedotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string teksti = string.Format("KaisaKaavio v.{0}{1}Copyright © Ilari Nieminen 2024{1}{1}" +
                "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.{1}{1}" +
                "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.{1}{1}" +
                "You should have received a copy of the GNU General Public License along with this program. If not, see https://www.gnu.org/licenses/",
                Assembly.GetEntryAssembly().GetName().Version,
                Environment.NewLine);

            MessageBox.Show(teksti, "Tietoa ohjelmasta", MessageBoxButtons.OK, MessageBoxIcon.Information);

            AvaaTiedosto("LICENSE.txt");
        }

        private void tietoaOhjelmastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string teksti = string.Format("KaisaKaavio v.{0}{1}Copyright © Ilari Nieminen 2024{1}{1}" +
                "Ohjelman suunnittelu ja toteutus: Ilari Nieminen{1}{1}" +
                "Ympyräkaavioiden asiantuntija: Jarmo Tainio{1}{1}" +
                "Testaus: Ilari Nieminen ja Jarmo Tainio{1}{1}" +
                "Grafiikka: Ilari Nieminen (valokuvat) sekä https://www.iconarchive.com (kuvakkeet){1}{1}" +
                "Ohjelman käyttöopas löytyy samasta kansiosta ohjelman kanssa, sekä 'tietoa' valikon kautta",
                Assembly.GetEntryAssembly().GetName().Version,
                Environment.NewLine);

            MessageBox.Show(teksti, "Tietoa ohjelmasta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void kayttoopasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AvaaTiedosto("Ohje.pdf");
        }

        private void versiohistoriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AvaaTiedosto("Versiohistoria.txt");
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

        private void testiajoBackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
        }

        private void testiajoBackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var testi = (Testaus.ITestiAjo)e.Argument;

            if (testi.Aja())
            {
                MessageBox.Show(
                    string.Format("Testi onnistui: {0} kaaviota pelattu oikein läpi", testi.OnnistuneitaTesteja),
                    "Testi valmis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = testi.VirheKansio,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                catch
                {
                }

                MessageBox.Show(
                    string.Format("Testi epäonnistui: {0} kaaviota pelattu virheellisesti", testi.EpaonnistuneitaTesteja),
                    "Testi valmis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void pelaaTestikaaviotToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            using (var popup = new Testaus.TestiPopup())
            {
                if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

                        Testaus.ITestiAjo testi = null;

                        if (popup.MonteCarloTestaus)
                        {
                            testi = new Testaus.MonteCarloTestiAjo(
                                popup.PoytienMaara,
                                popup.SatunnainenPeliJarjestys,
                                popup.MonteCarloKisoja,
                                popup.MonteCarloMinPelaajia,
                                popup.MonteCarloMaxPelaajia,
                                this);
                        }
                        else
                        {
                            testi = new Testaus.TestiAjo(
                                testiKansio,
                                popup.PoytienMaara,
                                popup.SatunnainenPeliJarjestys,
                                this);
                        }

                        this.Enabled = false;
                        this.testiajoBackgroundWorker1.RunWorkerAsync(testi);
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
            }
#endif
        }

        private void pelaaKaavioUudelleenToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            try
            {
                this.openFileDialog1.FileName = string.Empty;

                if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(this.openFileDialog1.FileName))
                    {
                        this.tabControl1.SelectedTab = this.kisaInfoTabPage;

                        SuspenAllDataBinding();

                        AvaaKilpailuUudelleenPelattavaksi(this.openFileDialog1.FileName);

                        ResumeAllDataBinding();
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Kaavion avaaminen uudelleen pelattavaksi epäonnistui", ex, true);
            }
#endif
        }

        private void AvaaKilpailuUudelleenPelattavaksi(string tiedosto)
        {
#if DEBUG
            try
            {
                if (!string.IsNullOrEmpty(this.kilpailu.Tiedosto))
                {
                    this.kilpailu.Tallenna();
                }

                if (this.uudelleenPelaaminen.AvaaKilpailu(tiedosto))
                {
                    this.kilpailu.Tallenna();
                    this.asetukset.ViimeisinKilpailu = this.kilpailu.Tiedosto;
                    this.uudelleenPelausButton.Visible = true;

                    PaivitaIkkunanNimi();
                    PaivitaKilpailuTyyppi();
                }            
            }
            catch (Exception e)
            {
                this.loki.Kirjoita(string.Format("KaisaKaavion avaaminen uudelleen pelattavaksi epäonnistui tiedostosta{0}", tiedosto), e, true);
            }
#endif
        }

        private void uudelleenPelausButton_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (this.uudelleenPelaaminen.SeuraavaPeli())
            {
                this.kilpailu.HakuTarvitaan = true;
            }
#endif
        }

        private void LisaaTestiPelaajia(int n)
        {
            var random = new Random();

            this.kilpailu.Osallistujat.RaiseListChangedEvents = false;
            this.osallistujatDataGridView.SuspendLayout();

            for (int i = 0; i < n; ++i)
            {
                this.kilpailu.LisaaPelaaja(Tyypit.Nimi.KeksiNimi(random));
            }

            this.osallistujatDataGridView.ResumeLayout();
            this.kilpailu.Osallistujat.RaiseListChangedEvents = true;
            this.kilpailu.Osallistujat.ResetBindings();
        }

        private void lisaa5TestiPelaajaaButton_Click(object sender, EventArgs e)
        {
            LisaaTestiPelaajia(5);
        }

        private void lisaa10TestiPelaajaaButton_Click(object sender, EventArgs e)
        {
            LisaaTestiPelaajia(10);
        }

        private void lisaa20TestiPelaajaaButton_Click(object sender, EventArgs e)
        {
            LisaaTestiPelaajia(20);
        }

        #endregion

        // ========={( Ranking )}============================================================================== //
        #region Ranking

        private void AlustaRankingTab()
        {
            this.ranking.TallennaAvatutSarjat();

            this.rankingSivuaRakennetaan = true;
            this.rankingTabPage.SuspendLayout();

            this.haeRankingSarjaButton.Enabled = false;

            this.rankingVuosiComboBox.Items.Clear();
            this.rankingVuosiComboBox.Items.AddRange(this.ranking.Vuodet.Select(x => (object)x).ToArray());

            this.ranking.ValitseRankingSarjaKilpailulle(this.kilpailu);

            this.rankingPituusComboBox.SelectedItem = this.kilpailu.RankingKisaTyyppi;
            this.rankingVuosiComboBox.SelectedItem = this.kilpailu.AlkamisAikaDt.Year;
            this.rankingHakuLajiComboBox.SelectedItem = this.kilpailu.RankingKisaLaji;

            this.rankingSarjatKausiCcomboBox.SelectedIndex = Tyypit.Aika.RankingSarjanNumeroAjasta(Ranking.RankingSarjanPituus.Vuodenaika, this.kilpailu.AlkamisAikaDt);
            this.rankingSarjatKuukausiComboBox.SelectedIndex = this.kilpailu.AlkamisAikaDt.Month - 1;
            this.rankingSarjatPuolivuottaComboBox.SelectedIndex = Tyypit.Aika.RankingSarjanNumeroAjasta(Ranking.RankingSarjanPituus.Puolivuotta, this.kilpailu.AlkamisAikaDt);

            PaivitaRankingsarjaBoksienNakyvyys();

            this.rankingKokonaistilanneRichTextBox.Rtf = this.ranking.KokonaisTilanneRtf;
            this.rankingOsakilpailuRichTextBox.Rtf = this.ranking.OsakilpailunTilanneRtf;

            this.rankingSivuaRakennetaan = false;

            this.rankingTabPage.ResumeLayout();
            PaivitaRankingTaulukko();
            this.tabControl1.Focus();
        }

        private void PaivitaRankingsarjaBoksienNakyvyys()
        {
            Ranking.RankingSarjanPituus pituus = (Ranking.RankingSarjanPituus)this.rankingPituusComboBox.SelectedItem;

            this.rankingSarjatPuolivuottaComboBox.Visible = pituus == Ranking.RankingSarjanPituus.Puolivuotta;
            this.rankingSarjatKuukausiComboBox.Visible = pituus == Ranking.RankingSarjanPituus.Kuukausi;
            this.rankingSarjatKausiCcomboBox.Visible = pituus == Ranking.RankingSarjanPituus.Vuodenaika;
        }

        private bool rankingSivuaRakennetaan = false;

        private void ranking_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (string.Equals(e.PropertyName, "ValittuSarja"))
                {
                    PaivitaRankingTaulukko();
                }

                if (string.Equals(e.PropertyName, "ValittuOsakilpailu"))
                {
                }
            }
            catch
            { 
            }
        }

        private void PaivitaRankingTaulukko()
        {
            if (this.rankingSivuaRakennetaan)
            {
                return;
            }

            try
            {
                this.rankingDataGridView.SuspendLayout();
                this.rankingPelaajaTietueBindingSource.SuspendBinding();

                this.rankingPelaajaTietueBindingSource.DataSource = this.ranking.ValittuSarja != null ? this.ranking.ValittuSarja.Osallistujat : null;

                int maxVisibleColumn = 3 + (this.ranking.ValittuSarja != null ? (this.ranking.ValittuSarja.Osakilpailut.Count * 2) : 0);

                foreach (var c in this.rankingDataGridView.Columns)
                {
                    DataGridViewColumn column = (DataGridViewColumn)c;
                    if (column != null)
                    {
                        column.Visible = column.Index <= maxVisibleColumn;

                        if (column.Visible)
                        {
                            if ((column.Index > 3))
                            {
                                if (column.Index % 2 == 1)
                                {
                                    int osakilpailu = SarakeOsakilpailuksi(column.Index);
                                    if (osakilpailu >= 0)
                                    {
                                        column.HeaderText = string.Format("{0}.{1}",
                                            this.ranking.ValittuSarja.Osakilpailut[osakilpailu].AlkamisAikaDt.Day,
                                            this.ranking.ValittuSarja.Osakilpailut[osakilpailu].AlkamisAikaDt.Month);
                                    }
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }

                this.rankingPelaajaTietueBindingSource.ResumeBinding();
                this.rankingDataGridView.ResumeLayout();

                if (maxVisibleColumn > 4)
                {
                    this.rankingDataGridView.FirstDisplayedScrollingColumnIndex = maxVisibleColumn;
                }
            }
            catch
            { 
            }
        }

        private void rankingKisaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.ranking.TyhjennaSarjatMuistista();

            if (this.rankingKisaCheckBox.Checked && this.kilpailu.RankingOsakilpailu == null)
            {
                this.kilpailu.RankingOsakilpailu = this.ranking.AvaaRankingTietueKilpailulle(this.kilpailu);
                this.kilpailu.RankingOsakilpailu.OnRankingOsakilpailu = true;
            }

            PaivitaRankingKontrollienNakyvyys();
        }

        private void PaivitaRankingKontrollienNakyvyys()
        {
            try
            {
                PaivitaSijoitettuSarake();

                this.rankingKisaTyyppiComboBox.Visible = this.rankingKisaCheckBox.Visible && this.rankingKisaCheckBox.Checked;
                this.rankingSarjanLajiComboBox.Visible = this.rankingKisaCheckBox.Visible && this.rankingKisaCheckBox.Checked;
                this.rankingSarjanLajiLabel.Visible = this.rankingKisaCheckBox.Visible && this.rankingKisaCheckBox.Checked;
                this.rankingSarjanTyyppiLabel.Visible = this.rankingKisaCheckBox.Visible && this.rankingKisaCheckBox.Checked;
                this.rankingKisaPisteytysButton.Visible = this.rankingKisaCheckBox.Visible && this.rankingKisaCheckBox.Checked;
            }
            catch
            {
            }
        }

        private void RankingComboBoxEditBegin(bool reset)
        {
            this.haeRankingSarjaButton.Enabled = true;
        }

        private void RankingComboBoxEditEnd(bool reset)
        {
            try
            {
                if (reset)
                {
                    //this.rankingSarjaComboBox.SelectedItem = this.ranking.ValittuSarja;
                    this.rankingOsakilpailuComboBox.SelectedItem = this.ranking.ValittuOsakilpailu;
                }
                this.haeRankingSarjaButton.Enabled = true;
            }
            catch
            { 
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

            if (sender == this.rankingPituusComboBox)
            {
                PaivitaRankingsarjaBoksienNakyvyys();
            }
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

            this.rankingOsakilpailuRichTextBox.Rtf = this.ranking.OsakilpailunTilanneRtf;
            this.rankingKokonaistilanneRichTextBox.Rtf = this.ranking.KokonaisTilanneRtf;
        }

        private void rankingSarjaComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            try
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
            catch
            { 
            }
        }

        private Ranking.RankingPelaajaTietue HaeRankingTietue(int sarake, int rivi)
        {
            int osakilpailu = SarakeOsakilpailuksi(sarake);
            if (osakilpailu >= 0 && rivi >= 0 && rivi < this.rankingDataGridView.Rows.Count)
            {
                Ranking.RankingOsakilpailu kilpailu = this.ranking.ValittuSarja.Osakilpailut[osakilpailu];
                if (kilpailu != null)
                {
                    Ranking.RankingPelaajaTietue pelaaja = (Ranking.RankingPelaajaTietue)this.rankingDataGridView.Rows[rivi].DataBoundItem;
                    if (pelaaja != null)
                    {
                        return kilpailu.Osallistujat.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, pelaaja.Nimi));
                    }
                }
            }

            return null;
        }

        private void rankingDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 0 || e.RowIndex < 0)
                {
                    return;
                }

                if (e.ColumnIndex == 2)
                {
                    e.CellStyle.BackColor = Color.Green;
                    e.CellStyle.ForeColor = Color.White;
                }

                if (e.ColumnIndex == 3)
                {
                    e.CellStyle.BackColor = Color.DarkGreen;
                    e.CellStyle.ForeColor = Color.White;

                    if (e.RowIndex >= 0 && e.RowIndex < this.rankingDataGridView.Rows.Count)
                    {
                        int kisoja = 0;

                        Ranking.RankingPelaajaTietue pelaaja = (Ranking.RankingPelaajaTietue)this.rankingDataGridView.Rows[e.RowIndex].DataBoundItem;
                        if (pelaaja != null)
                        {
                            if (this.ranking.ValittuSarja != null)
                            {
                                kisoja = this.ranking.ValittuSarja.Osakilpailut.Count(x => x.Osallistujat.Any(y => string.Equals(y.Nimi, pelaaja.Nimi, StringComparison.OrdinalIgnoreCase)));
                            }
                        }

                        e.Value = kisoja > 0 ? kisoja.ToString() : string.Empty;
                        e.FormattingApplied = true;
                    }
                    return;
                }

                if (e.ColumnIndex > 3)
                {
                    if ((e.ColumnIndex / 2) % 2 == 0)
                    {
                        e.CellStyle.BackColor = this.rankingRivinVari0;
                    }
                    else
                    {
                        e.CellStyle.BackColor = this.rankingRivinVari1;
                    }

                    var tietue = HaeRankingTietue(e.ColumnIndex, e.RowIndex);
                    if (tietue != null)
                    {
                        if (e.ColumnIndex % 2 == 0)
                        {
                            e.Value = tietue.Sijoitus > 0 ? tietue.Sijoitus.ToString() : string.Empty;
                            e.CellStyle.Font = this.paksuPieniFontti;
                            e.CellStyle.ForeColor = tietue.Sijoitus < 4 ? Color.Black : this.tummaHarmaa;
                            e.FormattingApplied = true;
                        }
                        else
                        {
                            e.Value = tietue.RankingPisteet > 0 ? tietue.RankingPisteet.ToString() : "-";
                            e.CellStyle.Font = this.isoOhutFontti;
                            e.CellStyle.ForeColor = tietue.RankingPisteet > 0 ? Color.Black : Color.DarkGray;
                            e.FormattingApplied = true;
                        }
                        return;
                    }
                    else
                    {
                        if (e.ColumnIndex % 2 == 0)
                        {
                            e.Value = "-";
                            e.FormattingApplied = true;
                            e.CellStyle.ForeColor = Color.Gray;
                        }
                        else 
                        {
                            e.Value = string.Empty;
                        }
                    }
                }
            }
            catch
            { 
            }
        }

        private int SarakeOsakilpailuksi(int sarake)
        {
            if (this.ranking.ValitutOsakilpailut != null && sarake > 3)
            {
                int osakilpailu = (sarake - 4) / 2;
                if (osakilpailu >= 0 && osakilpailu < this.ranking.ValitutOsakilpailut.Count)
                {
                    return this.ranking.ValitutOsakilpailut.Count - 1 - osakilpailu;
                }
            }

            return -1;
        }

        private void rankingDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0 && e.RowIndex >= 0)
                {
                    var t = (Ranking.RankingPelaajaTietue)this.rankingDataGridView.Rows[e.RowIndex].DataBoundItem;
                    if (t != null && t.Sijoitus >= 1 && t.Sijoitus <= 3)
                    {
                        e.PaintBackground(e.ClipBounds, true);

                        Point p = e.CellBounds.Location;
                        p.X += 2;

                        if (t.Sijoitus == 1)
                        {
                            e.Graphics.DrawImage(Properties.Resources.Gold, p);
                        }
                        else if (t.Sijoitus == 2)
                        {
                            e.Graphics.DrawImage(Properties.Resources.Silver, p);
                        }
                        else
                        {
                            e.Graphics.DrawImage(Properties.Resources.Bronze, p);
                        }
                        e.PaintContent(e.ClipBounds);
                        e.Handled = true;
                        return;
                    }
                }

                // Mitalirinkulan piirtäminen
                var tietue = HaeRankingTietue(e.ColumnIndex, e.RowIndex);
                if (tietue != null && tietue.Sijoitus >= 1 && tietue.Sijoitus <= 3)
                {
                    if (e.ColumnIndex % 2 == 0)
                    {
                        var row = this.rankingDataGridView.Rows[e.RowIndex];
                        var cell = row.Cells[e.ColumnIndex];

                        e.PaintBackground(e.ClipBounds, true);

                        int w = e.CellBounds.Width + cell.OwningColumn.DividerWidth;
                        int h = e.CellBounds.Height + cell.OwningRow.DividerHeight;

                        Rectangle rectDimensions = e.CellBounds;

                        int size = Math.Min(w, h) - 16;

                        rectDimensions.Width = size;
                        rectDimensions.Height = size;
                        rectDimensions.X = e.CellBounds.X + (w - size) / 2 - 2;
                        rectDimensions.Y = e.CellBounds.Y + (h - size) / 2 - 0;

                        var oldMode = e.Graphics.SmoothingMode;
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        if (tietue.Sijoitus == 1)
                        {
                            e.Graphics.FillEllipse(this.kultaHarja, rectDimensions);
                        }
                        else if (tietue.Sijoitus == 2)
                        {
                            e.Graphics.FillEllipse(this.hopeaHarja, rectDimensions);
                        }
                        else
                        {
                            e.Graphics.FillEllipse(this.pronssiHarja, rectDimensions);
                        }

                        e.Graphics.DrawEllipse(this.paksuRajaKyna, rectDimensions);
                        e.Graphics.SmoothingMode = oldMode;

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

        private void rankingDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void kopioiKokonaistilanneButton_Click(object sender, EventArgs e)
        {
            KopioiLeikepoydalle((string)this.rankingKokonaistilanneRichTextBox.Tag);
        }

        private void kopioOsakilpailuButton_Click(object sender, EventArgs e)
        {
            KopioiLeikepoydalle((string)this.rankingOsakilpailuRichTextBox.Tag);
        }

        private void rankingKisaTyyppiComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            Ranking.RankingSarjanPituus t = (Ranking.RankingSarjanPituus)e.ListItem;

            var field = typeof(Ranking.RankingSarjanPituus).GetField(t.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            e.Value = attributes.Length == 0 ? t.ToString() : ((DescriptionAttribute)attributes[0]).Description;
        }

        private void haeRankingSarjaButton_Click(object sender, EventArgs e)
        {
            try
            {
                Laji laji = (Laji)this.rankingHakuLajiComboBox.SelectedItem;
                int vuosi = (int)this.rankingVuosiComboBox.SelectedItem;
                Ranking.RankingSarjanPituus pituus = (Ranking.RankingSarjanPituus)this.rankingPituusComboBox.SelectedItem;

                int numero = 0;

                switch (pituus)
                {
                    case Ranking.RankingSarjanPituus.Puolivuotta: numero = this.rankingSarjatPuolivuottaComboBox.SelectedIndex; break;
                    case Ranking.RankingSarjanPituus.Vuodenaika: numero = this.rankingSarjatKausiCcomboBox.SelectedIndex; break;
                    case Ranking.RankingSarjanPituus.Kuukausi: numero = this.rankingSarjatKuukausiComboBox.SelectedIndex + 1; break;
                }

                this.ranking.ValitseRankingSarja(vuosi, laji, pituus, numero, this.kilpailu);
                this.haeRankingSarjaButton.Enabled = false;

                this.rankingKokonaistilanneRichTextBox.Rtf = this.ranking.KokonaisTilanneRtf;
                this.rankingOsakilpailuRichTextBox.Rtf = this.ranking.OsakilpailunTilanneRtf;

                PaivitaRankingTaulukko();
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Ranking sarjan avaus epäonnistui", ex, true);
            }
        }

        private void rankingKisaPisteytysButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var popup = new Ranking.RankingPisteytysPopup(this.asetukset.RankingPisteytys(this.kilpailu.RankingKisaLaji)))
                {
                    this.ranking.TallennaAvatutSarjat();

                    if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.loki.Kirjoita(string.Format("Ranking pisteytystä päivitettiin lajille {0}", this.kilpailu.RankingKisaLaji));

                        try
                        {
                            this.ranking.TyhjennaSarjatMuistista();
                            this.ranking.ValitseRankingSarjaKilpailulle(this.kilpailu);

                            if (this.ranking.ValittuSarja != null &&
                                this.kilpailu.RankingOsakilpailu != null &&
                                this.ranking.ValittuSarja.SisaltaaOsakilpailun(this.kilpailu.RankingOsakilpailu))
                            {
                                this.ranking.ValittuSarja.Asetukset.KopioiAsetuksista(this.asetukset.RankingPisteytys(this.kilpailu.RankingKisaLaji));
                            }

                            this.ranking.TallennaAvatutSarjat();
                        }
                        catch (Exception ex) 
                        {
                            this.loki.Kirjoita("Ranking pisteytysten päivitys epäonnistui!", ex, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Rankingasetusten editointi epäonnistui!", ex, false);
            }
        }

        private void rankingKisaTyyppiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ranking.TallennaAvatutSarjat();
            this.ranking.TyhjennaSarjatMuistista();
        }

        private void rankingSarjanLajiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ranking.TallennaAvatutSarjat();
            this.ranking.TyhjennaSarjatMuistista();
        }

        private void rankingHakuLajiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.haeRankingSarjaButton.Enabled = true;
        }

        private void rankingSarjatPuolivuottaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.haeRankingSarjaButton.Enabled = true;
        }

        private void rankingSarjatKausiCcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.haeRankingSarjaButton.Enabled = true;
        }

        private void rankingSarjatKuukausiComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.haeRankingSarjaButton.Enabled = true;
        }

        #endregion

        // ========={( Dokumenttien tulostaminen )}============================================================ //
        #region Tulostaminen

        public void TulostaDokumentti(PrintDocument dokumentti, string kuvaus)
        {
            try
            {
                this.printPreviewDialog1.Document = dokumentti;
                if (this.printPreviewDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.loki.Kirjoita(string.Format("Tulostettiin dokumentti {0}", kuvaus), null, false);
                }
                else 
                {
                    this.loki.Kirjoita(string.Format("Dokumentin {0} tulostaminen peruutettu", kuvaus), null, false);
                }
            }
            catch (Exception e)
            {
                this.loki.Kirjoita(string.Format("Dokumentin {0} tulostaminen epäonnistui", kuvaus), e, true);
            }
        }

        #endregion

        // ========={( Pelipaikat sivun päivitys )}============================================================ //
        #region Pelipaikat

        private void PaivitaPelipaikatUI()
        {
            this.kaavioidenYhdistaminenComboBox.Enabled = !this.kilpailu.KaavioArvottu;

            if (this.kilpailu.PeliPaikat.Any(x => !x.Tyhja))
            {
                this.useanPaikanKisaLabel.Text = "Valitse, missä vaiheessa kilpailua siirrytään pelaamaan yhdellä pelipaikalla:";
                this.kaavioidenYhdistaminenComboBox.Visible = true;
                this.kilpailu.KaavioidenYhdistaminenKierroksesta = (this.kaavioidenYhdistaminenComboBox.SelectedIndex + 4).ToString();

#if DEBUG
                Debug.WriteLine(string.Format("## Kaaviot yhdistetään kierroksesta {0}", this.kilpailu.KaavioidenYhdistaminenKierroksesta));
#endif
            }
            else 
            {
                this.useanPaikanKisaLabel.Text = "Lisää yksi tai useampia varasaleja alla olevalle listalle:";
                this.kaavioidenYhdistaminenComboBox.Visible = false;
                this.kilpailu.KaavioidenYhdistaminenKierroksesta = string.Empty;
            }
        }

        private void peliPaikatDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var rivi = peliPaikatDataGridView.Rows[e.RowIndex];
                    Sali sali = (Sali)rivi.DataBoundItem;
                    if (sali != null)
                    {
                        if (e.ColumnIndex == PelipaikanDetaljitColumn.Index)
                        {
                            using (var popup = new SalinTiedotPopup(sali, this.kilpailu.KaavioArvottu))
                            {
                                popup.ShowDialog();
                            }

                            this.peliPaikatDataGridView.Refresh();
                            PaivitaPelipaikatUI();
                        }
                        else if (e.ColumnIndex == PoistaPelipaikkaColumn.Index)
                        {
                            if (!this.kilpailu.KaavioArvottu)
                            {
                                this.kilpailu.PeliPaikat.Remove(sali);
                                this.kilpailu.PeliPaikat.ResetBindings();
                                PaivitaPelipaikatUI();
                            }
                            else 
                            {
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        private void peliPaikatDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var row = this.peliPaikatDataGridView.Rows[e.RowIndex];
                    Sali sali = (Sali)row.DataBoundItem;
                    if (sali != null)
                    {
                        sali.VarmistaAinakinYksiPoyta();
                    }
                    PaivitaPelipaikatUI();
                }
            }
            catch
            {
            }
        }

        private void varsinaisenPelipaikanTiedotButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var popup = new SalinTiedotPopup(this.asetukset.Sali, this.kilpailu.KaavioArvottu))
                {
                    popup.ShowDialog();
                }
            }
            catch
            { 
            }
        }

        private void kaavioidenYhdistaminenComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PaivitaPelipaikatUI();
        }

        #endregion

        // ========={( Integraatio muiden palveluiden kanssa )}============================================================ //
        #region Integraatio

        private void haeBiljardOrgSivultaButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var popup = new Integraatio.HaeOsallistujatBiljardiOrgistaPopup(this.kilpailu, this.loki))
                {
                    if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    { 
                    }
                }
            }
            catch (Exception ex)
            {
                this.loki.Kirjoita("Ilmoittautuneiden hakeminen biljardi.org sivulta epäonnistui", ex, false);
            }
        }

        #endregion

        private void splitContainer12_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
    }
}
