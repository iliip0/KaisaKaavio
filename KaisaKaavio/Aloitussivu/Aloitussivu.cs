using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public Aloitussivu(Form1 ikkuna, Asetukset asetukset, Kilpailu kilpailu)
        {
            this.ikkuna = ikkuna;
            this.asetukset = asetukset;
            this.kilpailu = kilpailu;

            InitializeComponent();

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

                using (var popup = new UusiKilpailuPopup())
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
    }
}
