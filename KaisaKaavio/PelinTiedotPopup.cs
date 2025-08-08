using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaisaKaavio
{
    /// <summary>
    /// Popup ikkuna pelin tuloksen ja muiden tietojen editoimiseen.
    /// </summary>
    public partial class PelinTiedotPopup : Form
    {
        private Kilpailu kilpailu = null;
        private Peli peli = null;
        private PelinTulos tulos = PelinTulos.EiTiedossa;
        private PelinTilanne tilanne = PelinTilanne.Tyhja;

        private PelinTulos muokattuTulos = PelinTulos.EiTiedossa;
        private PelinTilanne muokattuTilanne = PelinTilanne.Tyhja;

        private Loki loki = null;

        private bool tallenna = false;

        public PelinTiedotPopup(Kilpailu kilpailu, Peli peli, Loki loki)
        {
            this.loki = loki;

            this.kilpailu = kilpailu;
            this.peli = peli;
            this.tulos = peli.Tulos;
            this.tilanne = peli.Tilanne;

            this.muokattuTulos = this.tulos;
            this.muokattuTilanne = this.tilanne;

            InitializeComponent();

            Shown += PelinTiedotPopup_Shown;
            FormClosing += PelinTiedotPopup_FormClosing;
        }

        void PelinTiedotPopup_Shown(object sender, EventArgs e)
        {
            this.pelidetaljiNimi1textBox.Text = this.peli.Pelaaja1;
            this.pelidetaljiNimi2textBox.Text = this.peli.Pelaaja2;
            this.pisteet1TextBox.Text = this.peli.Pisteet1;
            this.pisteet2TextBox.Text = this.peli.Pisteet2;
            this.alkamisAikaTextBox.Text = this.peli.Alkoi;
            this.paattymisAikaTextBox.Text = this.peli.Paattyi;
            this.pelinKuvausLabel.Text = string.Format("{0}. kierroksen peli:", peli.Kierros);
            this.pelinNumeroLabel.Text = string.Format("#{0}", peli.PeliNumero);
            this.poytaTextBox.Text = this.peli.Poyta;
           
            this.lyontivuorojaTextBox.Text = this.peli.Lyontivuoroja;
            this.lyontivuorojaTextBox.Visible = this.kilpailu.Laji == Laji.Kara;
            this.lyontivuorojaLabel.Visible = this.kilpailu.Laji == Laji.Kara;

            this.pelidetaljiPelaaja1Sarja1textBox.Text = peli.PisinSarja1;
            this.pelidetaljiPelaaja1Sarja2textBox.Text = peli.ToiseksiPisinSarja1;

            this.pelidetaljiPelaaja2Sarja1textBox.Text = peli.PisinSarja2;
            this.pelidetaljiPelaaja2Sarja2textBox.Text = peli.ToiseksiPisinSarja2;

            this.pelidetaljiPelaaja1Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja1Sarja1textBox.Text);
            this.pelidetaljiPelaaja2Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja2Sarja1textBox.Text);

            this.luovutusTextBox1.Text = peli.LuovutusPelaaja1;
            this.luovutusTextBox2.Text = peli.LuovutusPelaaja2;

            this.pelidetaljiPelaaja1Sarja1textBox.Focus();
        }

        void PelinTiedotPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.tallenna)
            {
                this.peli.PisinSarja1 = this.pelidetaljiPelaaja1Sarja1textBox.Text;
                this.peli.PisinSarja2 = this.pelidetaljiPelaaja2Sarja1textBox.Text;
                this.peli.ToiseksiPisinSarja1 = this.pelidetaljiPelaaja1Sarja2textBox.Text;
                this.peli.ToiseksiPisinSarja2 = this.pelidetaljiPelaaja2Sarja2textBox.Text;

                this.peli.Poyta = this.poytaTextBox.Text;
                this.peli.Alkoi = this.alkamisAikaTextBox.Text;
                this.peli.Paattyi = this.paattymisAikaTextBox.Text;
                this.peli.Lyontivuoroja = this.lyontivuorojaTextBox.Text;

                this.peli.LuovutusPelaaja1 = this.luovutusTextBox1.Text;
                this.peli.LuovutusPelaaja2 = this.luovutusTextBox2.Text;

                if (this.loki != null)
                {
                    if (!string.Equals(this.peli.Pisteet1, this.pisteet1TextBox.Text))
                    {
                        this.loki.Kirjoita(string.Format("Pelin {0} pelaajan 1 pisteitä muokattu manuaalisesti pelin jälkeen {1} => {2}", 
                            this.peli.Kuvaus(),
                            this.peli.Pisteet1, 
                            this.pisteet1TextBox.Text));
                    }

                    if (!string.Equals(this.peli.Pisteet2, this.pisteet2TextBox.Text))
                    {
                        this.loki.Kirjoita(string.Format("Pelin {0} pelaajan 2 pisteitä muokattu manuaalisesti pelin jälkeen {1} => {2}",
                            this.peli.Kuvaus(),
                            this.peli.Pisteet2,
                            this.pisteet2TextBox.Text));
                    }
                }

                this.peli.Pisteet1 = this.pisteet1TextBox.Text;
                this.peli.Pisteet2 = this.pisteet2TextBox.Text;

                if (this.tilanne == PelinTilanne.Kaynnissa && this.muokattuTilanne == PelinTilanne.Tyhja)
                {
                    this.kilpailu.PeruutaKaynnissaOlevaPeli(this.peli);
                }
                else if (this.tulos != this.muokattuTulos || this.tilanne != this.muokattuTilanne)
                {
                    this.kilpailu.PaivitaPelatunPelinTulos(peli, this.muokattuTulos, this.muokattuTilanne);
                }
            }
        }

        private void tallennaButton_Click(object sender, EventArgs e)
        {
            this.tallenna = true;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void pelidetaljiPelaaja1Sarja1textBox_TextChanged(object sender, EventArgs e)
        {
            this.pelidetaljiPelaaja1Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja1Sarja1textBox.Text);
        }

        private void pelidetaljiPelaaja2Sarja1textBox_TextChanged(object sender, EventArgs e)
        {
            this.pelidetaljiPelaaja2Sarja2textBox.Enabled = !string.IsNullOrEmpty(this.pelidetaljiPelaaja2Sarja1textBox.Text);
        }

        private void mitatoiOtteluButton_Click(object sender, EventArgs e)
        {
            this.pisteet1TextBox.Text = string.Empty;
            this.pisteet2TextBox.Text = string.Empty;
            this.poytaTextBox.Text = string.Empty;
            this.alkamisAikaTextBox.Text = string.Empty;
            this.paattymisAikaTextBox.Text = string.Empty;
            this.pelidetaljiPelaaja1Sarja1textBox.Text = string.Empty;
            this.pelidetaljiPelaaja1Sarja2textBox.Text = string.Empty;
            this.pelidetaljiPelaaja2Sarja1textBox.Text = string.Empty;
            this.pelidetaljiPelaaja2Sarja2textBox.Text = string.Empty;

            PaivitaTilanne();
        }

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.tallenna = false;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void pisteet1TextBox_TextChanged(object sender, EventArgs e)
        {
            PaivitaTilanne();

        }

        private void pisteet2TextBox_TextChanged(object sender, EventArgs e)
        {
            PaivitaTilanne();
        }

        private void luovutusTextBox1_TextChanged(object sender, EventArgs e)
        {
            PaivitaTilanne();
        }

        private void luovutusTextBox2_TextChanged(object sender, EventArgs e)
        {
            PaivitaTilanne();
        }

        private void AsetaInfonVarit(Color tausta, Color teksti)
        {
            this.splitContainer4.Panel2.BackColor = tausta;
            this.infoRichTextBox.BackColor = tausta;
            this.infoRichTextBox.ForeColor = teksti;
        }

        private void PaivitaTilanne()
        { 
            string virhe = string.Empty;

            this.muokattuTulos = Peli.LaskePelinTulosJaTilannePisteista(
                this.pisteet1TextBox.Text, 
                this.pisteet2TextBox.Text, 
                (int)this.kilpailu.TavoitePistemaara, 
                out muokattuTilanne, 
                out virhe);

            if (this.tilanne == PelinTilanne.Pelattu && 
                this.muokattuTilanne == PelinTilanne.Tyhja && 
                this.muokattuTulos == PelinTulos.EiTiedossa && 
                this.peli.Kierros < 3)
            {
                this.tallennaButton.Enabled = false;
                AsetaInfonVarit(Color.LightPink, Color.Red);
                this.infoRichTextBox.Text =
                    "VIRHE!\nEnsimmäisen ja toisen kierroksen pelejä ei voi poistaa kaaviosta. Voit ainoastaan muuttaa pelin pisteitä ja/tai tuloksen";
            }
            else if (this.muokattuTulos == PelinTulos.Virheellinen)
            {
                this.tallennaButton.Enabled = false;
                AsetaInfonVarit(Color.LightPink, Color.Red);
                this.infoRichTextBox.Text =
                    string.Format("VIRHE!\nPelin pisteissä on virhe:\n{0}.\nKorjaa virhe, ennen kuin voit tallentaa pelin tilanteen", virhe);
            }
            else if (this.tilanne != PelinTilanne.Kaynnissa && this.muokattuTulos == PelinTulos.EiTiedossa && this.muokattuTilanne != PelinTilanne.Tyhja)
            {
                this.tallennaButton.Enabled = false;
                AsetaInfonVarit(Color.LightPink, Color.Red);
                this.infoRichTextBox.Text = 
                    "VIRHE!\nPelin pisteistä ei pysty päättelemään pelin voittajaa.\nKorjaa pisteet siten että jompi kumpi voittaa pelin";
            }
            else if (this.muokattuTulos != this.tulos)
            {
                if (this.kilpailu.KilpaSarja == KilpaSarja.Joukkuekilpailu)
                {
                    this.tallennaButton.Enabled = false;
                    AsetaInfonVarit(Color.LightPink, Color.Red);
                    this.infoRichTextBox.Text = "VIRHE!\nJoukkuekilpailussa ei tällä ohjelman versiolla pysty muuttamaan jo pelattujen pelien lopputulosta";
                }
                else
                {
                    this.tallennaButton.Enabled = true;
                    AsetaInfonVarit(Color.LightYellow, Color.Black);
                    this.infoRichTextBox.Text =
                        "VAROITUS!\nMuokkasit pisteitä siten, että ottelun tulos muuttuu. Mikäli tallennat muutoksen pelin tilanteeseen, " +
                        "ohjelma joutuu mahdollisesti mitätöimään yhden tai useampia tämän ottelun jälkeen alkaneet/pelatuista peleistä, joissa on mukana jompi kumpi tai molemmat tämän ottelun pelaajista!";
                }
            }
            else if (this.muokattuTilanne != this.tilanne)
            {
                if ((this.tilanne == PelinTilanne.Pelattu || this.tilanne == PelinTilanne.Kaynnissa) &&
                    this.muokattuTilanne == PelinTilanne.Tyhja)
                {
                    if (this.kilpailu.KilpaSarja == KilpaSarja.Joukkuekilpailu)
                    {
                        this.tallennaButton.Enabled = false;
                        AsetaInfonVarit(Color.LightPink, Color.Red);
                        this.infoRichTextBox.Text = "VIRHE!\nJoukkuekilpailussa ei tällä ohjelman versiolla pysty mitätöimään jo käynnistettyä peliä";
                    }
                    else
                    {
                        this.tallennaButton.Enabled = true;
                        AsetaInfonVarit(Color.LightYellow, Color.Black);
                        this.infoRichTextBox.Text =
                            "VAROITUS!\nMuokkasit pisteitä siten, että pelattu ottelu mitätöidään.\nMikäli tallennat muutoksen, " +
                            "ohjelma joutuu mahdollisesti mitätöimään yhden tai useampia tämän ottelun jälkeisistä peleistä, joissa on mukana jompi kumpi tai molemmat tämän ottelun pelaajista!";
                    }
                }
                else
                {
                    this.tallennaButton.Enabled = false;
                    AsetaInfonVarit(Color.LightPink, Color.Red);
                    this.infoRichTextBox.Text = string.Format(
                        "VIRHE!\nMuutoksesi pelin pisteisiin muuttivat pelin tilanteen tilanteesta {0} tilanteeseen {1}. Voit ainoastaan muuttaa pelin voittajaa tai mitätöidä pelin.",
                        this.tilanne,
                        this.muokattuTilanne);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.luovutusTextBox1.Text) && muokattuTulos == PelinTulos.Pelaaja1Voitti)
                {
                    this.tallennaButton.Enabled = false;
                    AsetaInfonVarit(Color.LightPink, Color.Red);
                    this.infoRichTextBox.Text = string.Format(
                        "VIRHE!\nPelin voittanut pelaaja on luovuttanut ottelun. Muuta pelin tulos tai luovutusstatus.");
                }
                else if (!string.IsNullOrEmpty(this.luovutusTextBox2.Text) && muokattuTulos == PelinTulos.Pelaaja2Voitti)
                {
                    this.tallennaButton.Enabled = false;
                    AsetaInfonVarit(Color.LightPink, Color.Red);
                    this.infoRichTextBox.Text = string.Format(
                        "VIRHE!\nPelin voittanut pelaaja on luovuttanut ottelun. Muuta pelin tulos tai luovutusstatus.");
                }
                else
                {
                    this.tallennaButton.Enabled = true;
                    AsetaInfonVarit(Color.White, Color.Black);
                    this.infoRichTextBox.Text = string.Empty;
                }
            }
        }
    }
}
