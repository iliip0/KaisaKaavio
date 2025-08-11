using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace KaisaKaavio.Kayttoliittyma
{
    public partial class TekstiIkkuna : Form
    { 
        public TekstiIkkuna(string otsikko, string teksti)
        {
            InitializeComponent();

            this.richTextBox1.Text = teksti;
            this.textBox1.Text = otsikko;

            Shown += TekstiIkkuna_Shown;
        }

        private void TekstiIkkuna_Shown(object sender, EventArgs e)
        {
            this.suljeButton.Focus();
        }

        public static void NaytaTekstiTiedostoMuistista(string tiedosto, string otsikko, Loki loki)
        {
            try
            {
                using (var resurssi = Assembly.GetExecutingAssembly().GetManifestResourceStream(tiedosto))
                {
                    using (var reader = new StreamReader(resurssi))
                    {
                        string teksti = reader.ReadToEnd();

                        using (var ikkuna = new TekstiIkkuna(otsikko, teksti))
                        {
                            ikkuna.ShowDialog();
                        }
                    }
                }
            } 
            catch (Exception e) 
            {
                if (loki != null)
                {
                    loki.Kirjoita("Tekstitiedoston purku ja näyttäminen epäonnistui", e, true);
                }
            }
        }
    }
}
