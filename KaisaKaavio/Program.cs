using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public static class Program
    {
        /// <summary>
        /// Tämä muuttuja kertoo, onko tämä ohjelmainstanssi ainoa/ensimmäinen aktiivinen KaisaKaavio ohjelma tietokoneella.
        /// Ainoastaan ensimmäinen ohjelma tallentaa käyttäjän asetukset ja Ranking tietueet
        /// </summary>
        public static bool UseampiKaisaKaavioAvoinna = false;

#if !LITE_VERSION
        /// <summary>
        /// Purkaa exen sisälle resurssiksi leivotun tiedoston levylle
        /// </summary>
        public static bool PuraResurssi(string resurssinNimi, string tiedostonNimi, Loki loki)
        {
            try
            {
                using (FileStream tiedosto = new FileStream(tiedostonNimi, FileMode.Create))
                {
                    using (var resurssi = Assembly.GetExecutingAssembly().GetManifestResourceStream(resurssinNimi))
                    {
                        byte[] buffer = new byte[32768];
                        int read;

                        while ((read = resurssi.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            tiedosto.Write(buffer, 0, read);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita("Resurssin purku epäonnistui", e, false);
                }

                return false;
            }
        }
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !LITE_VERSION
            string kansio = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Puretaan GNU GPL v3 lisenssi exen viereen jos se ei jo löydy sieltä
            string lisenssi = Path.Combine(kansio, "LICENSE.txt");
            if (!File.Exists(lisenssi))
            {
                PuraResurssi("KaisaKaavio.Resources.LICENSE", lisenssi, null);
            }

            //PuraResurssi("KaisaKaavio.Resources.KaisaKaavioOhje.pdf", Path.Combine(kansio, "Ohje.pdf"), null);
#endif

            using (Mutex kaisaKaavioRunning = new Mutex(false, "Global\\KaisaKaavioRunning"))
            {
                try
                {
                    if (!kaisaKaavioRunning.WaitOne(0, false))
                    {
                        UseampiKaisaKaavioAvoinna = true;
                    }
                }
                catch
                {
                    UseampiKaisaKaavioAvoinna = true;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
