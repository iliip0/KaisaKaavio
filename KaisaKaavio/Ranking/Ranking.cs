using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Ranking
{
    public class Ranking
        : NotifyPropertyChanged
    {
        private string rankingKansio = null;
        private string kansio = null;
        private Dictionary<int, Dictionary<RankingSarjanPituus, BindingList<RankingSarja>>> sarjat =
            new Dictionary<int, Dictionary<RankingSarjanPituus, BindingList<RankingSarja>>>();

        public List<int> Vuodet { get; private set; }

        private RankingAsetukset asetukset = null;
        public RankingAsetukset Asetukset
        {
            get
            {
                return this.asetukset;
            }
            set
            {
                if (this.asetukset != value)
                {
                    this.asetukset = value;
                    RaisePropertyChanged("Asetukset");

                    PaivitaKansio();
                    this.sarjat.Clear();
                }
            }
        }

        public Loki Loki = null;

        BindingSource sarjatBindingSource = new BindingSource();
        public BindingSource SarjatBindingSource
        {
            get
            {
                return this.sarjatBindingSource;
            }
        }

        BindingSource kilpailutBindingSource = new BindingSource();
        public BindingSource KilpailutBindingSource
        {
            get
            {
                return this.kilpailutBindingSource;
            }
        }

        private int valittuVuosi = DateTime.Now.Year;
        public int ValittuVuosi
        {
            get
            {
                return this.valittuVuosi;
            }

            set
            {
                if (this.valittuVuosi != value)
                {
                    this.valittuVuosi = value;
                    RaisePropertyChanged("ValittuVuosi");
                    PaivitaValitutSarjat();
                }
            }
        }

        private RankingSarjanPituus valittuPituus = RankingSarjanPituus.Kuukausi;
        public RankingSarjanPituus ValittuPituus
        {
            get
            {
                return this.valittuPituus;
            }

            set
            {
                if (this.valittuPituus != value)
                {
                    this.valittuPituus = value;
                    RaisePropertyChanged("ValittuPituus");
                    PaivitaValitutSarjat();
                }
            }
        }

        private BindingList<RankingSarja> valitutSarjat = null;
        public BindingList<RankingSarja> ValitutSarjat
        {
            get
            {
                return this.valitutSarjat;
            }

            set
            {
                if (this.valitutSarjat != value)
                {
                    this.valitutSarjat = value;
                    this.sarjatBindingSource.DataSource = this.valitutSarjat;

                    RaisePropertyChanged("ValitutSarjat");
                    RaisePropertyChanged("SarjojaValittavissa");

                    PaivitaValittuSarja();
                }
            }
        }

        public bool SarjojaValittavissa
        {
            get
            {
                return this.valitutSarjat != null && this.valitutSarjat.Count > 0;
            }
        }

        private RankingSarja valittuSarja = null;
        public RankingSarja ValittuSarja
        {
            get
            {
                return this.valittuSarja;
            }

            set
            {
                if (this.valittuSarja != value)
                {
                    this.valittuSarja = value;
                    this.kilpailutBindingSource.DataSource = this.valittuSarja != null ? this.valittuSarja.Osakilpailut : null;

                    RaisePropertyChanged("ValittuSarja");
                    RaisePropertyChanged("ValitutOsakilpailut");
                    RaisePropertyChanged("KilpailujaValittavissa");
                    RaisePropertyChanged("KokonaisTilanneRtf");
                    RaisePropertyChanged("KokonaisTilanneSbil");

                    PaivitaValittuOsakilpailu();
                }
            }
        }

        public bool KilpailujaValittavissa
        {
            get
            {
                return (this.valittuSarja != null) &&
                    (this.valittuSarja.Osakilpailut != null) &&
                    (this.valittuSarja.Osakilpailut.Count > 0);
            }
        }

        public BindingList<RankingOsakilpailu> ValitutOsakilpailut
        {
            get
            {
                return this.valittuSarja != null ? this.valittuSarja.Osakilpailut : null;
            }
        }

        private RankingOsakilpailu valittuOsakilpailu = null;
        public RankingOsakilpailu ValittuOsakilpailu
        {
            get
            {
                return this.valittuOsakilpailu;
            }

            set
            {
                if (this.valittuOsakilpailu != value)
                {
                    this.valittuOsakilpailu = value;

                    RaisePropertyChanged("ValittuOsakilpailu");
                    RaisePropertyChanged("OsakilpailunTilanneRtf");
                    RaisePropertyChanged("OsakilpailunTilanneSbil");
                }
            }
        }

        public string OsakilpailunTilanneRtf
        {
            get
            {
                return this.valittuOsakilpailu != null ? this.valittuOsakilpailu.TilanneRtf : string.Empty;
            }
        }

        public string OsakilpailunTilanneSbil
        {
            get
            {
                return this.valittuOsakilpailu != null ? this.valittuOsakilpailu.TilanneSbil : string.Empty;
            }
        }

        public string KokonaisTilanneRtf
        {
            get
            {
                return this.valittuSarja != null ? this.valittuSarja.TilanneRtf : string.Empty;
            }
        }

        public string KokonaisTilanneSbil
        {
            get
            {
                return this.valittuSarja != null ? this.valittuSarja.TilanneSbil : string.Empty;
            }
        }

        public Ranking()
        {
            this.rankingKansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot", "Ranking");

            Directory.CreateDirectory(this.rankingKansio);

            this.Vuodet = new List<int>();

            PaivitaKansio();
        }

        private void PaivitaKansio()
        {
            if (this.asetukset != null)
            {
                this.kansio = Path.Combine(this.rankingKansio, Enum.GetName(typeof(KaisaKaavio.Laji), this.asetukset.Laji));

                Directory.CreateDirectory(Path.Combine(this.kansio, DateTime.Now.Year.ToString()));

                this.Vuodet.Clear();
                DirectoryInfo dir = new DirectoryInfo(this.kansio);

                foreach (var vuosi in dir.EnumerateDirectories())
                {
                    int vuosiluku = 0;
                    if (Int32.TryParse(vuosi.Name, out vuosiluku))
                    {
                        this.Vuodet.Add(vuosiluku);
                    }
                }

                var vuodet = this.Vuodet
                    .OrderByDescending(x => x)
                    .ToArray();

                this.Vuodet.Clear();
                this.Vuodet.AddRange(vuodet);

            }
            else
            {
                this.kansio = this.rankingKansio;
            }

            Directory.CreateDirectory(this.kansio);
        }

        public void PaivitaValitutSarjat()
        {
            try
            {
                this.ValittuOsakilpailu = null;
                this.ValittuSarja = null;
                this.ValitutSarjat = null;

                this.ValitutSarjat = AvaaSarjat(this.valittuVuosi, this.valittuPituus);

                PaivitaValittuSarja();
            }
            catch (Exception e)
            {
                this.Loki.Kirjoita("Valittujen rankingsarjojen päivitys epäonnistui!", e, false);
                this.ValitutSarjat = null;
            }
        }

        private void PaivitaValittuSarja()
        {
            try
            {
                this.ValittuOsakilpailu = null;

                if (this.ValitutSarjat == null)
                {
                    this.ValittuSarja = null;
                }
                else if (this.ValittuSarja != null && this.ValitutSarjat.Contains(this.ValittuSarja))
                {
                }
                else
                {
                    this.ValittuSarja = this.ValitutSarjat.FirstOrDefault();
                }

                PaivitaValittuOsakilpailu();
            }
            catch (Exception e)
            {
                this.Loki.Kirjoita("Valitun rankingsarjan päivitys epäonnistui", e, false);
                this.ValittuSarja = null;
            }
        }

        private void PaivitaValittuOsakilpailu()
        {
            try
            {
                if (this.ValittuSarja == null)
                {
                    this.ValittuOsakilpailu = null;
                }
                else if (this.ValittuOsakilpailu != null && this.ValittuSarja.Osakilpailut.Contains(this.ValittuOsakilpailu))
                {
                }
                else
                {
                    this.ValittuOsakilpailu = this.ValittuSarja.Osakilpailut.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                this.Loki.Kirjoita("Valitun rankingosakilpailun päivitys epäonnistui", e, false);
                this.ValittuOsakilpailu = null;
            }
        }

        private string Kansio(int vuosi, RankingSarjanPituus pituus)
        {
            switch (pituus)
            {
                case RankingSarjanPituus.Kuukausi: return Path.Combine(this.kansio, vuosi.ToString(), "1kk");
                case RankingSarjanPituus.Vuodenaika: return Path.Combine(this.kansio, vuosi.ToString(), "3kk");
                case RankingSarjanPituus.Puolivuotta: return Path.Combine(this.kansio, vuosi.ToString(), "6kk");
                case RankingSarjanPituus.Vuosi:
                default: return Path.Combine(this.kansio, vuosi.ToString(), "12kk");
            }
        }

        private BindingList<RankingSarja> AvaaSarjat(int vuosi, RankingSarjanPituus pituus)
        {
            Dictionary<RankingSarjanPituus, BindingList<RankingSarja>> vuodenSarjat = null;
            if (this.sarjat.ContainsKey(vuosi))
            {
                vuodenSarjat = this.sarjat[vuosi];
            }
            else
            {
                vuodenSarjat = new Dictionary<RankingSarjanPituus, BindingList<RankingSarja>>();
                this.sarjat.Add(vuosi, vuodenSarjat);
            }


            if (vuodenSarjat.ContainsKey(pituus))
            {
                return vuodenSarjat[pituus];
            }

            BindingList<RankingSarja> sarjat = new BindingList<RankingSarja>();
            vuodenSarjat.Add(pituus, sarjat);

            string sarjaKansio = Kansio(vuosi, pituus);

            Directory.CreateDirectory(sarjaKansio);

            foreach (var sarjaTiedosto in Directory.EnumerateFiles(sarjaKansio, "*.xml", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    RankingSarja sarja = new RankingSarja();
                    sarja.Laji = this.asetukset.Laji;

                    sarja.Avaa(this.Loki, sarjaTiedosto);
                    sarja.PaivitaTilanne();
                    sarjat.Add(sarja);
                }
                catch (Exception e)
                {
                    this.Loki.Kirjoita(string.Format("Rankingsarjan {0} lataus epäonnistui", sarjaTiedosto), e, false);
                }
            }

            return sarjat;
        }

        public RankingOsakilpailu AvaaKilpailu(Kilpailu kilpailu)
        {
            if (kilpailu.RankingKisa)
            {
                var sarjat = AvaaSarjat(kilpailu.AlkamisAika.Year, kilpailu.RankingKisaTyyppi);
                if (sarjat != null)
                {
                    var sarja = sarjat.FirstOrDefault(x => x.SarjanNumero == kilpailu.RankingSarjanNumero());
                    if (sarja != null)
                    {
                        return sarja.Osakilpailut.FirstOrDefault(x => string.Equals(x.Nimi, kilpailu.Nimi));
                    }
                }
            }

            return null;
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
            try
            {
                if (kilpailu.RankingKisa)
                {
                    int vuosi = kilpailu.AlkamisAika.Year;

                    switch (kilpailu.RankingKisaTyyppi)
                    {
                        case RankingSarjanPituus.Kuukausi: LisaaKilpailu1kk(kilpailu, vuosi); break;
                        case RankingSarjanPituus.Vuodenaika: LisaaKilpailu3kk(kilpailu, vuosi); break;
                        case RankingSarjanPituus.Puolivuotta: LisaaKilpailu6kk(kilpailu, vuosi); break;
                        case RankingSarjanPituus.Vuosi: LisaaKilpailu12kk(kilpailu, vuosi); break;
                    }
                }
            }
            catch (Exception e)
            {
                this.Loki.Kirjoita("Rankingin päivitys epäonnistui", e, false);
            }
        }

        public void ValitseKilpailu(Kilpailu kilpailu)
        {
            try
            {
                if (kilpailu.RankingKisa)
                {
                    this.ValittuVuosi = kilpailu.AlkamisAika.Year;
                    this.ValittuPituus = kilpailu.RankingKisaTyyppi;

                    switch (this.ValittuPituus)
                    {
                        case RankingSarjanPituus.Kuukausi:
                            this.ValittuSarja = this.ValitutSarjat.FirstOrDefault(x => x.SarjanNumero == kilpailu.AlkamisAika.Month);
                            break;

                        case RankingSarjanPituus.Vuodenaika:
                            this.ValittuSarja = this.ValitutSarjat.FirstOrDefault(x => x.SarjanNumero == (kilpailu.AlkamisAika.Month - 1) / 3);
                            break;

                        case RankingSarjanPituus.Puolivuotta:
                            this.ValittuSarja = this.ValitutSarjat.FirstOrDefault(x => x.SarjanNumero == (kilpailu.AlkamisAika.Month - 1) / 6);
                            break;

                        case RankingSarjanPituus.Vuosi:
                            this.ValittuSarja = this.ValitutSarjat.FirstOrDefault();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                this.Loki.Kirjoita("Käynnissä olevan kilpailun asetus rankingsivulle epäonnistui!", e, false);
            }
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
                    Laji = this.asetukset.Laji
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
            int kvartaali = (kilpailu.AlkamisAika.Month - 1) / 3;
            var sarjat3kk = AvaaSarjat(vuosi, RankingSarjanPituus.Vuodenaika);
            RankingSarja sarja3kk = sarjat3kk.FirstOrDefault(x => x.SarjanNumero == kvartaali);
            if (sarja3kk == null)
            {
                sarja3kk = new RankingSarja()
                {
                    Pituus = RankingSarjanPituus.Vuodenaika,
                    SarjanNumero = kvartaali,
                    Laji = this.asetukset.Laji
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
            int puolVuosi = (kilpailu.AlkamisAika.Month - 1) / 6;
            var sarjat6kk = AvaaSarjat(vuosi, RankingSarjanPituus.Puolivuotta);
            RankingSarja sarja6kk = sarjat6kk.FirstOrDefault(x => x.SarjanNumero == puolVuosi);
            if (sarja6kk == null)
            {
                sarja6kk = new RankingSarja()
                {
                    Nimi = puolVuosi == 0 ? "Kevät" : "Syksy",
                    Pituus = RankingSarjanPituus.Puolivuotta,
                    SarjanNumero = puolVuosi,
                    Laji = this.asetukset.Laji
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
                    Nimi = "VuosiRanking",
                    Pituus = RankingSarjanPituus.Vuosi,
                    SarjanNumero = 0,
                    Laji = this.asetukset.Laji
                };
                vuosiSarjat.Add(vuosiSarja);
            }
            else
            {
                vuosiSarja = vuosiSarjat.First();
            }

            vuosiSarja.LisaaKilpailu(kilpailu, this.Asetukset);
        }

        private RankingSarja AvaaSarja(RankingSarjanPituus pituus, DateTime aika)
        {
            var sarjat = AvaaSarjat(aika.Year, pituus);
            if (sarjat != null)
            {
                switch (pituus)
                {
                    case RankingSarjanPituus.Kuukausi:
                        return sarjat.FirstOrDefault(x => x.SarjanNumero == aika.Month);

                    case RankingSarjanPituus.Vuodenaika:
                        return sarjat.FirstOrDefault(x => x.SarjanNumero == (aika.Month - 1) / 3);

                    case RankingSarjanPituus.Puolivuotta:
                        return sarjat.FirstOrDefault(x => x.SarjanNumero == (aika.Month - 1) / 6);

                    case RankingSarjanPituus.Vuosi:
                        return sarjat.FirstOrDefault();
                }
            }

            return null;
        }

        public bool HaeNykyinenRankingSijoitus(DateTime aika, RankingSarjanPituus pituus, string pelaaja, out int sijoitus)
        {
            var ranking = AvaaSarja(pituus, aika);

            if (ranking == null)
            {
                for (int i = 1; i < 3; ++i)
                {
                    switch (pituus)
                    {
                        case RankingSarjanPituus.Kuukausi: aika = aika.Subtract(new TimeSpan(31, 0, 0, 0, 0)); break;
                        case RankingSarjanPituus.Vuodenaika: aika = aika.Subtract(new TimeSpan(365 / 4, 0, 0, 0, 0)); break;
                        case RankingSarjanPituus.Puolivuotta: aika = aika.Subtract(new TimeSpan(365 / 2, 0, 0, 0, 0)); break;
                        case RankingSarjanPituus.Vuosi: aika = aika.Subtract(new TimeSpan(365, 0, 0, 0, 0)); break;
                    }

                    ranking = AvaaSarja(pituus, aika);
                    if (ranking != null)
                    {
                        break;
                    }
                }
            }

            if (ranking != null)
            {
                var r = ranking.RankingEnnenOsakilpailua(DateTime.Now);
                if (r != null)
                {
                    var p = r.FirstOrDefault(x => string.Equals(x.Nimi, pelaaja, StringComparison.OrdinalIgnoreCase));
                    if (p != null)
                    {
                        sijoitus = p.KumulatiivinenSijoitus;
                        return true;
                    }
                }
            }

            sijoitus = 0;
            return false;
        }
    }
}
