using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KaisaKaavio.HakuAlgoritmi;

namespace KaisaKaavio.ManuaalinenHaku
{
    public partial class ManuaalinenHaku : Form
    {
        private Kilpailu kilpailu = null;
        private BindingList<Peli> haetutPelit = new BindingList<Peli>();
        private BindingList<Pelaaja> hakijat = new BindingList<Pelaaja>();
        private BindingList<Pelaaja> vastustajat = new BindingList<Pelaaja>();
        private int rekursiossa = 0;

        public bool HaettiinJotain { get; private set; } = false;

        public ManuaalinenHaku(Kilpailu kilpailu)
        {
            InitializeComponent();
            this.kilpailu = kilpailu;

            PaivitaHaetutPelit();

            this.hakijaComboBox.DataSource = this.hakijat;
            this.vastustajaComboBox.DataSource = this.vastustajat;

            PaivitaHakijat();

            this.peliBindingSource.DataSource = haetutPelit;
        }

        private void PaivitaHakijat()
        {
            this.rekursiossa++;

            this.hakijat.Clear();
            this.vastustajat.Clear();

            foreach (var pelaaja in this.kilpailu.Osallistujat
                .Where(x => x.Id > 0)
                .OrderBy(x => x.Id)
                .OrderBy(x => kilpailu.LaskePelit(x.Id)))
            {
                if (this.kilpailu.VoiHakea(pelaaja))
                {
                    this.hakijat.Add(pelaaja);
                    this.vastustajat.Add(pelaaja);
                }
            }

            if (this.vastustajat.Count > 1)
            {
                this.hakijaComboBox.SelectedIndex = 0;
                this.vastustajaComboBox.SelectedIndex = 1;

                this.hakijaComboBox.Enabled = true;
                this.vastustajaComboBox.Enabled = true;
                this.haeButton.Enabled = true;
                this.tilanneLabel.Text = string.Empty;

                int parasHakuIndeksi = 1;
                int parhaanHaunTulos = TarkistaHaku();

                while ((parhaanHaunTulos < 2) && (vastustajaComboBox.SelectedIndex < (vastustajaComboBox.Items.Count - 1)))
                {
                    vastustajaComboBox.SelectedIndex = vastustajaComboBox.SelectedIndex + 1;
                    int tulos = TarkistaHaku();
                    if (tulos > parhaanHaunTulos)
                    {
                        parhaanHaunTulos = tulos;
                        parasHakuIndeksi = vastustajaComboBox.SelectedIndex;
                    }
                }

                if (vastustajaComboBox.SelectedIndex != parasHakuIndeksi)
                {
                    vastustajaComboBox.SelectedIndex = parasHakuIndeksi;
                    TarkistaHaku();
                }
            }
            else
            {
                this.hakijaComboBox.Enabled = false;
                this.vastustajaComboBox.Enabled = false;
                this.haeButton.Enabled = false;
                this.tilanneLabel.Text = "Ei mahdollisia hakuja";
                this.tilanneLabel.ForeColor = Color.DarkGray;
            }

            this.rekursiossa--;
        }

        private int TarkistaHaku()
        {
            var hakija = (Pelaaja)this.hakijaComboBox.SelectedItem;
            var vastustaja = (Pelaaja)this.vastustajaComboBox.SelectedItem;

            if (hakija == vastustaja)
            {
                this.tilanneLabel.Text = "Pelaaja ei voi hakea itseään";
                this.tilanneLabel.ForeColor = Color.Red;
                this.haeButton.Enabled = false;
                return 0;
            }

            var keskenaiset = this.kilpailu.Pelit.Where(x => x.SisaltaaPelaajat(hakija.Id, vastustaja.Id));
            if (keskenaiset.Any())
            {
                var peli = keskenaiset.First();

                if (peli.Tilanne != PelinTilanne.Pelattu)
                {
                    this.tilanneLabel.Text = "Pelaajille on jo haettu peli";
                }
                else
                {
                    this.tilanneLabel.Text = "Ovat pelanneet jo vastakkain";
                }

                this.tilanneLabel.ForeColor = Color.Orange;
                this.haeButton.Enabled = true;
                return 1;
            }

            int peleja1 = this.kilpailu.LaskePelit(hakija.Id);
            int peleja2 = this.kilpailu.LaskePelit(vastustaja.Id);
            if (Math.Abs(peleja1 - peleja2) > 1)
            {
                if (peleja1 > peleja2)
                {
                    this.tilanneLabel.Text = string.Format("{0} on 2 kierrosta edellä", hakija.LyhytNimi);
                }
                else
                {
                    this.tilanneLabel.Text = string.Format("{0} on 2 kierrosta edellä", vastustaja.LyhytNimi);
                }

                this.tilanneLabel.ForeColor = Color.Red;
                this.haeButton.Enabled = false;
                return 0;
            }

            this.tilanneLabel.Text = string.Empty;
            this.tilanneLabel.ForeColor = Color.Black;
            this.haeButton.Enabled = true;
            return 2;
        }

        private void PaivitaHaetutPelit()
        {
            this.rekursiossa++;

            haetutPelit.Clear();
            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Tilanne != PelinTilanne.Pelattu && x.Kierros > 2))
            {
                haetutPelit.Add(peli);
            }

            this.rekursiossa--;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void hakijaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rekursiossa == 0)
            {
                TarkistaHaku();
            }
        }

        private void vastustajaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rekursiossa == 0)
            {
                TarkistaHaku();
            }
        }

        private void haetutPelitDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == poistaPeliColumn.Index)
                {
                    var peli = (Peli)haetutPelitDataGridView.Rows[e.RowIndex].DataBoundItem;
                    if (peli != null)
                    {
                        kilpailu.PoistaPeli(peli, true);
                        haetutPelit.Remove(peli);
                        HaettiinJotain = true;

                        PaivitaHakijat();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void haeButton_Click(object sender, EventArgs e)
        {
            try
            {
                var hakija = (Pelaaja)this.hakijaComboBox.SelectedItem;
                var vastustaja = (Pelaaja)this.vastustajaComboBox.SelectedItem;

                var peli = this.kilpailu.LisaaPeli(
                    kilpailu.Osallistujat.FirstOrDefault(x => x.Id == hakija.Id),
                    kilpailu.Osallistujat.FirstOrDefault(x => x.Id == vastustaja.Id),
                    0);

                if (peli != null)
                {
                    peli.HakuKommentti = "Haettu käsin";
                    haetutPelit.Add(peli);
                    HaettiinJotain = true;

                    PaivitaHakijat();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void haetutPelitDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == viivaColumn.Index)
            {
                e.Value = "-";
            }
        }
    }
}
