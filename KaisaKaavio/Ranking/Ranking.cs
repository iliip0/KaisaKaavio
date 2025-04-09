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
    /// <summary>
    /// Pääluokka rankingsarjojen hallinnointiin
    /// </summary>
    public class Ranking
        : NotifyPropertyChanged
    {
        private List<RankingKuukausi> kuukaudet = new List<RankingKuukausi>();
        private List<RankingSarja> rankingSarjat = new List<RankingSarja>();

        public List<int> Vuodet { get; private set; }

        public Loki Loki = null;
        public Asetukset Asetukset = null;

        public string ValitunSarjanNimi
        {
            get
            {
                return this.ValittuSarja != null ? this.ValittuSarja.Nimi : "Valitussa sarjassa ei ole kilpailuja";
            }
        }

        public string ValitunSarjanKuvaus
        {
            get 
            {
                return this.ValittuSarja != null ? this.ValittuSarja.Kuvaus : string.Empty;
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
                    RaisePropertyChanged("ValitunSarjanNimi");
                    RaisePropertyChanged("ValitunSarjanKuvaus");
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
            string kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot", "Ranking");

            Directory.CreateDirectory(kansio);

            this.Vuodet = new List<int>();

            DirectoryInfo dir = new DirectoryInfo(kansio);
            foreach (var subdir in dir.EnumerateDirectories())
            {
                if (subdir.Name.Length == 4)
                {
                    int vuosi = 0;
                    if (Int32.TryParse(subdir.Name, out vuosi) && vuosi > 2023 && vuosi < 3000)
                    {
                        this.Vuodet.Add(vuosi);
                    }
                }
            }

            this.Vuodet.Add(DateTime.Now.Year);

            if (DateTime.Now.Month > 10)
            {
                this.Vuodet.Add(DateTime.Now.Year + 1);
            }

            var vuodet = this.Vuodet.Distinct().OrderByDescending(x => x).ToArray();
            this.Vuodet.Clear();
            this.Vuodet.AddRange(vuodet);
        }

        /// <summary>
        /// Asettaa UI:ssa tarkasteltavana olevan ranking sarjan
        /// </summary>
        public RankingSarja ValitseRankingSarjaKilpailulle(Kilpailu kilpailu)
        {
            return ValitseRankingSarja(
                kilpailu.AlkamisAikaDt.Year,
                kilpailu.RankingKisaLaji, 
                kilpailu.RankingKisaTyyppi, 
                Tyypit.Aika.RankingSarjanNumeroAjasta(kilpailu.RankingKisaTyyppi, kilpailu.AlkamisAikaDt), 
                kilpailu);
        }

        /// <summary>
        /// Asettaa UI:ssa tarkasteltavana olevan ranking sarjan
        /// </summary>
        public RankingSarja ValitseRankingSarja(int vuosi, Laji laji, RankingSarjanPituus pituus, int numero, Kilpailu kilpailu)
        {
            this.ValittuSarja = AvaaRankingSarja(vuosi, laji, pituus, numero, kilpailu, kilpailu.TestiKilpailu, true);
            return this.ValittuSarja;
        }

        public RankingSarja AvaaRankingSarja(Kilpailu kilpailu, bool paivitaAvoinKilpailu)
        {
            return AvaaRankingSarja(
                kilpailu.AlkamisAikaDt.Year,
                kilpailu.RankingKisaLaji,
                kilpailu.RankingKisaTyyppi,
                Tyypit.Aika.RankingSarjanNumeroAjasta(kilpailu.RankingKisaTyyppi, kilpailu.AlkamisAikaDt),
                kilpailu,
                kilpailu.TestiKilpailu,
                paivitaAvoinKilpailu);
        }

        /// <summary>
        /// Avaa valitun rankingsarjan tarkasteltavaksi.
        /// Sarjan tilanne luodaan mikäli tätä sarjaa ei ole tarkasteltu aiemmin.
        /// Sarjan tilanne päivitetään mikäli jotain on muuttunut edellisen tarkastelun jälkeen.
        /// Tämä funktio palauttaa tyhjän sarjan mikäli sarjaan ei ole merkitty yhtään osakilpailua
        /// </summary>
        private RankingSarja AvaaRankingSarja(int vuosi, Laji laji, RankingSarjanPituus pituus, int numero, Kilpailu kilpailu, bool testiSarja, bool paivitaAvoinKilpailu)
        {
#if PROFILE
            using (new Testaus.Profileri("Ranking.AvaaRankingSarja"))
#endif
            {
                var sarja = this.rankingSarjat.FirstOrDefault(x =>
                    x.Vuosi == vuosi &&
                    x.Laji == laji &&
                    x.Pituus == pituus &&
                    x.SarjanNumero == numero &&
                    x.TestiSarja == testiSarja);

                if (sarja != null)
                {
                    // Mikäli sarja sisältää ohjelmassa auki olevan kilpailun, sarja päivitetään joka kerta
                    if (paivitaAvoinKilpailu &&
                        kilpailu != null &&
                        !kilpailu.Tyhja &&
                        kilpailu.RankingOsakilpailu != null &&
                        sarja.SisaltaaOsakilpailun(kilpailu.RankingOsakilpailu))
                    {
                        this.rankingSarjat.Remove(sarja);
                        sarja = null;
                    }
                    else
                    {
                        return sarja;
                    }
                }

                sarja = new RankingSarja()
                {
                    Vuosi = vuosi,
                    Laji = laji,
                    Pituus = pituus,
                    SarjanNumero = numero,
                    TestiSarja = kilpailu != null ? kilpailu.TestiKilpailu : testiSarja
                };

                if (kilpailu != null &&
                   !kilpailu.Tyhja &&
                    kilpailu.RankingOsakilpailu != null &&
                    sarja.SisaltaaOsakilpailun(kilpailu.RankingOsakilpailu))
                {
                    if (sarja.Asetukset.Tyhja)
                    {
                        sarja.Asetukset.KopioiAsetuksista(this.Asetukset.RankingPisteytys(laji));
                    }

                    sarja.Luo(this, this.Loki);
                }
                else
                {
                    sarja.Avaa(this.Loki);

                    if (sarja.Asetukset.Tyhja)
                    {
                        sarja.Asetukset.KopioiAsetuksista(this.Asetukset.RankingPisteytys(laji));
                    }

                    if (sarja.TarkistaOnkoSarjaMuuttunut(this, this.Loki))
                    {
                        sarja.Luo(this, this.Loki);
                    }
                }

                this.rankingSarjat.Add(sarja);

                return sarja;
            }
        }

        public void PoistaRankingTietue(RankingOsakilpailuTietue tietue, bool testiRanking)
        {
            var kuukausi = AvaaRankingKuukausi(tietue.PvmDt.Year, tietue.PvmDt.Month, testiRanking);
            kuukausi.PoistaOsakilpailu(tietue, this.Loki);
        }

        public RankingKuukausi AvaaRankingKuukausi(int vuosi, int kuu, bool testiRanking)
        {
            var kuukausi = this.kuukaudet.FirstOrDefault(x => x.Vuosi == vuosi && x.Kuukausi == kuu);
            if (kuukausi == null)
            {
                kuukausi = new RankingKuukausi()
                {
                    Vuosi = vuosi,
                    Kuukausi = kuu,
                    TestiRanking = testiRanking
                };

                kuukausi.Avaa(this.Loki);

                this.kuukaudet.Add(kuukausi);
            }

            // Tarkista, että kisat ovat oikeassa kuussa
            while (true)
            {
                var kisa = kuukausi.Osakilpailut.FirstOrDefault(x => !kuukausi.SisaltaaOsakilpailunAjallisesti(x));
                if (kisa == null)
                {
                    break;
                }

                if (this.Loki != null)
                {
                    this.Loki.Kirjoita(string.Format("Poistetaan ranking osakilpailu {0} kuukaudesta {1}/{2}", kisa.Nimi, kuukausi.Kuukausi, kuukausi.Vuosi));
                }

                kuukausi.PoistaOsakilpailu(kisa, this.Loki);
                AvaaRankingKuukausi(kisa.PvmDt, testiRanking);
            }

            return kuukausi;
        }

        public RankingKuukausi AvaaRankingKuukausi(DateTime aika, bool testiRanking)
        {
            return AvaaRankingKuukausi(aika.Year, aika.Month, testiRanking);
        }

        /// <summary>
        /// Avaa ranking osakilpailutietueen annetulle kilpailulle. Samalla kaikki aiemmin ladatut sarjat ja tietueet pyyhitään muistista
        /// </summary>
        /// <param name="kilpailu"></param>
        /// <returns></returns>
        public RankingOsakilpailuTietue AvaaRankingTietueKilpailulle(Kilpailu kilpailu)
        {
            this.kuukaudet.Clear();
            this.rankingSarjat.Clear();

            var kuukausi = AvaaRankingKuukausi(kilpailu.AlkamisAikaDt, kilpailu.TestiKilpailu);

            return kuukausi.AvaaKilpailunTietue(kilpailu, this.Loki);
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
                if (this.Loki != null)
                {
                    this.Loki.Kirjoita("Valitun rankingosakilpailun päivitys epäonnistui", e, false);
                }
                this.ValittuOsakilpailu = null;
            }
        }

        public void TallennaAvatutSarjat()
        {
#if !ALLOW_MULTIPLE_INSTANCES // Rankingeja ei tallenneta kun useita KaisaKaavioita voi olla auki samanaikaisesti
            foreach (var kuukausi in this.kuukaudet)
            {
                try
                {
                    kuukausi.TallennaTarvittaessa(this.Loki);
                }
                catch (Exception e)
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita(string.Format("Ranking datan {0} tallennus epäonnistui!", kuukausi.TiedostonNimi), e, false);
                    }
                }
            }
