using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    static class Program
    {
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !DEBUG
            // Puretaan GNU GPL v3 lisenssi exen viereen jos se ei jo löydy sieltä
            try
            {
                string kansio = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string lisenssi = Path.Combine(kansio, "LICENSE.txt");
                if (!File.Exists(lisenssi))
                {
                    PuraResurssi("KaisaKaavio.Resources.LICENSE", lisenssi, null);
                }

                string versioloki = Path.Combine(kansio, "Versiohistoria.txt");
                PuraResurssi("KaisaKaavio.Resources.CHANGELOG.md", versioloki, null);
            }
            catch
            { 
            }
#endif

            using (Mutex kaisaKaavioRunning = new Mutex(false, "Global\\KaisaKaavioRunning"))
            {
                if (!kaisaKaavioRunning.WaitOne(0, false))
                {
                    MessageBox.Show(
                        "KaisaKaavio ohjelma on jo avattuna. Vain yksi kaavio voi olla kerrallaan auki.",
                        "KaisaKaavio",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
