using KaisaKaavio.Tyypit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Integraatio
{
    public partial class LataaOnlineKilpailuPopup : Form
    {
        private List<Tyypit.KilpailuTietue> kaikkiKilpailut = new List<Tyypit.KilpailuTietue>();
        private BindingList<Tyypit.KilpailuTietue> kilpailut = new BindingList<Tyypit.KilpailuTietue>();
        private Loki loki = null;

        private KilpailuTietue valittuKilpailu = null;

        public string LadattuTiedosto { get; private set; } = string.Empty;
        public string LadatunKilpailunId 
        {
            get 
            {
                return this.valittuKilpailu != null ? this.valittuKilpailu.Id : string.Empty;
            }
        }

        public LataaOnlineKilpailuPopup(Loki loki)
        {
            this.loki = loki;

            InitializeComponent();

            this.kilpailutBindingSource.DataSource = kilpailut;
        }

        private void LataaOnlineKilpailuPopup_Shown(object sender, EventArgs e)
        {
            Integraatio.KaisaKaavioFi.HaeDataa("OnlineKilpailut", string.Empty, this, this.loki, (tietue) =>
            {
                if (tietue != null && tietue.Rivit.Any())
                {
                    this.kaikkiKilpailut.Clear();

                    foreach (var rivi in tietue.Rivit)
                    {
                        this.kaikkiKilpailut.Add(new KilpailuTietue()
                        {
                            Id = rivi.Get("Id", string.Empty),
                            Nimi = rivi.Get("Nimi", string.Empty),
                            Pvm = rivi.Get("Pvm", string.Empty),
                            SalasanaHash = rivi.GetInt("SalasanaHash", 0)
                        });
                    }
                }

                PaivitaNakyma();

                this.salasanaTextBox.Enabled = true;
                this.salasanaTextBox.Focus();
            });
        }

        private void PaivitaNakyma()
        {
            int tn = Tyypit.Luku.LaskeTaikanumero(salasanaTextBox.Text);

            this.kilpailutDataGridView.ClearSelection();
            this.kilpailutBindingSource.SuspendBinding();

            this.kilpailut.Clear();

            foreach (var k in this.kaikkiKilpailut.Where(x => x.SalasanaHash == tn))
            {
                this.kilpailut.Add(k);
            }

            this.kilpailutBindingSource.ResumeBinding();
            this.kilpailutBindingSource.ResetBindings(false);
        }

        private void naytaSalasanaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.salasanaTextBox.UseSystemPasswordChar = !this.naytaSalasanaCheckBox.Checked;
        }

        private void salasanaTextBox_TextChanged(object sender, EventArgs e)
        {
            PaivitaNakyma();
        }

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var tiedostonNimi = Path.GetTempFileName();

            if (this.valittuKilpailu != null)
            { 
                string virhe = string.Empty;

                if (Integraatio.KaisaKaavioFi.LataaKilpailuServerilta(
                    this.valittuKilpailu.Id,
                    tiedostonNimi,
                    this.valittuKilpailu.SalasanaHash,
                    out virhe))
                {
                    this.LadattuTiedosto = tiedostonNimi;

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Tiedoston lataaminen ei onnistunut\n{0}", virhe), 
                        "Virhe", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Warning);
                }
            }
        }

        private void kilpailutDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.valittuKilpailu = null;

            try
            {
                if (this.kilpailutDataGridView.SelectedRows.Count > 0)
                {
                    var row = this.kilpailutDataGridView.SelectedRows[0];
                    this.valittuKilpailu = (KilpailuTietue)row.DataBoundItem;
                }
            }
            catch
            {
            }

            this.okButton.Enabled = this.valittuKilpailu != null;
            this.okButton.Visible = this.valittuKilpailu != null;
        }
    }
}
