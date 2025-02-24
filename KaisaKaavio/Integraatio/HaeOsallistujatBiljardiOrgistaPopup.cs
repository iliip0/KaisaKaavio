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
        private List<Integraatio.BiljardiOrgKilpailu> kisat = new List<BiljardiOrgKilpailu>();

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

            var k = Integraatio.BiljardiOrg.LataaTulevatKisat(this.loki, this.kilpailu.Laji, this.kilpailu.KilpaSarja);
            if (k.Any())
            {
                this.kisat.Add(new BiljardiOrgKilpailu() { Nimi = "(valitse kilpailu)" });
                this.kisat.AddRange(k);

                this.kisatComboBox.DataSource = this.kisat;
                this.kisatComboBox.SelectedIndex = 0;
                this.kisatComboBox.DisplayMember = "Nimi";

                if (!string.IsNullOrEmpty(this.kilpailu.BiljardiOrgId))
                {
                    var kisa = this.kisat.FirstOrDefault(x => string.Equals(x.Id, this.kilpailu.BiljardiOrgId));
                    if (kisa != null)
                    {
                        this.kisatComboBox.SelectedIndex = this.kisat.IndexOf(kisa);
                    }
                }
            }
            else 
            {
                this.kisatLabel.Visible = false;
                this.kisatComboBox.Visible = false;
            }
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
                    osallistuja.Joukkue = pelaaja.Joukkue;
                    osallistuja.Pelaajan1Nimi = pelaaja.Pelaajan1Nimi;
                    osallistuja.Pelaajan1Seura = pelaaja.Pelaajan1Seura;
                    osallistuja.Pelaajan2Nimi = pelaaja.Pelaajan2Nimi;
                    osallistuja.Pelaajan2Seura = pelaaja.Pelaajan2Seura;

                    if (!string.IsNullOrEmpty(osallistuja.Pelaajan1Seura) &&
                        !string.IsNullOrEmpty(osallistuja.Pelaajan2Seura))
                    {
                        osallistuja.Seura = string.Empty;
                    }

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
            this.ilmoittautuneetSivu = Integraatio.BiljardiOrg.LataaIlmoittautuneetSivu(
                this.kilpailunNumeroTextBox.Text, 
                this.kilpailu.Laji,
                this.kilpailu.KilpaSarja,
                this.loki);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var p = Integraatio.BiljardiOrg.ParsiIlmoittautuneetSivulta(this.ilmoittautuneetSivu, this.loki);

            this.pelaajat.Clear();

            switch (this.kilpailu.KilpaSarja)
            {
                case KilpaSarja.Joukkuekilpailu:
                    break;

                case KilpaSarja.Parikilpailu:
                case KilpaSarja.MixedDoubles:
                    var g = p.GroupBy(x => x.IlmoittautumisNumero);
                    int ilmoNumero = 1;
                    foreach (var j in g)
                    {
                        if (j.Count() == 2)
                        {
                            string nimi1 = j.First().Nimi;
                            string seura1 = j.First().Seura;
                            string nimi2 = j.Last().Nimi;
                            string seura2 = j.Last().Seura;

                            string seura = seura1;
                            if (!string.Equals(seura1, seura2))
                            {
                                if (string.IsNullOrEmpty(seura1))
                                {
                                    seura = seura2;
                                }
                                else if (string.IsNullOrEmpty(seura2))
                                {
                                    seura = seura1;
                                }
                                else
                                {
                                    seura = seura1 + "/" + seura2;
                                }
                            }

                            if (!string.IsNullOrEmpty(nimi1) &&
                                !string.IsNullOrEmpty(nimi2))
                            {
                                this.pelaajat.Add(new Pelaaja()
                                {
                                    Nimi = Tyypit.Nimi.NimiParikisassa(nimi1) + " & " + Tyypit.Nimi.NimiParikisassa(nimi2),
                                    Seura = seura,
                                    Pelaajan1Nimi = nimi1,
                                    Pelaajan1Seura = seura1,
                                    Pelaajan2Nimi = nimi2,
                                    Pelaajan2Seura = seura2,
                                    IlmoittautumisNumero = ilmoNumero.ToString()
                                });

                                ilmoNumero++;
                            }
                        }
                    }
                    
                    break;

                default:
                    this.pelaajat.AddRange(p);
                    break;
            }


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

        private void kisatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.kisatComboBox.SelectedIndex > 0)
            {
                BiljardiOrgKilpailu kisa = (BiljardiOrgKilpailu)this.kisatComboBox.SelectedItem;
                this.kilpailu.BiljardiOrgId = kisa.Id;
                this.kilpailunNumeroTextBox.Text = kisa.Id;
            }
        }
    }
}
