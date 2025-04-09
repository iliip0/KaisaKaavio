using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    /// <summary>
    /// Loki tiedoston toteutus
    /// </summary>
    public class Loki
    {
        private string kansio = null;
        private string tiedosto = null;

        public Loki(string kansio)
        {
            this.kansio = kansio;
            Directory.CreateDirectory(this.kansio);

            this.kansio = Path.Combine(this.kansio, "Loki");
            Directory.CreateDirectory(this.kansio);

#if ALLOW_MULTIPLE_INSTANCES
            this.tiedosto = Path.Combine(this.kansio, string.Format("loki_{0}_{1}.txt", 
                Process.GetCurrentProcess().Id, 
                DateTime.Today.ToShortDateString()));
#else
            this.tiedosto = Path.Combine(this.kansio, string.Format("loki_{0}.txt", DateTime.Today.ToShortDateString()));
#endif

#if !ALLOW_MULTIPLE_INSTANCES
            Tyypit.Tiedosto.PoistaVanhimmatTiedostotKansiosta(this.kansio, 40);
#endif
        }

        /// <summary>
        /// Kirjoittaa tekstiä lokitiedostoon
        /// </summary>
        public void Kirjoita(string text, Exception e = null, bool messageBox = false)
        {
#if DEBUG
            if (!string.IsNullOrEmpty(text))
            {
                Debug.WriteLine(text);
            }
#endif

            string teksti = string.Empty;

            if (e != null)
            {
                teksti = string.Format("ERR: {0}, {1}{2}{3}", text, e.Message, Environment.NewLine, e.StackTrace);
            }
            else
            {
                teksti = text;
            }

            lock (this)
            {
                try
                {
                    using (var writer = File.AppendText(this.tiedosto))
                    {
                        writer.WriteLine(DateTime.Now.ToString() + " - " + teksti);
                    }
                }
                catch
                {
                }
            }

            if (messageBox)
            {
                if (e != null)
                {
                    Error(teksti);
                }
                else 
                {
                    InfoBox(teksti);
                }
            }
        }

        private void Error(string text)
        {
            MessageBox.Show(text, "Virhe", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InfoBox(string text)
        {
            MessageBox.Show(text, "Tiedoksi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
