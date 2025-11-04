using KaisaKaavio.Integraatio;
using KaisaKaavio.Testaus;
using KaisaKaavio.Tyypit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Aloitussivu
{
    public partial class Aloitussivu : Form
    {
        private Form1 ikkuna = null;
        private Asetukset asetukset = null;
        private Kilpailu kilpailu = null;
        private List<OhjelmaVersioTietue> ohjelmaVersiot = new List<OhjelmaVersioTietue>();

        public Aloitussivu(Form1 ikkuna, Asetukset asetukset, Kilpailu kilpailu)
        {
            this.ikkuna = ikkuna;
            this.asetukset = asetukset;
            this.kilpailu = kilpailu;

            InitializeComponent();

            this.ylaPalkkiSplitContainer.Panel2Collapsed = true;

            if (asetukset.ViimeisimmatKilpailut.Count > 0)
            {
                this.viimeisimmatComboBox.DataSource = asetukset.ViimeisimmatKilpailut;
                this.viimeisimmatComboBox.SelectedIndex = 0;
            }
            else
            {
                this.viimeisimmatComboBox.Visible = false;
                this.viimeisimmatPictureBox.Visible = false;
                this.avaaKilpailuButton.Visible = false;
            }

            this.versioLabel.Text = string.Format("versio {0}", Assembly.GetEntryAssembly().GetName().Version);

#if DEBUG
            this.TopMost = false;
#endif
        }

        private void jatkaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void kaisaViikkokisaButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Kaisa, KilpailunTyyppi.Viikkokisa, false);
        }

        private void karaViikkokisaButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Kara, KilpailunTyyppi.Viikkokisa, false);
        }

        private void pyramidiViikkokisaButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Pyramidi, KilpailunTyyppi.Viikkokisa, false);
        }

        private void muuViikkokisaButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Pool, KilpailunTyyppi.Viikkokisa, false);
        }

        private void snookerViikkokisaButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Snooker, KilpailunTyyppi.Viikkokisa, false);
        }

        private void heyballViikkokisaButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Heyball, KilpailunTyyppi.Viikkokisa, false);
        }

        private void kaisaRGButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Kaisa, KilpailunTyyppi.KaisanRGKilpailu, false);
        }

        private void kaisaSMButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Kaisa, KilpailunTyyppi.KaisanSMKilpailu, false);
        }

        private void kaisaAvoinButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Kaisa, KilpailunTyyppi.AvoinKilpailu, false);
        }

        private void muuAvoinButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Pool, KilpailunTyyppi.AvoinKilpailu, true);
        }

        private void avaaKilpailuButton_Click(object sender, EventArgs e)
        {
            try
            {
                var tiedosto = this.viimeisimmatComboBox.SelectedItem;
                if (tiedosto != null)
                {
                    this.ikkuna.AvaaKilpailu(((Tyypit.Tiedosto)tiedosto).Polku);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
            }
            catch
            {
            }
        }

        private void avaaTiedostoButton_Click(object sender, EventArgs e)
        {
            Hide();

            if (this.ikkuna.AvaaTiedostoDialogista())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();

                try
                {
                    this.ikkuna.BringToFront();
                }
                catch
                { 
                }

                return;
            }
            else
            {
                Show();
            }
        }

        private void testiKilpailuButton_Click(object sender, EventArgs e)
        {
            LuoUusiKilpailu(Laji.Kaisa, KilpailunTyyppi.Viikkokisa, true, true);
        }

        private void LuoUusiKilpailu(Laji laji, KilpailunTyyppi tyyppi, bool salliVaihtaa, bool testiKilpailu = false)
        {
            try
            {
                Hide();

                using (var popup = new UusiKilpailuPopup(this.asetukset))
                {
                    popup.AsetaOletusarvot(this.asetukset.OletusAsetukset(laji), laji, tyyppi, salliVaihtaa, testiKilpailu);

                    if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.ikkuna.LuoKilpailu(popup);
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                        return;
                    }
                    else
                    {
                        Show();
                    }
                }
            }
            catch
            {
            }
        }

        private void Aloitussivu_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
            }
        }

        private void Aloitussivu_Shown(object sender, EventArgs e)
        {
            Integraatio.KaisaKaavioFi.HaeDataa("OhjelmaVersiot", string.Empty, this, this.ikkuna.Loki, (tietue) =>
            {
                if (tietue != null && tietue.Rivit.Any())
                {
                    this.ohjelmaVersiot.Clear();

                    foreach (var rivi in tietue.Rivit)
                    {
                        this.ohjelmaVersiot.Add(new OhjelmaVersioTietue()
                        {
                            Versio = rivi.Get("Versio", string.Empty),
                            Tiiseri = rivi.Get("Tiiseri", string.Empty),
                            Muutokset = rivi.Get("Muutokset", string.Empty),
                            Bugit = rivi.Get("Bugit", string.Empty)
                        });
                    }

                    TarkistaOhjelmaVersio();
                }
            });
        }

        private void TarkistaOhjelmaVersio()
        {
            try
            {
                var uusin = this.ohjelmaVersiot.OrderByDescending(x => x.VersioNumero).FirstOrDefault();
                if (uusin != null && uusin.VersioNumero > 0)
                {
                    int nykyinenVersio = Assembly.GetEntryAssembly().GetName().Version.Revision;
                    if (uusin.VersioNumero > nykyinenVersio)
                    {
                        this.lataaUusinVersioButton.Text = string.Format("Lataa uusin versio (1.0.0.{0})", uusin.VersioNumero);
                        this.ylaPalkkiSplitContainer.Panel2Collapsed = false;

                        StringBuilder teksti = new StringBuilder();

                        teksti.AppendLine("Ohjelmasta on saatavilla uudempi versio.");
                        teksti.AppendLine("Voit ladata sen yllä olevaa nappia painamalla");

                        var paivitykset = this.ohjelmaVersiot
                            .Where(x => x.VersioNumero > nykyinenVersio)
                            .Where(x => !string.IsNullOrEmpty(x.Tiiseri))
                            .OrderBy(x => x.VersioNumero);

                        if (paivitykset.Any())
                        {
                            teksti.AppendLine("Uutta:");

                            foreach (var p in paivitykset)
                            {
                                teksti.AppendLine(string.Format("* {0}", p.Tiiseri));
                            }
                        }

                        this.toolTip2.Show(teksti.ToString(), this.lataaUusinVersioButton, 32, this.lataaUusinVersioButton.Height - 10, 5000);
                    }
                }
            }
            catch
            {
            }
        }

        private void lataaUusinVersioButton_Click(object sender, EventArgs e)
        {
            try
            {
                int versio = this.ohjelmaVersiot.OrderByDescending(x => x.VersioNumero).First().VersioNumero;

                string virhe = string.Empty;
                string tiedosto = Integraatio.KaisaKaavioFi.LataaOhjelmaVersioServerilta(versio, out virhe);

                if (!string.IsNullOrEmpty(tiedosto))
                {
                    this.saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    this.saveFileDialog1.FileName = string.Format("KaisaKaavio_{0}.exe", versio);
                    this.saveFileDialog1.CheckFileExists = false;
                    this.saveFileDialog1.CheckPathExists = false;

                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(tiedosto, saveFileDialog1.FileName, true);
                        MessageBox.Show(this, "Uusin versio tallennettu onnistuneesti!\n" + this.saveFileDialog1.FileName + 
                            "\nVoit sulkea tämän ikkunan ja avata uuden version yllä mainitusta tiedostosta...");
                    }

                    if (File.Exists(tiedosto))
                    {
                        File.Delete(tiedosto);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Tiedoston lataus epäonnistui: {0}", virhe), "Virhe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Tiedoston lataus epäonnistui: {0}", ex.Message), "Virhe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void avaaOnlineKisaButton_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();

                using (var popup = new LataaOnlineKilpailuPopup(this.ikkuna.Loki))
                {
                    if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
                        !string.IsNullOrEmpty(popup.LadattuTiedosto))
                    {
                        if (this.ikkuna.AvaaOnlineKilpailuValiaikaisestaTiedostosta(popup.LadatunKilpailunId, popup.LadattuTiedosto))
                        {
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                }
            }
            catch
            {
            }

            Show();
        }
    }
}
