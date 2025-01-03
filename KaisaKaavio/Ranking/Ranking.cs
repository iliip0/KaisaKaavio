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
        public void ValitseRankingSarjaKilpailulle(Kilpailu kilpailu)
        {
            ValitseRankingSarja(
                kilpailu.AlkamisAikaDt.Year,
                kilpailu.RankingKisaLaji, 
                kilpailu.RankingKisaTyyppi, 
                Tyypit.Aika.RankingSarjanNumeroAjasta(kilpailu.RankingKisaTyyppi, kilpailu.AlkamisAikaDt), 
                kilpailu);
        }

        /// <summary>
        /// Asettaa UI:ssa tarkasteltavana olevan ranking sarjan
        /// </summary>
        public void ValitseRankingSarja(int vuosi, Laji laji, RankingSarjanPituus pituus, int numero, Kilpailu kilpailu)
        {
            this.ValittuSarja = AvaaRankingSarja(vuosi, laji, pituus, numero, kilpailu);
        }

        /// <summary>
        /// Avaa valitun rankingsarjan tarkasteltavaksi.
        /// Sarjan tilanne luodaan mikäli tätä sarjaa ei ole tarkasteltu aiemmin.
        /// Sarjan tilanne päivitetään mikäli jotain on muuttunut edellisen tarkastelun jälkeen.
        /// Tämä funktio palauttaa tyhjän sarjan mikäli sarjaan ei ole merkitty yhtään osakilpailua
        /// </summary>
        private RankingSarja AvaaRankingSarja(int vuosi, Laji laji, RankingSarjanPituus pituus, int numero, Kilpailu kilpailu)
        {
            var sarja = this.rankingSarjat.FirstOrDefault(x => 
                x.Vuosi == vuosi &&
                x.Laji == laji &&
                x.Pituus == pituus &&
                x.SarjanNumero == numero);

            if (sarja != null)
            {
                // Mikäli sarja sisältää ohjelmassa auki olevan kilpailun, sarja päivitetään joka kerta
                if (kilpailu != null &&
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
            };

            if (kilpailu != null &&
                kilpailu.RankingOsakilpailu != null &&
                sarja.SisaltaaOsakilpailun(kilpailu.RankingOsakilpailu))
            {
                sarja.Luo(this, this.Asetukset.RankingPisteytys(laji), this.Loki);
            }
            else
            {
                sarja.Avaa(this.Loki);
                if (sarja.TarkistaOnkoSarjaMuuttunut(this, this.Loki))
                {
                    sarja.Luo(this, this.Asetukset.RankingPisteytys(laji), this.Loki);
                }
            }

            this.rankingSarjat.Add(sarja);

            return sarja;
        }

        public RankingKuukausi AvaaRankingKuukausi(int vuosi, int kuu)
        {
            var kuukausi = this.kuukaudet.FirstOrDefault(x => x.Vuosi == vuosi && x.Kuukausi == kuu);
            if (kuukausi == null)
            {
                kuukausi = new RankingKuukausi()
                {
                    Vuosi = vuosi,
                    Kuukausi = kuu
                };

                kuukausi.Avaa(this.Loki);

                this.kuukaudet.Add(kuukausi);
            }

            return kuukausi;
        }

        public RankingKuukausi AvaaRankingKuukausi(DateTime aika)
        {
            return AvaaRankingKuukausi(aika.Year, aika.Month);
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

            var kuukausi = AvaaRankingKuukausi(kilpailu.AlkamisAikaDt);

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
                this.Loki.Kirjoita("Valitun rankingosakilpailun päivitys epäonnistui", e, false);
                this.ValittuOsakilpailu = null;
            }
        }

        public void TallennaAvatutSarjat()
        {
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

        public bool HaeNykyinenRankingSijoitus(Kilpailu kilpailu, string pelaaja, out int sijoitus)
        {
            var aika = kilpailu.AlkamisAikaDt;

            var ranking = AvaaRankingSarja(
                aika.Year,
                kilpailu.RankingKisaLaji,
                kilpailu.RankingKisaTyyppi,
                Tyypit.Aika.RankingSarjanNumeroAjasta(kilpailu.RankingKisaTyyppi, aika),
                kilpailu);

            if (ranking == null || ranking.Osakilpailut.Count == 0)
            {
                for (int i = 1; i < 3; ++i)
                {
                    switch (kilpailu.RankingKisaTyyppi)
                    {
                        case RankingSarjanPituus.Kuukausi: aika = aika.Subtract(new TimeSpan(31, 0, 0, 0, 0)); break;
                        case RankingSarjanPituus.Vuodenaika: aika = aika.Subtract(new TimeSpan(365 / 4, 0, 0, 0, 0)); break;
                        case RankingSarjanPituus.Puolivuotta: aika = aika.Subtract(new TimeSpan(365 / 2, 0, 0, 0, 0)); break;
                        case RankingSarjanPituus.Vuosi: aika = aika.Subtract(new TimeSpan(365, 0, 0, 0, 0)); break;
                    }

                    ranking = AvaaRankingSarja(
                        aika.Year,
                        kilpailu.RankingKisaLaji,
                        kilpailu.RankingKisaTyyppi,
                        Tyypit.Aika.RankingSarjanNumeroAjasta(kilpailu.RankingKisaTyyppi, aika),
                        null);

                    if (ranking != null)
                    {
                        break;
                    }
                }
            }

            if (ranking != null && ranking.Osakilpailut.Count > 0)
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
