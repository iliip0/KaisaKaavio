using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Integraatio
{
    public partial class HaeOsallistujatBiljardiOrgistaPopup : Form
    {
        private Kilpailu kilpailu = null;
        private Loki loki = null;
        private string ilmoittautuneetSivu = string.Empty;
        private List<Pelaaja> pelaajat = new List<Pelaaja>();

        private Color biljardiOrgVari = Color.FromArgb(255, 180, 22, 111);

        public HaeOsallistujatBiljardiOrgistaPopup(Kilpailu kilpailu, Loki loki)
        {
            this.kilpailu = kilpailu;
            this.loki = loki;
            
            InitializeComponent();

            this.kilpailuBindingSource.DataSource = this.kilpailu;

            this.richTextBox1.BackColor = biljardiOrgVari;
            this.splitContainer1.Panel1.BackColor = biljardiOrgVari;

            this.haeOsallistujatatButton.Enabled = !string.IsNullOrEmpty(this.kilpailunNumeroTextBox.Text);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.pelaajat.Any())
            {
                this.kilpailu.Osallistujat.RaiseListChangedEvents = false;

                this.kilpailu.Osallistujat.Clear();

                int sija = 1;
                foreach (var pelaaja in this.pelaajat)
                {
                    Pelaaja osallistuja = this.kilpailu.Osallistujat.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, pelaaja.Nimi));

                    if (osallistuja == null)
                    {
                        osallistuja = new Pelaaja() 
                        {
                            Nimi = pelaaja.Nimi,
                        };

                        this.kilpailu.Osallistujat.Add(osallistuja);
                    }

                    osallistuja.Seura = pelaaja.Seura;

                    if (this.kilpailu.Sijoittaminen == Sijoittaminen.Sijoitetaan8Pelaajaa && sija <= 8)
                    {
                        osallistuja.Sijoitettu = sija.ToString();
                    }
                    else if (this.kilpailu.Sijoittaminen == Sijoittaminen.Sijoitetaan24Pelaajaa && sija <= 24)
                    {
                        osallistuja.Sijoitettu = sija.ToString();
                    }
                    else
                    {
                        osallistuja.Sijoitettu = string.Empty;
                    }

                    sija++;
                }

                while (true)
                {
                    var p = this.kilpailu.Osallistujat.FirstOrDefault(x => !this.pelaajat.Any(y => Tyypit.Nimi.Equals(y.Nimi, x.Nimi)));
                    if (p != null)
                    {
                        this.kilpailu.Osallistujat.Remove(p);
                    }
                    else
                    {
                        break;
                    }
                }

                this.kilpailu.Osallistujat.RaiseListChangedEvents = true;
                this.kilpailu.Osallistujat.ResetBindings();
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void peruutaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void kilpailunNumeroTextBox_TextChanged(object sender, EventArgs e)
        {
            this.haeOsallistujatatButton.Enabled = !string.IsNullOrEmpty(this.kilpailunNumeroTextBox.Text);
        }

        private void haeOsallistujatatButton_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            this.Enabled = false;
            this.richTextBox2.Text = "Haetaan ilmoittautuneita biljardi.org sivustolta....";

            this.backgroundWorker1.RunWorkerAsync();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.ilmoittautuneetSivu = Integraatio.BiljardiOrg.LataaIlmoittautuneetSivu(this.kilpailunNumeroTextBox.Text, this.loki);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var p = Integraatio.BiljardiOrg.ParsiIlmoittautuneetSivulta(this.ilmoittautuneetSivu, this.loki);

            this.pelaajat.Clear();
            this.pelaajat.AddRange(p);

            this.progressBar1.Visible = false;
            this.Enabled = true;

            if (pelaajat.Any())
            {
                this.richTextBox2.Text = 
                    "Valmis!" + Environment.NewLine + 
                    string.Join(
                        Environment.NewLine,
                        pelaajat.Select(x => string.Format("{0}. {1} {2}", x.IlmoittautumisNumero, x.Nimi, x.Seura)));
            }
            else
            {
                this.richTextBox2.Text = "Valmis! Ei löytynyt ilmoittautuneita";
            }
        }
    }
}
