using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Ranking
{
    /// <summary>
    /// Popup ikkuna ranking pisteytyskriteerien editoimiseen
    /// </summary>
    public partial class RankingPisteytysPopup : Form
    {
        private RankingAsetukset asetukset = null;
        private Font paksuPieniFontti = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Bold);
        private Font ohutPieniFontti = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Regular);

        public RankingPisteytysPopup(RankingAsetukset asetukset)
        {
            this.asetukset = asetukset;

            InitializeComponent();

            Shown += RankingPisteytysPopup_Shown;
        }

        void RankingPisteytysPopup_Shown(object sender, EventArgs e)
        {
            PaivitaRankingPisteytysPelistaLiuku(RankingPisteetPelista.JokaisestaVoitosta, this.rpJokaPeliNumericUpDown);
            PaivitaRankingPisteytysPelistaLiuku(RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari, this.rpEkaKierrosNumericUpDown);
            PaivitaRankingPisteytysPelistaLiuku(RankingPisteetPelista.RankingYkkosenVoitosta, this.rpRgYkkonenNumericUpDown);
            PaivitaRankingPisteytysPelistaLiuku(RankingPisteetPelista.RankingKakkosenVoitosta, this.rpRgKakkonenNumericUpDown);
            PaivitaRankingPisteytysPelistaLiuku(RankingPisteetPelista.RankingKolmosenVoitosta, this.rpRgKolmonenNumericUpDown);

            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Voittajalle, this.rp1NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Kakkoselle, this.rp2NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Kolmoselle, this.rp3NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Neloselle, this.rp4NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Vitoselle, this.rp5NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Kutoselle, this.rp6NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Seiskalle, this.rp7NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Kasille, this.rp8NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Ysille, this.rp9NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.Kymmenennelle, this.rp10NumericUpDown);
            PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta.KaikilleOsallistujille, this.rpOsallistuminenNumericUpDown);

            this.rankingKarkiEdellisestaCheckBox.Checked = this.asetukset.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta;
            this.rankingSijaisetCheckBox.Checked = this.asetukset.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista;
        }

        void okButton_Click(object sender, EventArgs e)
        {
            PaivitaRankingPisteytysPelista(RankingPisteetPelista.JokaisestaVoitosta, this.rpJokaPeliNumericUpDown);
            PaivitaRankingPisteytysPelista(RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari, this.rpEkaKierrosNumericUpDown);
            PaivitaRankingPisteytysPelista(RankingPisteetPelista.RankingYkkosenVoitosta, this.rpRgYkkonenNumericUpDown);
            PaivitaRankingPisteytysPelista(RankingPisteetPelista.RankingKakkosenVoitosta, this.rpRgKakkonenNumericUpDown);
            PaivitaRankingPisteytysPelista(RankingPisteetPelista.RankingKolmosenVoitosta, this.rpRgKolmonenNumericUpDown);

            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Voittajalle, this.rp1NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kakkoselle, this.rp2NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kolmoselle, this.rp3NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Neloselle, this.rp4NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Vitoselle, this.rp5NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kutoselle, this.rp6NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Seiskalle, this.rp7NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kasille, this.rp8NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Ysille, this.rp9NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kymmenennelle, this.rp10NumericUpDown);
            PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.KaikilleOsallistujille, this.rpOsallistuminenNumericUpDown);

            this.asetukset.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista = this.rankingSijaisetCheckBox.Checked;
            this.asetukset.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta = this.rankingKarkiEdellisestaCheckBox.Checked;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void PaivitaRankingPisteytysPelistaLiuku(RankingPisteetPelista peliEhto, NumericUpDown liuku)
        {
            var asetus = this.asetukset.PistetytysPeleista.FirstOrDefault(x => x.Ehto == peliEhto);
            if (asetus != null)
            {
                liuku.Value = (decimal)asetus.Pisteet;
            }
            else
            {
                liuku.Value = 0;
            }
        }

        private void PaivitaRankingPisteytysSijoituksestaLiuku(RankingPisteetSijoituksesta ehto, NumericUpDown liuku)
        {
            var asetus = this.asetukset.PisteytysSijoituksista.FirstOrDefault(x => x.Ehto == ehto);
            if (asetus != null)
            {
                liuku.Value = (decimal)asetus.Pisteet;
            }
            else
            {
                liuku.Value = 0;
            }
        }

        private void PaivitaRankingPisteytysSijoituksesta(RankingPisteetSijoituksesta ehto, NumericUpDown liuku)
        {
            var asetus = this.asetukset.PisteytysSijoituksista.FirstOrDefault(x => x.Ehto == ehto);

            int pisteet = (int)liuku.Value;
            if (pisteet > 0)
            {
                if (asetus == null)
                {
                    this.asetukset.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta()
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
                    this.asetukset.PisteytysSijoituksista.Remove(asetus);
                }
            }
        }

        private void PaivitaRankingPisteytysPelista(RankingPisteetPelista peliEhto, NumericUpDown liuku)
        {
            var asetus = this.asetukset.PistetytysPeleista.FirstOrDefault(x => x.Ehto == peliEhto);

            int pisteet = (int)liuku.Value;
            if (pisteet > 0)
            {
                if (asetus == null)
                {
                    this.asetukset.PistetytysPeleista.Add(new RankingPisteytysPelista()
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
                    this.asetukset.PistetytysPeleista.Remove(asetus);
                }
            }
        }

        private void RankingPisteytys_ValueChanged(object sender, EventArgs e)
        {
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

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
