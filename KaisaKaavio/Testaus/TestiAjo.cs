using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    /// <summary>
    /// Testaa kaikki kilpailut kansiossa
    /// </summary>
    public class TestiAjo
    {
        public string Kansio { get; private set; }

        private IStatusRivi status = null;
        private string virheKansio = string.Empty;

        public TestiAjo(string kansio, IStatusRivi status)
        {
            this.Kansio = kansio;
            this.status = status;
            this.virheKansio = Path.Combine(
                Path.GetTempPath(), 
                "KaisaKaavioTestit", 
                string.Format("{0}_{1}_{2}_{3}_{4}",
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    DateTime.Now.Hour,
                    DateTime.Now.Minute));

            Directory.CreateDirectory(this.virheKansio);
        }

        public int Aja()
        {
            int onnistuneitaTesteja = 0;

            DirectoryInfo dir = new DirectoryInfo(this.Kansio);

            var tiedostot = dir.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly);

            int i = 0;

            foreach (var tiedosto in tiedostot)
            {
                Kilpailu testiKilpailu = new Kilpailu();

                string kansio = Path.Combine(this.virheKansio, tiedosto.Name);
                Directory.CreateDirectory(kansio);

                Loki loki = new Loki(kansio);
                testiKilpailu.Loki = loki;
                testiKilpailu.Avaa(tiedosto.FullName);

                TestiKilpailu testi = new TestiKilpailu(loki, testiKilpailu);

                try
                {
                    testi.PelaaKilpailu(this.status, i * 3, tiedostot.Count() * 3);
                }
                catch (Exception ee)
                {
                    // Tallenna testikaavio tutkimuksia varten
                    try
                    {
                        testi.OikeaKilpailu.TallennaNimella(Path.Combine(kansio, testiKilpailu.Nimi + ".xml"));
                        testi.TestattavaKilpailu.TallennaNimella(Path.Combine(kansio, testiKilpailu.Nimi + "_VIRHE.xml"));
                    }
                    catch
                    { 
                    }

                    throw new Exception(string.Format("{0} - {1}", testiKilpailu.Nimi, ee.Message));
                }

                onnistuneitaTesteja++;
            }

            this.status.PaivitaStatusRivi(string.Format("Testi onnistui. {0} kisaa pelattu oikein läpi", onnistuneitaTesteja), true, 100, 100);

            return onnistuneitaTesteja;
        }
    }
}
