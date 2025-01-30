using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    public class MonteCarloTestiAjo : ITestiAjo
    {
        public int Kisoja { get; private set; }
        public int MinPelaajia { get; private set; }
        public int MaxPelaajia { get; private set; }
        public int PoytienMaara { get; private set; }
        public bool SatunnainenPelienJarjestys { get; private set; }
        public int OnnistuneitaTesteja { get; set; }
        public int EpaonnistuneitaTesteja { get; set; }
        public string VirheKansio { get; set; }

        private IStatusRivi status = null;

        public MonteCarloTestiAjo(int poytienMaara, bool satunnainenPelienJarjestys, int kisoja, int minPelaajia, int maxPelaajia, IStatusRivi status)
        {
            this.PoytienMaara = poytienMaara;
            this.SatunnainenPelienJarjestys = satunnainenPelienJarjestys;
            this.Kisoja = kisoja;
            this.MinPelaajia = minPelaajia;
            this.MaxPelaajia = maxPelaajia;
            this.OnnistuneitaTesteja = 0;
            this.EpaonnistuneitaTesteja = 0;

            this.status = status;

            this.VirheKansio = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "KaisaKaaviot",
                "Testit",
                "MonteCarlo",
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
            Random random = new Random();

            for (int i = 0; i < this.Kisoja; ++i)
            {
                int pelaajia = random.Next(this.MinPelaajia, this.MaxPelaajia);
                string nimi = string.Format("TestiKilpailu_{0}", i + 1);

                string kansio = Path.Combine(this.VirheKansio, nimi);
                Directory.CreateDirectory(kansio);

                Loki loki = new Loki(kansio);

                MonteCarloTestiKilpailu testiKilpailu = new MonteCarloTestiKilpailu(
                    nimi,
                    this.PoytienMaara,
                    this.SatunnainenPelienJarjestys,
                    pelaajia,
                    loki);

                try
                {
                    testiKilpailu.PelaaKilpailu(this.status, i * 3, this.Kisoja * 3);
                    this.OnnistuneitaTesteja++;

                    try
                    {
                        Directory.Delete(kansio, true);
                    }
                    catch
                    { 
                    }
                }
                catch (Exception ee)
                {
                    this.EpaonnistuneitaTesteja++;

                    // Tallenna epäonnistuneen ajon testikaavio tutkimuksia varten
                    try
                    {
                        loki.Kirjoita(string.Format("Testi {0} epäonnistui: {1}", nimi, ee.Message), ee, false);

                        testiKilpailu.TestattavaKilpailu.TallennaNimella(Path.Combine(kansio, nimi + "_VIRHE.xml"), false);
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
