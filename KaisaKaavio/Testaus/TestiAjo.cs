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

        private Loki loki = null;
        private IStatusRivi status = null;

        public TestiAjo(string kansio, Loki loki, IStatusRivi status)
        {
            this.Kansio = kansio;
            this.loki = loki;
            this.status = status;
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

                testiKilpailu.Loki = this.loki;
                testiKilpailu.Avaa(tiedosto.FullName);

                TestiKilpailu testi = new TestiKilpailu(loki, testiKilpailu);

                try
                {
                    testi.PelaaKilpailu(this.status, i * 3, tiedostot.Count() * 3);
                }
                catch (Exception ee)
                {
                    throw new Exception(string.Format("{0} - {1}", testiKilpailu.Nimi, ee.Message));
                }

                onnistuneitaTesteja++;
            }

            this.status.PaivitaStatusRivi(string.Format("Testi onnistui. {0} kisaa pelattu oikein läpi", onnistuneitaTesteja), true, 100, 100);

            return onnistuneitaTesteja;
        }
    }
}
