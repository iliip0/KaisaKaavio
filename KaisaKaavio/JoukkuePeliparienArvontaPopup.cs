using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class JoukkuePeliparienArvontaPopup : Form
    {
        Kilpailu kilpailu = null;
        Loki loki = null;
        Peli peli = null;

        List<Pelaaja> pelaajat1 = new List<Pelaaja>();
        List<Pelaaja> pelaajat2 = new List<Pelaaja>();
        List<Peli> pelit = new List<Peli>();

        public JoukkuePeliparienArvontaPopup(Kilpailu kilpailu, Loki loki, Peli peli, string joukkue1, string joukkue2)
        {
            this.kilpailu = kilpailu;
            this.loki = loki;
            this.peli = peli;

            this.pelit.AddRange(this.kilpailu.Pelit.Where(x =>
                x.KierrosPelaaja1 == this.peli.KierrosPelaaja1 &&
                x.KierrosPelaaja2 == this.peli.KierrosPelaaja2 &&
                string.Equals(x.Joukkue1, this.peli.Joukkue1) &&
                string.Equals(x.Joukkue2, this.peli.Joukkue2)));

            InitializeComponent();

            this.joukkueLabel1.Text = joukkue1;
            this.joukkueLabel2.Text = joukkue2;
        }

        private void pelaajaComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tarkista();
        }

        private void Tarkista()
        {
            this.virheLabel.Text = string.Empty;

            if (this.pelit.Any(x => x.Tilanne == PelinTilanne.Pelattu))
            {
                this.okButton.Enabled = false;
                this.arvoButton.Enabled = false;
                this.pelaajaComboBox1.Enabled = false;
                this.pelaajaComboBox2.Enabled = false;
                this.pelaajaComboBox3.Enabled = false;
                this.pelaajaComboBox4.Enabled = false;
                this.pelaajaComboBox5.Enabled = false;
                this.pelaajaComboBox6.Enabled = false;

                this.virheLabel.Text = "Yksi tai useampi peli on jo pelattu. Pareja ei voi enää arpoa";

                return;
            }

            if (pelaajaComboBox1.SelectedIndex == pelaajaComboBox2.SelectedIndex ||
                pelaajaComboBox1.SelectedIndex == pelaajaComboBox3.SelectedIndex ||
                pelaajaComboBox2.SelectedIndex == pelaajaComboBox3.SelectedIndex ||
                pelaajaComboBox4.SelectedIndex == pelaajaComboBox5.SelectedIndex ||
                pelaajaComboBox4.SelectedIndex == pelaajaComboBox6.SelectedIndex ||
                pelaajaComboBox5.SelectedIndex == pelaajaComboBox6.SelectedIndex)
            {

                this.virheLabel.Text = "Sama pelaaja ei voi olla useassa ottelussa";

                this.okButton.Enabled = false;
            }
            else
            {
                this.okButton.Enabled = true;
            }
        }

        private void arvoButton_Click(object sender, EventArgs e)
        {
            Arvo();
        }

        private void Arvo()
        {
            List<int> joukkue1 = new List<int>();
            for (int i = 0; i < pelaajat1.Count; ++i)
            {
                joukkue1.Add(i);
            }

            List<int> joukkue2 = new List<int>();
            for (int j = 0; j < pelaajat2.Count; ++j)
            {
                joukkue2.Add(j);
            }

            Random r = new Random();

            var j1 = joukkue1.OrderBy(x => r.Next()).ToArray();
            var j2 = joukkue2.OrderBy(x => r.Next()).ToArray();

            pelaajaComboBox1.SelectedIndex = j1[0];
            pelaajaComboBox2.SelectedIndex = j1[1];
            pelaajaComboBox3.SelectedIndex = j1[2];
            pelaajaComboBox4.SelectedIndex = j2[0];
            pelaajaComboBox5.SelectedIndex = j2[1];
            pelaajaComboBox6.SelectedIndex = j2[2];
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                List<Peli> pelit = new List<Peli>();
                pelit.AddRange(this.kilpailu.Pelit.Where(x => 
                    x.KierrosPelaaja1 == this.peli.KierrosPelaaja1 &&
                    x.KierrosPelaaja2 == this.peli.KierrosPelaaja2 &&
                    string.Equals(x.Joukkue1, this.peli.Joukkue1) &&
                    string.Equals(x.Joukkue2, this.peli.Joukkue2)));

                if (pelit.Count == 3)
                {
                    pelit[0].PelaajaId1 = ((Pelaaja)pelaajaComboBox1.SelectedItem).Id.ToString();
                    pelit[0].PelaajaId2 = ((Pelaaja)pelaajaComboBox4.SelectedItem).Id.ToString();
                    pelit[0].JoukkueParitArvottu = true;

                    pelit[1].PelaajaId1 = ((Pelaaja)pelaajaComboBox2.SelectedItem).Id.ToString();
                    pelit[1].PelaajaId2 = ((Pelaaja)pelaajaComboBox5.SelectedItem).Id.ToString();
                    pelit[1].JoukkueParitArvottu = true;

                    pelit[2].PelaajaId1 = ((Pelaaja)pelaajaComboBox3.SelectedItem).Id.ToString();
                    pelit[2].PelaajaId2 = ((Pelaaja)pelaajaComboBox6.SelectedItem).Id.ToString();
                    pelit[2].JoukkueParitArvottu = true;
                }

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch 
            {
            }
        }

        private void JoukkuePeliparienArvontaPopup_Shown(object sender, EventArgs e)
        {

        }

        private void JoukkuePeliparienArvontaPopup_Load(object sender, EventArgs e)
        {
            pelaajat1.AddRange(this.kilpailu.Osallistujat.Where(x => string.Equals(x.Joukkue, this.joukkueLabel1.Text)));
            pelaajat2.AddRange(this.kilpailu.Osallistujat.Where(x => string.Equals(x.Joukkue, this.joukkueLabel2.Text)));

            pelaajaComboBox1.DataSource = pelaajat1.ToList();
            pelaajaComboBox2.DataSource = pelaajat1.ToList();
            pelaajaComboBox3.DataSource = pelaajat1.ToList();
            pelaajaComboBox4.DataSource = pelaajat2.ToList();
            pelaajaComboBox5.DataSource = pelaajat2.ToList();
            pelaajaComboBox6.DataSource = pelaajat2.ToList();

            if (pelit.Count == 3)
            {
                pelaajaComboBox1.SelectedIndex = pelaajat1.IndexOf(this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelit[0].Id1));
                pelaajaComboBox2.SelectedIndex = pelaajat1.IndexOf(this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelit[1].Id1));
                pelaajaComboBox3.SelectedIndex = pelaajat1.IndexOf(this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelit[2].Id1));
                pelaajaComboBox4.SelectedIndex = pelaajat2.IndexOf(this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelit[0].Id2));
                pelaajaComboBox5.SelectedIndex = pelaajat2.IndexOf(this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelit[1].Id2));
                pelaajaComboBox6.SelectedIndex = pelaajat2.IndexOf(this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == pelit[2].Id2));
            }
            else
            {
                pelaajaComboBox1.SelectedIndex = 0;
                pelaajaComboBox2.SelectedIndex = 1;
                pelaajaComboBox3.SelectedIndex = 2;
                pelaajaComboBox4.SelectedIndex = 0;
                pelaajaComboBox5.SelectedIndex = 1;
                pelaajaComboBox6.SelectedIndex = 2;
            }

            Tarkista();
        }
    }
}
