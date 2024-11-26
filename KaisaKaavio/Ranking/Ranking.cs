using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Ranking
{
    public class Ranking
    {
        private string kansio = null;
        private Dictionary<int, Dictionary<RankingSarjanPituus, List<RankingSarja>>> sarjat = new Dictionary<int, Dictionary<RankingSarjanPituus, List<RankingSarja>>>();

        public RankingAsetukset Asetukset { get; private set; }
        public List<string> Vuodet { get; private set; }
        public Loki Loki = null;

        public Ranking()
        {
            this.kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot", "Ranking");

            Directory.CreateDirectory(this.kansio);
            Directory.CreateDirectory(Path.Combine(this.kansio, DateTime.Now.Year.ToString()));

            this.Vuodet = new List<string>();

            foreach (var dir in Directory.EnumerateDirectories(this.kansio))
            {
                this.Vuodet.Add(dir);
            }

            this.Asetukset = new RankingAsetukset();
        }

        private string Kansio(int vuosi, RankingSarjanPituus pituus)
        {
            switch (pituus)
            {
                case RankingSarjanPituus.Kuukausi: return Path.Combine(this.kansio, vuosi.ToString(), "1kk");
                case RankingSarjanPituus.Vuodenaika: return Path.Combine(this.kansio, vuosi.ToString(), "3kk");
                case RankingSarjanPituus.Puolivuotta: return Path.Combine(this.kansio, vuosi.ToString(), "6kk");
                case RankingSarjanPituus.Vuosi: return Path.Combine(this.kansio, "12kk");
                default: return this.kansio;
            }
        }

        public List<RankingSarja> AvaaSarjat(int vuosi, RankingSarjanPituus pituus)
        {
            Dictionary<RankingSarjanPituus, List<RankingSarja>> vuodenSarjat = null;
            if (this.sarjat.ContainsKey(vuosi))
            {
                vuodenSarjat = this.sarjat[vuosi];
            }
            else
            {
                vuodenSarjat = new Dictionary<RankingSarjanPituus, List<RankingSarja>>();
                this.sarjat.Add(vuosi, vuodenSarjat);
            }


            if (vuodenSarjat.ContainsKey(pituus))
            {
                return vuodenSarjat[pituus];
            }

            List<RankingSarja> sarjat = new List<RankingSarja>();
            vuodenSarjat.Add(pituus, sarjat);

            string sarjaKansio = Kansio(vuosi, pituus);

            Directory.CreateDirectory(sarjaKansio);

            foreach (var sarjaTiedosto in Directory.EnumerateFiles(sarjaKansio, "*.xml", SearchOption.TopDirectoryOnly))
            { 
            }

            return sarjat;
        }

        public void TallennaAvatutSarjat()
        {
            foreach (var vuosi in this.sarjat)
            {
                foreach (var pituus in vuosi.Value)
                {
                    string sarjaKansio = Kansio(vuosi.Key, pituus.Key);

                    foreach (var sarja in pituus.Value)
                    {
                        try
                        {
                            sarja.Tallenna(this.Loki, sarjaKansio);
                        }
                        catch (Exception e)
                        {
                            if (this.Loki != null)
                            {
                                this.Loki.Kirjoita("Rankingsarjan tallennus epäonnistui!", e, false);
                            }
                        }
                    }
                }
            }
        }

        public void LisaaKilpailu(Kilpailu kilpailu)
        {
            int vuosi = kilpailu.AlkamisAika.Year;

            LisaaKilpailu12kk(kilpailu, vuosi);
            LisaaKilpailu6kk(kilpailu, vuosi);
            LisaaKilpailu3kk(kilpailu, vuosi);
            LisaaKilpailu1kk(kilpailu, vuosi);
        }

        private void LisaaKilpailu1kk(Kilpailu kilpailu, int vuosi)
        {
            int kuu = kilpailu.AlkamisAika.Month;
            var sarjat1kk = AvaaSarjat(vuosi, RankingSarjanPituus.Kuukausi);
            RankingSarja sarja1kk = sarjat1kk.FirstOrDefault(x => x.SarjanNumero == kuu);
            if (sarja1kk == null)
            {
                sarja1kk = new RankingSarja()
                {
                    Pituus = RankingSarjanPituus.Kuukausi,
                    SarjanNumero = kuu,
                };

                switch (kuu)
                {
                    case 1: sarja1kk.Nimi = "Tammikuu"; break;
                    case 2: sarja1kk.Nimi = "Helmikuu"; break;
                    case 3: sarja1kk.Nimi = "Maaliskuu"; break;
                    case 4: sarja1kk.Nimi = "Huhtikuu"; break;
                    case 5: sarja1kk.Nimi = "Toukokuu"; break;
                    case 6: sarja1kk.Nimi = "Kesäkuu"; break;
                    case 7: sarja1kk.Nimi = "Heinäkuu"; break;
                    case 8: sarja1kk.Nimi = "Elokuu"; break;
                    case 9: sarja1kk.Nimi = "Syyskuu"; break;
                    case 10: sarja1kk.Nimi = "Lokakuu"; break;
                    case 11: sarja1kk.Nimi = "Marraskuu"; break;
                    case 12:
                    default: sarja1kk.Nimi = "Joulukuu"; break;
                }

                sarjat1kk.Add(sarja1kk);
            }

            sarja1kk.LisaaKilpailu(kilpailu, this.Asetukset);
        }

        private void LisaaKilpailu3kk(Kilpailu kilpailu, int vuosi)
        {
            int kvartaali = kilpailu.AlkamisAika.Month / 4;
            var sarjat3kk = AvaaSarjat(vuosi, RankingSarjanPituus.Vuodenaika);
            RankingSarja sarja3kk = sarjat3kk.FirstOrDefault(x => x.SarjanNumero == kvartaali);
            if (sarja3kk == null)
            {
                sarja3kk = new RankingSarja()
                {
                    Pituus = RankingSarjanPituus.Vuodenaika,
                    SarjanNumero = kvartaali,
                };

                switch (kvartaali)
                {
                    case 0: sarja3kk.Nimi = "Tammikuu-Maaliskuu"; break;
                    case 1: sarja3kk.Nimi = "Huhtikuu-Kesäkuu"; break;
                    case 2: sarja3kk.Nimi = "Heinäkuu-Syyskuu"; break;
                    case 3:
                    default: sarja3kk.Nimi = "Lokakuu-Joulukuu"; break;
                }

                sarjat3kk.Add(sarja3kk);
            }

            sarja3kk.LisaaKilpailu(kilpailu, this.Asetukset);
        }

        private void LisaaKilpailu6kk(Kilpailu kilpailu, int vuosi)
        {
            int puolVuosi = (kilpailu.AlkamisAika.Month < 7) ? 0 : 1;
            var sarjat6kk = AvaaSarjat(vuosi, RankingSarjanPituus.Puolivuotta);
            RankingSarja sarja6kk = sarjat6kk.FirstOrDefault(x => x.SarjanNumero == puolVuosi);
            if (sarja6kk == null)
            {
                sarja6kk = new RankingSarja()
                {
                    Nimi = puolVuosi == 0 ? "Kevät" : "Syksy",
                    Pituus = RankingSarjanPituus.Puolivuotta,
                    SarjanNumero = puolVuosi,
                };

                sarjat6kk.Add(sarja6kk);
            }

            sarja6kk.LisaaKilpailu(kilpailu, this.Asetukset);
        }

        private void LisaaKilpailu12kk(Kilpailu kilpailu, int vuosi)
        {
            var vuosiSarjat = AvaaSarjat(vuosi, RankingSarjanPituus.Vuosi);
            RankingSarja vuosiSarja = null;
            if (vuosiSarjat.Count == 0)
            {
                vuosiSarja = new RankingSarja()
                {
                    Nimi = "RankingSarja",
                    Pituus = RankingSarjanPituus.Vuosi,
                    SarjanNumero = 0,
                };
                vuosiSarjat.Add(vuosiSarja);
            }
            else
            {
                vuosiSarja = vuosiSarjat.First();
            }

            vuosiSarja.LisaaKilpailu(kilpailu, this.Asetukset);
        }
    }
}
