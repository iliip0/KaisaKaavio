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
        public int PoytienMaara { get; private set; }
        public bool SatunnainenPelienJarjestys { get; private set; }
        public int OnnistuneitaTesteja { get; private set; }
        public int EpaonnistuneitaTesteja { get; private set; }
        public string VirheKansio { get; private set; }

        private IStatusRivi status = null;

        public TestiAjo(string kansio, int poytienMaara, bool satunnainenPelienJarjestys, IStatusRivi status)
        {
            this.PoytienMaara = poytienMaara;
            this.SatunnainenPelienJarjestys = satunnainenPelienJarjestys;
            this.Kansio = kansio;
            this.OnnistuneitaTesteja = 0;
            this.EpaonnistuneitaTesteja = 0;

            this.status = status;

            this.VirheKansio = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "KaisaKaaviot",
                "Testit", 
                string.Format("{0}_{1}_{2}_{3}_{4}",
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    DateTime.Now.Hour,
                    DateTime.Now.Minute));

            Directory.CreateDirectory(this.VirheKansio);
        }

        public bool Aja()
        {
            DirectoryInfo dir = new DirectoryInfo(this.Kansio);

            var tiedostot = dir.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly);

            int i = 0;

            foreach (var tiedosto in tiedostot)
            {
                Kilpailu testiKilpailu = new Kilpailu();

                string kansio = Path.Combine(this.VirheKansio, tiedosto.Name);
                Directory.CreateDirectory(kansio);

                Loki loki = new Loki(kansio);
                testiKilpailu.Loki = loki;
                testiKilpailu.Avaa(tiedosto.FullName);

                TestiKilpailu testi = new TestiKilpailu(this.PoytienMaara, this.SatunnainenPelienJarjestys, loki, testiKilpailu);

                try
                {
                    testi.PelaaKilpailu(this.status, i * 3, tiedostot.Count() * 3);
                    this.OnnistuneitaTesteja++;
                }
                catch (Exception ee)
                {
                    this.EpaonnistuneitaTesteja++;

                    // Tallenna epäonnistuneen ajon testikaavio tutkimuksia varten
                    try
                    {
                        loki.Kirjoita(string.Format("Testi {0} epäonnistui: {1}", testiKilpailu.Nimi, ee.Message), ee, false);

                        testi.OikeaKilpailu.TallennaNimella(Path.Combine(kansio, testiKilpailu.Nimi + ".xml"));
                        testi.TestattavaKilpailu.TallennaNimella(Path.Combine(kansio, testiKilpailu.Nimi + "_VIRHE.xml"));
                    }
                    catch
                    { 
                    }
                }
            }

            if (this.EpaonnistuneitaTesteja == 0)
            {
                this.status.PaivitaStatusRivi(string.Format("Testi onnistui. {0} kisaa pelattu oikein läpi", this.OnnistuneitaTesteja), true, 100, 100);
            }
            else
            {
                this.status.PaivitaStatusRivi(string.Format("Testi epäonnistui. {0} kisaa pelattu virheellisesti", this.EpaonnistuneitaTesteja), true, 100, 100);
            }

            return this.EpaonnistuneitaTesteja == 0;
        }
    }
}
