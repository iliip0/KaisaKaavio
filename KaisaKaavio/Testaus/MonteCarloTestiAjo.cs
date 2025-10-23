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

        private int UusintaHakuvirheita = 0;
        private Dictionary<int, int> UusintaHakuVirheitaPelaajaMaaranMukaan = new Dictionary<int, int>();
        private int EdellaHakuvirheita = 0;
        private int VirheellisiaKisoja = 0;

        private int Peleja = 0;
        private DateTime aloitusAika;
        private DateTime lopetusAika;

        public MonteCarloTestiAjo(int poytienMaara, bool satunnainenPelienJarjestys, int kisoja, int minPelaajia, int maxPelaajia, IStatusRivi status)
        {
            this.aloitusAika = DateTime.Now;

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

                    this.UusintaHakuvirheita += testiKilpailu.UusintaHakuvirheita;

                    foreach (var u in testiKilpailu.UusintaHakuvirheitaPelaajaMaaranMukaan)
                    {
                        if (this.UusintaHakuVirheitaPelaajaMaaranMukaan.ContainsKey(u.Key))
                        {
                            this.UusintaHakuVirheitaPelaajaMaaranMukaan[u.Key] += u.Value;
                        }
                        else
                        {
                            this.UusintaHakuVirheitaPelaajaMaaranMukaan.Add(u.Key, u.Value);
                        }
                    }

                    this.EdellaHakuvirheita += testiKilpailu.EdellaHakuvirheita;
                    this.Peleja += testiKilpailu.TestattavaKilpailu.Pelit.Count();

                    this.OnnistuneitaTesteja++;

                    testiKilpailu.TestattavaKilpailu.VapautaTarpeetonMuisti();

                    try
                    {
                        if (testiKilpailu.UusintaHakuvirheita > 0 ||
                            testiKilpailu.EdellaHakuvirheita > 0)
                        {
                            this.VirheellisiaKisoja++;
                            testiKilpailu.TestattavaKilpailu.TallennaNimella(Path.Combine(kansio, nimi + "_HAKUVIRHE.xml"), false, false);
                        }
                        else
                        {
                            Directory.Delete(kansio, true);
                        }
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

                        testiKilpailu.TestattavaKilpailu.TallennaNimella(Path.Combine(kansio, nimi + "_VIRHE.xml"), false, false);
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

            // Tallenna tulokset
            try
            {
                this.lopetusAika = DateTime.Now;

                string kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaavioTestitulokset");
                Directory.CreateDirectory(kansio);

                string tiedosto = Path.Combine(kansio, "MonteCarlo.txt");

                StringBuilder raportti = new StringBuilder();

                raportti.AppendLine(string.Format("======((   Ajo {0}, kisoja {1}, pelaajia {2}-{3}, pöytiä {4}:   ))======", 
                    DateTime.Now.ToShortTimeString(), 
                    this.Kisoja,
                    this.MinPelaajia,
                    this.MaxPelaajia,
                    this.PoytienMaara));

                if (this.Peleja > 0)
                {
                    raportti.AppendLine(string.Format("Pelejä: {0}", this.Peleja));
                    raportti.AppendLine(string.Format("Kesto: {0} sekuntia ({1}s per peli)",
                        (this.lopetusAika - this.aloitusAika).TotalSeconds,
                        ((this.lopetusAika - this.aloitusAika).TotalMilliseconds / this.Peleja) / 1000.0f));

                    raportti.AppendLine(string.Format("Uusintaotteluvirheitä: {0} ({1}%)  -- ({2})",
                        this.UusintaHakuvirheita,
                        (int)((this.UusintaHakuvirheita / ((float)this.Kisoja)) * 100),
                        string.Join(",", this.UusintaHakuVirheitaPelaajaMaaranMukaan.Select(x => string.Format("[{0}]:{1}", x.Key, x.Value)))));

                    raportti.AppendLine(string.Format("Kierrosvirheitä: {0} ({1}%)",
                        this.EdellaHakuvirheita,
                        (int)((this.EdellaHakuvirheita / ((float)this.Kisoja)) * 100)));
                }

                if (this.Kisoja > 0)
                {
                    raportti.AppendLine(string.Format("Väärin haettuja kisoja: {0} ({1}%)",
                        this.VirheellisiaKisoja,
                        (int)((this.VirheellisiaKisoja / ((float)this.Kisoja)) * 100)));
                }

                using (var writer = File.AppendText(tiedosto))
                {
                    writer.WriteLine(raportti.ToString());
                }
            }
            catch
            { 
            }

            return this.EpaonnistuneitaTesteja == 0;
        }
    }
}
