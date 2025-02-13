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
    public partial class ArvontaPopup : Form
    {
        private Kilpailu kilpailu = null;
        private Loki loki = null;

        private BindingList<Pelaaja> pelaajat = new BindingList<Pelaaja>();
        private BindingList<Sali> salit = new BindingList<Sali>();

        private List<string> virheet = new List<string>();
        private List<string> varoitukset = new List<string>();

        private bool alustettu = false;

        public ArvontaPopup(Sali sali, Kilpailu kilpailu, Loki loki)
        {
            this.kilpailu = kilpailu;
            this.loki = loki;

            InitializeComponent();

            try
            {
                sali.Kaytossa = true;

                KokoaSaliLista(sali);
                TarkistaPelipaikat(this.kilpailu.Osallistujat);

                KokoaPelaajalista();
                TarkistaSijoitukset();
                LaskePelaajaMaarat();
            }
            catch (Exception e)
            {
                this.loki.Kirjoita("Odottamaton virhe", e, false);
                KirjaaVirhe(string.Format("Odottamaton virhe: {0}", e.Message), true);
            }

            this.pelaajaBindingSource.DataSource = pelaajat;
            this.saliBindingSource.DataSource = salit;
        }

        private void TarkistaPelipaikat(IEnumerable<Pelaaja> pelaajat)
        {
            foreach (var p in pelaajat)
            {
                if (this.salit.Count(x => x.Toimitsijat.Any(y => Tyypit.Nimi.Equals(p.Nimi, y.Nimi))) > 1)
                {
                    KirjaaVirhe(string.Format("Pelaaja {0} on merkitty toimitsijaksi useammalle pelipaikalle tai useampaan rooliin samalla pelipaikalla", p.Nimi), true);
                    continue;
                }

                foreach (var s in this.salit)
                {
                    if (s.Toimitsijat.Any(x => Tyypit.Nimi.Equals(x.Nimi, p.Nimi)))
                    {
                        if (!string.IsNullOrEmpty(p.PeliPaikka) &&
                            !string.Equals(p.PeliPaikka, s.LyhytNimi))
                        {
                            KirjaaVirhe(string.Format("Pelaaja {0} on merkitty toimitsijaksi salilla {1}, mutta sijoitettu pelaamaan toisella salilla {2}",
                                p.Nimi, s.LyhytNimi, p.PeliPaikka), true);
                        }
                        else if (string.IsNullOrEmpty(p.PeliPaikka))
                        {
                            this.loki.Kirjoita(string.Format("Sijoitettiin toimitsija {0} automaattisesti salille {1} kaaviota arvottaessa", p.Nimi, s.LyhytNimi));
                            p.PeliPaikka = s.LyhytNimi;
                        }
                    }
                }
            }
        }

        private void KokoaSaliLista(Sali sali)
        {
            if (sali.Poytia < 1)
            {
                KirjaaVirhe("Järjestävällä salilla ei ole määritetty pöytiä käytettäväksi kisoissa. Paina peruuta, ja määritä pöydät 'Salin tiedot' välilehdellä", true);
            }

            this.salit.Add(sali);

            foreach (var s in this.kilpailu.PeliPaikat.OrderBy(x => x.Poytia))
            {
                this.salit.Add(s);
            }
        }

        private void KokoaPelaajalista()
        {
            List<Pelaaja> osallistujat = new List<Pelaaja>();

            foreach (var p in this.kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
            {
                if (!string.IsNullOrEmpty(p.Sijoitettu) ||
                    !string.IsNullOrEmpty(p.PeliPaikka))
                {
                    osallistujat.Add(p);
                }
            }

            foreach (var o in osallistujat
                .OrderBy(x => x.PeliPaikka)
                .OrderBy(x => x.SijoitusNumero))
            {
                this.pelaajat.Add(o);
            }
        }

        private void TarkistaSijoitukset()
        {
            int sijoitettuja = 0;
            switch (this.kilpailu.Sijoittaminen)
            {
                case Sijoittaminen.Sijoitetaan8Pelaajaa: sijoitettuja = 8; break;
                case Sijoittaminen.Sijoitetaan24Pelaajaa: sijoitettuja = 24; break;
            }

            if (sijoitettuja > 0 && this.pelaajat.Count > sijoitettuja)
            {
                for (int sijoitus = 1; sijoitus <= sijoitettuja; ++sijoitus)
                {
                    var lasku = this.pelaajat.Count(x => x.SijoitusNumero == sijoitus);
                    if (lasku == 0)
                    {
                        KirjaaVirhe(string.Format("{0}. sijoitettua pelaajaa ei ole merkitty osallistujalistaan", sijoitus), true);
                    }
                    else if (lasku > 1)
                    {
                        KirjaaVirhe(string.Format("{0}. sijoitettuja pelaajia on merkitty {1} kappaletta osallistujalistaan", sijoitus, lasku), true);
                    }
                }
            }
        }

        private void LaskePelaajaMaarat()
        {
            int poytiaYhteensa = this.salit
                .Where(x => x.Kaytossa)
                .Select(x => (int)x.PoytiaKaytettavissa).Sum();
            int osallistujia = this.kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)).Count();

            foreach (var s in this.salit)
            {
                s.Pelaajia = 0;
            }

            if (poytiaYhteensa > 0)
            {
                int pelaajiaSaleilla = 0;

                foreach (var s in this.salit)
                {
                    if (s.Kaytossa && s.PoytiaKaytettavissa > 0)
                    {
                        float d = ((float)s.PoytiaKaytettavissa) / (float)poytiaYhteensa;
                        int i = (int)Math.Round(d * (float)osallistujia);
                        pelaajiaSaleilla += i;
                        s.Pelaajia = i;
                    }
                }

                if (pelaajiaSaleilla != osallistujia)
                {
                    this.salit.First().Pelaajia += osallistujia - pelaajiaSaleilla;
                }
            }

            foreach (var s in this.salit)
            {
                if (s.Kaytossa && s.Pelaajia < 4)
                {
                    KirjaaVirhe(string.Format("Salilla {0} on alle neljä pelaajaa. Kisat lienee järkevää pelata vähemmällä salimäärällä", s.LyhytNimi), false);
                }
            }
        }

        private void KirjaaVirhe(string virhe, bool vakava)
        {
            if (vakava)
            {
                this.virheet.Add(virhe);
            }
            else 
            {
                this.varoitukset.Add(virhe);
            }

            PaivitaVirheUI();
        }

        private void PaivitaVirheUI()
        {
            if (this.virheet.Count > 0)
            {
                this.virheRichTextBox.BackColor = Color.Salmon;
                this.virheRichTextBox.ForeColor = Color.White;

                this.virheRichTextBox.Visible = true;
                this.virheRichTextBox.Text = string.Join(Environment.NewLine, this.virheet.ToArray());

                this.okButton.Enabled = false;
            }
            else if (this.varoitukset.Count > 0)
            {
                this.virheRichTextBox.BackColor = Color.LightYellow;
                this.virheRichTextBox.ForeColor = Color.Black;

                this.virheRichTextBox.Visible = true;
                this.virheRichTextBox.Text = string.Join(Environment.NewLine, this.varoitukset.ToArray());

                this.okButton.Enabled = true;
            }
            else
            {
                this.virheRichTextBox.Visible = false;
                this.virheRichTextBox.Text = string.Empty;

                this.okButton.Enabled = true;
            }
        }

        private void paikatDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            /*
            if (e.RowIndex >= 0 && e.ColumnIndex == kaytossaDataGridViewCheckBoxColumn.Index)
            {
                this.varoitukset.Clear();

                LaskePelaajaMaarat();

                PaivitaVirheUI();

                this.paikatDataGridView.Refresh();
            }
             */
        }

        private void paikatDataGridView_Click(object sender, EventArgs e)
        {
        }

        private void paikatDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == kaytossaDataGridViewCheckBoxColumn.Index)
            {
            }
        }

        private void paikatDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == kaytossaDataGridViewCheckBoxColumn.Index &&
                e.RowIndex > 0)
            {
                try
                {
                    Sali sali = (Sali)this.paikatDataGridView.Rows[e.RowIndex].DataBoundItem;
                    if (sali != null)
                    {
                        sali.Kaytossa = !sali.Kaytossa;

                        this.varoitukset.Clear();

                        LaskePelaajaMaarat();

                        PaivitaVirheUI();

                        this.paikatDataGridView.Refresh();

                        this.pelaajatDataGridView.Focus();
                    }
                }
                catch
                { 
                }
            }
        }

        private void paikatDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
        }

        private void paikatDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
        }

        private void ArvontaPopup_Shown(object sender, EventArgs e)
        {
            this.alustettu = true;
            this.paikatDataGridView.Rows[0].Cells[0].ReadOnly = true;
        }
    }
}