#endif
        }

        public void TyhjennaSarjatMuistista()
        {
            try
            {
                this.ValittuOsakilpailu = null;
                this.ValittuSarja = null;

                //foreach (var kuukausi in kuukaudet)
                //{
                //    kuukausi.TyhjennaTietueetMuistista(nykyinenKilpailu);
                //}

                this.rankingSarjat.Clear();
            }
            catch
            { 
            }
        }

        public RankingSarja AvaaEdellinenSarja(Kilpailu kilpailu)
        {
#if PROFILE
            using (new Testaus.Profileri("Ranking.AvaaEdellinenSarja"))
#endif
            {
                DateTime aika = kilpailu.AlkamisAikaDt;

                switch (kilpailu.RankingKisaTyyppi)
                {
                    case RankingSarjanPituus.Kuukausi: aika = aika.Subtract(new TimeSpan(aika.Day + 1, 0, 0, 0, 0)); break;
                    case RankingSarjanPituus.Vuodenaika: aika = aika.Subtract(new TimeSpan(365 / 4, 0, 0, 0, 0)); break;
                    case RankingSarjanPituus.Puolivuotta: aika = aika.Subtract(new TimeSpan(365 / 2, 0, 0, 0, 0)); break;
                    case RankingSarjanPituus.Vuosi: aika = aika.Subtract(new TimeSpan(365, 0, 0, 0, 0)); break;
                }

                return AvaaRankingSarja(
                    aika.Year,
                    kilpailu.RankingKisaLaji,
                    kilpailu.RankingKisaTyyppi,
                    Tyypit.Aika.RankingSarjanNumeroAjasta(kilpailu.RankingKisaTyyppi, aika),
                    null,
                    kilpailu.TestiKilpailu,
                    false);
            }
        }
    }
}
